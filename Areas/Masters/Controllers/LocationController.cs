using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Areas.Masters.Dtos.Location;
using Corno.Web.Areas.Masters.Models;
using Corno.Web.Areas.Masters.Services.Interfaces;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Corno.Web.Hubs;
using Corno.Web.Models;
using Corno.Web.Models.Location;
using Corno.Web.Models.Masters;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Services.Progress.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR;

namespace Corno.Web.Areas.Masters.Controllers;

public class LocationController : SuperController
{
    #region -- Constructors --
    public LocationController(ILocationService locationService, IProductService productService,
        IMiscMasterService miscMasterService, IBaseItemService itemService, IWebProgressService progressService, IUserService userService)
    {
        _locationService = locationService;
        _productService = productService;
        _miscMasterService = miscMasterService;
        _itemService = itemService;
        _progressService = progressService;
        _userService = userService;

        progressService.SetWebProgress();
        progressService.OnProgressChanged += OnProgressChanged;


        const string viewPath = "~/Areas/Masters/views/Location/";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";

        TypeAdapterConfig<LocationDto, Location>
            .NewConfig()
            .Map(dest => dest.LocationItemDetails, src => src.LocationItemDetails.Adapt<List<LocationItemDetail>>())
            .Map(dest => dest.LocationStockDetails, src => src.LocationStockDetails.Adapt<List<LocationStockDetail>>())
            .Map(dest => dest.LocationUserDetails, src => src.LocationUserDetails.Adapt<List<LocationUserDetail>>())
            .AfterMapping((src, dest) =>
            {
                dest.Id = src.Id;
                /*for (var index = 0; index < src.LocationItemDetails.Count; index++)
                {
                    dest.LocationItemDetails[index].Id = src.LocationItemDetails[index].Id;
                }
                for (var index = 0; index < src.LocationItemDetails.Count; index++)
                {
                    dest.LocationStockDetails[index].Id = src.LocationStockDetails[index].Id;
                }
                for (var index = 0; index < src.LocationItemDetails.Count; index++)
                {
                    dest.LocationUserDetails[index].Id = src.LocationUserDetails[index].Id;
                }*/
            });
        TypeAdapterConfig<Location, LocationDto>
            .NewConfig()
            .Map(dest => dest.LocationItemDetails, src => src.LocationItemDetails.Adapt<List<LocationItemDto>>())
            .Map(dest => dest.LocationStockDetails, src => src.LocationStockDetails.Adapt<List<LocationStockDto>>())
            .Map(dest => dest.LocationUserDetails, src => src.LocationUserDetails.Adapt<List<LocationUserDto>>())
            .AfterMapping((src, dest) =>
            {
                dest.Id = src.Id;
                /*for (var index = 0; index < src.LocationItemDetails.Count; index++)
                    dest.LocationItemDetails[index].Id = src.LocationItemDetails[index].Id;
                for (var index = 0; index < src.LocationStockDetails.Count; index++)
                    dest.LocationStockDetails[index].Id = src.LocationStockDetails[index].Id;
                for (var index = 0; index < src.LocationUserDetails.Count; index++)
                    dest.LocationUserDetails[index].Id = src.LocationUserDetails[index].Id;*/
            });

    }
    #endregion

    #region -- Data Mambers --
    private readonly ILocationService _locationService;
    private readonly IProductService _productService;
    private readonly IMiscMasterService _miscMasterService;
    private readonly IBaseItemService _itemService;
    private readonly IWebProgressService _progressService;
    private readonly IUserService _userService;

    private readonly string _indexPath;
    private readonly string _createPath;
    private readonly string _editPath;

    #endregion

    #region -- Action Methods --

    public ActionResult Index()
    {
        return View(_indexPath, new List<LocationDto>());
    }

    // GET: /Location/Create
    public ActionResult Create()
    {
        return View(_createPath, new LocationDto());
    }

    //POST: /Location/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(LocationDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);

