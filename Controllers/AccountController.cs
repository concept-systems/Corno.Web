using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Corno.Web.Logger;
using Corno.Web.Models;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Areas.Admin.Models;
using Corno.Web.Windsor;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Corno.Web.Controllers;

public class AccountController : BaseAccountController
{
    public AccountController(IUserService userService)
    {
        UserService = userService;
    }


    #region -- Data Members --
    private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
    private IUserService UserService;// => Bootstrapper.Get<IUserService>();
    #endregion

    //
    // GET: /Account/Login
    [AllowAnonymous]
    //[ValidateAntiForgeryToken]
    public async Task<ActionResult> Login(string returnUrl)
    {
        return await Task.FromResult(View()).ConfigureAwait(false);
    }

    // POST: /Account/Login
    [HttpPost]
    [AllowAnonymous]
    //[ValidateAntiForgeryToken]
    public async Task<ActionResult> Login(LoginViewModel viewModel, string returnUrl)
    {
        try
        {
            if (!ModelState.IsValid)
                throw new Exception(@"Invalid username or password.");

            if (string.IsNullOrEmpty(viewModel.UserName) || string.IsNullOrEmpty(viewModel.Password))
                throw new Exception("Invalid user name or password.");

            var user = await UserManager.FindAsync(viewModel.UserName, viewModel.Password) ?? throw new Exception("Invalid user name or password.");
            
            // Check for existing active session
            var hasActiveSession = await UserService.HasActiveSessionAsync(user.Id).ConfigureAwait(false);
            var existingSession = await UserService.GetActiveSessionAsync(user.Id).ConfigureAwait(false);
            
            if (hasActiveSession && existingSession != null)
            {
                // Option 2: Auto-logout previous session (RECOMMENDED)
                // Invalidate the previous session
                await UserService.InvalidateAllSessionsAsync(user.Id).ConfigureAwait(false);
                
                // Optionally, you can store a message to show to the user
                TempData["InfoMessage"] = "You were logged out from another session. Please login again.";
            }

            // Get client IP address
            var ipAddress = GetClientIpAddress();

            // Update login history BEFORE signing in
            try
            {
                await UserService.UpdateLoginHistoryAsync(
                    viewModel.UserName, 
                    ipAddress, 
                    LoginResult.Success
                ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Log the error but don't prevent login
                // You might want to log this to a logging service
                System.Diagnostics.Debug.WriteLine($"Error updating login history: {ex.Message}");
            }

            user.Email = user.UserName;
            var identity = await SignInManager.CreateUserIdentityAsync(user);

            // Sign in the user using the identity
            AuthenticationManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = viewModel.RememberMe
            }, identity);

            // Always redirect to home page after login
            // This ensures users don't get stuck on expired session pages
            // If returnUrl is provided and valid, we could redirect there, but for security
            // and to avoid expired session issues, we always go to home
            return RedirectToAction("Index", "Home");
        }
        catch (Exception exception)
        {
            ModelState.AddModelError("Error", LogHandler.GetDetailException(exception).Message);
        }

        return View(viewModel);
    }

    [HttpPost]
    public async Task<ActionResult> LogOff()
    {
        try
        {
            // Update logout history
            var userId = User.Identity.GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                await UserService.UpdateLogoutHistoryAsync(userId).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            // Log error but continue with logout
            System.Diagnostics.Debug.WriteLine($"Error updating logout history: {ex.Message}");
        }

        var ctx = Request.GetOwinContext();
        var authManager = ctx.Authentication;
        authManager.SignOut("ApplicationCookie");
        return await Task.FromResult(RedirectToAction("Login", "Account")).ConfigureAwait(false);
    }

    /// <summary>
    /// Keep-alive endpoint to refresh session on user activity
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult> KeepAlive()
    {
        // Touch the session to keep it alive
        if (Session != null)
        {
            Session["LastActivity"] = DateTime.Now;
        }

        return await Task.FromResult(Json(new { success = true, message = "Session refreshed" }, JsonRequestBehavior.AllowGet)).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the client IP address
    /// </summary>
    private string GetClientIpAddress()
    {
        try
        {
            var ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = Request.ServerVariables["REMOTE_ADDR"];
            }
            else
            {
                // X-Forwarded-For can contain multiple IPs, get the first one
                var addresses = ipAddress.Split(',');
                if (addresses.Length > 0)
                {
                    ipAddress = addresses[0].Trim();
                }
            }

            return ipAddress ?? "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }
}