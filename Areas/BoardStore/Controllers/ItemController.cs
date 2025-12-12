using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.BoardStore.Services.Interfaces;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Item = Corno.Web.Areas.BoardStore.Models.Item;

namespace Corno.Web.Areas.BoardStore.Controllers;

public class ItemController : SuperController
{
    #region -- Constructors --
    public ItemController(IItemService itemService)
    {
        _itemService = itemService;

        var viewPath = @"~/Areas/BoardStore/Views/Item";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
    }
    #endregion

    #region -- Data Mambers --
    private readonly IItemService _itemService;

    private readonly string _createPath;
    private readonly string _editPath;
    #endregion

    #region -- Actions --
    public ActionResult Index()
    {
        return View(new Item());
    }

    // GET: /Location/Create
    public ActionResult Create()
    {
        try
        {
            return View(new Item());
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }

    //POST: /Product/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(Item viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        try
        {
            // Check whether Name Already Exists
            if (await _itemService.ExistsAsync(viewModel.Code).ConfigureAwait(false))
                throw new Exception($"Item with code {viewModel.Code} already exists.");

            await AddAsync(viewModel).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(viewModel);
    }


    public async Task<ActionResult> Edit(int id)
    {
        try
        {
            if (id <= 0)
                throw new Exception("Invalid Id");

            var item = await _itemService.GetByIdAsync(id).ConfigureAwait(false);
            return View(_editPath, item);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_editPath, null);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(Item model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        try
        {
            await UpdateAsync(model).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(model);
    }

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //protected ActionResult Edit(Item model)
    //{
    //    try
    //    {
    //        var existing = _itemService.GetById(model.Id);
    //        if (null == existing)
    //            throw new Exception("Something went wrong item controller.");

    //        model.CopyPropertiesTo(existing);

    //        existing.ModifiedDate = DateTime.Now;

    //        return View(_editPath, existing);
    //    }
    //    catch (Exception exception)
    //    {
    //        HandleControllerException(exception);
    //    }

    //    return View(_editPath, model);
        
    //}

    public ActionResult GetIndexViewModels([DataSourceRequest] DataSourceRequest request)
    {
        var query = _itemService.GetQuery();
        var result = query.ToDataSourceResult(request);
        return Json(result, JsonRequestBehavior.AllowGet);
    }

    #endregion

    private async Task AddAsync(Item viewModel)
    {
        var item = new Item
        {
            Code = viewModel.Code,
            Name = viewModel.Name,
            Description = viewModel.Description,

            // General Tab
            UnitId = viewModel.UnitId,
            //ItemCategory = viewModel.ItemCategory,
            //StockQuantity = viewModel.StockQuantity,
            //ReorderLevel = viewModel.ReorderLevel,
            //BoxQuantity = viewModel.BoxQuantity,
            //Color = viewModel.Color,
            //DrawingNo = viewModel.DrawingNo,
            //ItemType = viewModel.ItemType,
            //Weighing = viewModel.Weighing,
            //QcCheck = viewModel.QcCheck,
            //PartialQc = viewModel.PartialQc,
            //RackNo = viewModel.RackNo,

            //Cost Tab
            Rate = viewModel.Rate,

            //Dimensions
            Weight = viewModel.Weight,
            WeightTolerance = viewModel.WeightTolerance,
            Length = viewModel.Length,
            LengthTolerance = viewModel.LengthTolerance,
            Width = viewModel.Width,
            WidthTolerance = viewModel.WidthTolerance,
            Thickness = viewModel.Thickness,
            ThicknessTolerance = viewModel.ThicknessTolerance,

            Status = StatusConstants.Active,
        };

        //foreach (var itemPacketViewModel in viewModel.ItemPacketDetails)
        //{
        //    item.ItemPacketDetails.Add(new ItemPacketDetail
        //    {
        //        PackingTypeId = itemPacketViewModel.PackingTypeId,
        //        Quantity = itemPacketViewModel.Quantity
        //    });
        //}

        await _itemService.AddAndSaveAsync(item).ConfigureAwait(false);
    }

    private async Task UpdateAsync(Item viewModel)
    {
        var item = await _itemService.GetByIdAsync(viewModel.Id).ConfigureAwait(false);

        if (item == null)
        {
            throw new Exception("Item not found.");
        }

        // Update the properties of the existing product
        item.Name = viewModel.Name;
        item.Description = viewModel.Description;

        // General Tab
        item.UnitId = viewModel.UnitId;
        //item.ItemCategory = viewModel.ItemCategory;
        item.StockQuantity = viewModel.StockQuantity;
        //item.ReorderLevel = viewModel.ReorderLevel;
        //item.BoxQuantity = viewModel.BoxQuantity;
        item.Color = viewModel.Color;
        item.DrawingNo = viewModel.DrawingNo;

        //Cost Tab
        item.Rate = viewModel.Rate;

        //Dimensions
        item.Weight = viewModel.Weight;
        item.WeightTolerance = viewModel.WeightTolerance;
        item.Length = viewModel.Length;
        item.LengthTolerance = viewModel.LengthTolerance;
        item.Width = viewModel.Width;
        item.WidthTolerance = viewModel.WidthTolerance;
        item.Thickness = viewModel.Thickness;
        item.ThicknessTolerance = viewModel.ThicknessTolerance;

        item.Status = StatusConstants.Active;

        await _itemService.UpdateAndSaveAsync(item).ConfigureAwait(false);
    }
}