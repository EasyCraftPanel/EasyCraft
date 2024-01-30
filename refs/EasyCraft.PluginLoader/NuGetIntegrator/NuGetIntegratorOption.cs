using System.Collections.Generic;

namespace EasyCraft.PluginLoader.NuGetIntegrator;

public class NuGetIntegratorOption
{
    public bool UsePrereleasePackage { get; init; }
    public List<string> Sources { get; set; } = [];
    public required string PackageDirectory { get; init; }
}