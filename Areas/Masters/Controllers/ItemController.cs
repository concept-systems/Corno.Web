using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Masters.Dtos.Item;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Corno.Web.Models.Masters;
using Corno.Web.Services.Interfaces;
using Corno.Web.Services.Masters.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;
using ILocationService = Corno.Web.Areas.Masters.Services.Interfaces.ILocationService;


namespace Corno.Web.Areas.Masters.Controllers;

public class ItemController : SuperController
{
    #region -- Constructors --
    public ItemController(IBaseItemService itemService, IMiscMasterService miscMasterService, ILocationService locationService)
    {
        _itemService = itemService;
        _miscMasterService = miscMasterService;
        _locationService = locationService;

        const string viewPath = "~/Areas/Masters/Views/Item/";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";

        TypeAdapterConfig<ItemDto, Item>
            .NewConfig()
            .Map(dest => dest.ItemPacketDetails, src => src.ItemPacketDtos.Adapt<List<ItemPacketDetail>>())
            .Map(dest => dest.ItemMachineDetails, src => src.ItemMachineDtos.Adapt<List<ItemMachineDetail>>())
            .Map(dest => dest.ItemProcessDetails, src => src.ItemProcessDtos.Adapt<List<ItemProcessDetail>>())
            .AfterMapping(static (src, dest) =>
            {
                dest.Id = src.Id;
                /* for (var index = 0; index < src.ItemPacketDtos.Count; index++)
                     dest.ItemPacketDetails[index].Id = src.ItemPacketDtos[index].Id;
                 for (var index = 0; index < src.ItemMachineDtos.Count; index++)
                     dest.ItemMachineDetails[index].Id = src.ItemMachineDtos[index].Id;
                 for (var index = 0; index < src.ItemPacketDtos.Count; index++)
                     dest.ItemProcessDetails[index].Id = src.ItemProcessDtos[index].Id;*/
            });
        TypeAdapterConfig<Item, ItemDto>
            .NewConfig()
            .Map(dest => dest.ItemPacketDtos, src => src.ItemPacketDetails.Adapt<List<ItemPacketDto>>())
            .Map(dest => dest.ItemMachineDtos, src => src.ItemMachineDetails.Adapt<List<ItemMachineDto>>())
            .Map(dest => dest.ItemProcessDtos, src => src.ItemProcessDetails.Adapt<List<ItemProcessDto>>())
            .AfterMapping((src, dest) =>
            {
                dest.Id = src.Id;
                /*for (var index = 0; index < src.ItemPacketDetails.Count; index++)
                    dest.ItemPacketDtos[index].Id = src.ItemPacketDetails[index].Id;
                for (var index = 0; index < src.ItemMachineDetails.Count; index++)
                    dest.ItemMachineDtos[index].Id = src.ItemMachineDetails[index].Id;
                for (var index = 0; index < src.ItemProcessDetails.Count; index++)
                    dest.ItemProcessDtos[index].Id = src.ItemProcessDetails[index].Id;*/
            });
    }
    #endregion

    #region -- Data Mambers --
    private readonly IBaseItemService _itemService;
    private readonly IMiscMasterService _miscMasterService;
    private readonly ILocationService _locationService;

    private readonly string _indexPath;
    private readonly string _createPath;
    private readonly string _editPath;
    #endregion

    #region -- Actions --
    public ActionResult Index()
    {
        return View(_indexPath, new List<ItemDto>());
    }

    // GET: /Location/Create
    public ActionResult Create()
    {
        try
        {
            return View(new ItemDto());
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, new ItemDto());
    }

    //POST: /Product/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(ItemDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);

        try
        {
            // Check whether Name Already Exists
            if (await _itemService.ExistsAsync(dto.Code).ConfigureAwait(false))
                throw new Exception($"Item with code {dto.Code} already exists.");

            //Add(dto);
            //var product = dto.CreateModel();
            var item = dto.Adapt<Item>();

            await _itemService.AddAndSaveAsync(item).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(dto);
    }

