namespace canopy.TestPen.Controllers

open System.Web.Mvc
open data
open helper
open types
open Newtonsoft.Json

type ReadinessController() =
    inherit BaseController()

    member this.Index (id : int) =
        let pfsns = data.getPFSNReadiness id
        
        let get criticality testStatus =
            let pfsn = pfsns |> List.tryFind (fun pfsn -> pfsn.Criticality = criticality && pfsn.TestStatus = testStatus)
            match pfsn with
            | Some(pfsn) -> pfsn.cnt.Value
            | _ -> 0

        let total testStatus =
            pfsns 
            |> List.filter (fun pfsn -> pfsn.TestStatus = testStatus)
            |> List.map (fun pfsn -> pfsn.cnt.Value)
            |> List.sum
        
        //todo - refator this, has to be a more concise way
        let totals = (total "Pass") + (total "Fail") + (total "Skip") + (total "None")
        this.ViewData?TotalPass <- (total "Pass")
        this.ViewData?TotalFail <- (total "Fail")
        this.ViewData?TotalSkip <- (total "Skip")
        this.ViewData?TotalNone <- (total "None")
        this.ViewData?TotalPassPercent <- percent (total "Pass") totals
        this.ViewData?TotalFailPercent <- percent (total "Fail") totals
        this.ViewData?TotalSkipPercent <- percent (total "Skip") totals
        this.ViewData?TotalNonePercent <- percent (total "None") totals

        let totals = (get "High" "Pass") + (get "High" "Fail") + (get "High" "Skip") + (get "High" "None")
        this.ViewData?HighPass <- get "High" "Pass"
        this.ViewData?HighFail <- get "High" "Fail"
        this.ViewData?HighSkip <- get "High" "Skip"
        this.ViewData?HighNone <- get "High" "None"
        this.ViewData?HighPassPercent <- percent (get "High" "Pass") totals
        this.ViewData?HighFailPercent <- percent (get "High" "Fail") totals
        this.ViewData?HighSkipPercent <- percent (get "High" "Skip") totals
        this.ViewData?HighNonePercent <- percent (get "High" "None") totals

        let totals = (get "Medium" "Pass") + (get "Medium" "Fail") + (get "Medium" "Skip") + (get "Medium" "None")
        this.ViewData?MediumPass <- get "Medium" "Pass"
        this.ViewData?MediumFail <- get "Medium" "Fail"
        this.ViewData?MediumSkip <- get "Medium" "Skip"
        this.ViewData?MediumNone <- get "Medium" "None"
        this.ViewData?MediumPassPercent <- percent (get "Medium" "Pass") totals
        this.ViewData?MediumFailPercent <- percent (get "Medium" "Fail") totals
        this.ViewData?MediumSkipPercent <- percent (get "Medium" "Skip") totals
        this.ViewData?MediumNonePercent <- percent (get "Medium" "None") totals
        
        let totals = (get "Low" "Pass") + (get "Low" "Fail") + (get "Low" "Skip") + (get "Low" "None")
        this.ViewData?LowPass <- get "Low" "Pass"
        this.ViewData?LowFail <- get "Low" "Fail"
        this.ViewData?LowSkip <- get "Low" "Skip"
        this.ViewData?LowNone <- get "Low" "None"
        this.ViewData?LowPassPercent <- percent (get "Low" "Pass") totals
        this.ViewData?LowFailPercent <- percent (get "Low" "Fail") totals
        this.ViewData?LowSkipPercent <- percent (get "Low" "Skip") totals
        this.ViewData?LowNonePercent <- percent (get "Low" "Pass") totals
                
        this.ViewData?ReadinessRanByUserByDay <- getRanByUserByDay id |> JsonConvert.SerializeObject 
        this.ViewData?ReadinessByUserByDayByCriticality <- getRanByUserByDayByCriticality id |> JsonConvert.SerializeObject 
        this.ViewData?ReadinessErrors <- getReadinessErrors id

        this.View()