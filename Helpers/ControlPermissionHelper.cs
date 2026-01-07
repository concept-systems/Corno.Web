using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Corno.Web.Areas.Admin.Services.Interfaces;
using Corno.Web.Windsor;
using Microsoft.AspNet.Identity;

namespace Corno.Web.Helpers;

/// <summary>
/// Helper class for control-level permission checking and rendering
/// </summary>
public static class ControlPermissionHelper
{
    /// <summary>
    /// Checks if user has access to a control
    /// </summary>
    public static bool HasControlAccess(this HtmlHelper helper, string controlId, string controller = null, string action = null)
    {
        var userId = HttpContext.Current?.User?.Identity?.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return false;

        var permissionService = Bootstrapper.Get<IPermissionService>();
        return permissionService.HasControlAccessAsync(userId, controlId, controller, action).Result;
    }

    /// <summary>
    /// Renders a button only if user has permission
    /// </summary>
    public static MvcHtmlString RenderButtonIfAllowed(this HtmlHelper helper, 
        string controlId, string buttonText, string action, string controller, 
        object htmlAttributes = null)
    {
        if (!helper.HasControlAccess(controlId, controller, action))
            return MvcHtmlString.Empty;

        var tag = new TagBuilder("button");
        tag.Attributes.Add("id", controlId);
        tag.Attributes.Add("type", "button");
        tag.SetInnerText(buttonText);

        if (htmlAttributes != null)
        {
            var props = htmlAttributes.GetType().GetProperties();
            foreach (var prop in props)
            {
                var value = prop.GetValue(htmlAttributes);
                if (value != null)
                    tag.Attributes.Add(prop.Name, value.ToString());
            }
        }

        return new MvcHtmlString(tag.ToString());
    }

    /// <summary>
    /// Renders a link only if user has permission
    /// </summary>
    public static MvcHtmlString RenderLinkIfAllowed(this HtmlHelper helper,
        string controlId, string linkText, string action, string controller, 
        string area = null, object htmlAttributes = null)
    {
        if (!helper.HasControlAccess(controlId, controller, action))
            return MvcHtmlString.Empty;

        var url = string.IsNullOrEmpty(area)
            ? helper.Action(action, controller).ToString()
            : helper.Action(action, controller, new { area }).ToString();

        var tag = new TagBuilder("a");
        tag.Attributes.Add("id", controlId);
        tag.Attributes.Add("href", url);
        tag.SetInnerText(linkText);

        if (htmlAttributes != null)
        {
            var props = htmlAttributes.GetType().GetProperties();
            foreach (var prop in props)
            {
                var value = prop.GetValue(htmlAttributes);
                if (value != null)
                    tag.Attributes.Add(prop.Name, value.ToString());
            }
        }

        return new MvcHtmlString(tag.ToString());
    }

    /// <summary>
    /// Checks if user has page access
    /// </summary>
    public static bool HasPageAccess(this HtmlHelper helper, string controller, string action, string area = null)
    {
        var userId = HttpContext.Current?.User?.Identity?.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return false;

        var permissionService = Bootstrapper.Get<IPermissionService>();
        return permissionService.HasPageAccessAsync(userId, controller, action, area).Result;
    }
}

