module data

open FSharp.Data
open helper
open types

[<Literal>]
let connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CanopyTestPen;Integrated Security=True"

/////////////
//Runs
/////////////
[<Literal>]
let private addRunQuery = """
INSERT INTO dbo.Runs
VALUES (getdate())

SELECT SCOPE_IDENTITY()"""

type AddRunQuery = SqlCommandProvider<addRunQuery, connectionString>

let addRun () =
    let cmd = new AddRunQuery()    
    let result =
        cmd.AsyncExecute() 
        |> Async.RunSynchronously
        |> Seq.head
    System.Convert.ToInt32(result.Value)

[<Literal>]
let private getRunsQuery = """SELECT TOP 5 Id, Date FROM dbo.Runs ORDER BY ID DESC"""

type GetRunsQuery = SqlCommandProvider<getRunsQuery, connectionString>

let getRuns () =
    let cmd = new GetRunsQuery()
    cmd.AsyncExecute()
    |> Async.RunSynchronously
    |> List.ofSeq

/////////////
//Pages
/////////////
[<Literal>]
let private addPageQuery = """
INSERT INTO dbo.Pages
VALUES (@RunId, @Area, @Section, @Name)

SELECT SCOPE_IDENTITY()"""

type AddPageQuery = SqlCommandProvider<addPageQuery, connectionString>

let addPage run (page : TestCases.Page) =
    let cmd = new AddPageQuery()    
    let result =
        cmd.AsyncExecute(RunId = run, Area = page.Area.Case, Section = page.Section.Case, Name = page.Name) 
        |> Async.RunSynchronously
        |> Seq.head
    System.Convert.ToInt32(result.Value)

/////////////
//Cases
/////////////
[<Literal>]
let private addCasesQuery = """
INSERT INTO dbo.Cases
VALUES (@RunId, @PageId, @Feature, @Description, @Criticality)

SELECT SCOPE_IDENTITY()"""

type AddCasesQuery = SqlCommandProvider<addCasesQuery, connectionString>

let addCase run page (case: TestCases.Root) =
    let cmd = new AddCasesQuery()    
    let result =
        cmd.AsyncExecute(RunId = run, PageId = page, Feature = case.Feature, Description = case.Description, Criticality = case.Criticality.Case) 
        |> Async.RunSynchronously
        |> Seq.head
    System.Convert.ToInt32(result.Value)

[<Literal>]
let private getCasesQuery = """SELECT Cases.Id as CaseId, Area, Section, Name, Criticality, 0 as Pass, 0 As Fail, 0 As Skip, 0 as None
FROM dbo.Pages JOIN dbo.Cases
ON Cases.RunId = Pages.RunId 
AND Cases.PageId = Pages.Id
WHERE Pages.RunId = @RunId"""

type getCasesQuery = SqlCommandProvider<getCasesQuery, connectionString>

let getCaseSummaries run =
    let cmd = new getCasesQuery()
    cmd.AsyncExecute(RunId = run)
    |> Async.RunSynchronously
    |> List.ofSeq

[<Literal>]
let private getCaseByIdQuery = """SELECT Cases.Id, Cases.RunId, Feature, Description, Criticality, Area, Section, Name
FROM dbo.Cases JOIN dbo.Pages
ON Cases.PageId = Pages.Id
WHERE Cases.Id = @CaseId"""

type getCaseByIdQuery = SqlCommandProvider<getCaseByIdQuery, connectionString>

let getCaseById caseId =
    let cmd = new getCaseByIdQuery()
    cmd.AsyncExecute(CaseId = caseId)
    |> Async.RunSynchronously
    |> Seq.head

[<Literal>]
let private getPassFailSkipNoneQuery = """SELECT CaseId, TestStatus, Count(*) as cnt
FROM [dbo].[Scenarios]
WHERE RunId = @RunId
GROUP BY CaseId, TestStatus"""

type getPassFailSkipNoneQuery = SqlCommandProvider<getPassFailSkipNoneQuery, connectionString>

let getPassFailSkipNone run =
    let cmd = new getPassFailSkipNoneQuery()
    cmd.AsyncExecute(RunId = run)
    |> Async.RunSynchronously
    |> List.ofSeq

