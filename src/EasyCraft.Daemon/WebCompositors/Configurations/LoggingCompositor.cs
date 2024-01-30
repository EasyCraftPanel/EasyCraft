using EasyCraft.Daemon.Abstraction.WebCompositor;
using EasyCraft.Daemon.SourceGenerator.WebCompositor;
using Serilog;

namespace EasyCraft.Daemon.WebCompositors.Configurations;

[WebCompositor(0)]
public class LoggingCompositor : IWebCompositor
{
    public static void ConfigureBuilder(WebApplicationBuilder builder)
    {
        var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("log/log.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);
    }

    public static void ConfigureApp(WebApplication app)
    {
        // ignore
    }
}