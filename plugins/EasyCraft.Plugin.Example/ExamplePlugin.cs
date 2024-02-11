using EasyCraft.PluginLoader.Abstraction;

namespace EasyCraft.Plugin.Example;

public class ExamplePlugin : PluginBase
{
    /// <summary>
    /// Plugin Id, 与 NuGet 包的 Id 相同
    /// </summary>
    public override string Id => "EasyCraft.Plugin.Example";

    /// <summary>
    /// 插件名称
    /// </summary>
    public override string Name => "EasyCraft 示例插件";

    /// <summary>
    /// 插件版本, 与 NuGet 包的版本相同
    /// </summary>
    public override string Version => "0.0.1";
    
    /// <summary>
    /// 作者
    /// </summary>
    public override string Author => "EasyCraft";

    /// <summary>
    /// 插件简介
    /// </summary>
    public override string Description => "这是 EasyCraft 示例插件";

    /// <summary>
    /// 插件加载时触发
    /// </summary>
    public override Task OnLoad()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 插件卸载时触发
    /// </summary>
    public override Task OnUnload()
    {
        return Task.CompletedTask;
    }
}