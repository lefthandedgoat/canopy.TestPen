namespace canopy.TestPen.Controllers

open System.Web.Mvc
open data
open helper
open types
open Newtonsoft.Json

type HistoryController() =
    inherit BaseController()

    member this.Index () =
        this.ViewData?HistoryRanData <- getHistoryRan () |> JsonConvert.SerializeObject 

        this.View()