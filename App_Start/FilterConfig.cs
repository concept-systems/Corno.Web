using System.Web.Mvc;
using Corno.Web.Attributes;

namespace Corno.Web;

public class FilterConfig
{
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
        // Add global authorization filter - all controllers require authentication by default
        // Controllers/actions with [AllowAnonymous] will still be accessible
        filters.Add(new AuthorizeAttribute());
        
        // Add session validation to ensure single session per user
        filters.Add(new SessionValidationAttribute());
        
        // Add global exception filter for professional error handling
        filters.Add(new GlobalExceptionFilterAttribute());
        filters.Add(new HandleErrorAttribute());
    }
}