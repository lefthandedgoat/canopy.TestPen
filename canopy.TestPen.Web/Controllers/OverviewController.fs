namespace canopy.TestPen.Controllers

open System.Web.Mvc
open data
open helper
open types

type OverviewController() =
    inherit BaseController()
    
    member this.Index (id : int) =
        let summaries = data.getCaseSummaries id
        let passFailSkipNone = data.getPassFailSkipNone id
        let summaries = data.mapPassFailSkipNoneToSummaries passFailSkipNone summaries
        this.ViewData?CaseSummaries <- summaries
        this.ViewData?PFSN <- data.getPFSNHeader id
        this.View()