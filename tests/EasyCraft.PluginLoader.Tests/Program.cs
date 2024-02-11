using EasyCraft.PluginLoader;
using EasyCraft.PluginLoader.Abstraction;
using EasyCraft.PluginLoader.NuGetIntegrator;
using FluentAssertions;

// Clean Previous Result
if (Directory.Exists("plugins"))
    Directory.Delete("plugins", true);

INuGetIntegrator nuGetIntegrator = new NuGetIntegrator();
IPluginLoader pluginLoader = new NuGetPluginLoader(nuGetIntegrator);

Console.WriteLine("Testing InstallPluginAsync");
var result = await pluginLoader.InstallPluginAsync("Depository");
result.Should().BeTrue();
Console.WriteLine("InstallPluginAsync Passed");
Console.WriteLine("Testing GetInstalledPluginsAsync");
var installedPlugins = await pluginLoader.GetInstalledPluginsAsync();
installedPlugins.Should().ContainKey("Depository");
Console.WriteLine("GetInstalledPluginsAsync Passed");
Console.WriteLine("Testing LoadPluginAsync");
var types = await pluginLoader.LoadPluginAsync("Depository");
types.Should().NotBeEmpty();
Console.WriteLine("LoadPluginAsync Passed");
Console.WriteLine("Testing UninstallPluginAsync");
result = await pluginLoader.UninstallPluginAsync("Depository");
result.Should().BeTrue();
Console.WriteLine("UninstallPluginAsync Passed");
Console.WriteLine("Testing GetInstalledPluginsAsync");
installedPlugins = await pluginLoader.GetInstalledPluginsAsync();
installedPlugins.Should().NotContainKey("Depository");
Console.WriteLine("GetInstalledPluginsAsync Passed");