using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Windsor;
using Microsoft.AspNet.Identity;

namespace Corno.Web.Attributes;

/// <summary>
/// Validates that the current user's session is still active in the database
/// </summary>
public class SessionValidationAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        // Skip validation for anonymous actions
        if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) ||
            filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
        {
            base.OnActionExecuting(filterContext);
            return;
        }

        var user = filterContext.HttpContext.User;
        
        if (user != null && user.Identity.IsAuthenticated)
        {
            try
            {
                var userService = Bootstrapper.Get<IUserService>();
                var userId = user.Identity.GetUserId();
                
                if (!string.IsNullOrEmpty(userId))
                {
                    // Check if user has an active session in database
                    // Use Task.Run to avoid deadlock when calling async from sync context
                    // This offloads the work to a thread pool thread without synchronization context
                    var hasActiveSession = Task.Run(async () => 
                        await userService.HasActiveSessionAsync(userId).ConfigureAwait(false)).GetAwaiter().GetResult();
                    
                    if (!hasActiveSession)
                    {
                        // Session was invalidated (e.g., user logged in elsewhere)
                        // Sign out and redirect to login
                        var ctx = HttpContext.Current;
                        var authManager = ctx.GetOwinContext().Authentication;
                        authManager.SignOut("ApplicationCookie");
                        
                        if (ctx.Session != null)
                        {
                            ctx.Session.Clear();
                            ctx.Session.Abandon();
                        }
                        
                        filterContext.Result = new RedirectToRouteResult(
                            new System.Web.Routing.RouteValueDictionary
                            {
                                { "controller", "Account" },
                                { "action", "Login" },
                                { "area", "" }
                            });
                        
                        // Optionally add a message
                        filterContext.Controller.TempData["ErrorMessage"] = 
                            "Your session has been terminated because you logged in from another location.";
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't prevent request
                System.Diagnostics.Debug.WriteLine($"Error validating session: {ex.Message}");
            }
        }
        
        base.OnActionExecuting(filterContext);
    }
}

