using Corno.Web.Areas.Euro.Dto.Carton;
using Corno.Web.Areas.Euro.Services.Interfaces;
using Corno.Web.Dtos;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Packing;
using Corno.Web.Windsor;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Reporting;

namespace Corno.Web.Areas.Euro.Services;

public class CartonService : BaseCartonService, ICartonService
{
    #region -- Constructors --
    public CartonService(IGenericRepository<Carton> genericRepository,
        ILabelService labelService, IMiscMasterService miscMasterService) : base(genericRepository)
    {
        _labelService = labelService;
        _miscMasterService = miscMasterService;

        TypeAdapterConfig<Carton, CartonIndexDto>
            .NewConfig()
            .AfterMapping((src, dest) =>
            {
                dest.CartonNo = src.GetCartonNoString();
            });
    }
    #endregion

    #region -- Data Members --

    private readonly ILabelService _labelService;
    private readonly IMiscMasterService _miscMasterService;

    #endregion

    #region  -- Protected Methods --

    private async Task<int> GetNextCartonNoForProductionOrderNoAsync(string productionOrderNo)
    {
        var max = await MaxAsync(c => c.ProductionOrderNo == productionOrderNo,
            c => c.CartonNo).ConfigureAwait(false);
        return max + 1;
    }

    protected async Task<Label> GetLabelAsync(string barcode, Plan plan)
    {
        // Expected statuses
        var oldStatus = new[] { StatusConstants.Sorted, StatusConstants.SubAssembled };

        // Fetch labels asynchronously
        var labels = await _labelService.GetAsync(p => p.Barcode == barcode, p => p,
            p => p.OrderByDescending(x => x.Id), true).ConfigureAwait(false);
        var labelList = labels.ToList();

        if (labelList.Count <= 0)
            throw new Exception($"Barcode '{barcode}' not available in system.");

        // Remove packed labels
        labelList.RemoveAll(p => p.Status == StatusConstants.Packed);
        if (labelList.Count <= 0)
            throw new Exception($"All labels for barcode value '{barcode}' are packed.");

        // Get latest label by Id
        var label = labelList
            .OrderByDescending(p => p.Id)
            .FirstOrDefault();

        if (label == null)
            throw new Exception($"Barcode '{barcode}' not available for packing.");

        // Validate status
        if (oldStatus.Contains(label.Status)) return label;

        // Get product details by product id
        if (plan == null)
        {
            var planService = Bootstrapper.Get<Services.Interfaces.IPlanService>();
            plan = await planService.GetByProductionOrderNoAsync(label.ProductionOrderNo).ConfigureAwait(false);
        }

        var planItemDetail = plan?.PlanItemDetails
            .FirstOrDefault(p => p.Position == label.Position);
        var boStatus = new[] { StatusConstants.Active, StatusConstants.Printed };
        if (boStatus.Contains(label.Status) && planItemDetail?.ItemType == FieldConstants.Bo)
            return label;

        throw new Exception($"Expected status: '{string.Join(", ", oldStatus)}', but current status is '{label.Status}'. Item Type : {planItemDetail?.ItemType}. Label Production order: '{label.ProductionOrderNo}'");

    }

    protected async Task<List<Label>> GetSubAssemblyLabelsAsync(string productionOrderNo, string assemblyCode)
    {
        var oldStatus = new List<string> { StatusConstants.Sorted, StatusConstants.SubAssembled };
        var labels = await _labelService.GetAsync(b =>
                    b.ProductionOrderNo == productionOrderNo &&
                    b.AssemblyCode == assemblyCode &&
                    b.LabelType != FieldConstants.SubAssembly &&
                    oldStatus.Contains(b.Status), l => l,
                    q => q.OrderBy(x => x.LabelDate), true).ConfigureAwait(false);
        var list = labels.DistinctBy(l => l.Position).ToList();

        return list.Count <= 1 ? throw new Exception($"No other sub-assembly labels found for Production order '{productionOrderNo}' and assembly code '{assemblyCode}'.") : list;
    }

    protected async Task ValidateLabelForAlreadyPacked(string productionOrderNo, string barcode, string position)
    {
        // Check if barcode is already present in any invoice detail
        var cartonId = await FirstOrDefaultAsync(p => p.ProductionOrderNo == productionOrderNo &&
                                           p.CartonDetails.Any(d => d.Barcode == barcode && d.Position == position),
            p => p.Id).ConfigureAwait(false);
        if (cartonId > 0)
            throw new Exception($"barcode '{barcode}' is already scanned in another carton.");
    }


