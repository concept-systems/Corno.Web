namespace Corno.Web.Services.Progress.Interfaces;

public interface IWebProgressService : IBaseProgressService
{
    /*#region -- Event Handlers --
    event EventHandler<ProgressModel> OnProgressChanged;
    #endregion*/

    void SetWebProgress();
}