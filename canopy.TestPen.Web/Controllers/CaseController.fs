namespace canopy.TestPen.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Web
open System.Web.Mvc
open System.Web.Mvc.Ajax
open data
open helper
open types

type CaseController() =
    inherit BaseController()
    
    member this.Index (id : int) =
        let case = data.getCaseById id
        let scenarios = data.getScenarios case.Id

        this.ViewData?Case <- case
        this.ViewData?Scenarios <- scenarios
        this.ViewData?PFSN <- data.getPFSNHeader case.RunId        
        
        this.ViewData?Manual <- scenarios |> List.filter(fun scenario -> scenario.TestExecutionType = "Manual")
        this.ViewData?Automated <- scenarios |> List.filter(fun scenario -> scenario.TestExecutionType = "Automated")
        this.ViewData?Bulk <- scenarios |> List.filter(fun scenario -> scenario.TestExecutionType = "Bulk")
        
        let a = getPFSNByCase case.Id "Manual"
        let b = getPFSNByCase case.Id "Automated"
        let c = getPFSNByCase case.Id "Bulk"

        this.ViewData?ManualPFSN <- getPFSNByCase case.Id "Manual"
        this.ViewData?AutomatedPFSN <- getPFSNByCase case.Id "Automated"
        this.ViewData?BulkPFSN <- getPFSNByCase case.Id "Bulk"
        
        this.View()