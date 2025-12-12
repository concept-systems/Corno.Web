using System;
using System.Threading.Tasks;
using Corno.Web.Globals;
using Corno.Web.Models;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Services.Progress.Interfaces;

public interface IBaseProgressService : IService {

    #region -- Event Handlers --
    event EventHandler<ProgressModel> OnProgressChanged;
    #endregion

    #region -- Properties --
    ProgressModel ProgressModel { get; set; }
    #endregion

    #region -- Methods --
    void ResetProgressModel();
    void Initialize(string filePath, int minimum, int maximum, int step);
    void Initialize(string header, int minimum, int maximum, int step,
        Action action);

    Task Report(int imported, int existing, int ignored);
    Task Report(int deleted);
    Task Report(string message, MessageType messageType = MessageType.General);
    Task Report(ImportResult importResult, int count);

    bool IsCancelled();
    void CancelRequested();

    bool Confirm(string message);
    #endregion
}