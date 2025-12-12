using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Corno.Web.Logger;
using Corno.Web.Models;
using Microsoft.Owin.Security;

namespace Corno.Web.Controllers;

public class AccountController : BaseAccountController
{
    #region -- Data Members --
    private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
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

            if (Url.IsLocalUrl(returnUrl) &&
                returnUrl.IndexOf("Account/Login", StringComparison.OrdinalIgnoreCase) == -1)
            {
                return Redirect(returnUrl);
            }

            if (string.IsNullOrEmpty(viewModel.UserName) || string.IsNullOrEmpty(viewModel.Password))
                throw new Exception("Invalid user name or password.");

            var user = await UserManager.FindAsync(viewModel.UserName, viewModel.Password) ?? throw new Exception("Invalid user name or password.");
            {
                user.Email = user.UserName;
                var identity = await SignInManager.CreateUserIdentityAsync(user);

                // Sign in the user using the identity
                AuthenticationManager.SignIn(new AuthenticationProperties
                {
                    IsPersistent = viewModel.RememberMe
                }, identity);

                // Redirect to the desired page after successful login
                return RedirectToAction("Index", "Home");
            }
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
        var ctx = Request.GetOwinContext();
        var authManager = ctx.Authentication;
        authManager.SignOut("ApplicationCookie");
        return await Task.FromResult(RedirectToAction("Login", "Account")).ConfigureAwait(false);
    }
}