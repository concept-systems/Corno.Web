using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Dto;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Controllers;
using Microsoft.AspNet.Identity;

namespace Corno.Web.Areas.Admin.Controllers;

public class ProfileController : SuperController
{
    #region -- Constructors --
    public ProfileController(IUserService userService, IAuditLogService auditLogService)
    {
        _userService = userService;
        _auditLogService = auditLogService;
        _indexPath = "~/Areas/Admin/Views/Profile/Index.cshtml";
        _changePasswordPath = "~/Areas/Admin/Views/Profile/ChangePassword.cshtml";
    }
    #endregion

    #region -- Data Members --
    private readonly IUserService _userService;
    private readonly IAuditLogService _auditLogService;
    private readonly string _indexPath;
    private readonly string _changePasswordPath;
    #endregion

    #region -- Actions --
    [Authorize]
    public async Task<ActionResult> Index()
    {
        var userId = User.Identity.GetUserId();
        var profile = await _userService.GetProfileAsync(userId).ConfigureAwait(false);
        return View(_indexPath, profile);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> UpdateProfile(ProfileDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_indexPath, dto);

            var userId = User.Identity.GetUserId();
            await _userService.UpdateProfileAsync(userId, dto).ConfigureAwait(false);

            // Audit log
            await _auditLogService.LogAsync(
                userId,
                "UpdateProfile",
                "User",
                userId,
                User.Identity.Name,
                "Updated profile information",
                Request.UserHostAddress,
                Request.UserAgent
            ).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_indexPath, dto);
    }

    public ActionResult ChangePassword()
    {
        return View(_changePasswordPath);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ChangePassword(ChangePasswordDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_changePasswordPath, dto);

            var userId = User.Identity.GetUserId();
            await _userService.ChangePasswordAsync(userId, dto.OldPassword ?? dto.CurrentPassword, dto.NewPassword).ConfigureAwait(false);

            // Audit log
            await _auditLogService.LogAsync(
                userId,
                "ChangePassword",
                "User",
                userId,
                User.Identity.Name,
                "Changed password",
                Request.UserHostAddress,
                Request.UserAgent
            ).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_changePasswordPath, dto);
    }

    [HttpPost]
    public async Task<ActionResult> GetActiveSessions()
    {
        try
        {
            var userId = User.Identity.GetUserId();
            var sessions = await _userService.GetActiveSessionsAsync(userId).ConfigureAwait(false);
            return Json(sessions, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }

    [HttpPost]
    public async Task<ActionResult> InvalidateSession(string sessionId)
    {
        try
        {
            var userId = User.Identity.GetUserId();
            await _userService.InvalidateSessionAsync(userId, sessionId).ConfigureAwait(false);

            // Audit log
            await _auditLogService.LogAsync(
                userId,
                "InvalidateSession",
                "Session",
                sessionId,
                "Session",
                $"Invalidated session: {sessionId}",
                Request.UserHostAddress,
                Request.UserAgent
            ).ConfigureAwait(false);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return GetJsonResult(exception);
        }
    }
    #endregion
}

