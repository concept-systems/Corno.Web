using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Masters.Dtos.Product;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Corno.Web.Models.Masters;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;

namespace Corno.Web.Areas.DemoProject.Controllers;

public class ProductController : SuperController
{
    #region -- Constructors --
    public ProductController(IProductService productService,
        IMiscMasterService miscMasterService, IBaseItemService itemService)
    {
        _productService = productService;
        _miscMasterService = miscMasterService;
        _itemService = itemService;

        const string viewPath = "~/Areas/DemoProject/views/Product/";
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
            });
    }
    #endregion

    #region -- Data Mambers --
    private readonly IProductService _productService;
    private readonly IMiscMasterService _miscMasterService;
    private readonly IBaseItemService _itemService;

    private readonly string _indexPath;
    private readonly string _createPath;
    private readonly string _editPath;
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
            // Check whether Code Already Exists
            var existingProduct = await _productService.FirstOrDefaultAsync(p => p.Code == dto.Code, p => p);
            if (existingProduct != null)
                throw new Exception($"Product with code {dto.Code} already exists.");

            // Map DTO to Entity (MRP handling is done in AfterMapping)
            var product = dto.Adapt<Product>();
            
            await _productService.AddAndSaveAsync(product);

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
        try
        {
            var product = await _productService.FirstOrDefaultAsync(p => p.Id == id, p => p);
            if (product == null)
                return HttpNotFound();

            // Map Entity to DTO (MRP loading is done in AfterMapping)
            var productDto = product.Adapt<ProductDto>();

            // Load names asynchronously in parallel
            await LoadRelatedNamesAsync(productDto).ConfigureAwait(false);

            return View(_editPath, productDto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            throw;
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(ProductDto dto)
    {
        if (!ModelState.IsValid)
            return View(_editPath, dto);
        
        try
        {
            var product = await _productService.FirstOrDefaultAsync(p => p.Id == dto.Id, p => p);
            if (product == null) 
                throw new Exception("Product not found.");

            product.ProductPacketDetails.Clear();

            // Map DTO to Entity (MRP handling is done in AfterMapping)
            dto.Adapt(product);
            
            await _productService.UpdateAndSaveAsync(product);

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
        var product = await _productService.FirstOrDefaultAsync(p => p.Id == (id ?? 0), p => p);

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
            var product = await _productService.FirstOrDefaultAsync(p => p.Id == dto.Id, p => p);
            product.Status = StatusConstants.Deleted;
            await _productService.UpdateAndSaveAsync(product);

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
            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }

    public async Task<ActionResult> GetIndexViewModels([DataSourceRequest] DataSourceRequest request)
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

    public async Task<ActionResult> GetProductPacketDetails(int productId)
    {
        try
        {
            var product = await _productService.FirstOrDefaultAsync(p => p.Id == productId, p => p);
            if (product == null)
                return Json(new { packets = new List<ProductPacketDto>(), unitName = "" }, JsonRequestBehavior.AllowGet);

            var productDto = product.Adapt<ProductDto>();
            
            // Load PackingType names
            var miscTasks = productDto.ProductPacketDtos
                .Where(d => d.PackingTypeId > 0)
                .Select(async d =>
                {
                    var misc = await _miscMasterService.FirstOrDefaultAsync(m => m.Id == d.PackingTypeId, m => m);
                    d.PackingTypeName = misc?.Name;
                });
            await Task.WhenAll(miscTasks);

            // Load MRP from ExtraProperties
            for (var index = 0; index < productDto.ProductPacketDtos.Count; index++)
            {
                var dto = productDto.ProductPacketDtos[index];
                var entity = product.ProductPacketDetails.FirstOrDefault(p => p.Id == dto.Id);
                if (entity != null && entity.ExtraProperties.ContainsKey(FieldConstants.Mrp))
                {
                    var mrpValue = entity.ExtraProperties.GetOrDefault(FieldConstants.Mrp);
                    if (mrpValue != null && double.TryParse(mrpValue.ToString(), out var mrp))
                        dto.Mrp = mrp;
                }
            }

            // Get Unit name
            string unitName = "";
            if (product.UnitId.HasValue && product.UnitId.Value > 0)
            {
                var unit = await _miscMasterService.FirstOrDefaultAsync(m => m.Id == product.UnitId.Value, m => m);
                unitName = unit?.Name ?? "";
            }

            return Json(new { packets = productDto.ProductPacketDtos, unitName = unitName }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(new { packets = new List<ProductPacketDto>(), unitName = "" }, JsonRequestBehavior.AllowGet);
        }
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public async Task<ActionResult> Inline_Create_Update_Destroy_Item([DataSourceRequest] DataSourceRequest request, ProductItemDto model)
    {
        var vm = await _itemService.GetViewModelAsync(model.ItemId).ConfigureAwait(false);
        model.ItemName = vm?.Name;

        return Json(await new[] { model }.ToDataSourceResultAsync(request, ModelState).ConfigureAwait(false));
    }
    
    [AcceptVerbs(HttpVerbs.Post)]
    public async Task<ActionResult> Inline_Create_Update_Destroy_Packet([DataSourceRequest] DataSourceRequest request, ProductPacketDto dto)
    {
        var vm = await _miscMasterService.GetViewModelAsync(dto.PackingTypeId).ConfigureAwait(false);
        dto.PackingTypeName = vm?.Name;

        return Json(await new[] { dto }.ToDataSourceResultAsync(request, ModelState).ConfigureAwait(false));
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public async Task<ActionResult> Inline_Create_Update_Destroy_Stock([DataSourceRequest] DataSourceRequest request, ProductStockDto dto)
    {
        return await Task.FromResult(Json(await new[] { dto }.ToDataSourceResultAsync(request, ModelState))).ConfigureAwait(false);
    }

    #endregion

    #region -- Helper Methods --
    private async Task LoadRelatedNamesAsync(ProductDto productDto)
    {
        var productTypeIds = productDto.ProductPacketDtos.Select(d => d.PackingTypeId);
        var packingTypes = await _miscMasterService.GetAsync(p => productTypeIds.Contains(p.Id), p => p).ConfigureAwait(false);
        productDto.ProductPacketDtos.ForEach(p => 
            p.PackingTypeName = packingTypes.FirstOrDefault(x => x.Id == p.PackingTypeId)?.Name);
        /*var tasks = new List<Task>();

        // Load PackingType names
        if (productDto.ProductPacketDtos.Any())
        {
            var miscTasks = productDto.ProductPacketDtos
                .Where(d => d.PackingTypeId > 0)
                .Select(async d =>
                {
                    var misc = await _miscMasterService.FirstOrDefaultAsync(m => m.Id == d.PackingTypeId, m => m);
                    d.PackingTypeName = misc?.Name;
                });
            tasks.AddRange(miscTasks);
        }

        await Task.WhenAll(tasks);*/
    }
    #endregion
}