    public async Task<ActionResult> Edit(int? id)
    {
        var item = await _itemService.GetByIdAsync(id).ConfigureAwait(false);
        var dto = item.Adapt<ItemDto>();

        var packingTypeIds = dto.ItemPacketDtos.Select(d => d.PackingTypeId).ToList();
        var packingTypes = await _miscMasterService.GetViewModelListAsync(p => packingTypeIds.Contains(p.Id)).ConfigureAwait(false);
        var machineItemIds = dto.ItemMachineDtos.Select(d => d.ItemId ?? 0).ToList();
        var machineItems = await _miscMasterService.GetViewModelListAsync(p => machineItemIds.Contains(p.Id)).ConfigureAwait(false);
        var processItemIds = dto.ItemProcessDtos.Select(d => d.ItemId ?? 0).ToList();
        var processItems = await _miscMasterService.GetViewModelListAsync(p => processItemIds.Contains(p.Id)).ConfigureAwait(false);

        dto.ItemPacketDtos.ForEach(d => d.PackingTypeName = packingTypes.FirstOrDefault(x => x.Id == d.PackingTypeId)?.Name);
        dto.ItemMachineDtos.ForEach(d => d.ItemName = machineItems.FirstOrDefault(x => x.Id == d.MachineId)?.Name);
        dto.ItemProcessDtos.ForEach(d => d.ShortEdge1 = processItems.FirstOrDefault(x => x.Id == d.ItemId)?.Name);

        return View(_editPath, dto);

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(ItemDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(_editPath, dto);
        }
        try
        {
            var item = await _itemService.GetByIdAsync(dto.Id).ConfigureAwait(false);
            if (item == null)
                throw new Exception("Item not found.");

            dto.Adapt(item);

            /*item.ItemPacketDetails.ForEach(d => d.PackingTypeId = _miscMasterService.GetName(d.PackingTypeId));
            item.ItemMachineDetails.ForEach(d => d.Item = _miscMasterService.GetName(d.ItemId));
            item.ItemProcessDetails.ForEach(d => d.ShortEdge1 = _miscMasterService.GetName(d.ItemId));*/

            await _itemService.UpdateAndSaveAsync(item).ConfigureAwait(false);

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
        var item = await _itemService.GetByIdAsync(id ?? 0).ConfigureAwait(false);

        var itemViewModel = new ItemDto
        {
            Code = item.Code,
            Name = item.Name,
            Description = item.Description
        };

        return View(itemViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(ItemDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }
        try
        {
            var item = await _itemService.GetByIdAsync(dto.Id).ConfigureAwait(false);
            item.Status = StatusConstants.Deleted;
            await _itemService.UpdateAndSaveAsync(item).ConfigureAwait(false);

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

    public async Task<ActionResult> GetIndexViewModels([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = _itemService.GetQuery().AsNoTracking();
            var data = from item in query
                       join location in _locationService.GetQuery().AsNoTracking()
                           on item.Id equals location.Id into defaultLocation
                       from location in defaultLocation.DefaultIfEmpty()
                       select new ItemIndexDto
                       {
                           Id = item.Id,
                           Name = item.Name,
                           Code = item.Code,
                           Description = item.Description,
                           CurrentStock = item.Rate.ToString(),
                           Cost = item.Rate,
                           Location = location.Code,
                           Weight = item.Weight,
                           Status = item.Status
                       };
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

    #endregion

    #region -- Actions --

    [AcceptVerbs(HttpVerbs.Post)]
    public async Task<ActionResult> Inline_Create_Item([DataSourceRequest] DataSourceRequest request, ItemPacketDto dto)
    {
        var packingType = await _miscMasterService.GetViewModelAsync(dto.PackingTypeId).ConfigureAwait(false);
        dto.PackingTypeName = packingType?.Name;
        return Json(new[] { dto }.ToDataSourceResultAsync(request, ModelState));
    }

    #endregion
}