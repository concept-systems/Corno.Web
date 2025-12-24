using System.Threading.Tasks;
using Corno.Web.Models.Plan;
using Corno.Web.Services.Plan.Interfaces;
using System.Collections;
using System;
using System.Collections.Generic;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Dto.Plan;
using Corno.Web.Services.Import.Interfaces;
using Kendo.Mvc.UI;

namespace Corno.Web.Areas.Kitchen.Services.Interfaces;

public interface IPlanService : IBasePlanService, IFileImportService<BmImportDto>
{
    Task UpdatePackQuantitiesAsync(Plan plan);
    Task UpdateQuantitiesAsync(Plan plan);
    Task IncreasePlanQuantityAsync(Plan plan, PlanItemDetail planDetail, double quantity, string newStatus);
    Task<IEnumerable<string>> GetFamiliesAsync(int locationId, List<PlanItemDetail> planItemDetails = null);
    Task<IEnumerable<string>> GetFamiliesAsync(int locationId, Plan plan);
    Task<IEnumerable> GetLotNosAsync(DateTime dueDate);
    // View Model Methods
    Task<PlanViewDto> GetViewModelAsync(Plan plan);
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
    
    // Pending Plan Methods
    Task<IEnumerable> GetPendingLotNosAsync(DateTime dueDate);
    Task<IEnumerable> GetPendingWarehouseOrdersAsync(string lotNo);
    Task<IEnumerable> GetPendingFamiliesAsync(Plan plan, List<string> selectedGroups = null);
}