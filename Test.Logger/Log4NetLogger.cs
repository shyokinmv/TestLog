﻿using log4net;
using log4net.Repository;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Test.Logger
{
    /// <summary>
    /// 
    /// https://www.michalbialecki.com/en/2018/12/21/adding-a-log4net-provider-in-net-core-console-app/
    ///  
    /// https://stackify.com/log4net-guide-dotnet-logging/
    /// 
    /// https://dotnetthoughts.net/how-to-use-log4net-with-aspnetcore-for-logging/
    /// 
    /// 
    /// </summary>
    public class Log4NetLogger : ILogger
    {
        private readonly string _name;

        private readonly ILog _log;

        private readonly bool _skipDiagnosticLogs;

        private ILoggerRepository _loggerRepository;

        public Log4NetLogger(string name, FileInfo fileInfo, bool skipDiagnosticLogs)
        {
            _name = name;
            _loggerRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            _log = LogManager.GetLogger(_loggerRepository.Name, name);
            _skipDiagnosticLogs = skipDiagnosticLogs;

            log4net.Config.XmlConfigurator.Configure(_loggerRepository, fileInfo);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return _log.IsFatalEnabled;
                case LogLevel.Debug:
                case LogLevel.Trace:
                    return _log.IsDebugEnabled && AllowDiagnostics();
                case LogLevel.Error:
                    return _log.IsErrorEnabled;
                case LogLevel.Information:
                    return _log.IsInfoEnabled && AllowDiagnostics();
                case LogLevel.Warning:
                    return _log.IsWarnEnabled;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            string message = $"{formatter(state, exception)} {exception}";

            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                switch (logLevel)
                {
                    case LogLevel.Critical:
                        _log.Fatal(message);
                        break;
                    case LogLevel.Debug:
                    case LogLevel.Trace:
                        _log.Debug(message);
                        break;
                    case LogLevel.Error:
                        _log.Error(message);
                        break;
                    case LogLevel.Information:
                        _log.Info(message);
                        break;
                    case LogLevel.Warning:
                        _log.Warn(message);
                        break;
                    default:
                        _log.Warn($"Encountered unknown log level {logLevel}, writing out as Info.");
                        _log.Info(message, exception);
                        break;
                }
            }
        }

        private bool AllowDiagnostics()
        {
            if (!_skipDiagnosticLogs)
            {
                return true;
            }

            return !(_name.ToLower().StartsWith("microsoft")
                || _name == "IdentityServer4.AccessTokenValidation.Infrastructure.NopAuthenticationMiddleware");
        }
    }
}
