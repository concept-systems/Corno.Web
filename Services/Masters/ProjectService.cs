using System;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Models.Masters;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Masters.Interfaces;

namespace Corno.Web.Services.Masters;

public class ProjectService : MasterService<Project>, IProjectService
{
    #region -- Constructors --
    public ProjectService(IGenericRepository<Project> genericRepository) : base(genericRepository)
    {
    }
    #endregion

    /*#region -- Protected Methods --
    protected override LinqKit.ExpressionStarter<Project> GetExtraPredicate()
    {
        return LinqKit.PredicateBuilder.New<Project>();
    }
    #endregion*/
    #region -- Public Methods --

    public async System.Threading.Tasks.Task<Project> GetActiveProjectAsync()
    {
        var activeProject = await FirstOrDefaultAsync(p => p.Status == StatusConstants.Active,
            p => p).ConfigureAwait(false);
        if (null == activeProject)
            throw new Exception("No Active Project in system.");
        return activeProject;
    }

    public async System.Threading.Tasks.Task<Project> GetProjectAsync(string status)
    {
        // Check whether ProjectId AppSettings is available in system.
        var projectId = System.Configuration.ConfigurationManager.AppSettings[FieldConstants.ProjectId]
            .ToInt();
        if (projectId > 0)
        {
            var project = await FirstOrDefaultAsync(p => p.Id == projectId, p => p).ConfigureAwait(false);
            if (null != project)
                return project;
        }

        // Try to get project by given status
        var activeProject = await FirstOrDefaultAsync(p => p.Status.Equals(status),
            p => p).ConfigureAwait(false);
        if (null != activeProject)
            return activeProject;

        status = StatusConstants.Active;
        activeProject = await FirstOrDefaultAsync(p => p.Status == status,
            p => p).ConfigureAwait(false);
        return activeProject ?? throw new Exception("No Active Project in system.");
    }
    #endregion
}