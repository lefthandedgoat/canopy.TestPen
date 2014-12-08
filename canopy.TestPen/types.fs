module types

open System.Runtime.Serialization

let NotApplicable = []
let TODO_SetMeToSomeNotApplicableOrARealListOfValues = []

type TestType =
    | Positive
    | Negative
    | Boundary
    | Exception
    | None

type Workflow =
    | Initial
    | Underwriter
    | Funder
    | None

type Applicant =
    | Individual
    | Joint    
    | None

type DealType =
    | Manual
    | RouteOne
    | DealerTrack
    | None

type Code =
    | Func of (unit -> unit)
    | Todo
    | None
    
type Criticality =
    | Critical
    | High
    | Medium
    | Low
    | None
    
type TestStatus =
    | Pass
    | Fail
    | Skip
    | None

type TestExecutionType =
    | Manual
    | Bulk
    | Automated
    | None

type Area =
    | ThirdParty
    | Configuration
    | Manual
    | Queues
    | Reports    
    | None
    
type Section =
    | Integration
    | Configuration
    | General
    | Tables
    | Notifications
    | Security
    | ThirdParty
    | Reporting
    | Wizard
    | Signup
    | None

type Attribute =
    | Workflow of Workflow
    | Applicant of Applicant
    | DealType of DealType

type Status =
    | Ready
    | Bullet
    | TestCase
    | Automation
    | Done
    | None

type TShirtSize =
    | XS
    | S
    | M
    | L
    | XL
    | XXL
    | None

[<CLIMutable>]
type TestScenario = {
    description: string                    
    criticality: Criticality
    testType: TestType
    testExecutionType: TestExecutionType
    configuration: int
    inputs: string list
    steps: string list
    expected: string list
    code: Code
    attributes: Attribute list
}

[<CLIMutable>]
type Page = { 
    area: Area
    section: Section
    name: string
}

[<CLIMutable>]
type TestCase = {
    page: Page
    bulletPoints: (string * Criticality) list
    status : Status
    tShirtSize : TShirtSize
    feature: string
    description: string
    criticality: Criticality
    affects: string list    
    configurations: string list
    testScenarios: TestScenario list
    documentation: string
}

[<CLIMutable>]
type reportResult = {
    id : int
    label : string
    count : int 
}

[<CLIMutable>]
type ToTestPen = {
    cases: TestCase list
    reportResults: reportResult list
}