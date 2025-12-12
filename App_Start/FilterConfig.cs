using System.Web.Mvc;
using Corno.Web.Attributes;

namespace Corno.Web;

public class FilterConfig
{
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
        // Add global exception filter for professional error handling
        filters.Add(new GlobalExceptionFilterAttribute());
        filters.Add(new HandleErrorAttribute());
    }
}