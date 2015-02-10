module data

open FSharp.Data
open types

/////////////
//Runs
/////////////
[<Literal>]
let private addRunQuery = """
INSERT INTO dbo.Runs
VALUES (getdate(), 1)

SELECT SCOPE_IDENTITY()"""

type AddRunQuery = SqlCommandProvider<addRunQuery, "name=TestPen">

let addRun () =
    let cmd = new AddRunQuery()    
    let result =
        cmd.AsyncExecute() 
        |> Async.RunSynchronously
        |> Seq.head
    result.Value |> int

[<Literal>]
let private getRunsQuery = """
SELECT TOP 5 Id, Date 
FROM dbo.Runs
WHERE IsActive = 1
ORDER BY ID DESC"""

type GetRunsQuery = SqlCommandProvider<getRunsQuery, "name=TestPen">

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

type AddPageQuery = SqlCommandProvider<addPageQuery, "name=TestPen">

let addPage run (page : TestCases.Page) =
    let cmd = new AddPageQuery()    
    let result =
        cmd.AsyncExecute(RunId = run, Area = page.Area.Case, Section = page.Section.Case, Name = page.Name) 
        |> Async.RunSynchronously
        |> Seq.head
    result.Value |> int

/////////////
//Cases
/////////////
[<Literal>]
let private addCasesQuery = """
INSERT INTO dbo.Cases
VALUES (@RunId, @PageId, @Feature, @Description, @Criticality, @Documentation, null)

SELECT SCOPE_IDENTITY()"""

type AddCasesQuery = SqlCommandProvider<addCasesQuery, "name=TestPen">

let addCase run page (case: TestCases.Casis) =
    let cmd = new AddCasesQuery()    
    let result =
        cmd.AsyncExecute(RunId = run, PageId = page, Feature = case.Feature, Description = case.Description, Criticality = case.Criticality.Case, Documentation = case.Documentation) 
        |> Async.RunSynchronously
        |> Seq.head
    result.Value |> int

[<Literal>]
let private getCasesQuery = """SELECT ClaimedBy, Cases.Id as CaseId, Area, Section, Name, Criticality, 0 as Pass, 0 As Fail, 0 As Skip, 0 as None
FROM dbo.Pages JOIN dbo.Cases
ON Cases.RunId = Pages.RunId 
AND Cases.PageId = Pages.Id
WHERE Pages.RunId = @RunId"""

type getCasesQuery = SqlCommandProvider<getCasesQuery, "name=TestPen">

let getCaseSummaries run =
    let cmd = new getCasesQuery()
    cmd.AsyncExecute(RunId = run)
    |> Async.RunSynchronously
    |> List.ofSeq

[<Literal>]
let private getCaseByIdQuery = """SELECT Cases.Id, Cases.RunId, Feature, Description, Criticality, Area, Section, Name, Documentation
FROM dbo.Cases JOIN dbo.Pages
ON Cases.PageId = Pages.Id
WHERE Cases.Id = @CaseId"""

type getCaseByIdQuery = SqlCommandProvider<getCaseByIdQuery, "name=TestPen">

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

