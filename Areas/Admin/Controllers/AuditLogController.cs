using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.Web.Areas.Admin.Controllers;

public class AuditLogController : SuperController
{
    #region -- Constructors --
    public AuditLogController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
        _indexPath = "~/Areas/Admin/Views/AuditLog/Index.cshtml";
    }
    #endregion

    #region -- Data Members --
    private readonly IAuditLogService _auditLogService;
    private readonly string _indexPath;
    #endregion

    #region -- Actions --
    [Authorize]
    public ActionResult Index()
    {
        return View(_indexPath);
    }

    [HttpPost]
    public async Task<ActionResult> GetLogs([DataSourceRequest] DataSourceRequest request, 
        DateTime? fromDate, DateTime? toDate, string userId = null, string action = null, string entityType = null)
    {
        try
        {
            var filter = new AuditLogFilterDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                UserId = userId,
                Action = action,
                EntityType = entityType,
                Page = request.Page,
                PageSize = request.PageSize
            };
            
            var logs = await _auditLogService.GetLogsAsync(filter).ConfigureAwait(false);
            
            var result = new DataSourceResult
            {
                Data = logs,
                Total = logs.Count
            };
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetGridErrorResult(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult> ExportToExcel(DateTime? fromDate, DateTime? toDate, 
        string userId = null, string action = null, string entityType = null)
    {
        try
        {
            var filter = new AuditLogFilterDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                UserId = userId,
                Action = action,
                EntityType = entityType,
                Page = 1,
                PageSize = int.MaxValue
            };
            
            var logs = await _auditLogService.GetLogsAsync(filter).ConfigureAwait(false);
            // TODO: Implement Excel export
            return Json(new { success = true, message = "Export functionality to be implemented" }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }
    #endregion
}

