using System.Reflection;
using System.Web.Mvc;
using Carnotaurus.GhostPubsMvc.Data;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;
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

            //container.RegisterPerWebRequest<IMailSender, MailSender>();

            //container.Scan(typeof (PortalManager).Assembly);

            //container.Scan(typeof (PortalCommandService).Assembly);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            // This is an extension method from the integration package as well.
            container.RegisterMvcIntegratedFilterProvider();

            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
    }
}