    protected async Task<ReportBook> CreateLabelReport(Carton carton, Plan plan, bool bDuplicate)
    {
        /*var contentLabelRpt = new CartonLabelRpt(carton, plan, bDuplicate);

        var warehouse = await _miscMasterService.FirstOrDefaultAsync(p => p.Id == plan.WarehouseId, p => p).ConfigureAwait(false);
        var branchLabelReport = new BranchLabelRpt(carton, plan, warehouse.Adapt<MasterDto>(), bDuplicate);*/

        var reportBook = new ReportBook();
        /*reportBook.ReportSources.Add(new InstanceReportSource { ReportDocument = contentLabelRpt });
        for (var index = 0; index < 4; index++)
            reportBook.ReportSources.Add(new InstanceReportSource { ReportDocument = branchLabelReport });*/

        return reportBook;
    }

    protected async Task<string> GetCartonNoAsync(int? cartonNo)
    {
        return await Task.FromResult($"C{cartonNo.ToString().PadLeft(3, '0')}").ConfigureAwait(false);
    }

    protected async Task<int> GetNextCartonNoAsync(string productionOrderNo)
    {
        var maxCartonNo = await GetNextCartonNoForProductionOrderNoAsync(productionOrderNo).ConfigureAwait(false);
        var distinctCartonNos = (await GetAsync(c =>
                c.ProductionOrderNo == productionOrderNo, c => c.CartonNo ?? 0).ConfigureAwait(false))
            .ToList();

        return Enumerable.Range(1, maxCartonNo)
            .Except(distinctCartonNos)
            .Min();
    }

    protected async Task<string> GetCartonBarcodeAsync(string productionOrderNo, int cartonNo)
    {
        return await Task.FromResult($"WP{productionOrderNo}{cartonNo.ToString().PadLeft(3, '0')}").ConfigureAwait(false);
    }

    #endregion

    #region -- Public Methods --

    public async Task<DataSourceResult> GetIndexDataSourceAsync(DataSourceRequest request)
    {
        TypeAdapterConfig<Carton, CartonIndexDto>
            .NewConfig()
            .AfterMapping((src, dest) => { dest.CartonNo = src.GetCartonNoString(); });

        var baseQuery = GetQuery();
        var result = await baseQuery.ToDataSourceResultAsync(request).ConfigureAwait(false);

        var data = ((List<Carton>)result.Data)
            .Select(x => x.Adapt<CartonIndexDto>())
            .ToList();

        result.Data = data;
        return result;
    }

    public async Task<CartonViewDto> ViewAsync(int? id)
    {
        var carton = await GetByIdAsync(id).ConfigureAwait(false);
        if (null == carton)
            throw new Exception($"Label with Id '{id}' not found.");

        var planService = Bootstrapper.Get<Services.Interfaces.IPlanService>();
        var plan = await planService.GetByProductionOrderNoAsync(carton.ProductionOrderNo).ConfigureAwait(false);

        var dto = new CartonViewDto
        {
            Id = carton.Id,
            ProductionOrderNo = carton.ProductionOrderNo,
            SoNo = carton.SoNo,
            LotNo = plan?.LotNo,
            DueDate = plan?.DueDate,
            OrderQuantity = plan?.OrderQuantity,
            PrintQuantity = plan?.PrintQuantity,

            CartonDetailsDtos = carton.CartonDetails.Select(d =>
            {
                var planItemDetail = plan?.PlanItemDetails.FirstOrDefault(x => x.Position == d.Position);
                return new CartonDetailsDto
                {
                    Id = d.Id,
                    Position = d.Position,
                    AssemblyCode = planItemDetail?.AssemblyCode,
                    CarcassCode = planItemDetail?.CarcassCode,
                    ItemCode = planItemDetail?.ItemCode,
                    Description = planItemDetail?.Description,
                    Quantity = d.Quantity,
                    OrderQuantity = d.OrderQuantity ?? 0,
                    NetWeight = d.NetWeight,
                    SystemWeight = d.SystemWeight,
                    Tolerance = d.Tolerance,
                    Barcode = d.Barcode
                };
            }).ToList(),

            CartonRackingDetailDtos = carton.CartonRackingDetails.Select(d => new CartonRackingDetailDto
            {
                ScanDate = d.ScanDate,
                PalletNo = d.PalletNo,
                RackNo = d.RackNo,
                Status = d.Status
            }).ToList(),
        };

        var reportBook = await CreateLabelReport(carton, plan, false).ConfigureAwait(false);
        // Convert report to Base64 for inline preview similar to PartLabel
        dto.Base64 = reportBook?.ToBase64();

        return dto;
    }
    #endregion
}

