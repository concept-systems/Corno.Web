using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Areas.Nilkamal.Dto;
using Corno.Web.Areas.Nilkamal.Dto.Plan;
using Corno.Web.Areas.Nilkamal.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Services.Plan;
using Corno.Web.Windsor;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;
using MoreLinq;
using IPlanService = Corno.Web.Areas.Nilkamal.Services.Interfaces.IPlanService;

namespace Corno.Web.Areas.Nilkamal.Services;

public class PlanService : BasePlanService, IPlanService
{
    #region -- Constructors --
    public PlanService(IGenericRepository<Plan> genericRepository,
        IBaseItemService itemService, IMiscMasterService miscMasterService, IProductService productService)
    : base(genericRepository)
    {
        _itemService = itemService;
        _miscMasterService = miscMasterService;
        _productService = productService;
    }

    #endregion

    #region -- Data Members --
    private readonly IBaseItemService _itemService;
    private readonly IMiscMasterService _miscMasterService;
    private readonly IProductService _productService;
    
    #endregion

    #region -- Private Methods --
    public void ConfigureMapping(MapContext context)
    {
        var userId = context.Parameters["UserId"] as string ?? "System";
        var isUpdate = context.Parameters.ContainsKey("IsUpdate") && (bool)context.Parameters["IsUpdate"];

        // Get pre-loaded dictionaries for batch lookups
        var warehouseDict = context.Parameters.ContainsKey("WarehouseDict")
            ? context.Parameters["WarehouseDict"] as Dictionary<string, int>
            : new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        var parentItemDict = context.Parameters.ContainsKey("ParentItemDict")
            ? context.Parameters["ParentItemDict"] as Dictionary<string, int>
            : new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        var baanItemDict = context.Parameters.ContainsKey("BaanItemDict")
            ? context.Parameters["BaanItemDict"] as Dictionary<string, int>
            : new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        TypeAdapterConfig<PlanImportDto, Plan>
            .NewConfig()
            .AfterMapping((src, dest) =>
            {
                dest.PlanDate = DateTime.Now;
                dest.DueDate = src.DueDate ?? DateTime.Now.AddDays(7);
                dest.SoNo = src.SoNo;
                dest.PlanQuantity = src.PlanQty;
                dest.Status = StatusConstants.InProcess;
                dest.ModifiedBy = userId;
                dest.ModifiedDate = DateTime.Now;

                // Use pre-loaded dictionary instead of database call
                if (!string.IsNullOrWhiteSpace(src.ProductionOrderNo) &&
                    warehouseDict != null &&
                    warehouseDict.TryGetValue(src.ProductionOrderNo.Trim(), out var warehouseId))
                {
                    dest.WarehouseId = warehouseId;
                }

                if (isUpdate) return;
                dest.CreatedBy = userId;
                dest.CreatedDate = DateTime.Now;
            });

        TypeAdapterConfig<PlanImportDto, PlanItemDetail>
            .NewConfig()
            .AfterMapping((src, dest) =>
            {
                dest.ProductionOrderNo = src.ProductionOrderNo;
                dest.PlanQuantity = src.PlanQty;
                dest.ItemCode = src.ProductCode;
                dest.Description = src.ProductDescription;

                dest.Status = StatusConstants.Active;

                // Use pre-loaded dictionaries instead of database calls
                if (!string.IsNullOrWhiteSpace(src.ProductCode) &&
                    baanItemDict != null &&
                    baanItemDict.TryGetValue(src.ProductCode.Trim(), out var baanProductId))
                {
                    dest.ProductId = baanProductId;
                }

                dest.ModifiedBy = userId;
                dest.ModifiedDate = DateTime.Now;

                if (isUpdate) return;

                dest.CreatedBy = userId;
                dest.CreatedDate = DateTime.Now;
            });
    }

    #endregion

