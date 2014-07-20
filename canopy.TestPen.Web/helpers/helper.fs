module helper

open System.Web.Mvc
open System.Web
open Newtonsoft.Json

let (?<-) (viewData:ViewDataDictionary) (name:string) (value:'T) = viewData.Add(name, box value)

let percent numerator denominator = 
    if numerator = 0 then 0M
    else if denominator = 0 then 0M
    else
        let result = (decimal numerator) / (decimal denominator) * 100M
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