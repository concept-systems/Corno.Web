using Corno.Web.Areas.Kitchen.Dto.Carton;
using Corno.Web.Areas.Kitchen.Labels;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Dtos;
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

namespace Corno.Web.Areas.Kitchen.Services;

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

    private async Task<int> GetNextCartonNoForWarehouseOrderNoAsync(string warehouseOrderNo)
    {
        var max = await MaxAsync(c => c.WarehouseOrderNo == warehouseOrderNo,
            c => c.CartonNo).ConfigureAwait(false);
        return max + 1;
    }

    /*protected Label GetLabel(string warehouseOrderNo, string barcode)
    {
        // Get label by barcode
        var oldStatus = new[] { StatusConstants.Sorted, StatusConstants.SubAssembled };
        /*var labels = _labelService.Get(p => p.Barcode == barcode &&
                                           oldStatus.Contains(p.Status), p => p)
            .ToList();#1#
        var labels = _labelService.Get(p => p.Barcode == barcode, p => p)
            .ToList();
        if (labels.Count <= 0)
            throw new Exception($"barcode '{barcode}' not available in system.");

        // Remove all packed labels
        labels.RemoveAll(p => p.Status == StatusConstants.Packed);
        if (labels.Count <= 0)
            throw new Exception($"All labels for barcode value '{barcode}' are packed.");

        var label = labels
            .OrderByDescending(p => p.Id)
            .FirstOrDefault();

        if (null == label)
            throw new Exception($"barcode '{barcode}' not available for packing.");

        if (!oldStatus.Contains(label.Status))
            throw new Exception($"Expected status: '{string.Join(", ", oldStatus)}', but current status is '{label.Status}'. Label warehouse order : '{label.WarehouseOrderNo}'");

        return label;
    }*/

    protected async Task<Label> GetLabelAsync(string barcode, Plan plan)
    {
        // Expected statuses
        var oldStatus = new[] { StatusConstants.Sorted, StatusConstants.SubAssembled };

        // Fetch labels asynchronously
        var labels = await _labelService.GetAsync(p => p.Barcode == barcode, p => p,
            p => p.OrderByDescending(x => x.Id), true).ConfigureAwait(false);
        //var label1 = await _labelService.FirstOrDefaultAsync(p => p.Barcode == barcode, p => p);
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
            var planService = Bootstrapper.Get<IPlanService>();
            plan = await planService.GetByWarehouseOrderNoAsync(label.WarehouseOrderNo).ConfigureAwait(false);
        }

        var planItemDetail = plan?.PlanItemDetails
            .FirstOrDefault(p => p.Position == label.Position);
        var boStatus = new[] { StatusConstants.Active, StatusConstants.Printed };
        if (boStatus.Contains(label.Status) && planItemDetail?.ItemType == FieldConstants.Bo)
            return label;

        throw new Exception($"Expected status: '{string.Join(", ", oldStatus)}', but current status is '{label.Status}'. Item Type : {planItemDetail?.ItemType}. Label warehouse order: '{label.WarehouseOrderNo}'");

    }

    protected async Task<List<Label>> GetSubAssemblyLabelsAsync(string warehouseOrderNo, string assemblyCode)
    {
        var oldStatus = new List<string> { StatusConstants.Sorted, StatusConstants.SubAssembled };
        var labels = await _labelService.GetAsync(b =>
                    b.WarehouseOrderNo == warehouseOrderNo &&
                    b.AssemblyCode == assemblyCode &&
                    b.LabelType != FieldConstants.SubAssembly &&
                    oldStatus.Contains(b.Status), l => l,
                    q => q.OrderBy(x => x.LabelDate), true).ConfigureAwait(false);
        var list = labels.DistinctBy(l => l.Position).ToList();

        if (list.Count <= 1)
            throw new Exception($"No other sub-assembly labels found for warehouse order '{warehouseOrderNo}' and assembly code '{assemblyCode}'.");
        return list;
    }

    protected async Task ValidateLabelForAlreadyPacked(string warehouseOrderNo, string barcode, string position)
    {
        // Check if barcode is already present in any invoice detail
        var cartonId = await FirstOrDefaultAsync(p => p.WarehouseOrderNo == warehouseOrderNo &&
                                           p.CartonDetails.Any(d => d.Barcode == barcode && d.Position == position),
            p => p.Id).ConfigureAwait(false);
        if (cartonId > 0)
            throw new Exception($"barcode '{barcode}' is already scanned in another carton.");
    }


    protected async Task<ReportBook> CreateLabelReport(Carton carton, Plan plan, bool bDuplicate)
    {
        var contentLabelRpt = new CartonLabelRpt(carton, plan, bDuplicate);

        var warehouse = await _miscMasterService.FirstOrDefaultAsync(p => p.Id == plan.WarehouseId, p => p).ConfigureAwait(false);
        var branchLabelReport = new BranchLabelRpt(carton, plan, warehouse.Adapt<MasterDto>(), bDuplicate);

        var reportBook = new ReportBook();
        reportBook.ReportSources.Add(new InstanceReportSource { ReportDocument = contentLabelRpt });
        for (var index = 0; index < 4; index++)
            reportBook.ReportSources.Add(new InstanceReportSource { ReportDocument = branchLabelReport });

        return reportBook;
    }

    protected string GetCartonNo(int? cartonNo)
    {
        return $"C{cartonNo.ToString().PadLeft(3, '0')}";
    }

    protected async Task<int> GetNextCartonNoAsync(string warehouseOrderNo)
    {
        var maxCartonNo = await GetNextCartonNoForWarehouseOrderNoAsync(warehouseOrderNo).ConfigureAwait(false);
        var distinctCartonNos = (await GetAsync(c =>
                c.WarehouseOrderNo == warehouseOrderNo, c => c.CartonNo ?? 0).ConfigureAwait(false))
            .ToList();

        return Enumerable.Range(1, maxCartonNo)
            .Except(distinctCartonNos)
            .Min();
    }

    protected string GetCartonBarcode(string warehouseOrderNo, int cartonNo)
    {
        return $"WP{warehouseOrderNo}" +
               $"{cartonNo.ToString().PadLeft(3, '0')}";
    }

    #endregion

    #region -- Public Methods --
    public DataSourceResult GetIndexDataSource(DataSourceRequest request)
    {

        TypeAdapterConfig<Carton, CartonIndexDto>
            .NewConfig()
            .AfterMapping((src, dest) =>
            {
                dest.CartonNo = src.GetCartonNoString();
            });

        var result = GetQuery().ToDataSourceResult(request);

        // 4. Adapt paged data to DTOs with AfterMapping
        var data = ((List<Carton>)result.Data)
            .Select(x => x.Adapt<CartonIndexDto>())
            .ToList();

        result.Data = data;

        return result;

        /*return GetQuery().ProjectToType<CartonIndexDto>()
            .ToDataSourceResult(request); // Paging in SQL*/
    }

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
    #endregion
}