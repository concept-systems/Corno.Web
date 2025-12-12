using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Kitchen.Dto.Carton;
using Corno.Web.Areas.Kitchen.Dto.Rack_In;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Attributes;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.Web.Areas.Kitchen.Controllers;

[System.Web.Http.Authorize]
public class RackInController : SuperController
{
    #region -- Constructors --
    public RackInController(ICartonService cartonService, IRackInService rackInService)
    {
        _cartonService = cartonService;
        _rackInService = rackInService;

        const string viewPath = "~/Areas/Kitchen/Views/RackIn";
        _createPath = $"{viewPath}/Create.cshtml";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
    }
    #endregion

    #region -- Data Members --
    private readonly IRackInService _rackInService;
    private readonly ICartonService _cartonService;
    #endregion
    #region -- Data Mambers --
    private readonly string _indexPath;
    private readonly string _createPath;
    #endregion
    #region -- Actions --
    public ActionResult Index()
    {
        return View(new CartonCrudDto());
    }

    public virtual ActionResult Create()
    {
        try
        {
            try
            {
                Session[FieldConstants.Label] = null;
                return View(new RackInViewDto());
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
            }
            return View();
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }
    [HttpPost]
    //[ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "Save")]
    public ActionResult Create(RackInViewDto viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        try
        {
            return View(_createPath, viewModel);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_createPath, viewModel);
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "Scan")]
    public ActionResult Scan(RackInViewDto dto)
    {
        if (!ModelState.IsValid)
            return View(_createPath, dto);
        try
        {

            _rackInService.PerformRackIn(dto);

        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        dto.RackNo = string.Empty;
        dto.PalletNo = string.Empty;
        return View(_createPath, dto);
    }

    public async Task<ActionResult> GetIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = _cartonService.GetQuery().AsNoTracking();
            query = query?.Where(p => p.Status == StatusConstants.RackIn);
            var data = from carton in query
                select new CartonIndexDto
                {
                    Id = carton.Id,
                    CartonNo = carton.CartonNo.ToString(), // Now safe
                    PackingDate = carton.PackingDate,
                    SoNo = carton.SoNo,
                    WarehouseOrderNo = carton.WarehouseOrderNo,
                    CartonBarcode = carton.CartonBarcode,
                    Status = carton.Status
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
}