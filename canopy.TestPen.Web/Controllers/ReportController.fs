namespace canopy.TestPen.Controllers

open System.Web.Mvc
open data
open helper
open types

type ReportController() =
    inherit BaseController()
    
    let tShirtSizeId = 1
    let statusId = 2
    let testExecutionTypeId = 3
    let totalScenariosId = 4
    let criticalityId = 5
    let totalCasesId = 6
    let percentAutomatedId = 7

    member this.Index (id : int) =        
        let reports = data.getReports id        
        let ran = getRan id
        let pfsn = data.getPFSNHeader id
        let totalScenarios = reports |> count totalScenariosId
        
        this.ViewData?Statuses <- reports |> toJson statusId
        this.ViewData?TShirtSizes <- reports |> toJson tShirtSizeId
        this.ViewData?TestTypes <- reports |> toJson testExecutionTypeId
        this.ViewData?Criticality <- reports |> toJson criticalityId
        this.ViewData?TotalScenarios <- totalScenarios
        this.ViewData?TotalCases <- reports |> count totalCasesId
        this.ViewData?PercentAutomated <- reports |> count percentAutomatedId
        this.ViewData?PercentRun <- percent ran totalScenarios
        this.ViewData?PercentPasssed <- percent pfsn.Pass totalScenarios
        this.ViewData?PercentFailed <- percent pfsn.Fail totalScenarios
        this.ViewData?PercentSkipped <- percent pfsn.Skip totalScenarios
        this.ViewData?PercentNone <- percent pfsn.None totalScenarios
        this.View()