let mapPassFailSkipNoneToSummaries (pfskns : getPassFailSkipNoneQuery.Record list) (summaries : getCasesQuery.Record list) =
    let getCount status (summary : getCasesQuery.Record) = 
        let count =
            pfskns 
            |> List.tryFind (fun pfskn -> pfskn.CaseId = summary.CaseId && pfskn.TestStatus = status)
        match count with
        | Some(x) -> x.cnt.Value
        | _ -> 0

    summaries
    |> List.map (fun summary ->
      let i = 0
      {
        CaseId = summary.CaseId
        Area = summary.Area
        Section = summary.Section
        Name = summary.Name
        Criticality = summary.Criticality
        Pass = getCount "Pass" summary
        Fail = getCount "Fail" summary
        Skip = getCount "Skip" summary
        None = getCount "None" summary
      }
    )

/////////////
//PFSN (Pass Fail Skip None)
/////////////

[<Literal>]
let private getPFSNHeaderQuery = """SELECT TestStatus, Count(*) as cnt
FROM [dbo].[Scenarios]
WHERE RunId = @RunId
GROUP BY TestStatus"""

type getPFSNHeaderQuery = SqlCommandProvider<getPFSNHeaderQuery, connectionString>

let getPFSNHeader run =
    let getCount status (pfskns : getPFSNHeaderQuery.Record list) = 
        let count =
            pfskns 
            |> List.tryFind (fun pfskn -> pfskn.TestStatus = status)
        match count with
        | Some(x) -> x.cnt.Value
        | _ -> 0

    let results =
        let cmd = new getPFSNHeaderQuery()
        cmd.AsyncExecute(RunId = run)
        |> Async.RunSynchronously
        |> List.ofSeq

    {
        Pass = getCount "Pass" results
        Fail = getCount "Fail" results
        Skip = getCount "Skip" results
        None = getCount "None" results
    }
        
[<Literal>]
let private getPFSNByCaseQuery = """SELECT TestStatus, Count(*) as cnt
FROM [dbo].[Scenarios]
WHERE CaseId = @CaseId
AND TestExecutionType = @TestExecutionType
GROUP BY TestStatus"""

type getPFSNByCaseQuery = SqlCommandProvider<getPFSNByCaseQuery, connectionString>

let getPFSNByCase case exceutionType =
    let getCount status (pfskns : getPFSNByCaseQuery.Record list) = 
        let count =
            pfskns 
            |> List.tryFind (fun pfskn -> pfskn.TestStatus = status)
        match count with
        | Some(x) -> x.cnt.Value
        | _ -> 0

    let results =
        let cmd = new getPFSNByCaseQuery()
        cmd.AsyncExecute(CaseId = case, TestExecutionType = exceutionType)
        |> Async.RunSynchronously
        |> List.ofSeq

    {
        Pass = getCount "Pass" results
        Fail = getCount "Fail" results
        Skip = getCount "Skip" results
        None = getCount "None" results
    }

type passQuery = SqlCommandProvider<"Update [dbo].[Scenarios] Set TestStatus = 'Pass' Where Id = @Id", connectionString>

let pass (id : int) =    
    let cmd = new passQuery()  
    cmd.Execute(Id = id)

type failQuery = SqlCommandProvider<"Update [dbo].[Scenarios] Set TestStatus = 'Fail' Where Id = @Id", connectionString>

let fail (id : int) =    
    let cmd = new failQuery()  
    cmd.Execute(Id = id)

type skipQuery = SqlCommandProvider<"Update [dbo].[Scenarios] Set TestStatus = 'Skip' Where Id = @Id", connectionString>

let skip (id : int) =
    let cmd = new skipQuery()
    cmd.Execute(Id = id)

/////////////
//Scenarios
/////////////
[<Literal>]
let private addScenarioQuery = """
INSERT INTO dbo.Scenarios
VALUES (@RunId, @CaseId, @Description, @Criticality, @TestType, @TestExecutionType, @TestStatus, @Configuration, @Code, null)

SELECT SCOPE_IDENTITY()"""

type AddScenariosQuery = SqlCommandProvider<addScenarioQuery, connectionString>

