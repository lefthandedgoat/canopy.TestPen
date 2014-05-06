namespace canopy.TestPen

open System
open System.Net.Http
open System.Web
open System.Web.Http
open System.Web.Mvc
open System.Web.Routing
open System.Web.Optimization

type BundleConfig() =
    static member RegisterBundles (bundles:BundleCollection) =
        
        // Use the development version of Modernizr to develop with and learn from. Then, when you're
        // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
        bundles.Add(ScriptBundle("~/bundles/modernizr").Include([|"~/js/modernizr-*"|]))

        bundles.Add(ScriptBundle("~/bundles/bootstrap").Include(
                        "~/js/jquery.tablesorter.min.js",
                        "~/js/bootstrap.js",
                        "~/js/theme.js",
                        "~/js/respond.js"));
                        
        bundles.Add(StyleBundle("~/css").Include(
                        "~/css/bootstrap.css",
                        "~/css/bootstrap-overrides.css",
                        "~/css/layout.css",
                        "~/css/elements.css",
                        "~/css/icons.css",
                        "~/css/font-awesome.css",
                        "~/css/tables.css"))

/// Route for ASP.NET MVC applications
type Route = { 
    controller : string
    action : string
    id : UrlParameter }

type HttpRoute = {
    controller : string
    id : RouteParameter }

type Global() =
    inherit System.Web.HttpApplication() 
    
    static member RegisterFilters(filters: GlobalFilterCollection) =
        filters.Add(new HandleErrorAttribute())

    static member RegisterRoutes(routes:RouteCollection) =
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}")
        routes.MapRoute(
            "Default", // Route name
            "{controller}/{action}/{id}", // URL with parameters
            { controller = "Home"; action = "Index"; id = UrlParameter.Optional } // Parameter defaults
        ) |> ignore

    member x.Application_Start() =
        AreaRegistration.RegisterAllAreas()
        Global.RegisterFilters(GlobalFilters.Filters)
        Global.RegisterRoutes(RouteTable.Routes)
        BundleConfig.RegisterBundles BundleTable.Bundles