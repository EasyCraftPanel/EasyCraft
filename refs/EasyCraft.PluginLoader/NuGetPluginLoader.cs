using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using EasyCraft.PluginLoader.Abstraction;
using EasyCraft.PluginLoader.NuGetIntegrator;
using NuGet.Protocol.Plugins;

namespace EasyCraft.PluginLoader;

public class NuGetPluginLoader : IPluginLoader
{
    private readonly INuGetIntegrator _nuGetIntegrator;
    private readonly Dictionary<string, AssemblyLoadContext> _contexts = [];

    public NuGetPluginLoader(INuGetIntegrator nuGetIntegrator)
    {
        _nuGetIntegrator = nuGetIntegrator;
    }

    public async Task<bool> InstallPluginAsync(string id, string version = "latest",
        CancellationToken cancellationToken = default)
    {
        return await _nuGetIntegrator.InstallPackageAsync(id, version, cancellationToken);
    }

    public async Task<bool> UninstallPluginAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _nuGetIntegrator.UninstallPackageAsync(id, cancellationToken);
    }

    public async Task<bool> UpdatePluginAsync(string id, string version = "latest",
        CancellationToken cancellationToken = default)
    {
        var package = await _nuGetIntegrator.GetPackageAsync(id, version, cancellationToken);
        if (!package.Exists) return false;
        return await _nuGetIntegrator.InstallPackageAsync(id, package.LatestVersion.ToString(), cancellationToken);
    }

    public async Task<List<Type>> LoadPluginAsync(string id, CancellationToken cancellationToken = default)
    {
        var result = new List<Type>();
        var locations =
            await _nuGetIntegrator.GetPackageAndDependenciesLocationsAsync(id, cancellationToken: cancellationToken);
        var context = new AssemblyLoadContext(id);
        foreach (var location in locations)
        {
            context.LoadFromAssemblyPath(location);
        }

        _contexts[id] = context;
        foreach (var contextAssembly in context.Assemblies)
        {
            result.AddRange(contextAssembly.ExportedTypes
                .Where(type => typeof(PluginBase).IsAssignableFrom(type))
                .ToList());
        }

        return result;
    }

    public Task UnloadPluginAsync(string id, CancellationToken cancellationToken = default)
    {
        if (!_contexts.TryGetValue(id, out var value)) return Task.CompletedTask;
        if (value.IsCollectible)
            value.Unload();
        _contexts.Remove(id);
        return Task.CompletedTask;
    }

    public async Task<Dictionary<string, string>> GetInstalledPluginsAsync(
        CancellationToken cancellationToken = default)
    {
        var installed = await _nuGetIntegrator.GetAllInstalledPackagesAsync(cancellationToken);
        return installed.ToDictionary(package => package.Id, package => package.Version.ToString());
    }
}