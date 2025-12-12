using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Corno.Web.Startup))]
namespace Corno.Web;

public partial class Startup
{
    public void Configuration(IAppBuilder app)
    {
        ConfigureAuth(app);
        app.MapSignalR();
    }
}