let addScenario run caseId (scenario: TestCases.TestScenario) =
    let cmd = new AddScenariosQuery()    
    let result =
        cmd.AsyncExecute(RunId = run, CaseId = caseId, Description = scenario.Description, Criticality = scenario.Criticality.Case, TestType = scenario.TestType.Case, TestExecutionType = scenario.TestExecutionType.Case,
            TestStatus = "None", Configuration = scenario.Configuration, Code = not (scenario.Code.Case = "None")) 
        |> Async.RunSynchronously
        |> Seq.head
    System.Convert.ToInt32(result.Value)

[<Literal>]
let private getScenariosQuery = """SELECT Id, CaseId, Description, Criticality, TestType, TestExecutionType, TestStatus, Configuration, Code, Comment
FROM [dbo].[Scenarios]
WHERE CaseId = @CaseId"""

type getScenariosQuery = SqlCommandProvider<getScenariosQuery, connectionString>

let getScenarios case =
    let cmd = new getScenariosQuery()
    cmd.AsyncExecute(CaseId = case)
    |> Async.RunSynchronously
    |> List.ofSeq

/////////////
//Inputs
/////////////
[<Literal>]
let private addInputsQuery = """
INSERT INTO dbo.Inputs
VALUES (@RunId, @CaseId, @ScenarioId, @Input)"""

type addInputsQuery = SqlCommandProvider<addInputsQuery, connectionString>

let addInputs run caseId scenarioId (inputs : string []) =
    let cmd = new addInputsQuery()    
    inputs 
    |> Array.iter (fun input ->
        cmd.AsyncExecute(RunId = run, CaseId = caseId, ScenarioId = scenarioId, Input = input) 
        |> Async.RunSynchronously |> ignore)

[<Literal>]
let private getInputsQuery = """SELECT CaseId, ScenarioId, Input
FROM [dbo].[Inputs]
WHERE CaseId = @CaseId"""

type getInputsQuery = SqlCommandProvider<getInputsQuery, connectionString>

let getInputs case =
    let cmd = new getInputsQuery()
    cmd.AsyncExecute(CaseId = case)
    |> Async.RunSynchronously
    |> List.ofSeq

/////////////
//Expected
/////////////
[<Literal>]
let private addExpectedsQuery = """
INSERT INTO dbo.Expecteds
VALUES (@RunId, @CaseId, @ScenarioId, @Expected)"""

type addExpectedsQuery = SqlCommandProvider<addExpectedsQuery, connectionString>

let addExpecteds run caseId scenarioId (expecteds : string []) =
    let cmd = new addExpectedsQuery()    
    expecteds 
    |> Array.iter (fun expected ->
        cmd.AsyncExecute(RunId = run, CaseId = caseId, ScenarioId = scenarioId, Expected = expected) 
        |> Async.RunSynchronously |> ignore)

[<Literal>]
let private getExpectedsQuery = """SELECT CaseId, ScenarioId, Expected
FROM [dbo].[Expecteds]
WHERE CaseId = @CaseId"""

type getExpectedsQuery = SqlCommandProvider<getExpectedsQuery, connectionString>

let getExpecteds case =
    let cmd = new getExpectedsQuery()
    cmd.AsyncExecute(CaseId = case)
    |> Async.RunSynchronously
    |> List.ofSeq
    
/////////////
//Attributes
/////////////
[<Literal>]
let private addAttributesQuery = """
INSERT INTO dbo.Attributes
VALUES (@RunId, @CaseId, @ScenarioId, @Name, @Value)"""

type addAttributesQuery = SqlCommandProvider<addAttributesQuery, connectionString>

let addAttributes run caseId scenarioId (attributes : TestCases.Attribute []) =
    let cmd = new addAttributesQuery()    
    attributes 
    |> Array.iter (fun attribute ->
        cmd.AsyncExecute(RunId = run, CaseId = caseId, ScenarioId = scenarioId, Name = attribute.Case, Value = attribute.Fields.[0].Case) 
        |> Async.RunSynchronously |> ignore)

[<Literal>]
let private getAttributesQuery = """SELECT CaseId, ScenarioId, Name, Value
FROM [dbo].[Attributes]
WHERE CaseId = @CaseId"""

type getAttributesQuery = SqlCommandProvider<getAttributesQuery, connectionString>

let getAttributes case =
    let cmd = new getAttributesQuery()
    cmd.AsyncExecute(CaseId = case)
    |> Async.RunSynchronously
    |> List.ofSeq