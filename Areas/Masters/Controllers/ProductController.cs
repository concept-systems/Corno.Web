using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Masters.Dtos.Product;
using Corno.Web.Areas.Nilkamal.Services.Interfaces;
using Corno.Web.Extensions;
using Corno.Web.Globals;
using Corno.Web.Logger;
using Corno.Web.Models.Masters;
using Corno.Web.Services.Import;
using Corno.Web.Services.Import.Interfaces;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;
using Telerik.Reporting;

namespace Corno.Web.Areas.Masters.Controllers;

public class ProductController : BaseImportController<BomImportDto>
{
    #region -- Constructors --
    public ProductController(IProductService productService,
        IMiscMasterService miscMasterService, IBaseItemService itemService,
        ICustomerService customerService, IFileImportService<BomImportDto> importService,
        IPartLabelService partLabelService)
    : base(importService)
    {
        _productService = productService;
        _miscMasterService = miscMasterService;
        _itemService = itemService;
        _customerService = customerService;
        _partLabelService = partLabelService;

        const string viewPath = "~/Areas/Masters/views/Product/";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";

        // Configure Mapster mappings for ProductDto <-> Product
        ConfigureMapsterMappings();
    }
    #endregion

    #region -- Private Methods --
    private static void ConfigureMapsterMappings()
    {
        // ProductDto -> Product mapping
        TypeAdapterConfig<ProductDto, Product>
            .NewConfig()
            .Map(dest => dest.ProductPacketDetails, src => src.ProductPacketDtos.Adapt<List<ProductPacketDetail>>())
            .AfterMapping((src, dest) =>
            {
                dest.Id = src.Id;

                for (var index = 0; index < src.ProductPacketDtos.Count && index < dest.ProductPacketDetails.Count; index++)
                {
                    dest.ProductPacketDetails[index].Id = src.ProductPacketDtos[index].Id;

                    // Save MRP to ExtraProperties
                    var packetDto = src.ProductPacketDtos[index];
                    var packetDetail = dest.ProductPacketDetails[index];

                    if (packetDto.Mrp.HasValue)
                        packetDetail.ExtraProperties[FieldConstants.Mrp] = packetDto.Mrp.Value;
                    else
                        packetDetail.ExtraProperties.Remove(FieldConstants.Mrp);
                }
            });

        // Product -> ProductDto mapping
        TypeAdapterConfig<Product, ProductDto>
            .NewConfig()
            .Map(dest => dest.ProductItemDtos, src => src.ProductItemDetails.Adapt<List<ProductItemDto>>())
            .Map(dest => dest.ProductPacketDtos, src => src.ProductPacketDetails.Adapt<List<ProductPacketDto>>())
            .Map(dest => dest.ProductStockDtos, src => src.ProductStockDetails.Adapt<List<ProductStockDto>>())
            .AfterMapping((src, dest) =>
            {
                dest.Id = src.Id;

                for (var index = 0; index < src.ProductPacketDetails.Count && index < dest.ProductPacketDtos.Count; index++)
                {
                    dest.ProductPacketDtos[index].Id = src.ProductPacketDetails[index].Id;

                    // Load MRP from ExtraProperties
                    var packetDetail = src.ProductPacketDetails[index];
                    var packetDto = dest.ProductPacketDtos[index];

                    if (!packetDetail.ExtraProperties.ContainsKey(FieldConstants.Mrp)) continue;
                    var mrpValue = packetDetail.ExtraProperties.GetOrDefault(FieldConstants.Mrp);
                    if (mrpValue != null && double.TryParse(mrpValue.ToString(), out var mrp))
                        packetDto.Mrp = mrp;
                }

                for (var index = 0; index < src.ProductItemDetails.Count && index < dest.ProductItemDtos.Count; index++)
                {
                    dest.ProductItemDtos[index].Id = src.ProductItemDetails[index].Id;

                    // Load MRP from ExtraProperties
                    var itemDetail = src.ProductItemDetails[index];
                    var packetDto = dest.ProductItemDtos[index];

                    if (!itemDetail.ExtraProperties.ContainsKey(FieldConstants.Mrp)) continue;
                    var mrpValue = itemDetail.ExtraProperties.GetOrDefault(FieldConstants.Mrp);
                    if (mrpValue != null && double.TryParse(mrpValue.ToString(), out var mrp)) ;
                    //packetDto.Mrp = mrp;
                }

                for (var index = 0; index < src.ProductStockDetails.Count && index < dest.ProductStockDtos.Count; index++)
                {
                    dest.ProductStockDtos[index].Id = src.ProductStockDetails[index].Id;

                    // Load MRP from ExtraProperties
                    var stockDetail = src.ProductStockDetails[index];
                    var packetDto = dest.ProductStockDtos[index];

                    if (!stockDetail.ExtraProperties.ContainsKey(FieldConstants.Mrp)) continue;
                    var mrpValue = stockDetail.ExtraProperties.GetOrDefault(FieldConstants.Mrp);
                    if (mrpValue != null && double.TryParse(mrpValue.ToString(), out var mrp)) ;
                        //packetDto.Mrp = mrp;
                }
            });
    }
    #endregion

    #region -- Data Mambers --
    private readonly IProductService _productService;
    private readonly IMiscMasterService _miscMasterService;
    private readonly IBaseItemService _itemService;
    private readonly ICustomerService _customerService;
    private readonly IPartLabelService _partLabelService;

    private readonly string _indexPath;
    private readonly string _createPath;
    private readonly string _editPath;
    #endregion

