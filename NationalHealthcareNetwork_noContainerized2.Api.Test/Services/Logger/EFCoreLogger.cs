using System;
using Microsoft.Extensions.Logging;

namespace NationalHealthcareNetwork_noContainerized2.Api.Test.Services.Logger
{
    internal class EFCoreLogger : ILogger
    {
        private readonly Action<string> _efCoreLogAction;
        private readonly LogLevel _logLevel;

        public EFCoreLogger(Action<string> efCoreLogAction, LogLevel logLevel)
        {
            _efCoreLogAction = efCoreLogAction;
            _logLevel = logLevel;
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _efCoreLogAction($"LogLevel: {logLevel}, {state}");
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _logLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
