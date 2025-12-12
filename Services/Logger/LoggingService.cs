using System;
using Corno.Web.Logger;
using Corno.Web.Services.Logger.Interfaces;
using NLog;
using NLog.Config;

namespace Corno.Web.Services.Logger;

public class LoggingService : NLog.Logger, ILoggingService
{
    private const string LoggerName = "NLogLogger";

    [Obsolete("Obsolete")]
    public static ILoggingService GetLoggingService()
    {
        ConfigurationItemFactory.Default.LayoutRenderers
            .RegisterDefinition("utc_date", typeof(UtcDateRenderer));

        var logger = (ILoggingService)LogManager.GetLogger(LoggerName, typeof(LoggingService));
        return logger;
    }

    //private static void AddMessageBoxTarget()
    //{
    //    var messageBoxTarget = new MessageBoxTarget
    //    {
    //        Layout = "${event-context:item=error-message}",
    //        Caption = "${level} message",
    //        Name = "messageBox"
    //    };
    //    var loggingRule = new LoggingRule("*", LogLevel.Trace, messageBoxTarget);

    //    LogManager.Configuration.AddTarget("messageBox", messageBoxTarget);
    //    LogManager.Configuration.LoggingRules.Add(loggingRule);
    //    LogManager.Configuration.Reload();

    //    //SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Trace);
    //}

    public new void Debug(Exception exception, string format, params object[] args)
    {
        if (!IsDebugEnabled) return;
        var logEvent = GetLogEvent(LoggerName, LogLevel.Debug, exception, format, args);
        Log(typeof(LoggingService), logEvent);
    }

    public new void Error(Exception exception, string format, params object[] args)
    {
        if (!IsErrorEnabled) return;
        var logEvent = GetLogEvent(LoggerName, LogLevel.Error, exception, format, args);
        Log(typeof(LoggingService), logEvent);
    }

    public new void Fatal(Exception exception, string format, params object[] args)
    {
        if (!IsFatalEnabled) return;
        var logEvent = GetLogEvent(LoggerName, LogLevel.Fatal, exception, format, args);
        Log(typeof(LoggingService), logEvent);
    }

    public new void Info(Exception exception, string format, params object[] args)
    {
        if (!IsInfoEnabled) return;
        var logEvent = GetLogEvent(LoggerName, LogLevel.Info, exception, format, args);
        Log(typeof(LoggingService), logEvent);
    }

    public new void Trace(Exception exception, string format, params object[] args)
    {
        if (!IsTraceEnabled) return;
        var logEvent = GetLogEvent(LoggerName, LogLevel.Trace, exception, format, args);
        Log(typeof(LoggingService), logEvent);
    }

    public new void Warn(Exception exception, string format, params object[] args)
    {
        if (!IsWarnEnabled) return;
        var logEvent = GetLogEvent(LoggerName, LogLevel.Warn, exception, format, args);
        Log(typeof(LoggingService), logEvent);
    }

    public void Debug(Exception exception)
    {
        Debug(exception, string.Empty);
    }

    public void Error(Exception exception)
    {
        Error(exception, string.Empty);
    }

    public void Fatal(Exception exception)
    {
        Fatal(exception, string.Empty);
    }

    public void Info(Exception exception)
    {
        Info(exception, string.Empty);
    }

    public void Trace(Exception exception)
    {
        Trace(exception, string.Empty);
    }

    public void Warn(Exception exception)
    {
        Warn(exception, string.Empty);
    }

    private LogEventInfo GetLogEvent(string loggerName, LogLevel level, Exception exception, string format, object[] args)
    {
        string assemblyProp = string.Empty;
        string classProp = string.Empty;
        string methodProp = string.Empty;
        string messageProp = string.Empty;
        string innerMessageProp = string.Empty;

        var logEvent = new LogEventInfo
            (level, loggerName, string.Format(format, args));

        if (exception != null)
        {
            assemblyProp = exception.Source;
            if (exception.TargetSite?.DeclaringType != null)
                classProp = exception.TargetSite.DeclaringType.FullName;
            methodProp = exception.TargetSite?.Name;
            messageProp = LogHandler.GetDetailException(exception)?.Message;// exception.Message;

            if (exception.InnerException != null)
            {
                innerMessageProp = exception.InnerException.Message;
            }
        }

        logEvent.Properties["error-source"] = assemblyProp;
        logEvent.Properties["error-class"] = classProp;
        logEvent.Properties["error-method"] = methodProp;
        logEvent.Properties["error-message"] = messageProp;
        logEvent.Properties["inner-error-message"] = innerMessageProp;

        logEvent.Message = messageProp?.TrimEnd('\n');

        return logEvent;
    }
}