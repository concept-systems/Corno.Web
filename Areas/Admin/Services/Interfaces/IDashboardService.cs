using System.Threading.Tasks;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Areas.Admin.Services.Interfaces;

public interface IDashboardService : IService
{
    Task<DashboardDto> GetDashboardDataAsync();
}

