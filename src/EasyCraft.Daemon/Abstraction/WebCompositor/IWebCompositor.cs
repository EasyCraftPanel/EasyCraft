namespace EasyCraft.Daemon.Abstraction.WebCompositor;

public interface IWebCompositor
{
    public static abstract void ConfigureBuilder(WebApplicationBuilder builder);
    public static abstract void ConfigureApp(WebApplication app);
}