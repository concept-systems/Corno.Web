using System.Web;
using Microsoft.AspNet.Identity;

namespace Corno.Web.Models;

public class SessionGlobals
{
    #region -- Properties --
    public int? CompanyId { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public int? CollegeId { get; set; }
    public string CollegeName { get; set; }
    #endregion

    #region -- Methods --   

    public static SessionGlobals MySession
    {
        get => GetOrCreateSession(HttpContext.Current.User.Identity.GetUserId());
        set
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            HttpContext.Current.Session[userId] = value;
        }
    }


    public static SessionGlobals GetOrCreateSession(string userId)
    {
        if (HttpContext.Current.Session[userId] is SessionGlobals session) return session;

        session = new SessionGlobals();
        HttpContext.Current.Session[userId] = session;

        return session;
    }
    #endregion
}