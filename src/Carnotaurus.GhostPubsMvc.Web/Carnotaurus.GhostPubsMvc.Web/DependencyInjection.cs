using System.Reflection;
using System.Web.Mvc;
using Carnotaurus.GhostPubsMvc.Common.Extensions.InjectionExtensions;
using Carnotaurus.GhostPubsMvc.Data;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;
using Carnotaurus.GhostPubsMvc.Managers.Implementation;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;

namespace Carnotaurus.GhostPubsMvc.Web
{
    public class DependencyInjection
    {
        public static void Configure()
        {
            var container = new Container();

            container.RegisterPerWebRequest<IDataContext>(() => new CmsContext());

            container.RegisterPerWebRequest<IReadStore, ReadStore>();

            container.RegisterPerWebRequest<IWriteStore, WriteStore>();

            container.Scan(typeof(CommandManager).Assembly);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            container.RegisterMvcIntegratedFilterProvider();

            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
    }
}