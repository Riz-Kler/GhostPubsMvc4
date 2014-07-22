using System.Web;
using Carnotaurus.GhostPubsMvc.Web.App_Start;

//using WebActivator;

[assembly: PreApplicationStartMethod(typeof (DontDropDbJustCreateTablesIfModelChangedStart), "Start")]

namespace Carnotaurus.GhostPubsMvc.Web.App_Start
{
    public static class DontDropDbJustCreateTablesIfModelChangedStart
    {
        public static void Start()
        {
            // Uncomment this line and replace CONTEXT_NAME with the name of your DbContext if you are
            // using your DbContext to create and manage your database
            // Database.SetInitializer(new DontDropDbJustCreateTablesIfModelChanged<CONTEXT_NAME>());
        }
    }
}