using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models;
using Corno.Web.Services.Progress.Interfaces;

namespace Corno.Web.Services.Progress;

public class BaseProgressService : IBaseProgressService
{
    #region -- Constructors --
    public BaseProgressService()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        ProgressModel = new ProgressModel();
        _progressModels = new Dictionary<string, ProgressModel>();

        Progress = new Progress<ProgressModel>(ProgressChanged);
    }
    #endregion

    #region -- Event Handlers --
    public event EventHandler<ProgressModel> OnProgressChanged;
    #endregion

    #region -- Data Members --

    protected IProgress<ProgressModel> Progress;

    private CancellationTokenSource _cancellationTokenSource;
    private IDictionary<string, ProgressModel> _progressModels;
    #endregion

    #region -- Propertes --

    public ProgressModel ProgressModel { get; set; }

    #endregion

    #region -- Private Methods --

    private ProgressModel GetProgressModel(string name)
    {
        var progressModel = _progressModels[name];
        if (null != progressModel)
            return progressModel;

        progressModel = new ProgressModel();
        _progressModels.Add(name, progressModel);

        return progressModel;
    }

    private void CreateMessage(ProgressModel progressModel)
    {
        var timeElapsed = DateTime.Now - (progressModel.StartTime ?? DateTime.Now);
        var tickTime = timeElapsed.Ticks / (progressModel.Progress <= 0 ? 1 : progressModel.Progress);
        var remainingProgress = progressModel.Maximum - progressModel.Progress;
        var timeRemaining = TimeSpan.FromTicks(tickTime * remainingProgress);
        progressModel.Percent = progressModel.Progress * 100 / (progressModel.Maximum <= 0 ? 1 : progressModel.Maximum);
        progressModel.Message = $"File: {Path.GetFileName(progressModel.FilePath)}, {Environment.NewLine}" +
                                $"Total : {progressModel.Maximum}, " +
                                $"Completed : {progressModel.Progress}, " +
                                $"Percent : {progressModel.Progress * 100 / (progressModel.Maximum <= 0 ? 1 : progressModel.Maximum)}%, " +
                                $"Imported: {progressModel.New}, " +
                                $"Existing: {progressModel.Existing}, " +
                                $"Ignored: {progressModel.Ignored}, {Environment.NewLine}, " +
                                $"Time Elapsed : {timeElapsed:d'.'hh':'mm':'ss}, " +
                                $"Time Remaining : {timeRemaining:d'.'hh':'mm':'ss}";
    }

    #endregion

    #region -- Public Methods --

    public virtual void ResetProgressModel()
    {
        ProgressModel.Reset();
    }

    public virtual void Initialize(string filePath, int minimum, int maximum,
        int step)
    {
        ProgressModel.Reset();

        ProgressModel.StartTime = DateTime.Now;
        ProgressModel.FilePath = filePath;

        ProgressModel.Minimum = minimum;
        ProgressModel.Maximum = maximum;
        ProgressModel.Step = step;

        _cancellationTokenSource = new CancellationTokenSource();

    }

    public virtual void Initialize(string header, int minimum, int maximum,
        int step, Action action)
    {
        ProgressModel.Reset();

        ProgressModel.Action = action;
        ProgressModel.Message = header;
        ProgressModel.StartTime = DateTime.Now;

        ProgressModel.Minimum = minimum;
        ProgressModel.Maximum = maximum;
        ProgressModel.Step = step;

        _cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task Report(int imported, int existing, int ignored)
    {
        if (ProgressModel.Maximum <= 0)
            ProgressModel.Maximum = 1;
        ProgressModel.MessageType = MessageType.Progress;
        ProgressModel.Progress += ProgressModel.Step;
        ProgressModel.Percent = ProgressModel.Progress * 100 / (ProgressModel.Maximum <= 0 ? 1 : ProgressModel.Maximum);
        ProgressModel.New += imported;
        ProgressModel.Existing += existing;
        ProgressModel.Ignored += ignored;

        CreateMessage(ProgressModel);

        Progress.Report(ProgressModel);
        OnProgressChanged?.Invoke(this, ProgressModel);

        await Task.Delay(0);
    }

    public async Task Report(ImportResult importResult, int count)
    {
        if (ProgressModel.Maximum <= 0)
            ProgressModel.Maximum = 1;
        ProgressModel.MessageType = MessageType.Progress;
        ProgressModel.Progress += count;
        ProgressModel.Percent = ProgressModel.Progress * 100 / (ProgressModel.Maximum <= 0 ? 1 : ProgressModel.Maximum);
        switch (importResult)
        {
            case ImportResult.New:
                ProgressModel.New += count;
                break;
            case ImportResult.Exists:
                ProgressModel.Existing += count;
                break;
            case ImportResult.Ignored:
                ProgressModel.Ignored += count;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(importResult), importResult, null);
        }

        CreateMessage(ProgressModel);

        Progress.Report(ProgressModel);
        OnProgressChanged?.Invoke(this, ProgressModel);

        await Task.Delay(0);
    }

    public Task Report(int deleted)
    {
        ProgressModel.MessageType = MessageType.Progress;
        ProgressModel.Progress += ProgressModel.Step;

        ProgressModel.Deleted += deleted;

        CreateMessage(ProgressModel);

        Progress.Report(ProgressModel);
        OnProgressChanged?.Invoke(this, ProgressModel);

        return Task.Delay(1);
    }

    public Task Report(string message, MessageType messageType = MessageType.General)
    {
        var progressModel = new ProgressModel
        {
            MessageType = messageType,
            Message = message
        };

        //CreateMessage(progressModel);

        Progress.Report(progressModel);
        OnProgressChanged?.Invoke(this, progressModel);

        return Task.Delay(1);
    }

    public Task UpdateJob(string name, DateTime? time, TimeSpan interval)
    {
        var progressModel = GetProgressModel(name);
        progressModel.JobInfo.Interval = interval;

        return Task.Delay(1);
    }

    public bool IsCancelled()
    {
        return _cancellationTokenSource.IsCancellationRequested;
    }

    public void CancelRequested()
    {
        Report("Cancellation requested");

        _cancellationTokenSource.Cancel();
    }

    public virtual bool Confirm(string message)
    {
        return false;
    }
    #endregion

    #region -- Events --

    protected virtual void ProgressChanged(ProgressModel progressModel)
    {
        LogHandler.LogInfo("ProgressChanged not implemented.");
    }
    #endregion
}