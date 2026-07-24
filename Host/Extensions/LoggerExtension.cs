using NLog;
using NLog.Extensions.Logging;

namespace Host.Extensions
{
    public static class LoggerExtension
    {
        private static readonly string LOG_FILE_CONFIG_PATH = Path.Combine("..","nlog.config");

        public static Logger AddNLogLogger(this IServiceCollection services)
        {
            var logger = LogManager.Setup().LoadConfigurationFromFile(LOG_FILE_CONFIG_PATH).GetCurrentClassLogger();
            try
            {
                services.AddLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                });
                services.AddSingleton<ILoggerProvider,NLogLoggerProvider>();
                logger.Info("NLog logger setup successfully.");
                return logger;
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Error initializing NLog");
                throw;
            }
        }
    }
}
