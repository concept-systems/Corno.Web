using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using Corno.Web.Services.Logger;
using Corno.Web.Services.Logger.Interfaces;

namespace Corno.Web.Logger;

public static class LogHandler
{
    #region -- Data Members --

    private static readonly ILoggingService Logger = LoggingService.GetLoggingService();
    #endregion

    #region -- Properties --
    public static bool ShowMessageBox { get; set; }
    #endregion

    #region -- Methods --
    
    public static void LogError(string message)
    {
        LogError(new Exception(message));
    }

    public static void LogError(Exception exception)
    {
        var detailException = GetDetailException(exception);
        Logger.Error(detailException);

        //if (ShowMessageBox)
        //    MessageBox.Show(detailException.Message);
    }

    public static void LogInfo(Exception exception)
    {
        Logger.Info(GetDetailException(exception));
    }

    public static void LogInfo(string message)
    {
        Logger.Info(new Exception(message));
    }
    #endregion

    public static Exception GetDetailException(Exception exception)
    {
        var message = exception.Message + "\n";
        if (exception.GetType() == typeof(DbEntityValidationException))
        {
            if (exception is DbEntityValidationException dbEntityValidationException)
            {
                foreach (var eve in dbEntityValidationException.EntityValidationErrors)
                {
                    message =
                        $"Entity of type \"{eve.Entry.Entity.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation errors: \n";
                    foreach (var ve in eve.ValidationErrors)
                        message += $"\n  - Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"";
                }
            }
        }

        if (exception.InnerException?.GetType() == typeof(System.Data.SqlClient.SqlException))
        {
            if (exception.InnerException is System.Data.SqlClient.SqlException sqlException)
            {
                foreach (var error in sqlException.Errors)
                {
                    //message =
                    //    $"Entity of type \"{error.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation errors: \n";
                    //foreach (var ve in eve.ValidationErrors)
                    //    message += $"\n  - Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"";
                }
            }
        }

        if (exception.InnerException != null)
        {
            message += exception.InnerException.Message + Environment.NewLine;
            exception = exception.InnerException;

            if (exception.InnerException != null)
            {
                message += exception.InnerException.Message + Environment.NewLine;
                exception = exception.InnerException;
            }
        }

        if (exception.Data.Count > 0)
        {
            foreach (DictionaryEntry de in exception.Data)
            {
                if (de.Value is List<Exception> exceptions)
                {
                    message += "\n";
                    foreach (var value in exceptions)
                        message += $"\nError: \"{value.Message}\"";
                }
                else
                {
                    message += $"\n{de.Key} : {de.Value}";
                }
            }
        }

        //var stackInfo = GetCallStackInfo();
        ////message = $"Exception in {nameof(MyFunction)}. Call stack:\n{stackInfo}\nOriginal message: {message}";
        //message = $"Call stack:\n{stackInfo}\nOriginal message: {message}";

        // message += $" Stack Trace : {exception.StackTrace}";

        return new Exception(message);
    }

    public static string GetCallStackInfo()
    {
        var trace = new StackTrace(true);
        var frames = trace.GetFrames();
        if (frames == null) return "No stack info available.";

        var stackDetails = "";
        foreach (var frame in frames)
        {
            var method = frame.GetMethod();
            var className = method.DeclaringType?.FullName ?? "UnknownClass";
            var methodName = method.Name;
            stackDetails += $"{className}.{methodName} (Line {frame.GetFileLineNumber()})\n";
        }
        return stackDetails;
    }


}