    #region -- BaseImportController Implementation --
    protected override string ControllerName => "Product";
    #endregion

    #region -- Actions --

    public ActionResult Index()
    {
        return View(_indexPath, new List<ProductDto>());
    }

    // GET: /Location/Create
    public ActionResult Create()
    {
        return View(_createPath, new ProductDto());
    }

    //POST: /Product/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(ProductDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);

        try
        {
            // Check whether Name Already Exists
            if (await _productService.ExistsAsync(dto.Code).ConfigureAwait(false))
                throw new Exception($"Product with code {dto.Code} already exists.");

            //var product = dto.CreateModel();
            var product = dto.Adapt<Product>();
            product.Status = StatusConstants.Active;
            await _productService.AddAndSaveAsync(product).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_createPath, dto);
    }

    public async Task<ActionResult> Edit(int? id)
    {
        var product = await _productService.GetByIdAsync(id).ConfigureAwait(false);

        var itemIds = product.ProductItemDetails.Select(d => d.ItemId).ToList();
        var items = await _itemService.GetViewModelListAsync(p => itemIds.Contains(p.Id)).ConfigureAwait(false);
        var packingTypeIds = product.ProductPacketDetails.Select(d => d.PackingTypeId).ToList();
        var packingTypes = await _miscMasterService.GetViewModelListAsync(p => packingTypeIds.Contains(p.Id)).ConfigureAwait(false);
        var customerIds = product.ProductStockDetails.Select(d => d.CustomerId).ToList();
        var customers =  await _customerService.GetViewModelListAsync(p => customerIds.Contains(p.Id)).ConfigureAwait(false);

        var productDto = product.Adapt<ProductDto>();
        productDto.ProductItemDtos.ForEach(d => d.ItemName = items.FirstOrDefault(x => x.Id == d.ItemId)?.NameWithCode);
        productDto.ProductPacketDtos.ForEach(d => d.PackingTypeName = packingTypes.FirstOrDefault(x => x.Id == d.PackingTypeId)?.NameWithCode);
        productDto.ProductStockDtos.ForEach(d => d.CustomerName = customers.FirstOrDefault(x => x.Id == d.CustomerId)?.NameWithCode);
        return View(_editPath, productDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(ProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(_editPath, dto);
        }
        try
        {
            var product = await _productService.GetByIdAsync(dto.Id).ConfigureAwait(false);
            if (product == null) 
                throw new Exception("Product not found.");

            dto.Adapt(product);
            product.Status = StatusConstants.Active;
            await _productService.UpdateAndSaveAsync(product).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_editPath, dto);
    }

    public async Task<ActionResult> Delete(int? id)
    {
        var product = await _productService.GetByIdAsync(id ?? 0).ConfigureAwait(false);

        var productViewModel = new ProductDto
        {
            Code = product.Code,
            Name = product.Name,
            Description = product.Description
        };

        return View(productViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(ProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }
        try
        {
            var product = await _productService.GetByIdAsync(dto.Id).ConfigureAwait(false);
            product.Status = StatusConstants.Deleted;
            await _productService.UpdateAndSaveAsync(product).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(dto);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
        try
        {
            /* var location = _locationService.GetById(id);
             location.Status = StatusConstants.Deleted;
             _locationService.UpdateAndSave(location);*/

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> PrintBom(int productId, int packingTypeId, int itemId, int quantity)
    {
        try
        {
            var reportBook = await _partLabelService.GenerateBomLabels(productId,packingTypeId,itemId, quantity);

            // Further processing with the product and quantity
            return File(reportBook.ToDocumentBytes(), "application/pdf");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            //ModelState.AddModelError("Error",  exception.Message);
            ModelState.AddModelError("Error", LogHandler.GetDetailException(exception).Message);
            return Json(new DataSourceResult { Errors = ModelState });
        }

        // Save or process the product quantity
        // Example: save to database
        //return Json(new { success = true, message = "Quantity processed successfully" });
    }

    public async Task<ActionResult> GetIndexViewModels([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = (IEnumerable<Product>)_productService.GetQuery();
        var data = query.Select(p => new ProductIndexDto
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            Status = p.Status
        });
        var result = await data.ToDataSourceResultAsync(request).ConfigureAwait(false);
        return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            ModelState.AddModelError("Error", exception.Message);
            return Json(new DataSourceResult { Errors = ModelState });
        }
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public async Task<ActionResult> Inline_Create_Update_Destroy_Item([DataSourceRequest] DataSourceRequest request, ProductItemDto model)
    {
        var item = await _itemService.GetViewModelAsync(model.ItemId).ConfigureAwait(false);
        model.ItemName = item?.Name;

        return Json(new[] { model }.ToDataSourceResultAsync(request, ModelState));
    }
    
    [AcceptVerbs(HttpVerbs.Post)]
    public async Task<ActionResult> Inline_Create_Update_Destroy_Packet([DataSourceRequest] DataSourceRequest request, ProductPacketDetail dto)
    {
        var packingType  = await _miscMasterService.GetViewModelAsync(dto.PackingTypeId).ConfigureAwait(false);
        dto.PackingTypeName = packingType?.Name;

        return Json(new[] { dto }.ToDataSourceResultAsync(request, ModelState));
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public async Task<ActionResult> Inline_Create_Update_Destroy_Stock([DataSourceRequest] DataSourceRequest request, ProductStockDto dto)
    {
        return Json(new[] { dto }.ToDataSourceResult(request, ModelState));
    }

    #endregion
}