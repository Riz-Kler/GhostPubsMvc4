using System;
using System.Linq;
using System.Reflection;
using SimpleInjector;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions.InjectionExtensions
{
    public static class DependencyInjectionScanner
    {
        public static void Scan(this Container container, Assembly repositoryAssembly,
            string namespacePrefix = "ClientPortal")
        {
            var registrations =
                from type in repositoryAssembly.GetExportedTypes()
                where type.Namespace != null && type.Namespace.StartsWith(namespacePrefix)
                where type.GetInterfaces().Any(x => x.Namespace.StartsWith(namespacePrefix))
                where Attribute.GetCustomAttribute(type, typeof (ExcludeFromDiRegistrationAttribute)) == null
                select new
                {
                    Service = type.GetInterfaces().First(x => x.Namespace.StartsWith(namespacePrefix)),
                    Implementation = type
                };

            foreach (var reg in registrations)
            {
                container.Register(reg.Service, reg.Implementation, Lifestyle.Transient);
            }
        }
    }
}