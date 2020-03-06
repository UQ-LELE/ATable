using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ATable
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Restaurants", action = "Index", id = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "Default_Slug",
                url: "{controller}/{action}/{id}/{slug}",
                defaults: new { controller = "Restaurants", action = "Index", id = UrlParameter.Optional, slug = UrlParameter.Optional }
                   );
        }
    }
}