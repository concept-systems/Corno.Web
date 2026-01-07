using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Corno.Web.Attributes;
using Corno.Web.Models;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Controllers;

//[Authorize]
[Compress]
public class MasterController<TEntity> : BaseController<TEntity>
    where TEntity : MasterModel, new()
{
    #region -- Constructor --

    public MasterController(IMasterService<TEntity> genericMasterService)
        : base(genericMasterService)
    {
        _genericMasterService = genericMasterService;
    }

       
    #endregion

    #region -- Data Members --

    private readonly IMasterService<TEntity> _genericMasterService;

    #endregion

    #region -- Protected Methods -- 
    protected override async Task<ActionResult> IndexGetAsync(int? pageNo, string type)
    {
        var list = await _genericMasterService.GetViewModelListAsync();
        return View(list);
    }

    #endregion

    #region -- Public Methods --
    /*[HttpPost]
    public override async Task<ActionResult> ImportMaster(IEnumerable<HttpPostedFileBase> files)
    {
        ActionResult jsonResult = Json(new {error = false }, JsonRequestBehavior.AllowGet); 
        try
        {
            var httpPostedFileBases = files.ToList();
            if (httpPostedFileBases.FirstOrDefault() == null )
                throw new Exception("No file selected for import");

            var fileBase = httpPostedFileBases.FirstOrDefault();
            // Save file
            var filePath = Server.MapPath("~/Upload/" + fileBase?.FileName);
            fileBase?.SaveAs(filePath);

            // Import file
            _progressService.SetWebProgress();
            await _genericMasterService.ImportAsync(filePath, _progressService);
        }
        catch (Exception exception)
        {
            jsonResult = Json(new
            {
                error = true,
                message = exception.Message
            }, JsonRequestBehavior.AllowGet);
        }
        return jsonResult;
    }*/
    #endregion
}