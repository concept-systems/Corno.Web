using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Models.Plan;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Plan;

namespace Corno.Web.Areas.Kitchen.Services;

public class DispatchService : BasePlanService, IDispatchService
{
    #region -- Constructors --
    public DispatchService(IGenericRepository<Plan> genericRepository) : base(genericRepository)
    {
    }
    #endregion
}