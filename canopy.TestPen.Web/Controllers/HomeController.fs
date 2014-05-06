namespace canopy.TestPen.Controllers

open System.Web.Mvc

type HomeController() =
    inherit BaseController()

    member this.Index () = this.View()