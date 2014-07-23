using System.Web;

namespace VideoStore.ECommerce
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using NServiceBus;

    public class MvcApplication : HttpApplication
    {
        public static IBus Bus;

        protected void Application_Start()
        {
            var configure = Configure.With(builder => builder.Conventions(UnobtrusiveMessageConventions.Init) )
                .UseTransport<SqlServer>()
                .PurgeOnStartup(true)
                .RijndaelEncryptionService()
                .EnableInstallers();
            var startableBus = configure.CreateBus();
            Bus = startableBus.Start();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

    }
}
