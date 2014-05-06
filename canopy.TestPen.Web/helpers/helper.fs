module helper

open System.Web.Mvc

let (?<-) (viewData:ViewDataDictionary) (name:string) (value:'T) = viewData.Add(name, box value)