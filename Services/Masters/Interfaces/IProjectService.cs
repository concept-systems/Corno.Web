using Corno.Web.Models.Masters;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Services.Masters.Interfaces;

public interface IProjectService : IMasterService<Project>
{
    #region -- Methods --

    System.Threading.Tasks.Task<Project> GetActiveProjectAsync();
    System.Threading.Tasks.Task<Project> GetProjectAsync(string status);
    #endregion
}