using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Areas.Kitchen.Dto.Label;
using Corno.Web.Areas.Kitchen.Dto.Put_To_Light;
using Corno.Web.Areas.Kitchen.Services.Interfaces;
using Corno.Web.Controllers;
using Corno.Web.Globals;
using Corno.Web.Hubs;
using Corno.Web.Models;
using Corno.Web.Models.Packing;
using Corno.Web.Services.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.SignalR;

namespace Corno.Web.Areas.Kitchen.Controllers;

[System.Web.Mvc.Authorize]
public class PutToLightKittingController : SuperController
{
    #region -- Constructors --
    public PutToLightKittingController(ITrolleyConfigService trolleyConfigService,
        ILabelService labelService, IPlanService planService, 
        IMiscMasterService miscMasterService, IUserService userService)
    {
        _trolleyConfigService = trolleyConfigService;
        _labelService = labelService;
        _planService = planService;
        _miscMasterService = miscMasterService;
        _userService = userService;
    }
    #endregion
    #region -- Data Members --
    private readonly ITrolleyConfigService _trolleyConfigService;
    private readonly ILabelService _labelService;
    private readonly IPlanService _planService;
    private readonly IMiscMasterService _miscMasterService;
    private readonly IUserService _userService;
    #endregion

    #region -- Actions --
    public ActionResult Index()
    {
        return View(new LabelViewDto());
    }

    public virtual ActionResult Create()
    {
        try
        {
            try
            {
                Session[FieldConstants.Label] = null;
                return View(new PutToLightkittingViewDto());
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


    //[ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<ActionResult> Create(PutToLightkittingViewDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            dto.LedOn = false;
            var labels = await _labelService.GetAsync(d => d.Barcode == dto.Barcode, d => d,
                q => q.OrderByDescending(x => x.CreatedDate), true).ConfigureAwait(false);
            var label = labels.FirstOrDefault();
            if (null == label)
                throw new Exception($"No label found for barcode {dto.Barcode}");
            if (label.Status != StatusConstants.Active)
                throw new Exception($"Expected label status is 'Active', but current label status is {label.Status}");
            var plan = await _planService.GetByWarehouseOrderNoAsync(label.WarehouseOrderNo).ConfigureAwait(false);
            var planItemDetail = plan?.PlanItemDetails?.FirstOrDefault(d =>
                d.Position == label.Position);
            var trolleyConfig = await _trolleyConfigService.FirstOrDefaultAsync(p => p.Status == StatusConstants.Active, p => p).ConfigureAwait(false);
            if (null == trolleyConfig)
                throw new Exception("No trolley configuration is active");

            var colorCode = planItemDetail?.Reserved1;//.GetProperty(FieldConstants.Color, string.Empty);
            var color = await _miscMasterService.FirstOrDefaultAsync(p => p.Code == colorCode, p => p).ConfigureAwait(false);
            if (null == color)
                throw new Exception($"No color in master for color code '{colorCode}");
            var locationIds = trolleyConfig.TrolleyLightDetails
                .Where(d => d.ColorId == color.Id)
                .Select(d => d.LocationId)
                .ToList();

            if (locationIds.Count <= 0)
                throw new Exception("Location is not assigned to this item.");
            var locations = (await _miscMasterService.GetViewModelListAsync(p => locationIds.Contains(p.Id)).ConfigureAwait(false)).ToList();
            var locationNames = string.Join(",", locations.Select(p => p.Name));
            var userIds = label.LabelDetails.Select(d => d.ModifiedBy)
                .Distinct().ToList();
            var users = (await _userService.GetAsync(p => userIds.Contains(p.Id),
                p => p).ConfigureAwait(false)).ToList();
            // Update Label
            label.Status = StatusConstants.Bent;
            label.LabelDetails.Add(new LabelDetail
            {
                CompanyId = 1,
                ScanDate = DateTime.Now,
                Status = StatusConstants.Bent,
            });

            // Uddate Plan 
            if (planItemDetail != null)
            {
                planItemDetail.BendQuantity ??= 0;
                planItemDetail.BendQuantity += label.Quantity;
            }

            // Save updates
            await _planService.UpdateAsync(plan).ConfigureAwait(false);
            await _labelService.UpdateAndSaveAsync(label).ConfigureAwait(false);

            dto.ItemCode = planItemDetail?.ItemCode;
            dto.ItemName = planItemDetail?.Description;
            dto.SoNo = plan?.SoNo;
            dto.WarehousePosition = planItemDetail?.WarehousePosition;
            dto.ColorName = color.Name;
            dto.LocationNames = locationNames;
            dto.LedOn = true;
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(dto);
    }
    public async Task<ActionResult> GetIndexViewDto([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = _labelService.GetQuery(); /*as IEnumerable<Label>*/
            //query = query?.Where(p => p.Status == StatusConstants.Bent);
            var result = await query.ToDataSourceResultAsync(request).ConfigureAwait(false);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return Json(new DataSourceResult { Errors = exception.Message });
        }
    }



    #endregion
}