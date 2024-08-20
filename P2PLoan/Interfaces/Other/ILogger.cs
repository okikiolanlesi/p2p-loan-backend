using System;
using Microsoft.Extensions.Logging;

namespace P2PLoan.Logging
{
    public class SimpleLogger<T> : ILogger<T>
    {
        private readonly string _categoryName;

        public SimpleLogger()
        {
            _categoryName = typeof(T).FullName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true; // Enable all log levels for simplicity
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var logMessage = formatter(state, exception);
            Console.WriteLine($"{logLevel}: {_categoryName} - {logMessage}");
        }
    }
}
