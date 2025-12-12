using System;
using System.IO;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models;
using Corno.Web.Services.Progress.Interfaces;

namespace Corno.Web.Services.Progress;

public class WebProgressService : BaseProgressService, IWebProgressService
{
    /*#region -- Event Handlers --
    public virtual event EventHandler<ProgressModel> OnProgressChanged;
    #endregion*/

    #region -- Public Methods --

    public void SetWebProgress()
    {
        Progress = new Progress<ProgressModel>(ProgressChanged);
    }
    #endregion

    #region -- Events --

    protected override void ProgressChanged(ProgressModel progressModel)
    {
        try
        {
            if (null == progressModel)
                throw new Exception("Invalid Progress");

            switch(progressModel.MessageType)
            {
                /* case MessageType.Progress:
                     {
                         var timeElapsed = DateTime.Now - (progressModel.StartTime ?? DateTime.Now);
                         var tickTime = timeElapsed.Ticks / progressModel.Progress;
                         var remainingProgress = progressModel.Maximum - progressModel.Progress;
                         var timeRemaining = TimeSpan.FromTicks(tickTime * remainingProgress);
                         progressModel.Percent = progressModel.Progress * 100 / progressModel.Maximum;
                         progressModel.Message = $"File: {Path.GetFileName(progressModel.FilePath)}, {Environment.NewLine}" +
                                   $"Total : {progressModel.Maximum}, " +
                                   $"Completed : {progressModel.Progress}, " +
                                   $"Percent : {progressModel.Progress * 100 / progressModel.Maximum}%, " +
                                   $"Imported: {progressModel.New}, " +
                                   $"Existing: {progressModel.Existing}, " +
                                   $"Ignored: {progressModel.Ignored}, {Environment.NewLine}, " +
                                   $"Time Elapsed : {timeElapsed:d'.'hh':'mm':'ss}, " +
                                   $"Time Remaining : {timeRemaining:d'.'hh':'mm':'ss}";
                     }
                     break;
                 default:
                     break;*/

                case MessageType.Progress:
                    {
                        var timeElapsed = DateTime.Now - (progressModel.StartTime ?? DateTime.Now);

                        if (progressModel.Progress > 0)
                        {
                            var tickTime = timeElapsed.Ticks / progressModel.Progress;
                            var remainingProgress = progressModel.Maximum - progressModel.Progress;
                            var timeRemaining = TimeSpan.FromTicks(tickTime * remainingProgress);
                            progressModel.Percent = progressModel.Progress * 100 / progressModel.Maximum;

                            progressModel.Message = $"File: {Path.GetFileName(progressModel.FilePath)}, {Environment.NewLine}" +
                                      $"Total : {progressModel.Maximum}, " +
                                      $"Completed : {progressModel.Progress}, " +
                                      $"Percent : {progressModel.Percent}%, " +
                                      $"Imported: {progressModel.New}, " +
                                      $"Existing: {progressModel.Existing}, " +
                                      $"Ignored: {progressModel.Ignored}, {Environment.NewLine}" +
                                      $"Time Elapsed : {timeElapsed:d'.'hh':'mm':'ss}, " +
                                      $"Time Remaining : {timeRemaining:d'.'hh':'mm':'ss}";
                        }
                        else
                        {
                            progressModel.Percent = 0;
                            progressModel.Message = $"File: {Path.GetFileName(progressModel.FilePath)}, {Environment.NewLine}" +
                                      $"Total : {progressModel.Maximum}, " +
                                      $"Completed : {progressModel.Progress}, " +
                                      $"Percent : 0%, " +
                                      $"Imported: {progressModel.New}, " +
                                      $"Existing: {progressModel.Existing}, " +
                                      $"Ignored: {progressModel.Ignored}, {Environment.NewLine}" +
                                      $"Time Elapsed : {timeElapsed:d'.'hh':'mm':'ss}, " +
                                      $"Time Remaining : Calculating...";
                        }
                    }
                    break;
            }
            

            //OnProgressChanged?.Invoke(null, progressModel);
        }
        catch (Exception exception)
        {
            LogHandler.LogInfo(exception.Message);
        }
    }
    #endregion
}