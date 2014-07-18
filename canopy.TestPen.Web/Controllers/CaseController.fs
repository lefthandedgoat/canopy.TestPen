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

    member this.Pass id = data.pass id this.user
    
    member this.Fail id = data.fail id this.user
    
    member this.Skip id = data.skip id this.user

    member this.Comment id comment = data.saveComment id (sprintf "%s- %s" this.user comment)

    member this.Claim id = data.claim id this.user
    
    member this.Unclaim id = data.unclaim id