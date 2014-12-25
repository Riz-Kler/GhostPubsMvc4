using System;
using System.IO;
using System.Web.Mvc;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static string RenderRazorViewToString(this ControllerBase controller, string viewName, object model)
        {
            if (controller == null)
                throw new ArgumentNullException("controller", "Extension method called on a null controller");
            if (viewName == null) throw new ArgumentNullException("viewName");
            if (model == null) throw new ArgumentNullException("model");

            if (controller.ControllerContext == null)
            {
                return string.Empty;
            }

            controller.ViewData.Model = model;

            using (var stringWriter = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);

                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData,
                    controller.TempData, stringWriter);

                viewResult.View.Render(viewContext, stringWriter);

                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);

                var result = stringWriter.GetStringBuilder().ToString();

                return result;
            }
        }

        public static string GetViewTemplate(this ControllerBase controller, string viewName = null)
        {
            if (viewName == null) throw new ArgumentNullException("viewName");

            var physicalViewPath = GetPhysicalViewPath(controller, viewName);

            var result = File.ReadAllText(physicalViewPath);

            return result;
        }

        public static string GetPhysicalViewPath(this ControllerBase controller, string viewName = null)
        {
            if (viewName == null) throw new ArgumentNullException("viewName");

            var virtualViewPath = GetVirtualViewPath(controller, viewName);

            var result = controller.ControllerContext.HttpContext.Server.MapPath(virtualViewPath);

            return result;
        }

        public static string PrepareView(this ControllerBase controller, Object model, String viewName = null)
        {
            if (model == null) throw new ArgumentNullException("model");

            if (viewName == null) throw new ArgumentNullException("viewName");

            if (viewName.IsNullOrEmpty())
            {
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");
            }

            var viewPath = GetVirtualViewPath(controller, viewName);

            var result = RenderRazorViewToString(controller, viewPath, model);

            return result;
        }

        public static string GetVirtualViewPath(this ControllerBase controller, string viewName = null)
        {
            String result = null;

            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            if (viewName == null) throw new ArgumentNullException("viewName");

            var context = controller.ControllerContext;

            if (viewName.IsNullOrEmpty())
            {
                viewName = context.RouteData.GetRequiredString("action");
            }

            var findView = ViewEngines.Engines.FindView(context, viewName, null);

            var buildManagerCompiledView = findView.View as BuildManagerCompiledView;

            if (buildManagerCompiledView != null)
            {
                result = buildManagerCompiledView.ViewPath;
            }

            return result;
        }
    }
}