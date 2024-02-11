using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NuGet.Client;
using NuGet.Configuration;
using NuGet.ContentModel;
using NuGet.Frameworks;
using NuGet.PackageManagement;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Packaging.PackageExtraction;
using NuGet.Packaging.Signing;
using NuGet.ProjectManagement;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.RuntimeModel;
using NuGet.Versioning;
using JsonSerializer = System.Text.Json.JsonSerializer;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace EasyCraft.PluginLoader.NuGetIntegrator;

public class NuGetIntegrator : INuGetIntegrator
{
    // Code rearranged from https://github.com/waf/CSharpRepl/blob/main/CSharpRepl.Services/Nuget/NugetPackageInstaller.cs
    // Licenced Under Mozilla Public License 2.0
    private readonly NuGetIntegratorOption _option;
    private readonly ILogger<NuGetIntegrator>? _logger;
    private readonly NuGetFramework _nugetFramework;
    private readonly FolderNuGetProject _nugetProject;
    private readonly SourceRepositoryProvider _sourceRepositoryProvider;
    private readonly NuGetPackageManager _packageManager;
    private readonly ISettings _nugetSetting;
    private readonly SourceCacheContext _sourceCacheContext;
    private readonly ResolutionContext _resolutionContext;
    private readonly List<SourceRepository> _repositories;
    private readonly NuGetLogger _nuGetLogger;
    private readonly ClientPolicyContext _clientPolicyContext;
    private readonly RuntimeGraph _runtimeGraph;
    private readonly ConsoleProjectContext _projectContext;
    private readonly ManagedCodeConventions _managedCodeConventions;
    private readonly Dictionary<string, string> _installedPackages;
    private readonly string _packagesInfoPath;


    public NuGetIntegrator(NuGetIntegratorOption? option = null, ILogger<NuGetIntegrator>? logger = null)
    {
        _option = option ?? new NuGetIntegratorOption();
        _logger = logger;

        _logger?.LogTrace("Initializing NuGetIntegrator");
        // Initialize NuGetProject
        _nuGetLogger = new NuGetLogger(logger);
        if (!Directory.Exists(_option.PackageDirectory)) Directory.CreateDirectory(_option.PackageDirectory);
        _runtimeGraph = new RuntimeGraph();
        _nugetFramework = NuGetFramework.Parse(GetCurrentFramework());
        _option.PackageDirectory = Path.GetFullPath(_option.PackageDirectory);
        _nugetProject = new FolderNuGetProject(_option.PackageDirectory,
            new PackagePathResolver(_option.PackageDirectory), _nugetFramework);
        _nugetSetting = GetNuGetSetting();
        _sourceRepositoryProvider =
            new SourceRepositoryProvider(new PackageSourceProvider(_nugetSetting, GetPackageSources()),
                Repository.Provider.GetCoreV3());
        _repositories = _sourceRepositoryProvider.GetRepositories().ToList();
        _packageManager = new NuGetPackageManager(_sourceRepositoryProvider, _nugetSetting, _nugetProject.Root)
        {
            PackagesFolderNuGetProject = _nugetProject
        };
        _sourceCacheContext = new SourceCacheContext();
        _resolutionContext = new ResolutionContext(
            DependencyBehavior.Lowest,
            includePrelease: _option.UsePrereleasePackage,
            includeUnlisted: _option.UsePrereleasePackage,
            VersionConstraints.None,
            new GatherCache(),
            _sourceCacheContext);
        _clientPolicyContext = ClientPolicyContext.GetClientPolicy(_nugetSetting, _nuGetLogger);
        _projectContext = new ConsoleProjectContext(_nuGetLogger)
        {
            PackageExtractionContext = new PackageExtractionContext(
                PackageSaveMode.Defaultv3,
                PackageExtractionBehavior.XmlDocFileSaveMode,
                _clientPolicyContext,
                _nuGetLogger)
            {
                CopySatelliteFiles = false
            }
        };
        _managedCodeConventions = new ManagedCodeConventions(_runtimeGraph);
        _packagesInfoPath = Path.Combine(_option.PackageDirectory, "packages.json");
        if (File.Exists(_packagesInfoPath))
        {
            try
            {
                _installedPackages =
                    JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(_packagesInfoPath)) ??
                    throw new JsonSerializationException();
            }
            catch
            {
                // ignored
            }
        }

