using System;
using Corno.Web.Models.Base;
using Corno.Web.Services.Interfaces;
using Microsoft.AspNet.Identity;

namespace Corno.Web.Controllers;

public class BaseController<TEntity> : CornoController<TEntity> where TEntity : BaseModel, new()
{
    public BaseController(IBaseService<TEntity> baseService)
        : base((ICornoService<TEntity>)baseService)
    {
    }

    protected override void UpdateCommonCreateFields(TEntity model)
    {
        var userId = User.Identity.GetUserId();
        model.CreatedBy = userId;
        model.CreatedDate = DateTime.Now;
        model.ModifiedBy = userId;
        model.ModifiedDate = DateTime.Now;
    }

    protected override void UpdateCommonEditFields(TEntity model)
    {
        var userId = User.Identity.GetUserId();
        model.ModifiedBy = userId;
        model.ModifiedDate = DateTime.Now;
    }

    protected override void UpdateCommonDeleteFields(TEntity model)
    {
        model.ModifiedDate = DateTime.Now;
        model.DeletedDate = DateTime.Now;
    }
}