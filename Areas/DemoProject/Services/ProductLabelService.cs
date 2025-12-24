using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corno.Web.Areas.DemoProject.Dtos;
using Corno.Web.Areas.DemoProject.Labels;
using Corno.Web.Areas.DemoProject.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Models.Masters;
using Corno.Web.Models.Packing;
using Corno.Web.Reports;
using Corno.Web.Repository.Interfaces;
using Corno.Web.Services;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Windsor;
using Volo.Abp.Data;

namespace Corno.Web.Areas.DemoProject.Services;

public sealed class ProductLabelService : BaseService<Label>, IProductLabelService
{
    #region -- Constructors --
    public ProductLabelService(IGenericRepository<Label> genericRepository, IProductService productService)
        : base(genericRepository)
    {
        _genericRepository = genericRepository;
        _productService = productService;

        SetIncludes(nameof(Label.LabelDetails));
    }
    #endregion

    #region -- Data Members --

    private readonly IGenericRepository<Label> _genericRepository;
    private readonly IProductService _productService;

    private const string NewStatus = StatusConstants.Printed;

    #endregion

    #region -- Protected Methods --

    private static void ValidateDto(LabelCrudDto dto)
    {
        if (dto.ProductId.ToInt() <= 0)
            throw new Exception("Invalid Product.");
        if (dto.PackingTypeId.ToInt() <= 0)
            throw new Exception("Invalid Packet.");
        if (dto.LabelFormatId.ToInt() <= 0)
            throw new Exception("Invalid Label Format.");
        if (null == dto.ExpiryDate)
            throw new Exception("Invalid Expiry Date.");
        if (null == dto.ManufacturingDate)
            throw new Exception("Invalid Manufacturing Date.");
        if (dto.Quantity.ToInt() <= 0)
            throw new Exception("Invalid Label Quantity.");
    }

    #endregion

    #region -- Public Methods --

    public async Task<List<Label>> CreateLabelsAsync(LabelCrudDto dto, string userId)
    {
        // 1. Validate Dto
        ValidateDto(dto);

        // 2. Get Product (async)
        var product = await _productService.FirstOrDefaultAsync(p => p.Id == dto.ProductId, p => p).ConfigureAwait(false);
        if (product == null)
            throw new Exception("Product not found.");

        var productPacketDetail = product.ProductPacketDetails.FirstOrDefault(d => 
            d.PackingTypeId == dto.PackingTypeId);
        if (null == productPacketDetail)
            throw new Exception("Packing type not available in system");
        var mrp = productPacketDetail.GetProperty(FieldConstants.Mrp, 0);
        if (mrp <= 0)
        {
            var miscMasterService = Bootstrapper.Get<IMiscMasterService>();
            var packingType = await miscMasterService.GetViewModelAsync(productPacketDetail.PackingTypeId);
            throw new Exception($@"No MRP defined for product '{product.Name}' and Unit '{packingType.Name}'");
        }

        // 3. Get Max SerialNo (async)
        var maxSerialNo = await _genericRepository.MaxAsync(c => c.CompanyId > 0, c => c.SerialNo).ConfigureAwait(false);
        var serialNo = maxSerialNo + 1;

        // 4. Create Labels
        var labels = new List<Label>();
        
        for (var index = 0; index < dto.Quantity; index++)
        {
            var label = new Label
            {
                SerialNo = serialNo,
                LabelDate = DateTime.Now,
                ProductId = product.Id,
                NetWeight = productPacketDetail.Quantity,

                PackingTypeId = dto.PackingTypeId,
                Barcode = $"{serialNo}",
                Quantity = 1,
                
                CreatedBy = userId,
                ModifiedBy = userId,

                Status = NewStatus,
            };

            label.LabelDetails.Add(new LabelDetail()
            {
                ScanDate = DateTime.Now,

                CreatedBy = userId,
                ModifiedBy = userId,

                Status = NewStatus,
            });

            label.SetProperty(FieldConstants.ManufacturingDate, dto.ManufacturingDate);
            label.SetProperty(FieldConstants.ExpiryDate, dto.ExpiryDate);
            label.SetProperty(FieldConstants.Mrp, mrp);
            
            labels.Add(label);

            serialNo++;
        }


        return labels;
    }

    public Task<BaseReport> CreateLabelReportAsync(List<Label> labels, Product product, int? labelFormatId, bool bDuplicate)
    {
        // Determine which report format to use based on LabelFormatId
        // 1 = Big Label, 2 = Medium Label, 3 = Small Label
        //var packingType = await _miscMasterService.GetViewModelAsync(labels.FirstOrDefault()?.PackingTypeId ?? 0);
        BaseReport labelRpt = labelFormatId switch
        {
            1 => new ProductLabelBigRpt(labels, product),
            _ => new ProductLabelSmallRpt(labels, product)
        };

        return Task.FromResult(labelRpt);
    }

    public async Task UpdateDatabaseAsync(List<Label> labels)
    {
        if (labels == null || !labels.Any())
            throw new Exception("Invalid labels");

        // Save labels to database (async)
        await AddRangeAsync(labels).ConfigureAwait(false);
        await SaveAsync().ConfigureAwait(false);
    }

    #endregion
}