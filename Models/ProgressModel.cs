using System;
using Corno.Web.Globals;

namespace Corno.Web.Models;

public class ProgressModel
{
    #region -- Constructors --
    public ProgressModel()
    {
        Reset();
    }
    #endregion

    #region -- Properties --
    public string Operation { get; set; }
    public Action Action { get; set; }
    public string Name { get; set; }

    public DateTime? StartTime { get; set; }
    public MessageType MessageType { get; set; }
    public string Message { get; set; }
    public string FilePath { get; set; }

    public int Minimum { get; set; }
    public int Maximum { get; set; }
    public int Step { get; set; }
    public int Progress { get; set; }
    public int Percent { get; set; }
    public int New { get; set; }
    public int Existing { get; set; }
    public int Ignored { get; set; }
    public int Deleted { get; set; }
        
    public JobInfo JobInfo { get; set; }
        
    #endregion

    #region -- Methods --

    public void Reset()
    {
        Minimum = 0;
        Maximum = 0;
        Step = 1;
        Progress = 0;
        Percent = 0;
        New = 0;
        Existing = 0;
        Ignored = 0;
        FilePath = string.Empty;
        JobInfo = new JobInfo();
    }
    #endregion
}

public class JobInfo
{
    public JobInfo()
    {
        Interval = TimeSpan.Zero;
        StartTime = null;
        EndTime = null;
    }

    public string GroupName { get; set; }
    public string JobName { get; set; }
    public string JobDescription { get; set; }
    public string TriggerName { get; set; }
    public string TriggerGroupName { get; set; }
    public string TriggerType { get; set; }
    public string TriggerState { get; set; }
        
    public DateTime? NextFireTime { get; set; }
    public DateTime? PreviousFireTime { get; set; }
    public TimeSpan? Interval { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}