    #region -- Public Methods --
    public async Task<IEnumerable> GetPendingProductsAsync(Plan plan)
    {
        var products = await _productService.GetAsync(null, p => new { p.Id, p.Code, p.Name }).ConfigureAwait(false);

        var pendingProducts = from planItemDetail in plan.PlanItemDetails
            join product in products
                on planItemDetail.ProductId equals product.Id
            select new
            {
                Id = product.Id,
                Code = product.Code,
                Name = product.Name,
            };

        return await Task.FromResult(pendingProducts.DistinctBy(p => p.Code)).ConfigureAwait(false);
    }

    public async Task<IEnumerable> GetPendingPacketsAsync(string productionOrderNo, int productId)
    {
        var plan = await GetByProductionOrderNoAsync(productionOrderNo).ConfigureAwait(false);
        if (null == plan)
            throw new Exception($"Plan not found for production order no '{productionOrderNo}'.");
        if (!plan.ProductId.Equals(productId))
            throw new Exception($"Product id '{productId}' does not match with plan product id '{plan.ProductId}' for production order no '{productionOrderNo}'.");

        var pendingPackingTypes = from planItemDetail in plan.PlanItemDetails
            join packingType in await _miscMasterService.GetViewModelListAsync().ConfigureAwait(false)
                on planItemDetail.PackingTypeId equals packingType.Id into defaultPackingType
            from packet in defaultPackingType.DefaultIfEmpty()
            select packet;

        return await Task.FromResult(pendingPackingTypes.DistinctBy(p => p.Code)).ConfigureAwait(false);
    }

    public async Task IncreasePlanQuantityAsync(Plan plan, PlanItemDetail planItemDetail, double quantity, string newStatus)
    {
        var planDetail = plan?.PlanItemDetails.FirstOrDefault(d =>
            d.Position == planItemDetail.Position);
        if (null == planDetail)
            throw new Exception($"No plan detail available production order no {plan?.ProductionOrderNo} & " +
                                $"Item Id {planItemDetail.ItemId} & Position {planItemDetail.Position}");

        await UpdateAsync(plan).ConfigureAwait(false);
    }

    public async Task<IEnumerable> GetLotNosAsync(DateTime dueDate)
    {
        var lotNos = await GetAsync(p => DbFunctions.TruncateTime(p.DueDate) == DbFunctions.TruncateTime(dueDate),
            p => new { p.LotNo }).ConfigureAwait(false);
        return lotNos.DistinctBy(p => p.LotNo);
    }

    public async Task UpdateQuantitiesAsync(Plan plan)
    {
        if (null == plan)
            throw new Exception("Invalid Plan");

        var labelService = Bootstrapper.Get<IPartLabelService>();
        var cartonService = Bootstrapper.Get<ICartonService>();

        // Load data
        var labels = await labelService.GetAsync(l => l.ProductionOrderNo == plan.ProductionOrderNo, l => l).ConfigureAwait(false);
        var cartons = await cartonService.GetAsync(c => c.WarehouseOrderNo == plan.WarehouseOrderNo, c => new { c.Id, c.CartonDetails }).ConfigureAwait(false);

        // Get packed barcodes for quick lookup (inline to work with anonymous types)
        var packedBarcodes = new HashSet<string>(
            cartons.SelectMany(c => c.CartonDetails.Select(cd => cd.Barcode))
                .Where(b => !string.IsNullOrEmpty(b)),
            StringComparer.OrdinalIgnoreCase
        );

        // Update label statuses based on packedBarcodes
        var modifiedLabels = UpdateLabelStatuses(labels, packedBarcodes);

        // Calculate quantities based on updated label statuses
        CalculateQuantities(plan, labels);

        if (modifiedLabels.Any())
        {
            var labelServiceTemp = Bootstrapper.Get<IPartLabelService>();
            await labelServiceTemp.UpdateRangeAndSaveAsync(modifiedLabels).ConfigureAwait(false);
        }

        // Save changes
        await UpdateAndSaveAsync(plan).ConfigureAwait(false);
    }

    // Import functionality is now handled by PlanBusinessImportService and FileImportService<PlanImportModel>
    // The old ImportAsync and ValidateImportData methods have been removed

