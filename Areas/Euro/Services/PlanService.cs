using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Web.Areas.Euro.Services.Interfaces;
using Corno.Web.Globals;
using Corno.Web.Models.Plan;
using Corno.Web.Services.Plan;
using Corno.Web.Windsor;
using Corno.Web.Areas.Euro.Dto.Label;
using Corno.Web.Areas.Euro.Dto.Plan;
using Corno.Web.Services.Interfaces;
using Mapster;
using Corno.Web.Repository.Interfaces;
using System.Threading.Tasks;
using Corno.Web.Areas.Euro.Dto.Carton;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.Web.Areas.Euro.Services;

public class PlanService : BasePlanService, IPlanService
{
    #region -- Constructors --

    public PlanService(IGenericRepository<Plan> genericRepository, IMiscMasterService miscMasterService)
        : base(genericRepository)
    {
        _miscMasterService = miscMasterService;
    }

    #endregion

    #region -- Data Members --

    private readonly IMiscMasterService _miscMasterService;

    #endregion

    #region -- Public Methods --

    public async Task IncreasePlanQuantityAsync(Plan plan, PlanItemDetail planItemDetail, double quantity,
        string newStatus)
    {
        var planDetail = plan?.PlanItemDetails.FirstOrDefault(d =>
            d.Position == planItemDetail.Position);
        if (null == planDetail)
            throw new Exception($"No plan detail available Production Order no {plan?.WarehouseOrderNo} & " +
                                $"Item Id {planItemDetail.ItemId} & Position {planItemDetail.Position}");

        await UpdateAsync(plan).ConfigureAwait(false);
    }

    #region -- View Model Methods --

    public async Task<PlanViewDto> GetViewModelAsync(Plan plan)
    {
        TypeAdapterConfig<Plan, PlanViewDto>
            .NewConfig()
            .Map(dest => dest.OneLineItemCode, src => src.System)
            .Map(dest => dest.PlanViewItemDtos, src => src.PlanItemDetails);

        TypeAdapterConfig<PlanItemDetail, PlanViewItemDto>
            .NewConfig();

        // Map Plan to DTO using Mapster
        var dto = plan.Adapt<PlanViewDto>();

        // Populate Warehouse Name from MiscMaster (optimized - only select Name)
        if (plan.WarehouseId.HasValue)
        {
            var warehouseName = await _miscMasterService.FirstOrDefaultAsync(m => m.Id == plan.WarehouseId.Value &&
                    m.MiscType == MiscMasterConstants.Warehouse,
                m => m.Name).ConfigureAwait(false);
            dto.WarehouseName = warehouseName;
        }

        // Populate Import File Name from Reserved1
        dto.ImportFileName = plan.Reserved1;

        // Fetch label dates projection asynchronously
        var labelService = Bootstrapper.Get<ILabelService>();
        var labelDates = await labelService.GetAsync(
            p => p.WarehouseOrderNo == plan.WarehouseOrderNo && p.LabelDate.HasValue,
            p => new { p.LabelDate }).ConfigureAwait(false);
        var labelChart = labelDates
            .GroupBy(p => p.LabelDate?.Date)
            .Select(g => new PlanViewLabelChartDto
            {
                LabelDate = g.Key,
                Count = g.Count()
            })
            .ToList();

        // Fetch carton dates projection asynchronously
        var cartonService = Bootstrapper.Get<ICartonService>();
        var cartonDates = await cartonService.GetAsync(
            p => p.WarehouseOrderNo == plan.WarehouseOrderNo && p.PackingDate.HasValue,
            p => new { p.PackingDate }).ConfigureAwait(false);
        var cartonChart = cartonDates
            .GroupBy(p => p.PackingDate?.Date)
            .Select(g => new CartonViewChartDto
            {
                PackingDate = g.Key,
                Count = g.Count()
            })
            .ToList();

        dto.PlanViewChartDtos = labelChart;
        dto.CartonViewChartDtos = cartonChart;

        return dto;
    }

    public async Task<PositionDetailDto> FillLabelInformationAsync(PositionDetailDto positionView)
    {
        var labelService = Bootstrapper.Get<ILabelService>();
        var labels = await labelService.GetAsync(p => p.WarehouseOrderNo == positionView.ProductionOrderNo &&
                                                      p.Position == positionView.Position,
            p => new
            {
                p.Id,
                p.AssemblyCode,
                p.CarcassCode,
                p.LabelDate,
                p.Barcode,
                p.Status

            }).ConfigureAwait(false);

        var firstLabel = labels.FirstOrDefault();

        positionView.AssemblyCode = firstLabel?.AssemblyCode;
        positionView.CarcassCode = firstLabel?.CarcassCode;

        var cartonService = Bootstrapper.Get<ICartonService>();
        var cartons = await cartonService.GetAsync(p => p.WarehouseOrderNo == positionView.ProductionOrderNo &&
                                                        p.CartonDetails.Any(d => d.Position == positionView.Position),
            p => new { p.Id, p.PackingDate, p.CartonNo, p.CartonBarcode, p.Status }).ConfigureAwait(false);

        var positionLabels = labels.Select(x =>
            new PositionDetailDto.PositionLabelDto
            {
                Id = x.Id,
                LabelDate = x.LabelDate,
                Barcode = x.Barcode,
                Status = x.Status,
            }).ToList();
        var positionCartons = cartons.Select(x => new PositionDetailDto.PositionLabelDto
        {
            CartonNo = x.CartonNo,
        }).ToList();
        if (positionView.PositionLabelDtos == null)
        {
            positionView.PositionLabelDtos = new List<PositionDetailDto.PositionLabelDto>();
        }

        positionLabels.AddRange(positionCartons); // Merging labels and cartons
        positionView.PositionLabelDtos.AddRange(positionLabels); // Adding to final list

        return positionView;
    }

