namespace canopy.TestPen.Controllers

open System
open System.Linq
open System.Web
open System.Web.Mvc
open System.Web.Mvc.Ajax
open data
open FSharp.Data
open System.IO
open helper
open types

type ApiController() =
    inherit BaseController()

    let read (controller : Controller) =
        use strea = controller.HttpContext.Request.InputStream
        let reader = new StreamReader(strea)
        reader.ReadToEnd()

    member this.AddAreas () : JsonResult =
        let run = data.addRun()
        
        let testCases = read this |> TestCases.Parse                               
        testCases |> Array.iter(fun tc -> 
            let pageId = data.addPage run tc.Page
            let caseId = data.addCase run pageId tc
            tc.TestScenarios |> Array.iter (fun scenario ->
                let scenarioId = data.addScenario run caseId scenario
                data.addInputs run caseId scenarioId scenario.Input
                data.addExpecteds run caseId scenarioId scenario.Expected
                ()
                )
            )
                
        this.Json(String.Empty, JsonRequestBehavior.AllowGet)