    #region -- Private Helper Methods for UpdateQuantities --

    private HashSet<Label> UpdateLabelStatuses(List<Label> labels, HashSet<string> packedBarcodes)
    {
        var modifiedLabels = new HashSet<Label>();
        var currentTime = DateTime.Now;

        foreach (var label in labels)
        {
            if (string.IsNullOrEmpty(label.Barcode))
                continue;

            // Check if label has Packed status but is NOT in packedBarcodes - need to revert
            if (label.Status == StatusConstants.Packed && !packedBarcodes.Contains(label.Barcode))
            {
                RevertPackedStatus(label, currentTime);
                modifiedLabels.Add(label);
            }
            // Check if this label's barcode is in the packed barcodes set
            else if (packedBarcodes.Contains(label.Barcode))
            {
                // Only update if not already packed
                if (label.Status != StatusConstants.Packed)
                {
                    UpdateToPackedStatus(label, currentTime);
                    modifiedLabels.Add(label);
                }
                else
                {
                    // Check if Packed entry exists in LabelDetails, if not add it
                    var hasPackedDetail = label.LabelDetails?.Any(d => d.Status == StatusConstants.Packed) ?? false;
                    if (!hasPackedDetail)
                    {
                        UpdateToPackedStatus(label, currentTime);
                        modifiedLabels.Add(label);
                    }
                }
            }
        }

        return modifiedLabels;
    }

    private void RevertPackedStatus(Label label, DateTime currentTime)
    {
        // Find the previous status from LabelDetails (excluding Packed entries)
        var previousDetail = label.LabelDetails?
            .Where(d => d.Status != StatusConstants.Packed)
            .OrderByDescending(d => d.ScanDate)
            .FirstOrDefault();

        // Revert to previous status or default to Sorted
        var newStatus = previousDetail?.Status ?? StatusConstants.Sorted;
        label.Status = newStatus;
        label.ModifiedDate = currentTime;

        // Delete all Packed entries from LabelDetails
        if (label.LabelDetails != null)
        {
            label.LabelDetails.RemoveAll(d => d.Status == StatusConstants.Packed);
        }
    }

    private void UpdateToPackedStatus(Label label, DateTime currentTime)
    {
        label.Status = StatusConstants.Packed;
        label.ModifiedDate = currentTime;

        // Check if Packed entry already exists in LabelDetails
        var hasPackedDetail = label.LabelDetails?.Any(d => d.Status == StatusConstants.Packed) ?? false;
        if (!hasPackedDetail)
        {
            // Add Packed entry in LabelDetails
            var labelDetail = new LabelDetail
            {
                ScanDate = currentTime,
                Status = StatusConstants.Packed
            };
            labelDetail.UpdateCreated("System");
            labelDetail.UpdateModified("System");
            label.LabelDetails ??= [];
            label.LabelDetails.Add(labelDetail);
        }
    }

    private void CalculateQuantities(Plan plan, List<Label> labels)
    {
        // Group labels by position for efficient lookup
        var labelsByPosition = labels.GroupBy(l => l.Position)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

        foreach (var planItemDetail in plan.PlanItemDetails)
        {
            if (!labelsByPosition.TryGetValue(planItemDetail.Position, out var positionLabels))
            {
                ResetQuantities(planItemDetail);
                continue;
            }

            // Calculate quantities based on updated label statuses
            planItemDetail.BendQuantity = SumQuantitiesByDetailStatus(positionLabels, StatusConstants.Bent);
            planItemDetail.SortQuantity = SumQuantitiesByDetailStatus(positionLabels, StatusConstants.Sorted);
            planItemDetail.SubAssemblyQuantity = SumQuantitiesByDetailStatus(positionLabels, StatusConstants.SubAssembled);
            planItemDetail.PackQuantity = SumQuantitiesByDetailStatus(positionLabels, StatusConstants.Packed);
        }
    }

