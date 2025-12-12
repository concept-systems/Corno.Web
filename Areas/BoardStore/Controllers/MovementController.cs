using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.BoardStore.Models;
using Corno.Web.Areas.BoardStore.Services.Interfaces;
using Corno.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.Web.Areas.BoardStore.Controllers;

public class MovementController : SuperController
{
    #region -- Constructors --
    public MovementController(IMovementService movementService) 
    {
        _movementService = movementService;
    }
    #endregion

    #region -- Data Mambers --
    private readonly IMovementService _movementService;
    #endregion

    #region -- Actions --
    public ActionResult Index()
    {
        return View(new Movement());
    }

    public async Task<ActionResult> View(int? id)
    {
        try
        {
            var movement = await _movementService.GetByIdAsync(id ?? 0).ConfigureAwait(false);
            if (null == movement)
                throw new Exception("Plan with Id not found");

            return View(movement);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }

    //[HttpPost] // Use HttpPost for deletion to comply with RESTful conventions
    public async Task<ActionResult> DeleteMovement(int? id)
    {
        try
        {
            if (null == id)
                return Json(new { success = false, message = "ID cannot be null." });

            var movement = await _movementService.FirstOrDefaultAsync(p => p.Id == id, p => p).ConfigureAwait(false);
            if (movement == null)
                return Json(new { success = false, message = "Movement with the specified ID not found." });

            // Perform the deletion
            await _movementService.DeleteAsync(movement).ConfigureAwait(false);
            await _movementService.SaveAsync().ConfigureAwait(false);

            return Json(new { success = true, message = "Movement deleted successfully." });
        }
        catch (Exception exception)
        {
            // Log the exception and return an error message
            HandleControllerException(exception);
        }
        return Json(new { success = false, message = "An error occurred while deleting the movement." });
    }

    public async Task<ActionResult> GetIndexViewModels([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = _movementService.GetQuery();
            var result = await query.ToDataSourceResultAsync(request).ConfigureAwait(false);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    #endregion
}