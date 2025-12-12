using Corno.Web.Areas.Euro.Dto.Label;
using Corno.Web.Areas.Euro.Services.Interfaces;
using Corno.Web.Controllers;
using Corno.Web.Hubs;
using Corno.Web.Models;
using Corno.Web.Services.Import;
using Corno.Web.Services.Import.Interfaces;
using Corno.Web.Services.Progress.Interfaces;
using Corno.Web.Windsor;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Kitchen.Services;

namespace Corno.Web.Areas.Euro.Controllers;

public class ItemLabelController : BaseImportController<LabelImportDto>
{
    #region -- Constructors --
    public ItemLabelController(IFileImportService<LabelImportDto> importService,
        IWebProgressService progressService,
        IEuroLabelService euroLabelService)
        : base(importService, progressService)
    {
        _euroLabelService = euroLabelService;
        ProgressService.OnProgressChanged += OnProgressChanged;
    }
    #endregion

    #region -- Data Members --
    private readonly IEuroLabelService _euroLabelService;
    #endregion

    #region -- BaseImportController Implementation --
    protected override string ControllerName => "ItemLabel";
    #endregion

    #region -- Actions --
    public ActionResult Index()
    {
        return View();
    }

    public async Task<ActionResult> View(int? id)
    {
        try
        {
            if (id == null)
                throw new Exception("Invalid Id");

            var label = await _euroLabelService.FirstOrDefaultAsync(l => l.Id == id.Value, l => l).ConfigureAwait(false);
            if (label == null)
                throw new Exception($"Label with Id {id} not found");

            return View(label);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return RedirectToAction("Index");
    }

    private void OnProgressChanged(object sender, ProgressModel e)
    {
        var context = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
        context.Clients.All.receiveProgress(e);
    }

    public async Task<ActionResult> GetIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var result = await Task.Run(() => _euroLabelService.GetIndexDataSource(request)).ConfigureAwait(false);
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
