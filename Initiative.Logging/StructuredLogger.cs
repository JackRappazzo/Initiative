using Serilog;

namespace Initiative.Logging
{
    public class StructuredLogger : IStructuredLogger
    {
        private readonly ILogger _logger;
        public StructuredLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Debug(string messageTemplate, params object?[]? propertyValues)
        {
            _logger.Debug(messageTemplate, propertyValues);
        }

        public void Error(string messageTemplate, params object?[]? propertyValues)
        {
            _logger.Error(messageTemplate, propertyValues);
        }

        public void Error(Exception ex, string messageTemplate, params object?[]? propertyValues)
        {
            _logger.Error(ex, messageTemplate, propertyValues);
        }

        public void Information(string messageTemplate, params object?[]? propertyValues)
        {
            _logger.Information(messageTemplate, propertyValues);
        }

        public void Verbose(string messageTemplate, params object?[]? propertyValues)
        {
            _logger.Verbose(messageTemplate, propertyValues);
        }

        public void Warning(Exception ex, string messageTemplate, params object?[]? propertyValues)
        {
            _logger.Warning(ex, messageTemplate, propertyValues);
        }

        public void Warning(string messageTemplate, params object?[]? propertyValues)
        {
            _logger.Warning(messageTemplate, propertyValues);
        }

        public IStructuredLogger ForContext(string propertyName, object? value)
        {
            return new StructuredLogger(_logger.ForContext(propertyName, value));
        }
    }
}
