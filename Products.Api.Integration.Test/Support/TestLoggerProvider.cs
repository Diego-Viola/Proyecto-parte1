using Microsoft.Extensions.Logging;

namespace Products.Api.Integration.Test.Support;
public class TestLoggerProvider : ILoggerProvider
{
    private readonly List<string> _logs = new();
    public ILogger CreateLogger(string categoryName) => new TestLogger(_logs);
    public void Dispose() { }
    public IReadOnlyList<string> GetLogs() => _logs;

    private class TestLogger : ILogger
    {
        private readonly List<string> _logs;
        public TestLogger(List<string> logs) => _logs = logs;
        public IDisposable BeginScope<TState>(TState state) => null!;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _logs.Add(formatter(state, exception));
        }
    }
}
