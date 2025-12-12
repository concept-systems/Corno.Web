using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Attributes;
using Corno.Web.Models.Base;
using Corno.Web.Services.Interfaces;

namespace Corno.Web.Controllers;

//[Authorize]
[Compress]
public class TransactionController<TEntity> : BaseController<TEntity>
    where TEntity : TransactionModel, new()
{
    #region -- Constructor --

    public TransactionController(IBaseService<TEntity> transactionService)
        : base(transactionService)
    {
        _transactionService = transactionService;
    }
    #endregion

    #region -- Controllers --
    private readonly IBaseService<TEntity> _transactionService;
    #endregion

    #region -- Protected Methods --
    //protected override bool ValidateModel(TEntity model)
    //{
    //    GenericService.

    //    return true;
    //}
    #endregion

    #region -- Action Methods --

    protected async Task<ActionResult> IndexGet(int? pageNo, string type)
    {
        // For Admin users
        // if (User.IsInRole(RoleNames.Admin))
        return View(await _transactionService.GetListAsync());
    }

    #endregion

    //[AcceptVerbs(HttpVerbs.Post)]
    //public virtual ActionResult Inline_Create([DataSourceRequest] DataSourceRequest request, TransactionModel model)
    //{
    //    return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    //}

    //[AcceptVerbs(HttpVerbs.Post)]
    //public virtual ActionResult Inline_Update([DataSourceRequest] DataSourceRequest request, TransactionModel model)
    //{
    //    return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    //}

    //[AcceptVerbs(HttpVerbs.Post)]
    //public virtual ActionResult Inline_Destroy([DataSourceRequest] DataSourceRequest request, TransactionModel model)
    //{
    //    return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    //}
}