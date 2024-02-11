using System.Collections.Generic;

namespace EasyCraft.PluginLoader.NuGetIntegrator;

public class NuGetIntegratorOption
{
    public bool UsePrereleasePackage { get; init; } = false;
    public List<string> Sources { get; set; } = [];
    public string PackageDirectory { get; set; } = "plugins";
}