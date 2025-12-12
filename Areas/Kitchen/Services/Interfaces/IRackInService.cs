using Corno.Web.Areas.Kitchen.Dto.Rack_In;
using System.Threading.Tasks;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface IRackInService : ILabelService
{
    Task PerformRackIn(RackInViewDto dto);
}