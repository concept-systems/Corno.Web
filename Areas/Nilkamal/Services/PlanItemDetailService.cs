using Corno.Web.Areas.Nilkamal.Services.Interfaces;
using Corno.Web.Models.Plan;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services;

namespace Corno.Web.Areas.Nilkamal.Services;

public class PlanItemDetailService : BaseService<PlanItemDetail>, IPlanItemDetailService
{
    #region -- Constructors --
    public PlanItemDetailService(IGenericRepository<PlanItemDetail> genericRepository) : base(genericRepository)
    {
    }
    #endregion
}