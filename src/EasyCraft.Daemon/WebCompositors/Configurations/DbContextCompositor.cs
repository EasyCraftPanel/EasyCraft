using Depository.Abstraction.Interfaces;
using EasyCraft.Daemon.Abstraction.WebCompositor;
using EasyCraft.Daemon.SourceGenerator.WebCompositor;
using EasyCraft.DataManagement;
using EasyCraft.DataManagement.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace EasyCraft.Daemon.WebCompositors.Configurations;

[WebCompositor(10)]
public class DbContextCompositor : IWebCompositor
{

    public static void ConfigureBuilder(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContextPool<DaemonDbContext>(options => { options.UseInMemoryDatabase("daemon"); });
        DaemonDbContext.EntityTypes = builder.Services.Where(t => t.ServiceType == typeof(IEntity))
            .Select(t => t.ImplementationType).Cast<Type>().ToList();
    }

    public static void ConfigureApp(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DaemonDbContext>();
        dbContext.Database.EnsureCreated();
    }
}

public class DaemonDbContext : DataManagementDbContext<DaemonDbContext>
{
    
    public DaemonDbContext(DbContextOptions<DaemonDbContext> options) : base(options)
    {
    }
}