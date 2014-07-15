namespace canopy.TestPen.Controllers

open System.Web
open System.Web.Mvc
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
        
        this.ViewData?ManualPFSN <- getPFSNByCase case.Id "Manual"
        this.ViewData?AutomatedPFSN <- getPFSNByCase case.Id "Automated"
        this.ViewData?BulkPFSN <- getPFSNByCase case.Id "Bulk"
        
        this.ViewData?Inputs <- data.getInputs id
        this.ViewData?Expecteds <- data.getExpecteds id
        this.ViewData?Attributes <- data.getAttributes id
        this.ViewData?Affects <- data.getAffects id
        this.ViewData?Configurations <- data.getConfigurations id
        this.ViewData?Steps <- data.getSteps id

        this.View()

    member this.Pass (id: int) = 
        data.pass id
    
    member this.Fail (id: int) = data.fail id
    
    member this.Skip (id: int) = data.skip id

    member this.Comment (id: int) (comment: string) = 
        data.saveComment id comment