    private int SumQuantitiesByDetailStatus(List<Label> labels, string status)
    {
        return labels.Where(l => l.LabelDetails?.Any(d => d.Status == status) ?? false).Sum(l => l.Quantity ?? 0).ToInt();
    }

    private void ResetQuantities(PlanItemDetail planItemDetail)
    {
        planItemDetail.BendQuantity = 0;
        planItemDetail.SortQuantity = 0;
        planItemDetail.SubAssemblyQuantity = 0;
        planItemDetail.PackQuantity = 0;
    }

    #endregion

    #region -- View Model Methods --

    public async Task<Nilkamal.Dto.Plan.PlanViewDto> GetViewModelAsync(Plan plan)
    {
        TypeAdapterConfig<Plan, PlanViewDto>
            .NewConfig()
            .Map(dest => dest.OneLineItemCode, src => src.System)
            .Map(dest => dest.PlanViewItemDtos, src => src.PlanItemDetails);

        /* TypeAdapterConfig<PlanItemDetail, PlanViewItemDto>
             .NewConfig();*/

        TypeAdapterConfig<PlanItemDetail, PlanViewItemDto>
            .NewConfig()
            .Map(dest => dest.ProductId, src => src.ProductId)
            .Map(dest => dest.PackingTypeId, src => src.PackingTypeId)
            .Map(dest => dest.ItemId, src => src.ItemId);

        // Map Plan to DTO using Mapster
        var dto = plan.Adapt<PlanViewDto>();

        var productIds = dto.PlanViewItemDtos
            .Select(x => x.ProductId)
            .Distinct()
            .ToList();

        var packingTypeIds = dto.PlanViewItemDtos
            .Select(x => x.PackingTypeId)
            .Distinct()
            .ToList();

        var itemIds = dto.PlanViewItemDtos
            .Select(x => x.ItemId)
            .Distinct()
            .ToList();

        var products = await _productService.GetAsync(
            p => productIds.Contains(p.Id),
            p => new { p.Id, p.Code, p.Name }
        ).ConfigureAwait(false);

        var packingTypes = await _miscMasterService.GetAsync(
            p => packingTypeIds.Contains(p.Id),
            p => new { p.Id, p.Code, p.Name }
        ).ConfigureAwait(false);

        var items = await _itemService.GetAsync(
            p => itemIds.Contains(p.Id),
            p => new { p.Id, p.Code, p.Name }
        ).ConfigureAwait(false);

        // ------------------ ASSIGN DATA ID-WISE ------------------
        foreach (var row in dto.PlanViewItemDtos)
        {
            var product = products.FirstOrDefault(x => x.Id == row.ProductId);
            row.ProductCode = product?.Code;
            row.ProductName = product?.Name;

            var packing = packingTypes.FirstOrDefault(x => x.Id == row.PackingTypeId);
            row.PackingTypeCode = packing?.Code;
            row.PackingTypeName = packing?.Name;

            var item = items.FirstOrDefault(x => x.Id == row.ItemId);
            row.ItemCode = item?.Code;
            row.Description = item?.Name;
        }
        
        // Populate Production Name from MiscMaster (optimized - only select Name)
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
        var labelService = Bootstrapper.Get<IPartLabelService>();
        var labelDates = await labelService.GetAsync(
                p => p.ProductionOrderNo == plan.ProductionOrderNo && p.LabelDate.HasValue,
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
                p => p.ProductionOrderNo == plan.ProductionOrderNo && p.PackingDate.HasValue,
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
        var labelService = Bootstrapper.Get<IPartLabelService>();
        var labels = await labelService.GetAsync(p => p.ProductionOrderNo == positionView.WarehouseOrderNo &&
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
        var cartons = await cartonService.GetAsync(p => p.ProductionOrderNo == positionView.WarehouseOrderNo &&
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
        var cartons = await cartonService.GetAsync(p => p.ProductionOrderNo == positionView.WarehouseOrderNo &&
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

    #region -- Plan Operations --

    public async Task<Nilkamal.Dto.Plan.PlanViewDto> DeletePositionAsync(string warehouseOrderNo, string position)
    {
        if (null == position)
            throw new Exception("Position cannot be null.");

        var plan = await GetByWarehouseOrderNoAsync(warehouseOrderNo).ConfigureAwait(false);
        if (plan == null)
            throw new Exception("Warehouse Order with the specified Position not found.");

        plan.PlanItemDetails.RemoveAll(d =>
            d.Position == position);

        await UpdateAndSaveAsync(plan).ConfigureAwait(false);

        var viewModel = await GetViewModelAsync(plan).ConfigureAwait(false);

        return viewModel;
    }

    public async Task DeletePlanAsync(string productionOrderNo)
    {
        var labelService = Bootstrapper.Get<IPartLabelService>();
        var packedLabel = await labelService.FirstOrDefaultAsync(p => p.ProductionOrderNo == productionOrderNo &&
                                                               p.Status == StatusConstants.Packed, p => p).ConfigureAwait(false);
        if (null != packedLabel)
            throw new Exception("Cannot delete plan. There are labels with Packed status for this production order. Please unpack the labels first.");

        var deleteStatus = new List<string> { StatusConstants.Active, StatusConstants.Printed };
        var label = await labelService.FirstOrDefaultAsync(p => p.ProductionOrderNo == productionOrderNo &&
                                                          !deleteStatus.Contains(p.Status), p => p).ConfigureAwait(false);
        if (null != label)
            throw new Exception("There are labels which are scanned for operations.");

        var plan = await GetByProductionOrderNoAsync(productionOrderNo).ConfigureAwait(false);
        await DeleteAsync(plan).ConfigureAwait(false);

        var labels = await labelService.GetAsync(p => p.ProductionOrderNo == productionOrderNo, p => p).ConfigureAwait(false);
        await labelService.DeleteRangeAsync(labels).ConfigureAwait(false);

        await SaveAsync().ConfigureAwait(false);
    }

    public async Task UpdateDueDateAsync(string productionOrderNo, DateTime dueDate)
    {
        if (string.IsNullOrEmpty(productionOrderNo))
            throw new Exception("Warehouse Order No is required");

        var plan = await FirstOrDefaultAsync(p => p.WarehouseOrderNo == productionOrderNo,
            p => p).ConfigureAwait(false);
        if (null == plan)
            throw new Exception($"Plan not found for warehouse order {productionOrderNo}");

        plan.DueDate = dueDate;
        await UpdateAndSaveAsync(plan).ConfigureAwait(false);
    }

    public async Task UpdateLotNoAsync(string productionOrderNo, string lotNo)
    {
        if (string.IsNullOrEmpty(productionOrderNo))
            throw new Exception("Production Order No is required");

        if (string.IsNullOrEmpty(lotNo))
            throw new Exception("Lot No is required");

        var plan = await GetByWarehouseOrderNoAsync(productionOrderNo).ConfigureAwait(false);
        if (plan == null)
            throw new Exception($"Plan not found for production order {productionOrderNo}");

        plan.LotNo = lotNo;
        await UpdateAndSaveAsync(plan).ConfigureAwait(false);
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
        SetIncludes($"{nameof(Plan.PlanItemDetails)}");
        var result = await GetAsync(p => planIds.Contains(p.Id), p => new PlanIndexDto
        {
            Id = p.Id,
            ProductionOrderNo = p.ProductionOrderNo,
            PlanDate = p.PlanDate,
            DueDate = p.DueDate,
            OrderQuantity = p.OrderQuantity ?? 0,
            PrintQuantity = p.PlanItemDetails.Sum(d => d.PrintQuantity ?? 0),
            PackedQuantity = p.PlanItemDetails.Sum(d => d.PackQuantity ?? 0)
        }, ignoreInclude: false).ConfigureAwait(false);

        pagedPlans.Data = result;
        return pagedPlans;
    }

    public async Task<DataSourceResult> GetLabelsForPlanAsync(DataSourceRequest request, string productionOrderNo)
    {
        if (string.IsNullOrEmpty(productionOrderNo))
            return new DataSourceResult { Errors = new Dictionary<string, object>() };

        var labelService = Bootstrapper.Get<IPartLabelService>();
        var query = await labelService.GetAsync(c => c.ProductionOrderNo == productionOrderNo, p => p,
            null, true);

        var data = from label in query
                   select new LabelIndexDto
                   {
                       Id = label.Id,
                       LabelDate = label.LabelDate,
                       WarehouseOrderNo = label.WarehouseOrderNo,
                       CarcassCode = label.CarcassCode,
                       AssemblyCode = label.AssemblyCode,
                       Position = label.Position,
                       Barcode = label.Barcode,
                       LotNo = label.LotNo,
                       Family = label.Reserved1,
                       OrderQuantity = label.OrderQuantity,
                       Quantity = label.Quantity,
                       Status = label.Status
                   };

        var result = await data.ToDataSourceResultAsync(request).ConfigureAwait(false);
        return result;
    }

    public async Task<DataSourceResult> GetCartonsForPlanAsync(DataSourceRequest request, string warehouseOrderNo)
    {
        if (string.IsNullOrEmpty(warehouseOrderNo))
            return new DataSourceResult { Errors = new Dictionary<string, object>() };

        var cartonService = Bootstrapper.Get<ICartonService>();
        var query = await cartonService.GetAsync(c => c.WarehouseOrderNo == warehouseOrderNo, p => p,
            null, true).ConfigureAwait(false);

        var data = from carton in query
                   select new CartonIndexDto
                   {
                       Id = carton.Id,
                       PackingDate = carton.PackingDate,
                       SoNo = carton.SoNo,
                       WarehouseOrderNo = carton.WarehouseOrderNo,
                       CartonNo = carton.CartonNo.ToString(),
                       CartonBarcode = carton.CartonBarcode,
                       Status = carton.Status
                   };

        var result = await data.ToDataSourceResultAsync(request).ConfigureAwait(false);
        return result;
    }

    #endregion

    #region -- Pending Plan Methods --

    public async Task<IEnumerable> GetPendingLotNosAsync(DateTime dueDate)
    {
        var lotNos = await GetAsync<object>(p => DbFunctions.TruncateTime(p.DueDate) ==
                                          DbFunctions.TruncateTime(dueDate) &&
                                          p.PlanItemDetails.Any(d => d.ItemType.Equals(FieldConstants.Bo) &&
                                                                     (d.OrderQuantity ?? 0) > (d.PrintQuantity ?? 0)),
                p => new { p.LotNo }).ConfigureAwait(false);
        return lotNos.Distinct().ToList();
    }

    public async Task<IEnumerable> GetPendingWarehouseOrdersAsync(string lotNo)
    {
        var warehouseOrderNos = await GetAsync<object>(p => p.LotNo.Equals(lotNo) &&
                                           p.PlanItemDetails.Any(d => d.ItemType.Equals(FieldConstants.Bo) &&
                                                                      (d.OrderQuantity ?? 0) > (d.PrintQuantity ?? 0)),
                p => new { p.WarehouseOrderNo }).ConfigureAwait(false);
        return warehouseOrderNos.Distinct().ToList();
    }

    public async Task<IEnumerable> GetPendingFamiliesAsync(Plan plan, List<string> selectedGroups = null)
    {
        var query = plan.PlanItemDetails.Where(d => d.ItemType.Equals(FieldConstants.Bo) &&
                                                       (d.OrderQuantity ?? 0) > (d.PrintQuantity ?? 0));

        // Apply selectedGroups filter if provided (for trolley labels)
        if (selectedGroups != null && selectedGroups.Any())
        {
            query = query.Where(d => selectedGroups.Contains(d.Group));
        }

        var families = query
            .Select(p => new { Family = p.Group })
            .Distinct()
            .ToList();

        return await Task.FromResult(families).ConfigureAwait(false);
    }




    #endregion
}

#endregion
