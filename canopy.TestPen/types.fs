module types

open System.Runtime.Serialization

let NotApplicable = []

type TestType =
    | Postive
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
    | Funding
    | Manual
    | Queues
    | Reports
    | Underwriting
    | Wizard
    | Workflows
    | None

type Section =
    | Integration
    | Configuration
    | General
    | Applications
    | Tables
    | Notifications
    | Verifications
    | RiskSetup
    | Structure
    | Decisions
    | PageConfigurations
    | Security
    | ThirdParty
    | Dealers
    | Queues
    | Lease
    | LeasingWorkflow
    | Reporting
    | AppEntry
    | Application
    | Wizard
    | UnderwritingWorkflow
    | FunderWorkflow
    | NewAppWorkflowDT
    | NewAppWorkflowR1
    | NewAppWorkflowMAN
    | PaymentCallWorkflow
    | None

type Attribute =
    | Workflow of Workflow
    | Applicant of Applicant
    | DealType of DealType

[<CLIMutable>]
type TestScenario = {
    description: string                    
    criticality: Criticality
    testType: TestType
    testExecutionType: TestExecutionType
    configuration: int
    inputs: string list
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
    feature: string
    description: string
    criticality: Criticality
    affects: string list    
    configurations: string list
    testScenarios: TestScenario list
}