using EasyCraft.Daemon.Abstraction.WebCompositor;
using EasyCraft.Daemon.SourceGenerator.WebCompositor;

namespace EasyCraft.Daemon.WebCompositors.Configurations;

[WebCompositor]
public class SwaggerCompositor : IWebCompositor
{
    public static void ConfigureBuilder(WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen();
        builder.Services.AddEndpointsApiExplorer();
    }

    public static void ConfigureApp(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}