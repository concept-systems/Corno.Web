using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.BoardStore.Models;
using Corno.Web.Areas.BoardStore.Services.Interfaces;
using Corno.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;

namespace Corno.Web.Areas.BoardStore.Controllers;

public class LocationController : MasterController <Location>
{
    #region -- Constructors --
    public LocationController(ILocationService locationService) 
        : base(locationService)
    {
        _locationService = locationService;
    }
    #endregion

    #region -- Data Mambers --
    private readonly ILocationService _locationService;
    #endregion

    #region -- Actions --
    public override async Task<ActionResult> Index(int? page, string locations)
    {
        return await base.Index(page, locations);
    }

    protected override async Task<Location> EditGetAsync(int? id)
    {
        var location = await _locationService.GetByIdAsync(id ?? 0);
        return location;
    }

    protected override Location EditPost(Location model)
    {
        // Note: This is called from base controller's Edit method which is async
        // but EditPost itself is kept sync as it's called synchronously from the base
        var existing = _locationService.GetByIdAsync(model.Id).GetAwaiter().GetResult();
        if (null == existing)
            throw new Exception("Something went wrong Product controller.");

        model.Adapt(existing);
        existing.ModifiedDate = DateTime.Now;

        return existing;
    }

    public ActionResult GetIndexViewModels([DataSourceRequest] DataSourceRequest request)
    {
        var query = _locationService.GetQuery();
        var result = query.ToDataSourceResult(request);
        return Json(result, JsonRequestBehavior.AllowGet);
    }

    #endregion
}