        if (_installedPackages is null)
        {
            _installedPackages = new Dictionary<string, string>();
            _ = SaveInstalledPackagesAsync();
        }

        _logger?.LogTrace("NuGetIntegrator Initialized");
    }


    private string GetCurrentFramework()
    {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly is null ||
            // ReSharper disable once StringLiteralTypo
            // ReSharper disable once CommentTypo
            assembly.Location.EndsWith("testhost.dll",
                StringComparison
                    .OrdinalIgnoreCase)) //for unit tests (testhost.dll targets netcoreapp2.1 instead of net6.0)
        {
            assembly = Assembly.GetExecutingAssembly();
        }

        var targetFrameworkAttribute = assembly.GetCustomAttribute<TargetFrameworkAttribute>();
        return targetFrameworkAttribute?.FrameworkName ?? "netstandard2.0";
    }

    private IEnumerable<PackageSource> GetPackageSources()
    {
        return _option.Sources.Select(source => new PackageSource(source)).ToList();
    }

    private ISettings GetNuGetSetting()
    {
        var curDir = _option.PackageDirectory;
        var settings = File.Exists(Path.Combine(curDir, Settings.DefaultSettingsFileName))
            ? Settings.LoadSpecificSettings(curDir, Settings.DefaultSettingsFileName)
            : Settings.LoadDefaultSettings(curDir);
        return settings;
    }

    public async Task<ResolvedPackage> GetPackageLatestVersionAsync(string packageId,
        CancellationToken cancellationToken = default)
    {
        return await NuGetPackageManager.GetLatestVersionAsync(packageId, _nugetProject, _resolutionContext,
            _repositories.AsEnumerable(), _nuGetLogger, cancellationToken);
    }

    public async Task<bool> InstallPackageAsync(string packageId, string version = "latest",
        CancellationToken cancellationToken = default)
    {
        if (version == "latest")
        {
            // Get latest version
            var package = await GetPackageLatestVersionAsync(packageId, cancellationToken);
            if (!package.Exists)
            {
                _logger?.LogWarning("Package {PackageId} not found", packageId);
                return false;
            }

            Debug.Assert(package.LatestVersion.OriginalVersion != null,
                "package.LatestVersion.OriginalVersion is null");
            version = package.LatestVersion.OriginalVersion;
        }

        var packageIdentity = new PackageIdentity(packageId, NuGetVersion.Parse(version));
        var installed = _nugetProject.PackageExists(packageIdentity);
        if (installed)
        {
            _logger?.LogWarning("Package {PackageId} already installed", packageId);
            return true;
        }

        await _packageManager.InstallPackageAsync(_nugetProject, packageIdentity, _resolutionContext,
            _projectContext,
            _repositories, Array.Empty<SourceRepository>(), cancellationToken);
        _installedPackages[packageIdentity.Id] = packageIdentity.Version.ToString();
        await SaveInstalledPackagesAsync();
        return true;
    }

    public async Task<bool> UninstallPackageAsync(string packageId, CancellationToken cancellationToken = default)
    {
        var projectContext = new UninstallationContext(true);
        await _packageManager.UninstallPackageAsync(_nugetProject, packageId, projectContext, _projectContext,
            cancellationToken);
        _installedPackages.Remove(packageId);
        await SaveInstalledPackagesAsync();
        return true;
    }

    private async Task SaveInstalledPackagesAsync()
    {
        await File.WriteAllTextAsync(_packagesInfoPath, JsonSerializer.Serialize(_installedPackages));
    }

    public Task<Dictionary<string, string>> GetAllInstalledPackagesAsync(
        CancellationToken cancellationToken = default)
    {
        // NuGet don't record installed info for us, we need record ourself
        return Task.FromResult(_installedPackages);
    }

    public async Task<string> GetPackageDirectoryAsync(string packageId, string version = "latest",
        CancellationToken cancellationToken = default)
    {
        NuGetVersion nugetVersion;
        if (version == "latest")
        {
            var dependencyInfos = await GetAllInstalledPackagesAsync(cancellationToken);
            if (dependencyInfos.TryGetValue(packageId, out var latestVersion))
            {
                nugetVersion = new NuGetVersion(latestVersion);
            }
            else
            {
                throw new VersionNotFoundException();
            }
        }
        else
        {
            nugetVersion = new NuGetVersion(version);
        }

        var identity = new PackageIdentity(packageId, nugetVersion);
        var installedPath = _nugetProject.GetInstalledPath(identity);
        if (installedPath == null) throw new VersionNotFoundException();
        return installedPath;
    }

    public async Task<List<string>> GetPackageAndDependenciesLocationsAsync(string packageId, string version = "latest",
        CancellationToken cancellationToken = default)
    {
        var installedPath = await GetPackageDirectoryAsync(packageId, version, cancellationToken);
        return await GetDllPathInFolderWithReferencesAsync(installedPath, cancellationToken);
    }

    private async Task<List<string>> GetDllPathInFolderWithReferencesAsync(string path,
        CancellationToken cancellationToken)
    {
        var result = new List<string>();
        var reader = new PackageFolderReader(path);
        var files = (await reader.GetFilesAsync(cancellationToken));
        var collection = new ContentItemCollection();
        collection.Load(files);


        var frameworks = (await reader.GetSupportedFrameworksAsync(cancellationToken)).ToList();
        var supportedFrameworks = (frameworks)
            .Where(f => GetDllPathInFramework(f, collection, path).Count !=
                        0) //Not all supported frameworks contains dlls. E.g. Microsoft.CSharp.4.7.0\lib\netcoreapp2.0 contains only empty file '_._'.
            .ToList();
        var selectedFramework = NuGetFramework.AnyFramework;
        if (supportedFrameworks.Count > 0)
            selectedFramework = supportedFrameworks[0];
        if (supportedFrameworks.Count > 1)
        {
            var frameworkReducer = new FrameworkReducer();
            selectedFramework = frameworkReducer.GetNearest(_nugetFramework, supportedFrameworks);
            if (selectedFramework == null)
            {
                if (supportedFrameworks.Count != 0)
                {
                    _logger?.LogWarning("Cannot find framework {Framework} in package {PackagePath}",
                        _nugetFramework.Framework,
                        path);
                    return result;
                }

                
            }
        }

        var dllPaths = GetDllPathInFramework(selectedFramework, collection, path);
        result.AddRange(dllPaths);

        // Now load dependencies
        var dependencies = (await reader.GetPackageDependenciesAsync(cancellationToken)).ToList()
            .FirstOrDefault(pdg => pdg.TargetFramework == selectedFramework);
        if (dependencies == null) return result;
        foreach (var dependency in dependencies.Packages)
        {
            if (dependency.VersionRange.MinVersion is null) continue;
            var dependencyPath = await GetPackageDirectoryAsync(dependency.Id,
                dependency.VersionRange.MinVersion.ToString(), cancellationToken);
            var dependencyDllPaths = await GetDllPathInFolderWithReferencesAsync(dependencyPath, cancellationToken);
            result.AddRange(dependencyDllPaths);
        }

        return result.Distinct().ToList();
    }

    private List<string> GetDllPathInFramework(NuGetFramework framework, ContentItemCollection collection,
        string packageRoot)
    {
        var result = new List<string>();
        var bestItems = collection.FindBestItemGroup(
            _managedCodeConventions.Criteria.ForFrameworkAndRuntime(framework,
                RuntimeInformation.RuntimeIdentifier),
            _managedCodeConventions.Patterns.RuntimeAssemblies
        );
        if (bestItems is null || bestItems.Items.Count == 0) return result;
        foreach (var item in bestItems.Items)
        {
            if (!item.Path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)) continue;
            var fullPath = Path.GetFullPath(Path.Combine(packageRoot, item.Path));
            if (File.Exists(fullPath)) result.Add(fullPath);
        }

        return result;
    }
}