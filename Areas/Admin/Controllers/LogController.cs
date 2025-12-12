using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;
using System.Linq;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Controllers;
using Corno.Web.Extensions;

namespace Corno.Web.Areas.Admin.Controllers;


public class LogController : SuperController
{
    public ActionResult Index()
    {
        var logEntries = new List<LogDto>();
        try
        {
            var logFolderPath = Server.MapPath("~/logs/");
            if (!Directory.Exists(logFolderPath))
                Directory.CreateDirectory(logFolderPath);
            var latestFiles = Directory.GetFiles(logFolderPath)
                .Select(file => new FileInfo(file))
                .OrderByDescending(file => file.LastWriteTime)
                .Take(5).ToList();
            var logLines = new List<string>();
            foreach (var filePath in latestFiles)
                logLines.AddRange(ReadLogsFromFile(filePath.FullName));
            logEntries = ParseLogs(logLines);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(logEntries);
    }

    public List<string> ReadLogsFromFile(string filePath)
    {
        var logs = new List<string>();

        if (!System.IO.File.Exists(filePath)) return logs;

        // Open the file in read mode and allow sharing for reading/writing
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(fileStream);
        while (reader.ReadLine() is { } line)
            logs.Add(line);

        return logs;
    }

    public List<LogDto> ParseLogs(List<string> logLines)
    {
        var logEntries = new List<LogDto>();

        foreach (var line in logLines)
        {
            var parts = line.Split(new[] { ';' });
            if (parts.Length >= 3)
            {
                var logEntry = new LogDto
                {
                    Timestamp = parts[0].After("Date:").Trim(),
                    Level = parts[1].After(":").Trim(),
                    Message = parts[2].After(":").Trim()
                };
                logEntries.Add(logEntry);
            }
        }

        return logEntries;
    }
}