type getPassFailSkipNoneQuery = SqlCommandProvider<getPassFailSkipNoneQuery, "name=TestPen">

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
          let pass = getCount "Pass" summary
          let fail = getCount "Fail" summary
          let skip = getCount "Skip" summary
          let none = getCount "None" summary          
          let total = pass + fail + skip + none
          let percent = if total = 0 then 0.0M else System.Math.Round((decimal (pass + fail)) / (decimal total) * 100M, 1)
          {
            ClaimedBy = summary.ClaimedBy
            CaseId = summary.CaseId
            Area = summary.Area
            Section = summary.Section
            Name = summary.Name
            Criticality = summary.Criticality
            Pass = pass
            Fail = fail
            Skip = skip
            None = none
            Total = total
            Percent = percent
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

type getPFSNHeaderQuery = SqlCommandProvider<getPFSNHeaderQuery, "name=TestPen">

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
        
    let pass = getCount "Pass" results
    let fail = getCount "Fail" results
    let skip = getCount "Skip" results
    let none = getCount "None" results          
    let total = pass + fail + skip + none
    let percent = if total = 0 then 0.0M else System.Math.Round((decimal (pass + fail)) / (decimal total) * 100M, 1)
    {
        Pass = pass
        Fail = fail
        Skip = skip
        None = none
        Total = total
        Percent = percent
    }
        
[<Literal>]
let private getPFSNByCaseQuery = """SELECT TestStatus, Count(*) as cnt
FROM [dbo].[Scenarios]
WHERE CaseId = @CaseId
AND TestExecutionType = @TestExecutionType
GROUP BY TestStatus"""

type getPFSNByCaseQuery = SqlCommandProvider<getPFSNByCaseQuery, "name=TestPen">

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

    let pass = getCount "Pass" results
    let fail = getCount "Fail" results
    let skip = getCount "Skip" results
    let none = getCount "None" results          
    let total = pass + fail + skip + none
    let percent = if total = 0 then 0.0M else System.Math.Round((decimal (pass + fail)) / (decimal total) * 100M, 1)
    {
        Pass = pass
        Fail = fail
        Skip = skip
        None = none
        Total = total
        Percent = percent
    }

type passQuery = SqlCommandProvider<"Update [dbo].[Scenarios] Set TestStatus = 'Pass', TestedBy = @User, UpdateDate = getdate() Where Id = @Id", "name=TestPen">

let pass id user =    
    let cmd = new passQuery()  
    cmd.Execute(Id = id, User = user) |> ignore

type failQuery = SqlCommandProvider<"Update [dbo].[Scenarios] Set TestStatus = 'Fail', TestedBy = @User, UpdateDate = getdate() Where Id = @Id", "name=TestPen">

let fail id user =    
    let cmd = new failQuery()  
    cmd.Execute(Id = id, User = user) |> ignore

type skipQuery = SqlCommandProvider<"Update [dbo].[Scenarios] Set TestStatus = 'Skip', TestedBy = @User, UpdateDate = getdate() Where Id = @Id", "name=TestPen">

let skip id user =
    let cmd = new skipQuery()
    cmd.Execute(Id = id, User = user) |> ignore

type logPassFailSkipQuery = SqlCommandProvider<"Insert Into [dbo].[PassFailSkipLog] VALUES (@ScenarioId, @User, getdate())", "name=TestPen">

let logPassFailSkip id user =
    let cmd = new logPassFailSkipQuery()
    cmd.Execute(ScenarioId = id, User = user) |> ignore

type saveCommentQuery = SqlCommandProvider<"Update [dbo].[Scenarios] Set Comment = @Comment Where Id = @Id", "name=TestPen">

let saveComment id comment =
    let cmd = new saveCommentQuery()
    cmd.Execute(Id = id, Comment = comment)

/////////////
//Scenarios
/////////////
[<Literal>]
let private addScenarioQuery = """
INSERT INTO dbo.Scenarios
VALUES (@RunId, @CaseId, @Description, @Criticality, @TestType, @TestExecutionType, @TestStatus, @Configuration, @Code, null, null, null)

SELECT SCOPE_IDENTITY()"""

type AddScenariosQuery = SqlCommandProvider<addScenarioQuery, "name=TestPen">

let addScenario run caseId (scenario: TestCases.TestScenario) =
    let cmd = new AddScenariosQuery()    
    let result =
        let hasCode = scenario.Code.Case = "Func"
        let testExecutionType = if scenario.Code.Case <> "Func" then "Manual" else scenario.TestExecutionType.Case
        cmd.AsyncExecute(RunId = run, CaseId = caseId, Description = scenario.Description, Criticality = scenario.Criticality.Case, TestType = scenario.TestType.Case, TestExecutionType = testExecutionType,
            TestStatus = "None", Configuration = scenario.Configuration, Code = hasCode) 
        |> Async.RunSynchronously
        |> Seq.head
    result.Value |> int

[<Literal>]
let private getScenariosQuery = """SELECT Id, CaseId, Description, Criticality, TestType, TestExecutionType, TestStatus, Configuration, Code, Comment, TestedBy
FROM [dbo].[Scenarios]
WHERE CaseId = @CaseId"""

type getScenariosQuery = SqlCommandProvider<getScenariosQuery, "name=TestPen">

let getScenarios case =
    let cmd = new getScenariosQuery()
    cmd.AsyncExecute(CaseId = case)
    |> Async.RunSynchronously
    |> List.ofSeq

type claimQuery = SqlCommandProvider<"Update [dbo].[Cases] Set ClaimedBy = @User Where Id = @Id AND ClaimedBy IS NULL", "name=TestPen">

let claim id user =
    let cmd = new claimQuery()
    cmd.Execute(Id = id, User = user)

type unclaimQuery = SqlCommandProvider<"Update [dbo].[Cases] Set ClaimedBy = null Where Id = @Id", "name=TestPen">

let unclaim id =
    let cmd = new unclaimQuery()
    cmd.Execute(Id = id)

[<Literal>]
let private passScenarioQuery = """
  UPDATE scenario
  SET TestStatus = 'Pass'
  FROM Scenarios AS scenario
  JOIN Cases AS cases
  ON scenario.CaseId = cases.Id
  JOIN Pages as pages
  ON cases.PageId = pages.Id
  WHERE pages.Area = @Area
  AND pages.Section = @Section
  AND pages.Name = @Name
  AND scenario.[Description] = @Description
  AND scenario.RunId = @RunId"""
  
type PassScenariosQuery = SqlCommandProvider<passScenarioQuery, "name=TestPen">

let passScenario run area section name description =
    let cmd = new PassScenariosQuery()    
    cmd.Execute(RunId = run, Area = area, Section = section, Name = name, Description = description) |> ignore

[<Literal>]
let private claimCaseQuery = """
  UPDATE cases
  SET [ClaimedBy] = @User  
  FROM Cases AS cases  
  JOIN Pages as pages
  ON cases.PageId = pages.Id
  WHERE pages.Area = @Area
  AND pages.Section = @Section
  AND pages.Name = @Name
  AND cases.RunId = @RunId"""
  
type ClaimCaseQuery = SqlCommandProvider<claimCaseQuery, "name=TestPen">

let claimCase run area section name user =
    let cmd = new ClaimCaseQuery()    
    cmd.Execute(RunId = run, Area = area, Section = section, Name = name, User = user) |> ignore

/////////////
//Inputs
/////////////
[<Literal>]
let private addInputsQuery = """
INSERT INTO dbo.Inputs
VALUES (@RunId, @CaseId, @ScenarioId, @Input)"""

type addInputsQuery = SqlCommandProvider<addInputsQuery, "name=TestPen">

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

type getInputsQuery = SqlCommandProvider<getInputsQuery, "name=TestPen">

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

type addExpectedsQuery = SqlCommandProvider<addExpectedsQuery, "name=TestPen">

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

type getExpectedsQuery = SqlCommandProvider<getExpectedsQuery, "name=TestPen">

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

type addAttributesQuery = SqlCommandProvider<addAttributesQuery, "name=TestPen">

let addAttributes run caseId scenarioId (attributes : TestCases.Attribute []) =
    let cmd = new addAttributesQuery()    
    attributes 
    |> Array.iter (fun attribute ->        
        if not <| Array.isEmpty attribute.Fields then
            cmd.AsyncExecute(RunId = run, CaseId = caseId, ScenarioId = scenarioId, Name = attribute.Case, Value = attribute.Fields.[0].Case) 
            |> Async.RunSynchronously |> ignore)

[<Literal>]
let private getAttributesQuery = """SELECT CaseId, ScenarioId, Name, Value
FROM [dbo].[Attributes]
WHERE CaseId = @CaseId"""

type getAttributesQuery = SqlCommandProvider<getAttributesQuery, "name=TestPen">

let getAttributes case =
    let cmd = new getAttributesQuery()
    cmd.AsyncExecute(CaseId = case)
    |> Async.RunSynchronously
    |> List.ofSeq

/////////////
//Affects
/////////////
[<Literal>]
let private addAffectsQuery = """
INSERT INTO dbo.Affects
VALUES (@RunId, @CaseId, @Value)"""

type addAffectsQuery = SqlCommandProvider<addAffectsQuery, "name=TestPen">

let addAffects run caseId (tc : TestCases.Casis) =    
    let cmd = new addAffectsQuery()    
    tc.Affects
    |> Array.iter (fun affected ->
        cmd.AsyncExecute(RunId = run, CaseId = caseId, Value = affected ) 
        |> Async.RunSynchronously |> ignore)

[<Literal>]
let private getAffectsQuery = """SELECT Value
FROM [dbo].[Affects]
WHERE CaseId = @CaseId"""

type getAffectsQuery = SqlCommandProvider<getAffectsQuery, "name=TestPen">

let getAffects case =
    let cmd = new getAffectsQuery()
    cmd.AsyncExecute(CaseId = case)
    |> Async.RunSynchronously
    |> List.ofSeq

/////////////
//Configurations
/////////////
[<Literal>]
let private addConfigurationsQuery = """
INSERT INTO dbo.Configurations
VALUES (@RunId, @CaseId, @Value)"""

type addConfigurationsQuery = SqlCommandProvider<addConfigurationsQuery, "name=TestPen">

let addConfigurations run caseId (tc : TestCases.Casis) =    
    let cmd = new addConfigurationsQuery()    
    tc.Configurations
    |> Array.iter (fun affected ->
        cmd.AsyncExecute(RunId = run, CaseId = caseId, Value = affected ) 
        |> Async.RunSynchronously |> ignore)

[<Literal>]
let private getConfigurationsQuery = """SELECT Value
FROM [dbo].[Configurations]
WHERE CaseId = @CaseId"""

type getConfigurationsQuery = SqlCommandProvider<getConfigurationsQuery, "name=TestPen">

let getConfigurations case =
    let cmd = new getConfigurationsQuery()
    cmd.AsyncExecute(CaseId = case)
    |> Async.RunSynchronously
    |> List.ofSeq
    
/////////////
//Steps
/////////////
[<Literal>]
let private addStepsQuery = """
INSERT INTO dbo.Steps
VALUES (@RunId, @CaseId, @ScenarioId, @Step)"""

type addStepsQuery = SqlCommandProvider<addStepsQuery, "name=TestPen">

let addSteps run caseId scenarioId (steps : string []) =
    let cmd = new addStepsQuery()    
    steps 
    |> Array.iter (fun step ->
        cmd.AsyncExecute(RunId = run, CaseId = caseId, ScenarioId = scenarioId, Step = step) 
        |> Async.RunSynchronously |> ignore)

[<Literal>]
let private getStepsQuery = """SELECT CaseId, ScenarioId, Value
FROM [dbo].[Steps]
WHERE CaseId = @CaseId"""

type getStepsQuery = SqlCommandProvider<getStepsQuery, "name=TestPen">

let getSteps case =
    let cmd = new getStepsQuery()
    cmd.AsyncExecute(CaseId = case)
    |> Async.RunSynchronously
    |> List.ofSeq

/////////////
//Reports
/////////////
[<Literal>]
let private addReportsQuery = """
INSERT INTO dbo.Reports
VALUES (@RunId, @SectionId, @Value, @Count)"""

type addReportsQuery = SqlCommandProvider<addReportsQuery, "name=TestPen">

let addReports run (reports : TestCases.ReportResult []) =
    let cmd = new addReportsQuery()    
    reports 
    |> Array.iter (fun report ->
        cmd.AsyncExecute(RunId = run, SectionId = report.Id, Value = report.Label, Count = report.Count) 
        |> Async.RunSynchronously |> ignore)

[<Literal>]
let private getReportsQuery = """SELECT SectionId, Value, Count
FROM [dbo].[Reports]
WHERE RunId = @RunId"""

type getReportsQuery = SqlCommandProvider<getReportsQuery, "name=TestPen">

let getReports run =
    let cmd = new getReportsQuery()
    cmd.AsyncExecute(RunId = run)
    |> Async.RunSynchronously
    |> List.ofSeq

[<Literal>]
let private getRanQuery = """SELECT count(*)
FROM [CanopyTestPen].[dbo].[Scenarios]
WHERE RunId = @RunId
AND TestStatus IN ('Pass', 'Fail')"""

type getRanQuery = SqlCommandProvider<getRanQuery, "name=TestPen">

let getRan run =
    let cmd = new getRanQuery()
    let result =
        cmd.AsyncExecute(RunId = run)
        |> Async.RunSynchronously
        |> List.ofSeq
        |> Seq.head
    result.Value |> int

/////////////
//Readiness
/////////////

[<Literal>]
let private getPFSNReadinessQuery = """SELECT
	[Criticality]
    ,[TestStatus]
	,COUNT(*) as cnt
FROM [CanopyTestPen].[dbo].[Scenarios]
WHERE RunId = @RunId
GROUP BY Criticality, TestStatus"""

type getPFSNReadinessQuery = SqlCommandProvider<getPFSNReadinessQuery, "name=TestPen">

let getPFSNReadiness run =
    let cmd = new getPFSNReadinessQuery()
    cmd.AsyncExecute(RunId = run)
    |> Async.RunSynchronously
    |> List.ofSeq
    
[<Literal>]
let private getHistoryRanQuery = """SELECT
r.Date
,r.Id
,s.Criticality
,COUNT(*) as cnt
FROM [CanopyTestPen].[dbo].[Scenarios] as s
JOIN [CanopyTestPen].[dbo].[Runs]as r
ON s.RunId = r.Id
WHERE (TestStatus = 'Pass'
OR TestStatus = 'Fail')
AND r.IsActive = 1
GROUP BY r.Id, r.Date, s.Criticality
ORDER BY r.Id DESC"""

type getHistoryRanQuery = SqlCommandProvider<getHistoryRanQuery, "name=TestPen">

let getHistoryRan () =
    let cmd = new getHistoryRanQuery()
    let results =
        cmd.AsyncExecute()
        |> Async.RunSynchronously
        |> List.ofSeq
        |> List.map (fun result -> 
            { 
                Date = System.String.Format("{0:yyyy-MM-dd}", result.Date)
                Count = result.cnt.Value
                RunId = result.Id 
                Criticality = result.Criticality
                TestedBy = ""
            } )
    let runIds = 
        results 
        |> Seq.distinctBy (fun result -> result.RunId)
        |> Seq.map (fun result -> result.RunId)
    
    runIds 
    |> Seq.map (fun runId ->
        {
            Date = (results |> List.find (fun result -> result.RunId = runId)).Date
            RunId = runId
            Total = results |> List.filter (fun result -> result.RunId = runId) |> List.map (fun result -> result.Count) |> List.sum
            High = results |> List.filter (fun result -> result.RunId = runId && result.Criticality = "High") |> List.map (fun result -> result.Count) |> List.sum
            Medium = results |> List.filter (fun result -> result.RunId = runId && result.Criticality = "Medium") |> List.map (fun result -> result.Count) |> List.sum
            Low = results |> List.filter (fun result -> result.RunId = runId && result.Criticality = "Low") |> List.map (fun result -> result.Count) |> List.sum
        } )
    |> List.ofSeq

[<Literal>]
let private getReadinessErrorsQuery = """SELECT 
	s.TestedBy
    ,s.Criticality
    ,Area
	,Section
	,Name
	,s.[Description]    
    ,ISNULL([Comment],'') as Comment
FROM [CanopyTestPen].[dbo].[Scenarios] as s
JOIN [CanopyTestPen].[dbo].[Cases] as c
ON s.CaseId = c.Id
JOIN [CanopyTestPen].[dbo].[Pages] as p
ON c.PageId = p.Id
WHERE s.RunId = @RunId
AND TestStatus = 'Fail'
"""

type getReadinessErrorsQuery = SqlCommandProvider<getReadinessErrorsQuery, "name=TestPen">

let getReadinessErrors runId =
    let cmd = new getReadinessErrorsQuery()    
    cmd.AsyncExecute(RunId = runId)
    |> Async.RunSynchronously
    |> List.ofSeq

[<Literal>]
let getRanByUserByDayQuery = """
SELECT
CAST(s.UpdateDate AS DATE) as UpdateDate
,r.Id
,s.TestedBy
,COUNT(*) as cnt
FROM [CanopyTestPen].[dbo].[Scenarios] as s
JOIN [CanopyTestPen].[dbo].[Runs]as r
ON s.RunId = r.Id
WHERE (TestStatus = 'Pass'
OR TestStatus = 'Fail')
AND r.Id = @RunId
AND r.IsActive = 1
AND s.TestedBy IS NOT NULL
AND s.UpdateDate IS NOT NULL
GROUP BY r.Id, CAST(s.UpdateDate AS DATE), s.TestedBy
ORDER BY r.Id DESC
"""

type getRanByUserByDayQuery = SqlCommandProvider<getRanByUserByDayQuery, "name=TestPen">

let getRanByUserByDay runId =
    let cmd = new getRanByUserByDayQuery()    
    cmd.AsyncExecute(RunId = runId)
    |> Async.RunSynchronously
    |> List.ofSeq
    |> List.map (fun result -> 
            { 
                Date = System.String.Format("{0:yyyy-MM-dd}", result.UpdateDate.Value)
                Count = result.cnt.Value
                RunId = result.Id 
                TestedBy = result.TestedBy.Value
                Criticality = ""
            } )

[<Literal>]
let getRanByUserByDayByCriticalityQuery = """
SELECT
r.Id
,s.TestedBy
,s.Criticality
,COUNT(*) as cnt
FROM [CanopyTestPen].[dbo].[Scenarios] as s
JOIN [CanopyTestPen].[dbo].[Runs]as r
ON s.RunId = r.Id
WHERE (TestStatus = 'Pass'
OR TestStatus = 'Fail')
AND r.Id = @RunId
AND r.IsActive = 1
AND s.TestedBy IS NOT NULL
AND s.UpdateDate IS NOT NULL
GROUP BY r.Id, s.TestedBy, s.Criticality
ORDER BY r.Id DESC
"""

type getRanByUserByDayByCriticalityQuery = SqlCommandProvider<getRanByUserByDayByCriticalityQuery, "name=TestPen">

let getRanByUserByDayByCriticality runId =
    let cmd = new getRanByUserByDayByCriticalityQuery()    
    cmd.AsyncExecute(RunId = runId)
    |> Async.RunSynchronously
    |> List.ofSeq
    |> List.map (fun result -> 
            { 
                Date = ""
                Count = result.cnt.Value
                RunId = result.Id 
                TestedBy = result.TestedBy.Value
                Criticality = result.Criticality
            } )

/////////////
//Stories
/////////////

[<Literal>]
let private addStoryQuery = """
INSERT INTO dbo.Stories
VALUES (@RunId, @Story, @Client, @Team, @Description)

SELECT SCOPE_IDENTITY()"""

type addStoryQuery = SqlCommandProvider<addStoryQuery, "name=TestPen">
let addStory runId story client team description =
    let cmd = new addStoryQuery()    
    let result =
        cmd.AsyncExecute(RunId = runId, Story = story, Team = team, Client = client, Description = description)
        |> Async.RunSynchronously
        |> Seq.head
    result.Value |> int

[<Literal>]
let private deleteStoryQuery = """
DELETE FROM dbo.Stories
WHERE Id = @Id
"""

type deleteStoryQuery = SqlCommandProvider<deleteStoryQuery, "name=TestPen">
let deleteStory id =
    let cmd = new deleteStoryQuery()    
    cmd.Execute(Id = id)

[<Literal>]
let private getStoriesQuery = """SELECT 
	Id
    ,Story
    ,Client
    ,Team
    ,Description
FROM [CanopyTestPen].[dbo].[Stories]
WHERE RunId = @RunId
"""

type getStoriesQuery = SqlCommandProvider<getStoriesQuery, "name=TestPen">

let getStories runId =
    let cmd = new getStoriesQuery()    
    cmd.AsyncExecute(RunId = runId)
    |> Async.RunSynchronously
    |> List.ofSeq
