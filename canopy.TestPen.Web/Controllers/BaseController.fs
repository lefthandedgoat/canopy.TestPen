namespace canopy.TestPen.Controllers

open System
open System.IO
open System.Collections.Generic
open System.Linq
open System.Web
open System.Web.Mvc
open System.Web.Mvc.Ajax
open Newtonsoft.Json
open helper
open types

//http://wingkaiwan.com/2012/12/28/replacing-mvc-javascriptserializer-with-json-net-jsonserializer/
type JsonNetResult() =
    inherit JsonResult()
    
    let mutable Settings : JsonSerializerSettings = null
       
    override x.ExecuteResult context =
        if context = null then raise (System.ArgumentException("context"))
        if x.JsonRequestBehavior = JsonRequestBehavior.DenyGet && System.String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase) then
            raise (InvalidOperationException("JSON GET is not allowed"))

        let response = context.HttpContext.Response;
        response.ContentType <- if System.String.IsNullOrEmpty(x.ContentType) then "application/json" else x.ContentType
 
        if x.ContentEncoding <> null then response.ContentEncoding <- x.ContentEncoding;
        if x.Data = null then ()
 
        let scriptSerializer = JsonSerializer.Create(Settings)

        use sw = new StringWriter()        
        scriptSerializer.Serialize(sw, x.Data);
        response.Write(sw.ToString());

//http://wingkaiwan.com/2012/12/28/replacing-mvc-javascriptserializer-with-json-net-jsonserializer/
type BaseController() =    
    inherit Controller()
    //http://stackoverflow.com/questions/8149127/set-a-property-on-viewbag-dynamic-object-in-f
        
    override x.Initialize(ctx) =
        base.Initialize(ctx)
        x.ViewData?Runs <- data.getRuns()
        ()

    override x.Json(data, contentType, contentEncoding, behavior) =               
        
        let result = JsonNetResult()
        result.Data <- data
        result.ContentType <- contentType
        result.ContentEncoding <- contentEncoding
        result.JsonRequestBehavior <- behavior
        result :> JsonResult