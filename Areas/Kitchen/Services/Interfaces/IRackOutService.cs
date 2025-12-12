using System.Threading.Tasks;
using Corno.Web.Areas.Kitchen.Dto.Rack_Out;
using Corno.Web.Services.Warehouse.Interfaces;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface IRackOutService : IBaseRackOutService
{
    Task PerformRackOut(RackOutViewDto dto);
}