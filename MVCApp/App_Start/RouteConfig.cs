using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                name: "nameIs",
                url: "user/{name}",
                defaults: new { controller = "Person", action = "FindByFullName" },
                constraints: new { name = @"^[a-zA-Z]+_[a-zA-z]+$" }
            );

            routes.MapRoute(
                name: "nameHas",
                url: "users/{name}",
                defaults: new { controller = "Person", action = "FindByName"},
                constraints: new {name = @"^[a-zA-Z]+$"}
                );

            routes.MapRoute(
                name: "edit",
                url: "user/{id}/edit",
                defaults: new { controller = "Person", action = "EditById"},
                constraints: new { id = @"\d*" }
            );

            routes.MapRoute(
                name: "awards",
                url: "awards",
                defaults: new { controller = "Badge", action = "FindAll" }
            );


            routes.MapRoute(
                name: "awardById",
                url: "award/{id}",
                defaults: new { controller = "Badge", action = "FindById" },
                constraints: new { id = @"\d*" }
            );

            routes.MapRoute(
                name: "awardByFullTitle",
                url: "award/{title}",
                defaults: new { controller = "Badge", action = "FindByFullTitle" }
            );

            routes.MapRoute(
                name: "awardByTitle",
                url: "awards/{title}",
                defaults: new { controller = "Badge", action = "FindByTitle" }
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
