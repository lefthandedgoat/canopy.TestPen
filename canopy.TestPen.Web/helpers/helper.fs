module helper

open System.Web.Mvc
open System.Web
open Newtonsoft.Json

let (?<-) (viewData:ViewDataDictionary) (name:string) (value:'T) = viewData.Add(name, box value)

let percent a b = 
    let result = (decimal a) / (decimal b) * 100M
    System.Math.Round(result, 1)

let toJson sectionId (reports : data.getReportsQuery.Record list) =     
    let reports =
        reports 
        |> List.filter (fun report -> report.SectionId = sectionId)
        |> JsonConvert.SerializeObject 
    HtmlString(reports)

let count sectionId (reports : data.getReportsQuery.Record list) =
    (reports 
    |> List.filter (fun report -> report.SectionId = sectionId)
    |> List.head).Count