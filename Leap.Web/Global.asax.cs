using System.Web.Routing;
using System.ServiceModel.Activation;
using System;
using System.Linq;

namespace Leap.Web
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            log4net.Config.XmlConfigurator.Configure();
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            // Edit the base address of Service1 by replacing the "Service1" string below
            RouteTable.Routes.Add(new ServiceRoute("Leap", new WebServiceHostFactory(), typeof(Leap)));
        }
    }
}