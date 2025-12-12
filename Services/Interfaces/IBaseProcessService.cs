using Corno.Web.Models;

namespace Corno.Web.Services.Interfaces;

public interface IBaseProcessService : IMasterService<Process>
{
    #region -- Public Methods --

    System.Threading.Tasks.Task<Process> GetBySymbolAsync(string symbol);

    #endregion
}