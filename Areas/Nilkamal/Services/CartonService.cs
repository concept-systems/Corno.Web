using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Areas.Admin.Models;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Areas.Nilkamal.Dto.PacketLabel;
using Corno.Web.Areas.Nilkamal.Labels;
using Corno.Web.Areas.Nilkamal.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Packing;
using Corno.Web.Models.Plan;
using Corno.Web.Reports;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Services.Packing;
using Corno.Web.Windsor;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;

namespace Corno.Web.Areas.Nilkamal.Services;

public class CartonService : BaseCartonService, ICartonService
{
    #region -- Constructors --
    public CartonService(IGenericRepository<Carton> genericRepository,
        IPartLabelService labelService, IMiscMasterService miscMasterService, IPlanService planService, IProductService productService, IUserService userService) : base(genericRepository)
    {
        _labelService = labelService;
        _miscMasterService = miscMasterService;
        _planService = planService;
        _productService = productService;
        _userService = userService;

        TypeAdapterConfig<Carton, PacketIndexDto>
            .NewConfig()
            .AfterMapping((src, dest) =>
            {
                //dest.CartonNo = src.GetCartonNoString();
            });
    }
    #endregion

    #region -- Data Members --

    private readonly IPartLabelService _labelService;
    private readonly IMiscMasterService _miscMasterService;
    private readonly IPlanService _planService;
    private readonly IProductService _productService;
    private readonly IUserService _userService;

    #endregion

    #region  -- Protected Methods --

    protected bool ValidateDto(PacketLabelCrudDto dto)
    {
        if (string.IsNullOrEmpty(dto.ProductionOrderNo))
            throw new Exception("Invalid production Order.");

        if (dto.ProductId <= 0)
            throw new Exception("Invalid product.");

        if (dto.PackingTypeId <= 0)
            throw new Exception("Invalid packing type.");

        if (dto.Quantity <= 0)
            throw new Exception("Invalid quantity.");

        return true;
    }

    #endregion

    #region -- Public Methods --
    public async Task<BaseReport> CreateLabelsAsync(PacketLabelCrudDto dto, Plan plan, bool bSave = false)
    {
        ValidateDto(dto);

        var alreadyPrintedCount = await CountAsync(p =>
            p.ProductionOrderNo == dto.ProductionOrderNo &&
            p.PackingTypeId == dto.PackingTypeId);

        var remainingQty = plan.OrderQuantity - alreadyPrintedCount;
        if (dto.Quantity > remainingQty)
            throw new Exception($"You can print only '{remainingQty}' quantity");

        var packet = await _miscMasterService.FirstOrDefaultAsync(
            p => p.Id == dto.PackingTypeId, p => p).ConfigureAwait(false);

        var planItemDetails = plan.PlanItemDetails
            .Where(d => d.PackingTypeId == dto.PackingTypeId)
            .ToList();

        var productionOrderNo = dto.ProductionOrderNo;

        var cartons = new List<Carton>();
        var serialNo = await GetNextSerialNoAsync();
        const string newStatus = StatusConstants.Printed;

        for (var i = 0; i < dto.Quantity; i++, serialNo++)
        {
            var packetBarcode = $"{productionOrderNo},{packet.Code},{serialNo}";
            var carton = new Carton
            {
                SerialNo = serialNo,
                Code = packetBarcode,
                PackingDate = DateTime.Now,
                ProductionOrderNo = productionOrderNo,
                ProductId = dto.ProductId,
                PackingTypeId = dto.PackingTypeId,
                CartonBarcode = packetBarcode,
                Status = newStatus,
                CartonDetails = planItemDetails
                    .SelectMany(detail => Enumerable.Range(0, detail.BomQuantity.ToInt())
                        .Select(_ => new CartonDetail
                        {
                            ProductId = dto.ProductId,
                            PackingTypeId = dto.PackingTypeId,
                            Quantity = 1,
                            Status = newStatus
                        }))
                    .ToList()
            };
            cartons.Add(carton);
        }

        // TODO: 1. How to calculate the printed quantity in PlanItemDetail for packet
        // TODO: 2. How man carton details to be created for packet if BomQuantity > 1

        if (bSave)
            await AddRangeAndSaveAsync(cartons);

        return new PacketLabelRpt(cartons, false);
    }
    
    public async Task<BaseReport> CreateLabelReportAsync(List<Carton> cartons, bool bDuplicate)
    {
        var labelRpt = new PacketLabelRpt(cartons, bDuplicate);
        return await Task.FromResult<BaseReport>(labelRpt).ConfigureAwait(false);
    }

    public async Task<PacketViewDto> CreateViewDtoAsync(int? id)
    {
        var carton = await FirstOrDefaultAsync(p => p.Id == id, p => p).ConfigureAwait(false);
        if (carton == null)
            throw new Exception($"Carton with Id '{id}' not found.");

        var dto = await GetLabelViewDto(carton).ConfigureAwait(false);

        /*var report = await CreateLabelReportAsync(null, true).ConfigureAwait(false);
        dto.Base64 = Convert.ToBase64String(report.ToDocumentBytes());*/

        return dto;
    }

