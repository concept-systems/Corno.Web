using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Areas.DemoProject.Dtos;
using Corno.Web.Areas.DemoProject.Services.Interfaces;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Corno.Web.Models.Masters;
using Corno.Web.Models.Packing;
using Corno.Web.Services.Masters.Interfaces;
using Corno.Web.Windsor;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Volo.Abp.Data;

namespace Corno.Web.Areas.DemoProject.Controllers;

public class LabelController : SuperController
{
    #region -- Constructors --
    public LabelController(ILabelService labelService, IBaseItemService itemService)
    {
        _labelService = labelService;
        _itemService = itemService;
    }
    #endregion

    #region -- Data Mambers --

    private readonly ILabelService _labelService;
    private readonly IBaseItemService _itemService;

    #endregion

    #region -- Private Methods --

    private async Task<LabelDto> CreateLabelDtoAsync(int? id)
    {
        var label = await _labelService.FirstOrDefaultAsync<Label>(l => l.Id == id, l => l);
        if (label == null)
            throw new Exception("Label not found.");

        var item = await _itemService.FirstOrDefaultAsync<Item>(i => i.Id == (label.ItemId ?? 0), i => i);

        var userIds = label.LabelDetails.Select(d => d.ModifiedBy).ToList();
        var userService = Bootstrapper.Get<IUserService>();
        var users = await userService.GetAsync(p => userIds.Contains(p.Id), p => p);

        var dto = new LabelDto
        {
            ItemId = item?.Id,
            ItemName = item?.Name,
            LabelDate = label?.LabelDate,
            Weight = label.NetWeight ?? 0,
            Rate = label.GetProperty(FieldConstants.Rate, 0),
            ManufacturingDate = label.GetProperty(FieldConstants.ManufacturingDate, DateTime.MinValue),
            ExpiryDate = label.GetProperty("ExpiryDate", DateTime.MinValue),

            PrintToPrinter = false,

            Details = label.LabelDetails.Select(d => new LabelDetailDto
            {
                ScanDate = d.ScanDate,
                ModifiedBy = users.FirstOrDefault(p => p.Id == d.ModifiedBy)?.UserName,
                ModifiedDate = d.ModifiedDate,
                Status = d.Status
            }).ToList()
        };

        return dto;
    }
    #endregion

    #region -- Actions --
    public ActionResult Index()
    {
        return View(new LabelDto());
    }

    public virtual ActionResult Create()
    {
        try
        {
            Session[FieldConstants.Label] = null;
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(new LabelDto());
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    //[MultipleButton(Name = "action", Argument = "Print")]
    public async Task<ActionResult> Create(LabelDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);
        try
        {
            switch (dto.SubmitType)
            {
                default:
                    // Create Labels (async)
                    var labels = await _labelService.CreateLabelsAsync(dto);
                    
                    // Get Item for report (async)
                    var item = await _itemService.FirstOrDefaultAsync<Item>(i => i.Id == (dto.ItemId ?? 0), i => i);
                    
                    // Create Label Report (async)
                    var report = await _labelService.CreateLabelReportAsync(labels, item, false);
                    Session[FieldConstants.Label] = report;
                    
                    // Save in database (async)
                    await _labelService.UpdateDatabaseAsync(labels);

                    // Clear model
                    dto.Weight = 0;

                    ModelState.Clear();
                    dto.PrintToPrinter = true;
                    break;
            }

            dto.SubmitType = default;
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(dto);
    }

    public async Task<ActionResult> View(int? id)
    {
        try
        {
            var label = await _labelService.FirstOrDefaultAsync<Label>(l => l.Id == id, l => l);
            if (null == label)
                throw new Exception($"Label with Id '{id}' not found.");

            var item = await _itemService.FirstOrDefaultAsync<Item>(i => i.Id == (label.ItemId ?? 0), i => i);

            // Create Label Report (async)
            var report = await _labelService.CreateLabelReportAsync(new List<Label> { label }, item, true).ConfigureAwait(false);
            Session[FieldConstants.Label] = report;

            var dto = await CreateLabelDtoAsync(id).ConfigureAwait(false);
            return View(dto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return RedirectToAction("Index");
    }

    public async Task<ActionResult> GetIndexViewModels([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = (IEnumerable<Label>)_labelService.GetQuery();

            var itemService = Bootstrapper.Get<IBaseItemService>();
            var itemQuery = (IEnumerable<Item>)itemService.GetQuery();

            var data = from label in query
                       where (label.ItemId ?? 0) > 0
                       join item in itemQuery on label.ItemId equals item.Id
                       select new LabelDto
                       {
                           Id = label.Id,
                           LabelDate = label.LabelDate,
                           ItemName = item?.Name,
                           Weight = label.Quantity,
                           Rate = label.GetProperty(FieldConstants.Rate, 0) > 0 ? label.GetProperty(FieldConstants.Rate, 0) : item?.Rate,
                           ExpiryDate = label.GetProperty("ExpiryDate", DateTime.MinValue),
                           ManufacturingDate = label.GetProperty(FieldConstants.ManufacturingDate, DateTime.MinValue),
                       };
            var result = await data.ToDataSourceResultAsync(request).ConfigureAwait(false);
            //var result = await query.ToDataSourceResultAsync(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return Json(new DataSourceResult { Errors = exception.Message });
        }
    }

    #endregion
}