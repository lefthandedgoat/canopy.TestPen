module types

open System.Runtime.Serialization

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
    | None
    
type Criticality =
    | High
    | Medium
    | Low
    
type TestStatus =
    | Pass
    | Fail
    | Skip
    | None

type TestExecutionType =
    | Manual
    | Bulk
    | Automated

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
    input: string list
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