using System;
using System.IO;
using System.Web.Mvc;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static string RenderRazorViewToString(this Controller controller, string viewName, object model)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller", "Extension method called on a null controller");
            }

            if (controller.ControllerContext == null)
            {
                return string.Empty;
            }

            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData,
                    controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public static string GetViewTemplate(this ControllerBase controller, string viewName = null)
        {
            // http://stackoverflow.com/questions/10128684/asp-net-mvc3-physical-location-of-view-from-controller/10129852#10129852
            // string viewPath = @"C:\test\GhostPubsMvc4\Carnotaurus.GhostPubsMvc.Web\Carnotaurus.GhostPubsMvc.Web\Views\Home\about.cshtml"; //"Hello @Model.Name! Welcome to Razor!";

            var viewPath = GetPhysicalViewPath(controller, viewName);

            var template = File.ReadAllText(viewPath);

            return template;
        }

        public static string GetPhysicalViewPath(this ControllerBase controller, string viewName = null)
        {
            var virtualPath = GetVirtualViewPath(controller, viewName);

            return controller.ControllerContext.HttpContext.Server.MapPath(virtualPath);
        }

        public static string PrepareView(this Controller controller, Object model, String viewName = null)
        {
            if (String.IsNullOrEmpty(viewName))
            {
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");
            }

            var viewPath = GetVirtualViewPath(controller, viewName);

            var html = RenderRazorViewToString(controller, viewPath, model);

            return html;
        }

        public static string GetVirtualViewPath(this ControllerBase controller, string viewName = null)
        {
            String path = null;

            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            var context = controller.ControllerContext;

            if (string.IsNullOrEmpty(viewName))
            {
                viewName = context.RouteData.GetRequiredString("action");
            }

            var result = ViewEngines.Engines.FindView(context, viewName, null);

            var compiledView = result.View as BuildManagerCompiledView;

            if (compiledView != null)
            {
                path = compiledView.ViewPath;
            }

            return path;
        }
    }
}