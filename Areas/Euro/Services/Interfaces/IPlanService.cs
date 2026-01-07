using System.Threading.Tasks;
using Corno.Web.Models.Plan;
using Corno.Web.Services.Plan.Interfaces;
using System.Collections;
using System;
using System.Collections.Generic;
using Corno.Web.Areas.Euro.Dto.Label;
using Corno.Web.Areas.Euro.Dto.Plan;
using Corno.Web.Services.Import.Interfaces;
using Kendo.Mvc.UI;

namespace Corno.Web.Areas.Euro.Services.Interfaces;

public interface IPlanService : IBasePlanService
{
    Task IncreasePlanQuantityAsync(Plan plan, PlanItemDetail planDetail, double quantity, string newStatus);

    // View Model Methods
    Task<PlanViewDto> GetViewModelAsync(Plan plan);
    Task<PositionDetailDto> FillLabelInformationAsync(PositionDetailDto positionView);
    Task FillCartonInformationAsync(PositionDetailDto positionView);
    
    // Data Retrieval Methods
    Task<DataSourceResult> GetIndexViewDtoAsync(DataSourceRequest request);
    Task<DataSourceResult> GetLabelsForPlanAsync(DataSourceRequest request, string productionOrderNo);
    Task<DataSourceResult> GetCartonsForPlanAsync(DataSourceRequest request, string productionOrderNo);
    
}

