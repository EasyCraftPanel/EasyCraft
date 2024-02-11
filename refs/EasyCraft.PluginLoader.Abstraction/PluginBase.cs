using System.Threading.Tasks;

namespace EasyCraft.PluginLoader.Abstraction
{
    public abstract class PluginBase
    {
        public abstract string Id { get; }
        public abstract string Name { get; }
        public abstract string Version { get; }
        public abstract string Author { get; }
        public abstract string Description { get; }
        public abstract Task OnLoad();
        public abstract Task OnUnload();
    }
}