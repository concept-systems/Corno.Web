using System;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Dto.SqlQuery;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.Web.Areas.Admin.Controllers;

public class SqlQueryController : SuperController
{
    #region -- Constructors --
    public SqlQueryController(IRoleService roleService, ISqlService sqlService)
    {
        _roleService = roleService;
        _sqlService = sqlService;

        const string viewPath = "~/Areas/admin/views/SqlQuery/";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IRoleService _roleService;
    private readonly ISqlService _sqlService;

    private readonly string _indexPath;
    private readonly string _createPath;
    private readonly string _editPath;
    #endregion

    #region -- Actions --
    public ActionResult Create()
    {
        return View(_createPath, new SqlQueryDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(SqlQueryDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_createPath, dto);

            var sqlQuery = dto.Query;
            ViewBag.DataTable = _sqlService.ExecuteQuery(sqlQuery);

            ViewBag.Message = "Query executed successfully!";
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, dto);
    }

   
    public ActionResult GetIndexModels([DataSourceRequest] DataSourceRequest request)
    {
        var query = _roleService.GetQuery();
        var result = query.ToDataSourceResult(request);
        return Json(result, JsonRequestBehavior.AllowGet);
    }

    #endregion
}