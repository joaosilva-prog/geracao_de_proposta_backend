using System.Collections.Concurrent;

namespace GeracaoPropostaOrigo.Logs
{
    public class CustomLoggerProvider : ILoggerProvider
    {
        readonly CustomLoggerProviderConfiguration configuration;

        readonly ConcurrentDictionary<string, CustomLogger> loggers =
            new ConcurrentDictionary<string, CustomLogger>();
        public CustomLoggerProvider(CustomLoggerProviderConfiguration config)
        {
            configuration = config;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, name => new CustomLogger(name, configuration));
        }
        public void Dispose()
        {
            loggers.Clear();
        }
    }
}
