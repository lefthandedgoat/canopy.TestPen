module caseReporting

open types
open Microsoft.FSharp.Reflection

let tShirtSizeId = 1
let statusId = 2
let testExecutionTypeId = 3
let totalScenariosId = 4
let criticalityId = 5
let totalCasesId = 6
let percentAutomatedId = 7

let toResults id duType (breakdowns : seq<'a * int>) =    
    FSharpType.GetUnionCases(duType)
    |> Seq.map (fun dut -> dut.Name)
    |> Seq.map (fun dut ->         
        let breakdown = breakdowns |> Seq.tryFind (fun (value, _) -> (sprintf "%A" value) = dut)
        match breakdown with
            | Some(_, count) -> { id = id; label = dut; count = count }
            | _ -> { id = id; label = dut; count = 0 })
    |> Seq.sortBy (fun result -> -result.count)
    |> List.ofSeq

let tshirtSizeBreakdown (cases : TestCase list) =
    cases 
    |> Seq.countBy (fun case -> case.tShirtSize)
    |> toResults tShirtSizeId typeof<TShirtSize>

let statusBreakdown (cases : TestCase list) =
    cases 
    |> Seq.countBy (fun case -> case.status)
    |> toResults statusId typeof<Status>

let testExecutionTypeBreakdown (cases : TestCase list) =
    cases 
    |> Seq.map (fun case -> case.testScenarios)
    |> Seq.concat
    |> Seq.countBy (fun scenario -> scenario.testExecutionType)
    |> toResults testExecutionTypeId typeof<TestExecutionType>
        
let totalScenarios (cases : TestCase list) =
    let length =
        cases 
        |> Seq.map (fun case -> case.testScenarios)
        |> Seq.concat
        |> Seq.length
    [ { id = totalScenariosId; label = "total"; count = length } ]

let criticalityBreakdown (cases : TestCase list) =
    cases 
    |> Seq.countBy (fun case -> case.criticality)
    |> toResults criticalityId typeof<Criticality>

let totalCases (cases : TestCase list) =
    let length = cases |> List.length
    [ { id = totalCasesId; label = "total"; count = length } ]
  
let percentAutomated (cases : TestCase list) =
    let scenarios =
        cases 
        |> Seq.map (fun case -> case.testScenarios)
        |> Seq.concat
    let total = scenarios |> Seq.length |> decimal
    let automated = 
        scenarios
        |> Seq.filter (fun scenario -> scenario.testExecutionType = TestExecutionType.Automated)
        |> Seq.length
        |> decimal
    let percent = (automated / total) * 100M |> int

    [ { id = percentAutomatedId; label = "total"; count = percent } ]
    
let generate (cases : TestCase list) =
    [
        cases |> tshirtSizeBreakdown 
        cases |> statusBreakdown 
        cases |> testExecutionTypeBreakdown 
        cases |> totalScenarios 
        cases |> criticalityBreakdown 
        cases |> totalCases 
        cases |> percentAutomated 
    ]
    |> List.concat