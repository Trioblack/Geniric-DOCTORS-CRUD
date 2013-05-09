using System.Web;
using System.Web.Http;
using System.Web.Http.WebHost;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace AB2EDEMO
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional }
            );

            routes.MapRoute("Default", "{controller}/{action}/{id}",
                //defaults: new { controller = "WorkWithCustomer", action = "HospitalGrid", id = UrlParameter.Optional }
                new { controller = "Login", action = "Index", id = UrlParameter.Optional }

            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            RazorViewEngine rve = new RazorViewEngine();
            rve.PartialViewLocationFormats =
                rve.ViewLocationFormats =
                    rve.MasterLocationFormats =
                        new[] {
                   "~/Views/{0}.cshtml",
                   "~/Views/Shared/{0}.cshtml"};
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(rve);
            BundleTable.Bundles.RegisterTemplateBundles();

        }
    }
    public class MyHttpControllerHandler : HttpControllerHandler, IRequiresSessionState
    {
        public MyHttpControllerHandler(RouteData routeData)
            : base(routeData)
        {
        }
    }
    public class MyHttpControllerRouteHandler : HttpControllerRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new MyHttpControllerHandler(requestContext.RouteData);
        }
    }

}