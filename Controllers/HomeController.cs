using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Corno.Web.Globals;
using Corno.Web.Services.Masters.Interfaces;
using Kendo.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SiteMapNode = Kendo.Mvc.SiteMapNode;

namespace Corno.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    #region -- Constructors --

    public HomeController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    #endregion

    #region -- Data Members --

    private readonly IProjectService _projectService;
    private IList<string> _userRoles;

    #endregion

    #region -- Private Methods --

    private void AddXmlNode(IEnumerable documentChild, SiteMapNode sitemapNode)
    {
        foreach (XmlNode node in documentChild)
        {
            var assignedRoles = node?.Attributes?["roles"]?.Value?.Split().ToList();
            if (assignedRoles == null) continue;
            for (var index = 0; index < assignedRoles.Count; index++)
            {
                assignedRoles[index] = assignedRoles[index].Trim();
                assignedRoles[index] = assignedRoles[index].ToLower();
            }
            if (!_userRoles.Intersect(assignedRoles).Any())
                continue;

            if (node?.Attributes == null) continue;

            var siteMapNode = new SiteMapNode
            {
                Title = node.Attributes["title"]?.Value,
                ControllerName = node.Attributes["controller"]?.Value,
                ActionName = node.Attributes["action"]?.Value,
            };

            // Preserve additional attributes from sitemap for later use (e.g., icons, names)
            var miscType = node.Attributes[FieldConstants.MiscType.ToLower()]?.Value;
            if (!string.IsNullOrEmpty(miscType))
                siteMapNode.Attributes.Add(new KeyValuePair<string, object>(FieldConstants.MiscType, miscType));

            var icon = node.Attributes["icon"]?.Value;
            if (!string.IsNullOrEmpty(icon))
            {
                siteMapNode.Attributes.Add(new KeyValuePair<string, object>("icon", icon));
            }

            var name = node.Attributes["name"]?.Value;
            if (!string.IsNullOrEmpty(name))
            {
                siteMapNode.Attributes.Add(new KeyValuePair<string, object>("name", name));
            }
            var area = node.Attributes["area"]?.Value;
            siteMapNode.RouteValues.Add("area", area);
            siteMapNode.RouteValues.Add(FieldConstants.MiscType, node.Attributes[FieldConstants.MiscType.ToLower()]?.Value);
            siteMapNode.RouteValues.Add(FieldConstants.ReportName, node.Attributes["reportName"]?.Value);
            siteMapNode.RouteValues.Add(FieldConstants.Title, node.Attributes[FieldConstants.Title.ToLower()]?.Value);
            siteMapNode.RouteValues.Add("web", node.Attributes["web".ToLower()]?.Value);

            sitemapNode.ChildNodes.Add(siteMapNode);
            if (node.HasChildNodes)
                AddXmlNode(node, siteMapNode);
        }
    }
    #endregion

    #region -- Actions --
    public async Task<ActionResult> Index()
    {
        var project = await _projectService.GetProjectAsync("Active").ConfigureAwait(false);
        ApplicationGlobals.ProjectId = project.Id;

        //_userRoles = _identityService.GetUserRoles(User.Identity.GetUserId()).ToList();
        var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        var userId = User.Identity.GetUserId();
        _userRoles = await Task.Run(() => userManager.GetRoles(userId).ToList()).ConfigureAwait(false);
        for (var index = 0; index < _userRoles.Count; index++)
        {
            _userRoles[index] = _userRoles[index].Trim();
            _userRoles[index] = _userRoles[index].ToLower();
        }

        // Load SiteMap
        TextReader textReader = new StringReader(project.MenuXml);
        var document = new XmlDocument();
        document.Load(textReader);

        var siteMap = new XmlSiteMap();
        AddXmlNode(document.FirstChild, siteMap.RootNode);

        //SiteMapManager.SiteMaps.Remove(FieldConstants.Project);
        ////SiteMapManager.SiteMaps.Add(new KeyValuePair<string, SiteMapBase>(FieldConstants.Project, siteMap));
        //var sitemapName = User.Identity.GetUserId();
        //SiteMapManager.SiteMaps.Add(new KeyValuePair<string, SiteMapBase>(sitemapName, siteMap));


        var sitemapName = User.Identity.GetUserId();

        // Remove existing entry if it exists
        SiteMapManager.SiteMaps.Remove(sitemapName);

        // Now add the new sitemap
        SiteMapManager.SiteMaps.Add(new KeyValuePair<string, SiteMapBase>(sitemapName, siteMap));

        // Pass sitemap root node to view for menu display
        ViewBag.SiteMapRoot = siteMap.RootNode;

        return View();
    }

    public async Task<ActionResult> About()
    {
        ViewBag.Message = "Your application description page.";

        return await Task.FromResult(View()).ConfigureAwait(false);
    }

    public async Task<ActionResult> Contact()
    {
        ViewBag.Message = "Your contact page.";

        return await Task.FromResult(View()).ConfigureAwait(false);
    }

    public async Task<ActionResult> Chat()
    {
        return await Task.FromResult(View()).ConfigureAwait(false);
    }
    #endregion
}