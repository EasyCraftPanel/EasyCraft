﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NuGet.PackageManagement;
using NuGet.Packaging;
using NuGet.Packaging.Core;

namespace EasyCraft.PluginLoader.NuGetIntegrator;

public interface INuGetIntegrator
{
    public Task<ResolvedPackage> GetPackageLatestVersionAsync(string packageId, CancellationToken cancellationToken = default);
    public Task<bool> InstallPackageAsync(string packageId, string version = "latest",
        CancellationToken cancellationToken = default);
    public Task<bool> UninstallPackageAsync(string packageId, CancellationToken cancellationToken = default);
    public Task<Dictionary<string, string>> GetAllInstalledPackagesAsync(
        CancellationToken cancellationToken = default);
    public Task<string> GetPackageDirectoryAsync(string packageId,string version = "latest", CancellationToken cancellationToken = default);
    public Task<List<string>> GetPackageAndDependenciesLocationsAsync(string packageId, string version = "latest", CancellationToken cancellationToken = default);
}