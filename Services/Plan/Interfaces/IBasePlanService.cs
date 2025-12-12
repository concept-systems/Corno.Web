using Corno.Web.Services.Base.Interfaces;
using System.Threading.Tasks;

namespace Corno.Web.Services.Plan.Interfaces;

public interface IBasePlanService : IPrintService<Models.Plan.Plan>
{
    #region -- Public Get Methods --

    Task<Models.Plan.Plan> GetByWarehouseOrderNoAsync(string warehouseOrderNo);

    #endregion
}