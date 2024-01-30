using System;
using System.Threading;
using System.Threading.Tasks;
using EasyCraft.Abstraction.Instance;
using EasyCraft.PluginLoader.Abstraction;

namespace EasyCraft.Abstraction.Starter;

public abstract class StarterBase : PluginBase
{
    public abstract Task StartAsync(InstanceBase instance, CancellationToken cancellationToken);
    public abstract Task StopAsync(InstanceBase instance, CancellationToken cancellationToken);
}