    public async Task<PacketViewDto> GetLabelViewDto(Carton carton)
    {
        try
        {
            // Perform all async operations first while DbContext is still alive
            var planService = Bootstrapper.Get<IPlanService>();
            var plan = await planService.GetByProductionOrderNoAsync(carton.ProductionOrderNo).ConfigureAwait(false);
            var planItemDetail = plan?.PlanItemDetails?.FirstOrDefault(x => x.ProductionOrderNo == carton.ProductionOrderNo);

            // Get users while DbContext is still alive
            var labelViewDetailDtos = carton.CartonDetails.Adapt<List<PacketViewDetailDto>>();
            var userIds = labelViewDetailDtos.Select(d => d.CreatedBy).Where(id => id != null).Distinct().ToList();
            var users = userIds.Any()
                ? await _userService.GetAsync(p => userIds.Contains(p.Id), p => p).ConfigureAwait(false)
                : new List<AspNetUser>();

            // Get carton information using label barcode and warehouseOrderNo
            // Optimized: Use ignoreInclude to avoid loading all CartonDetails
            // The .Any() will be translated to SQL EXISTS subquery which is efficient with proper indexes
            var cartonService = Bootstrapper.Get<ICartonService>();
            var cartons = await cartonService.GetAsync(
                c => c.ProductionOrderNo == carton.ProductionOrderNo ,
                     //c.CartonDetails.Any(d => d.Barcode == carton.CartonBarcode),
                c => c,
                ignoreInclude: true).ConfigureAwait(false);
            //carton = cartons.FirstOrDefault();

            // Perform the mapping using existing configuration
            var dto = carton.Adapt<PacketViewDto>();

            // Set additional properties directly after mapping
            dto.Family = planItemDetail?.Group;
            dto.Color = planItemDetail?.Reserved1;
            dto.DrawingNo = planItemDetail?.DrawingNo;
            dto.DueDate = plan?.DueDate;
            dto.ItemCode = planItemDetail?.ItemCode;
            dto.Description = planItemDetail?.Description;

            // Add status and quantities from PlanItemDetail
            dto.LabelStatus = carton.Status;
            dto.PrintQuantity = planItemDetail?.PrintQuantity;
            dto.BendQuantity = planItemDetail?.BendQuantity;
            dto.SortQuantity = planItemDetail?.SortQuantity;
            dto.PackQuantity = planItemDetail?.PackQuantity;

            // Set carton information
            if (carton != null)
            {
                dto.CartonNo = carton.CartonNo;
                dto.CartonBarcode = carton.CartonBarcode;
                var cartonDetail = carton.CartonDetails?.FirstOrDefault(d => d.Barcode == carton.CartonBarcode);
                dto.CartonQuantity = cartonDetail?.Quantity;
            }

            // Set label details with user names
            dto.PacketViewDetailDto = labelViewDetailDtos;
            dto.PacketViewDetailDto.ForEach(d =>
            {
                d.UserName = users.FirstOrDefault(x => x.Id == d.CreatedBy)?.UserName;
            });

            var report = await CreateLabelReportAsync(cartons, true).ConfigureAwait(false);
            dto.Base64 = Convert.ToBase64String(report.ToDocumentBytes());

            return dto;
        }
        catch (Exception exception)
        {
            LogHandler.LogInfo(exception);
            // Return a basic mapped DTO even if additional data loading fails
            var dto = carton.Adapt<PacketViewDto>();
            dto.LabelStatus = carton.Status;
            return dto;
        }
    }

    public DataSourceResult GetIndexDataSource(DataSourceRequest request)
    {

        TypeAdapterConfig<Carton, PacketIndexDto>
            .NewConfig()
            .AfterMapping((src, dest) =>
            {
                //dest.CartonNo = src.GetCartonNoString();
            });

        var result = GetQuery().ToDataSourceResult(request);

        // 4. Adapt paged data to DTOs with AfterMapping
        var data = ((List<Carton>)result.Data)
            .Select(x => x.Adapt<PacketIndexDto>())
            .ToList();

        result.Data = data;

        return result;

        /*return GetQuery().ProjectToType<CartonIndexDto>()
            .ToDataSourceResult(request); // Paging in SQL*/
    }

    public async Task<DataSourceResult> GetIndexDataSourceAsync(DataSourceRequest request)
    {
        TypeAdapterConfig<Carton, PacketIndexDto>
            .NewConfig()
            .AfterMapping((src, dest) => { /*dest.CartonNo = src.GetCartonNoString();*/ });

        var baseQuery = GetQuery();
        var result = await baseQuery.ToDataSourceResultAsync(request).ConfigureAwait(false);

        var data = ((List<Carton>)result.Data)
            .Select(x => x.Adapt<PacketIndexDto>())
            .ToList();

        result.Data = data;
        return result;
    }
    #endregion
}