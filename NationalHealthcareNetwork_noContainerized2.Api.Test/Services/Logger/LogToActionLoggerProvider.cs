using System;
using Microsoft.Extensions.Logging;

namespace NationalHealthcareNetwork_noContainerized2.Api.Test.Services.Logger
{
    internal class LogToActionLoggerProvider : ILoggerProvider
    {
        private readonly Action<string> _efCoreLogAction;
        private readonly LogLevel _logLevel;

        public LogToActionLoggerProvider(Action<string> efCoreLogAction, LogLevel logLevel = LogLevel.Information)
        {
            _efCoreLogAction = efCoreLogAction;
            _logLevel = logLevel;
        }
        public void Dispose()
        {
            // nothing to dispose
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new EFCoreLogger(_efCoreLogAction, _logLevel);

        }
    }
}
