using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasyCraft.PluginLoader.Abstraction;

public interface IPluginLoader
{
    public Task<bool> InstallPluginAsync(string id, string version = "latest", CancellationToken cancellationToken = default);
    public Task<bool> UninstallPluginAsync(string id, CancellationToken cancellationToken = default);
    public Task<bool> UpdatePluginAsync(string id, string version = "latest", CancellationToken cancellationToken = default);
    public Task<List<Type>> LoadPluginAsync(string id, CancellationToken cancellationToken = default);
    public Task UnloadPluginAsync(string id, CancellationToken cancellationToken = default);
    public Task<Dictionary<string,string>> GetInstalledPluginsAsync(CancellationToken cancellationToken = default); 
}