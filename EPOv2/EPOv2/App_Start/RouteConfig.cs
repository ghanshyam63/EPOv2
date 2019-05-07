using System.Web.Mvc;
using System.Web.Routing;

namespace EPOv2
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "ApproveCapex",
               url: "ApproveCapex/{id}",
               defaults: new { controller = "Capex", action = "ApproveCapexExternal", id = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "ViewOrder",
                url: "ViewOrder/{id}",
                defaults: new { controller = "PurchaseOrder", action = "ViewOrderExternal", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "EditOrder",
                url: "EditOrder/{id}",
                defaults: new { controller = "PurchaseOrder", action = "EditOrderExternal", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "AuthoriseOrder",
               url: "AuthoriseOrder/{id}",
               defaults: new { controller = "PurchaseOrder", action = "AuthoriseOrderExternal", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "DeleteOrderExternal",
               url: "DeleteOrder/{id}",
               defaults: new { controller = "PurchaseOrder", action = "DeleteOrderExternal", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "OpenPDFPOExternal",
               url: "OpenPDFPO/{id}",
               defaults: new { controller = "PurchaseOrder", action = "OpenPDFPOExternal", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "SendPObyEmailExternal",
               url: "SendPObyEmail/{id}",
               defaults: new { controller = "PurchaseOrder", action = "SendPObyEmailExternal", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "CancelOrderExternal",
               url: "CancelOrder/{id}",
               defaults: new { controller = "PurchaseOrder", action = "CancelOrderExternal", id = UrlParameter.Optional }
           );

            routes.MapRoute(
              name: "MatchOrderExternal",
              url: "MatchOrder/{id}",
              defaults: new { controller = "PurchaseOrder", action = "MatchOrderExternal", id = UrlParameter.Optional }
          );

            routes.MapRoute(
              name: "CloseOrderExternal",
              url: "CloseOrder/{id}",
              defaults: new { controller = "PurchaseOrder", action = "CloseOrderExternal", id = UrlParameter.Optional }
          );

            routes.MapRoute(
              name: "AuthoriseInvoiceExternal",
              url: "AuthoriseInvoice/{id}",
              defaults: new { controller = "Ettacher", action = "AuthoriseInvoiceExternal", id = UrlParameter.Optional }
          );

            routes.MapRoute(
                name: "SearchHomeRoute",
                url: "Search/{id}",
                defaults: new { controller = "Home", action = "Search", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "HomeRoute",
                url: "Dashboard/{id}",
                defaults: new { controller = "Home", action = "Dashboard", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
