using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace EasyCraft.PluginLoader.NuGetIntegrator;

public class NuGetLogger : NuGet.Common.LoggerBase
{
    private readonly ILogger<NuGetIntegrator> _logger;

    public NuGetLogger(ILogger<NuGetIntegrator> logger)
    {
        _logger = logger;
    }
    
    public override void Log(ILogMessage message)
    {
        _logger.LogTrace(message.Message);
    }

    public override Task LogAsync(ILogMessage message)
    {
        _logger.LogTrace(message.Message);
        return Task.CompletedTask;
    }
}