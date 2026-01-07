using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Corno.Web.Areas.Nilkamal.Dto.Plan;
using Corno.Web.Models.Plan;
using Corno.Web.Services.Plan.Interfaces;
using Kendo.Mvc.UI;

namespace Corno.Web.Areas.Nilkamal.Services.Interfaces;

public interface IPlanService : IBasePlanService
{
    Task IncreasePlanQuantityAsync(Plan plan, PlanItemDetail planDetail, double quantity, string newStatus);

    // View Model Methods
    Task<PlanViewDto> GetViewModelAsync(Plan plan);
    Task<IEnumerable> GetLotNosAsync(DateTime dueDate);
    Task<PositionDetailDto> FillLabelInformationAsync(PositionDetailDto positionView);
    Task FillCartonInformationAsync(PositionDetailDto positionView);
    
    // Plan Operations
    Task<PlanViewDto> DeletePositionAsync(string warehouseOrderNo, string position);
    Task DeletePlanAsync(string warehouseOrderNo);
    Task UpdateDueDateAsync(string warehouseOrderNo, DateTime dueDate);
    Task UpdateLotNoAsync(string warehouseOrderNo, string lotNo);
    
    // Data Retrieval Methods
    Task<DataSourceResult> GetIndexViewDtoAsync(DataSourceRequest request);
    Task<DataSourceResult> GetLabelsForPlanAsync(DataSourceRequest request, string warehouseOrderNo);
    Task<DataSourceResult> GetCartonsForPlanAsync(DataSourceRequest request, string warehouseOrderNo);
    Task UpdateQuantitiesAsync(Plan plan);

    // Pending Plan Methods
    Task<IEnumerable> GetPendingLotNosAsync(DateTime dueDate);
    Task<IEnumerable> GetPendingWarehouseOrdersAsync(string lotNo);
    Task<IEnumerable> GetPendingFamiliesAsync(Plan plan, List<string> selectedGroups = null);

    // Pending Products & Packets PacketLabel Form
    Task<IEnumerable> GetPendingProductsAsync(Plan plan);
    Task<IEnumerable> GetPendingPacketsAsync(string productionOrderNo, int productId);
}