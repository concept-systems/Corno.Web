using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Models.Plan;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Plan;

namespace Corno.Web.Areas.Kitchen.Services;

public class PalletInService : BasePlanService, IPalletInService
{
    #region -- Constructors --
    public PalletInService(IGenericRepository<Plan> genericRepository) : base(genericRepository)
    {
    }
    #endregion
}