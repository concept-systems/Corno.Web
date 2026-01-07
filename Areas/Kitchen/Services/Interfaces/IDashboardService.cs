using System;
using System.Threading.Tasks;
using Corno.Web.Areas.Kitchen.Dto.Dashboard;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface IDashboardService : IService
{
    // Main Dashboard
    Task<MainDashboardDto> GetMainDashboardAsync(DateTime? startDate = null, DateTime? endDate = null);
    
    // Plan Dashboard
    Task<PlanDashboardDto> GetPlanDashboardAsync(DateTime? startDate = null, DateTime? endDate = null);
    
    // Label Dashboard
    Task<LabelDashboardDto> GetLabelDashboardAsync(DateTime? startDate = null, DateTime? endDate = null);
    
    // Carton Dashboard
    Task<CartonDashboardDto> GetCartonDashboardAsync(DateTime? startDate = null, DateTime? endDate = null);
    
    // Real-time data updates (for AJAX calls)
    Task<PlanDashboardDto> GetPlanDashboardDataAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<LabelDashboardDto> GetLabelDashboardDataAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<CartonDashboardDto> GetCartonDashboardDataAsync(DateTime? startDate = null, DateTime? endDate = null);
}

