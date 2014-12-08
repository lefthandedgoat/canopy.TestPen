open types
open System.Net
open System.IO
open Newtonsoft.Json

(*
NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE
Currently the webpage uses integrated security (thats what im using at work)
pass the credentials of your local computer in line 15
*)

let post page =    
    let address = "http://localhost:48213/Api/AddAreas"
    use client = new System.Net.WebClient()
    client.Credentials <- NetworkCredential("username", "password", "") //last is domain which is blank for me
    let json = JsonConvert.SerializeObject(page)
    client.UploadString(address, json) |> ignore
    
let cases = 
    [
        { 
            page = { 
                    area = Area.ThirdParty;
                    section = Section.Integration;
                    name = "Vendor 1 Intergrations" 
            }
            status = Done
            tShirtSize = XS
            feature = "Feature 1"
            bulletPoints = 
                [
                    ("Does this work?", Medium) 
                    ("Does that work??", High)
                ]
            description = "Auto Decline are the rules that are defined to decline a request when it fails to meet a prescribed rule"
            criticality = Critical
            documentation = "http://www.google.com"
            affects = 
                [
                    "Applications With AutoDecline Rule"
                    "Publish Rules"
                ]
            configurations = 
                [
                    "1: Default deploy"
                    "2: 55 Rules in grid, Add many rows via JavaScript"
                    "3: Deactive all Auto Decline Rules"
                ]
            testScenarios = 
                [
                    {
                        description = "Does this work?"
                        criticality = High
                        testType = Positive
                        testExecutionType = TestExecutionType.Manual
                        configuration = 1
                        inputs = ["Test1 asdasf"; "Test2 ffff"]
                        steps =  [ "STEPS 1"; "STEPS 2" ]
                        expected = [ "Yada 1"; "Yada 2" ]
                        attributes = 
                            [
                                Workflow(Initial);
                                DealType(DealType.Manual);
                            ]
                        code = Code.None
                    };
                    {
                        description = "Does that work??"
                        criticality = High
                        testType = Positive
                        testExecutionType = TestExecutionType.Automated
                        configuration = 1
                        inputs = ["Test1 aaa"; "Test2 bbbb"]
                        steps =  [ "STEPS 1"; "STEPS 2" ]
                        expected = [ "Yada 3"; "Yada 4" ]
                        attributes = 
                            [
                                Workflow(Initial);
                                Applicant(Individual);
                                DealType(DealType.RouteOne);
                            ]
                        code = Code.None
                    };                    
                ]
        }
        { 
            page = { 
                    area = Area.ThirdParty;
                    section = Section.Integration;
                    name = "Vendor 2 Intergrations" 
            }
            status = Done
            tShirtSize = XS
            feature = "Feature 2"
            bulletPoints = 
                [
                    ("Does this work?", Medium) 
                    ("Does that work??", High)
                ]
            description = "Auto Decline are the rules that are defined to decline a request when it fails to meet a prescribed rule"
            criticality = Critical
            documentation = "http://www.google.com"
            affects = 
                [
                    "Main page"
                    "Page 4"
                ]
            configurations = 
                [
                    "1: Default deploy"
                ]
            testScenarios = 
                [
                    {
                        description = "Does this work?"
                        criticality = High
                        testType = Positive
                        testExecutionType = TestExecutionType.Manual
                        configuration = 1
                        inputs = ["Test1 asdasf"; "Test2 ffff"]
                        steps =  [ "STEPS 1"; "STEPS 2" ]
                        expected = [ "Yada 1"; "Yada 2" ]
                        attributes = 
                            [
                                Workflow(Initial);
                                DealType(DealType.Manual);
                            ]
                        code = Code.None
                    };
                    {
                        description = "Does that work??"
                        criticality = High
                        testType = Positive
                        testExecutionType = TestExecutionType.Automated
                        configuration = 1
                        inputs = ["Test1 aaa"; "Test2 bbbb"]
                        steps =  [ "STEPS 1"; "STEPS 2" ]
                        expected = [ "Yada 3"; "Yada 4" ]
                        attributes = 
                            [
                                Workflow(Initial);
                                Applicant(Individual);
                                DealType(DealType.RouteOne);
                            ]
                        code = Code.None
                    };                    
                ]
        }
        { 
            page = { 
                    area = Area.ThirdParty;
                    section = Section.Integration;
                    name = "Vendor 3 Intergrations" 
            }
            status = Done
            tShirtSize = XS
            feature = "Feature 3"
            bulletPoints = 
                [
                    ("Does this work?", Medium) 
                    ("Does that work??", High)
                ]
            description = "Auto Decline are the rules that are defined to decline a request when it fails to meet a prescribed rule"
            criticality = Critical
            documentation = "http://www.google.com"
            affects = 
                [
                    "Applications With AutoDecline Rule"
                    "Publish Rules"
                ]
            configurations = 
                [
                    "1: Default deploy"
                ]
            testScenarios = 
                [
                    {
                        description = "Does this work?"
                        criticality = High
                        testType = Positive
                        testExecutionType = TestExecutionType.Manual
                        configuration = 1
                        inputs = ["Test1 asdasf"; "Test2 ffff"]
                        steps =  [ "STEPS 1"; "STEPS 2" ]
                        expected = [ "Yada 1"; "Yada 2" ]
                        attributes = 
                            [
                                Workflow(Initial);
                                DealType(DealType.Manual);
                            ]
                        code = Code.None
                    };
                    {
                        description = "Does that work??"
                        criticality = High
                        testType = Positive
                        testExecutionType = TestExecutionType.Automated
                        configuration = 1
                        inputs = ["Test1 aaa"; "Test2 bbbb"]
                        steps =  [ "STEPS 1"; "STEPS 2" ]
                        expected = [ "Yada 3"; "Yada 4" ]
                        attributes = 
                            [
                                Workflow(Initial);
                                Applicant(Individual);
                                DealType(DealType.RouteOne);
                            ]
                        code = Code.None
                    };                    
                ]
        };
    ]

post { cases = cases; reportResults = caseReporting.generate cases }
