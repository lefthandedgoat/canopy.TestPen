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
        
        let total2 criticality testStatus =
            pfsns 
            |> List.filter (fun pfsn -> pfsn.Criticality = criticality && pfsn.TestStatus = testStatus)
            |> List.map (fun pfsn -> pfsn.cnt.Value)
            |> List.sum
        
        let totals = (total "Pass") + (total "Fail") + (total "Skip") + (total "None")
        this.ViewData?TotalPass <- (get "High" "Pass") + (get "Medium" "Pass") + (get "Low" "Pass")
        this.ViewData?TotalFail <- (get "High" "Fail") + (get "Medium" "Fail") + (get "Low" "Fail")
        this.ViewData?TotalSkip <- (get "High" "Skip") + (get "Medium" "Skip") + (get "Low" "Skip")
        this.ViewData?TotalNone <- (get "High" "None") + (get "Medium" "None") + (get "Low" "None")
        this.ViewData?TotalPassPercent <- percent (total "Pass") totals
        this.ViewData?TotalFailPercent <- percent (total "Fail") totals
        this.ViewData?TotalSkipPercent <- percent (total "Skip") totals
        this.ViewData?TotalNonePercent <- percent (total "None") totals

        let totals = (total2 "High" "Pass") + (total2 "High" "Fail") + (total2 "High" "Skip") + (total2 "High" "None")
        this.ViewData?HighPass <- get "High" "Pass"
        this.ViewData?HighFail <- get "High" "Fail"
        this.ViewData?HighSkip <- get "High" "Skip"
        this.ViewData?HighNone <- get "High" "None"
        this.ViewData?HighPassPercent <- percent (total2 "High" "Pass") totals
        this.ViewData?HighFailPercent <- percent (total2 "High" "Fail") totals
        this.ViewData?HighSkipPercent <- percent (total2 "High" "Skip") totals
        this.ViewData?HighNonePercent <- percent (total2 "High" "None") totals

        let totals = (total2 "Medium" "Pass") + (total2 "Medium" "Fail") + (total2 "Medium" "Skip") + (total2 "Medium" "None")
        this.ViewData?MediumPass <- get "Medium" "Pass"
        this.ViewData?MediumFail <- get "Medium" "Fail"
        this.ViewData?MediumSkip <- get "Medium" "Skip"
        this.ViewData?MediumNone <- get "Medium" "None"
        this.ViewData?MediumPassPercent <- percent (total2 "Medium" "Pass") totals
        this.ViewData?MediumFailPercent <- percent (total2 "Medium" "Fail") totals
        this.ViewData?MediumSkipPercent <- percent (total2 "Medium" "Skip") totals
        this.ViewData?MediumNonePercent <- percent (total2 "Medium" "None") totals
        
        let totals = (total2 "Low" "Pass") + (total2 "Low" "Fail") + (total2 "Low" "Skip") + (total2 "Low" "None")
        this.ViewData?LowPass <- get "Low" "Pass"
        this.ViewData?LowFail <- get "Low" "Fail"
        this.ViewData?LowSkip <- get "Low" "Skip"
        this.ViewData?LowNone <- get "Low" "None"
        this.ViewData?LowPassPercent <- percent (total2 "Low" "Pass") totals
        this.ViewData?LowFailPercent <- percent (total2 "Low" "Pass") totals
        this.ViewData?LowSkipPercent <- percent (total2 "Low" "Pass") totals
        this.ViewData?LowNonePercent <- percent (total2 "Low" "Pass") totals
        
        this.ViewData?ReadinessRanData <- getReadinessRan id |> JsonConvert.SerializeObject 
        this.ViewData?ReadinessRanByUserByDay <- getRanByUserByDay id |> JsonConvert.SerializeObject 
        this.ViewData?ReadinessByUserByDayByCriticality <- getRanByUserByDayByCriticality id |> JsonConvert.SerializeObject 
        this.ViewData?ReadinessErrors <- getReadinessErrors id

        this.View()