        try
        {
            // Check whether Name Already Exists
            if (await _locationService.ExistsAsync(dto.Code))
                throw new Exception($"Location with code {dto.Code} already exists.");
            
            var location = dto.Adapt<Location>();

            location.Status = StatusConstants.Active;
            
            await _locationService.AddAndSaveAsync(location);

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
        var location = await _locationService.GetByIdAsync(id);
        var dto = location.Adapt<LocationDto>();
        
        foreach (var detail in dto.LocationItemDetails)
            detail.ItemName = await _itemService.GetNameAsync(detail.ItemId);
        foreach (var detail in dto.LocationStockDetails)
            detail.ItemName = await _itemService.GetNameAsync(detail.ItemId);
        foreach (var detail in dto.LocationUserDetails)
            detail.UserName = await _itemService.GetNameAsync(detail.UserId);

        return View(_editPath, dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(LocationDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(_editPath, dto);
        }
        try
        {
            var location = await _locationService.GetByIdAsync(dto.Id);
            if (location == null)
                throw new Exception("Location not found.");

            dto.Adapt(location);
            
            /*location.LocationItemDetails.ForEach(d => d.ItemId = _miscMasterService.GetName(d.ItemId));
            location.LocationStockDetails.ForEach(d => d.ItemId = _miscMasterService.GetName(d.ItemId));*/
            foreach (var detail in location.LocationUserDetails)
                detail.UserId = await _miscMasterService.GetNameAsync(detail.UserId);

            location.Status = StatusConstants.Active;

            await _locationService.UpdateAndSaveAsync(location);

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
        var location = await _locationService.GetByIdAsync(id ?? 0);

        var dto = new LocationDto
        {
            Code = location.Code,
            Name = location.Name,
            Description = location.Description
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(LocationDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }
        try
        {
            var location = await _locationService.GetByIdAsync(dto.Id);
            location.Status = StatusConstants.Deleted;
            await _locationService.UpdateAndSaveAsync(location);

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

    #endregion

    #region -- Import --

    [HttpPost]
    public async Task<ActionResult> Import(IEnumerable<HttpPostedFileBase> files)
    {
        try
        {
            var file = files.FirstOrDefault();
            if (file == null)
                throw new Exception("No file selected for import");

            /*// Save file
            var filePath = Server.MapPath("~/Upload/" + file?.FileName);
            file?.SaveAs(filePath);*/

            // Import file
            _progressService.SetWebProgress();
            var importModels = await _locationService.ImportAsync(file, _progressService);

            return Json(new { success = true, importModels }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);

            return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
        //return Json(new { success = true, importModels = new List<LocationImportModel>() }, JsonRequestBehavior.AllowGet);
    }

    public virtual ActionResult Import()
    {
        try
        {
            return View();
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }

    private void OnProgressChanged(object sender, ProgressModel e)
    {
        var context = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
        context.Clients.All.receiveProgress(e);
    }

    public ActionResult CancelImport(string[] fileNames)
    {
        _progressService.CancelRequested();
        return Json(new { success = true, importModels = new List<LocationImportModel>() }, JsonRequestBehavior.AllowGet);
    }

    #endregion

    #region -- Actions --

    [AcceptVerbs(HttpVerbs.Post)]
    public async Task<ActionResult> Inline_Create_Item([DataSourceRequest] DataSourceRequest request, LocationItemDto dto)
    {
        var itemViewModel = await _itemService.GetViewModelAsync(dto.ItemId).ConfigureAwait(false);
        dto.ItemName = itemViewModel?.NameWithCode;
        var productViewModel = await _productService.GetViewModelAsync(dto.ProductId).ConfigureAwait(false);
        dto.ProductName = productViewModel?.NameWithCode;

        return Json(new[] { dto }.ToDataSourceResult(request, ModelState));
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public async Task<ActionResult> Inline_Create_User([DataSourceRequest] DataSourceRequest request, LocationUserDto dto)
    {
        var user = await _userService.GetByIdAsync(dto.UserId).ConfigureAwait(false);
        dto.UserName = user?.UserName;

        return Json(new[] { dto }.ToDataSourceResult(request, ModelState));
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public async Task<ActionResult> Inline_Create_Stock([DataSourceRequest] DataSourceRequest request, LocationStockDto dto)
    {
        var itemViewModel = await _itemService.GetViewModelAsync(dto.ItemId).ConfigureAwait(false);
        dto.ItemName = itemViewModel?.NameWithCode;

        return Json(new[] { dto }.ToDataSourceResult(request, ModelState));
    }

    public ActionResult GetIndexViewModels([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            //TypeAdapterConfig<Location, LocationIndexDto>.NewConfig()
            //    .Ignore(p => p.ProductCode);
            //var query = _locationService.GetQuery();
            //// Apply filtering, sorting, and paging using ToDataSourceResult
            //var result = query.ProjectToType(typeof(LocationIndexDto)).ToDataSourceResult(request);

            //return Json(result, JsonRequestBehavior.AllowGet);

            var query = ((IQueryable<Location>)_locationService.GetQuery().AsNoTracking());
            var data = from location in query
                       join product in (IEnumerable<Product>)_productService.GetQuery().AsNoTracking()
                           on location.Id equals product.Id into defaultProduct
                       from product in defaultProduct.DefaultIfEmpty()
                       select new LocationIndexDto
                       {
                           Id = location.Id,
                           Code = location.Code,
                           Name = location.Name,
                           ProductCode = product.Code,
                           ProductName = product.Name,
                           BayId = location.BayId,
                           AreaId = location.AreaId,
                           RowId = location.RowId,
                           ShelfId = location.LevelId,
                           Status = location.Status
                       };
            var result = data.ToList().ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            ModelState.AddModelError("Error", exception.Message);
            return Json(new DataSourceResult { Errors = ModelState });
        }
        
    }

    #endregion
}