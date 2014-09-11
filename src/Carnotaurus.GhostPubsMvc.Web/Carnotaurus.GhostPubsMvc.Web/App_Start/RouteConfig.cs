using System.Web.Mvc;
using System.Web.Routing;

namespace Carnotaurus.GhostPubsMvc.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Default", "{controller}/{action}/{id}",
                new {controller = "Template", action = "Generate", id = UrlParameter.Optional}
                );
        }
    }
}