    public async Task FillCartonInformationAsync(PositionDetailDto positionView)
    {
        var cartonService = Bootstrapper.Get<ICartonService>();
        var cartons = await cartonService.GetAsync(p => p.WarehouseOrderNo == positionView.ProductionOrderNo &&
                                                        p.CartonDetails.Any(d => d.Position == positionView.Position),
            p => new { p.Id, p.PackingDate, p.CartonNo, p.CartonBarcode, p.Status }).ConfigureAwait(false);

        var positionLabels = cartons.Select(x =>
            new PositionDetailDto.PositionCartonDto
            {
                Id = x.Id,
                PackingDate = x.PackingDate,
                CartonBarcode = x.CartonBarcode,
                CartonNo = x.CartonNo,
                Status = x.Status
            }).ToList();
        positionView.PositionCartonDtos.AddRange(positionLabels);
    }

    #endregion


    #region -- Data Retrieval Methods --

    public async Task<DataSourceResult> GetIndexViewDtoAsync(DataSourceRequest request)
    {
        // Step 1: Get paged Plans (only IDs and basic fields) - no includes needed for pagination
        var baseQuery = GetQuery();
        var pagedPlans = await baseQuery.ToDataSourceResultAsync(request).ConfigureAwait(false);

        // Step 2: Get aggregates for those IDs
        // Optimized: The Sum() aggregations in the projection will be translated to SQL GROUP BY
        // EF will generate efficient SQL even without explicit includes when using projections
        var planIds = (pagedPlans.Data as List<Plan>)?.Select(x => x.Id).ToList();
        if (planIds == null || !planIds.Any())
        {
            pagedPlans.Data = new List<PlanIndexDto>();
            return pagedPlans;
        }

        // Enable includes for PlanItemDetails since we need them for aggregation
        // However, with projection, EF should optimize this to use SQL GROUP BY
        var result = await GetAsync(p => planIds.Contains(p.Id), p => new PlanIndexDto
        {
            Id = p.Id,
            PlanDate = p.PlanDate,
            ProductionOrderNo = p.ProductionOrderNo,
            OrderQuantity = p.PlanItemDetails.Sum(d => d.OrderQuantity ?? 0),
            PrintQuantity = p.PlanItemDetails.Sum(d => d.PrintQuantity ?? 0),
            PackedQuantity = p.PlanItemDetails.Sum(d => d.PackQuantity ?? 0)
        }).ConfigureAwait(false);

        pagedPlans.Data = result;
        return pagedPlans;
    }

    public async Task<DataSourceResult> GetLabelsForPlanAsync(DataSourceRequest request, string productionOrderNo)
    {
        if (string.IsNullOrEmpty(productionOrderNo))
            return new DataSourceResult { Errors = new Dictionary<string, object>() };

        var labelService = Bootstrapper.Get<ILabelService>();
        var query = labelService.GetQuery().Where(c => c.ProductionOrderNo == productionOrderNo);
        var data = query.ProjectToType<LabelIndexDto>();
        return await data.ToDataSourceResultAsync(request).ConfigureAwait(false);
    }

    public async Task<DataSourceResult> GetCartonsForPlanAsync(DataSourceRequest request, string productionOrderNo)
    {
        if (string.IsNullOrEmpty(productionOrderNo))
            return new DataSourceResult { Errors = new Dictionary<string, object>() };

        var cartonService = Bootstrapper.Get<ICartonService>();
        var query = await cartonService.GetAsync(c => c.ProductionOrderNo == productionOrderNo, p => p,
            null, true).ConfigureAwait(false);

        var data = from carton in query
                   select new CartonIndexDto
                   {
                       Id = carton.Id,
                       PackingDate = carton.PackingDate,
                       ProductionOrderNo = carton.ProductionOrderNo,
                       CartonNo = carton.CartonNo.ToString(),
                       CartonBarcode = carton.CartonBarcode,
                       Status = carton.Status
                   };

        var result = await data.ToDataSourceResultAsync(request).ConfigureAwait(false);
        return result;
    }
    #endregion

    #endregion
}




















