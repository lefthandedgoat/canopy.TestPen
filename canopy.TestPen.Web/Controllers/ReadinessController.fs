namespace canopy.TestPen.Controllers

open System.Web.Mvc
open data
open helper
open types

type ReadinessController() =
    inherit BaseController()

    member this.Index (id : int) =
        let pfsns = data.getPFSNReadiness id
        let get criticality testStatus =
            let pfsn = pfsns |> List.tryFind (fun pfsn -> pfsn.Criticality = criticality && pfsn.TestStatus = testStatus)
            match pfsn with
            | Some(pfsn) -> pfsn.cnt.Value
            | _ -> 0
                
        this.ViewData?TotalPass <- (get "High" "Pass") + (get "Medium" "Pass") + (get "Low" "Pass")
        this.ViewData?TotalFail <- (get "High" "Fail") + (get "Medium" "Fail") + (get "Low" "Fail")
        this.ViewData?TotalSkip <- (get "High" "Skip") + (get "Medium" "Skip") + (get "Low" "Skip")
        this.ViewData?TotalNone <- (get "High" "None") + (get "Medium" "None") + (get "Low" "None")
        this.ViewData?HighPass <- get "High" "Pass"
        this.ViewData?HighFail <- get "High" "Fail"
        this.ViewData?HighSkip <- get "High" "Skip"
        this.ViewData?HighNone <- get "High" "None"
        this.ViewData?MediumPass <- get "Medium" "Pass"
        this.ViewData?MediumFail <- get "Medium" "Fail"
        this.ViewData?MediumSkip <- get "Medium" "Skip"
        this.ViewData?MediumNone <- get "Medium" "None"
        this.ViewData?LowPass <- get "Low" "Pass"
        this.ViewData?LowFail <- get "Low" "Fail"
        this.ViewData?LowSkip <- get "Low" "Skip"
        this.ViewData?LowNone <- get "Low" "None"
        this.View()