using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Xml;
using Corno.Concept.Portal.Controllers;
using Corno.Globals.Constants;
using Corno.Globals;
using Corno.Models.Dto;
using Corno.Services.Account.Interfaces;
using Corno.Services.Windsor;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;
using Microsoft.AspNet.Identity;
using Corno.Concept.Portal.Areas.Admin.Dto.SqlQuory;

namespace Corno.Concept.Portal.Areas.Admin.Controllers;

public class SqlQuoryController : SuperController
{
    #region -- Constructors --
    public SqlQuoryController(IRoleService roleService)
    {
        _roleService = roleService;

        const string viewPath = "~/Areas/admin/views/SqlQuory/";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IRoleService _roleService;

    private readonly string _indexPath;
    private readonly string _createPath;
    private readonly string _editPath;
    #endregion

    #region -- Actions --
    [Authorize]
    public ActionResult Index(int? page)
    {
        return View(_indexPath, null);
    }

    public ActionResult Create()
    {
        return View(_createPath, new SqlQuoryDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(SqlQuoryDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_createPath, dto);

            // Create Role
            //_roleService.Create(dto);

            return RedirectToAction("Index");
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