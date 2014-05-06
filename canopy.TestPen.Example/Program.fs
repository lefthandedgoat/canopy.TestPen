open types
open System.Net
open System.IO
open Newtonsoft.Json

let post page =    
    let address = "http://localhost:48213/Api/AddAreas"
    use client = new System.Net.WebClient()
    let json = JsonConvert.SerializeObject(page)
    client.UploadString(address, json) |> ignore
    
let tests = 
    [
        { 
            page = { 
                    area = Area.ThirdParty;
                    section = Section.Integration;
                    name = "Dealer Track Intergrations" 
            }
            feature = "Dealer Track Intergrations"
            description = "Integer fermentum at ipsum vitae pharetra"
            criticality = High
            affects = 
                [
                    "Vestibulum arcu ligula, faucibus vel volutpat id, facilisis sodales elit"
                ]
            configurations = 
                [
                    "Proin laoreet dignissim nisl, in euismod lorem cursus vel"
                    "Morbi justo nulla, facilisis eget elementum sagittis, luctus ac lectus"
                ]
            testScenarios = 
                [
                    {
                        description = "Scenario 1"
                        criticality = High
                        testType = Postive
                        testExecutionType = TestExecutionType.Manual
                        configuration = 1
                        input = ["Test1 asdasf"; "Test2 ffff"]
                        expected = [ "Yada 1"; "Yada 2" ]
                        attributes = 
                            [
                                Workflow(Initial);
                                DealType(DealType.Manual);
                            ]
                        code = Code.None
                    };
                    {
                        description = "Scenario 2"
                        criticality = High
                        testType = Postive
                        testExecutionType = TestExecutionType.Automated
                        configuration = 1
                        input = ["Test1 aaa"; "Test2 bbbb"]
                        expected = [ "Yada 3"; "Yada 4" ]
                        attributes = 
                            [
                                Workflow(Initial);
                                Applicant(Individual);
                                DealType(DealType.RouteOne);
                            ]
                        code = Code.None
                    };
                    {
                        description = "Scenario 3"
                        criticality = High
                        testType = Postive
                        testExecutionType = TestExecutionType.Automated
                        configuration = 1
                        input = ["Test1 asdf "; "Test2 asdf"]
                        expected = [ "Yada 5"; "Yada 6" ]
                        attributes = 
                            [
                                Workflow(Initial);
                                Applicant(Individual);
                                DealType(DealType.RouteOne);
                            ]
                        code = Code.None
                    };
                    {
                        description = "Scenario 4"
                        criticality = High
                        testType = Postive
                        testExecutionType = TestExecutionType.Automated
                        configuration = 1
                        input = ["Test1f "; "Testff2"]
                        expected = [ "Yada 7"; "Yada 8" ]
                        attributes = 
                            [
                                Workflow(Initial);
                                Applicant(Individual);
                                DealType(DealType.RouteOne);
                            ]
                        code = Code.None
                    };
                    {
                        description = "Scenario 5"
                        criticality = High
                        testType = Postive
                        testExecutionType = TestExecutionType.Bulk
                        configuration = 1
                        input = ["Testasdf1"; "Testasdasdf2"]
                        expected = [ "Yada 9"; "Yada 10" ]
                        attributes = 
                            [
                                Workflow(Initial);
                                Applicant(Individual);
                                DealType(DealType.RouteOne);
                            ]
                        code = Code.None
                    };
                    {
                        description = "Scenario 6"
                        criticality = High
                        testType = Postive
                        testExecutionType = TestExecutionType.Bulk
                        configuration = 1
                        input = ["Testaaa1"; "Testfff2"]
                        expected = [ "Yada 11"; "Yada 12" ]
                        attributes = 
                            [
                                Workflow(Initial);
                                Applicant(Individual);
                                DealType(DealType.RouteOne);
                            ]
                        code = Code.None
                    }
                ]
        };

        { 
            page = {
                    area = Area.Configuration
                    section = Section.General
                    name = "Publish Rules"
            }
            feature = "Publish Rules"
            description = "Vestibulum iaculis viverra tellus, nec ullamcorper lorem mattis vel"
            criticality = Medium
            affects = 
                [
                    "Etiam neque est, eleifend ac erat at, luctus facilisis arcu"
                    "Cras blandit lectus sit amet odio elementum ultrices eu ac odio"
                    "Praesent nisi elit, laoreet sit amet porttitor vel, rhoncus vel elit"
                ]
            configurations = 
                [
                    "Nulla iaculis, nisi id interdum suscipit, turpis purus consequat magna, lobortis commodo purus ligula in elit"
                    "Donec posuere mauris a tincidunt lacinia"
                    "Mauris neque neque, interdum tristique tincidunt ut, facilisis non ante"
                ]
            testScenarios = 
                [                    
                    {
                        description = "Scenario 2"
                        criticality = High
                        testType = Postive
                        testExecutionType = TestExecutionType.Automated
                        configuration = 1
                        input = ["Test1"; "Test2"]
                        expected = [ "Yada"; "Yada" ]
                        attributes = 
                            [
                                Workflow(Initial);
                                Applicant(Individual);
                                DealType(DealType.RouteOne);
                            ]
                        code = Code.None
                    }
                ]
        };

        {
            page = {
                    area = Area.Configuration
                    section = Section.RiskSetup
                    name = "Usury"
            }
            feature = "Usury"
            description = "Vestibulum nec diam ut tortor vulputate mollis eget ut dolor"
            criticality = Low
            affects = 
                [
                    "Suspendisse malesuada faucibus fermentum"
                    "Cras sed posuere mi, ut pulvinar libero"
                    "Sed sed venenatis quam, sed tempus tortor"
                    "Phasellus vitae leo dolor"
                ]
            configurations = 
                [
                    "Vestibulum scelerisque vel metus porttitor convallis"
                ]
            testScenarios = 
                [
                    {
                        description = "Scenario 1"
                        criticality = High
                        testType = Postive
                        testExecutionType = TestExecutionType.Manual
                        configuration = 1
                        input = ["Test1"; "Test2"]
                        expected = [ "Yada"; "Yada" ]
                        attributes = 
                            [
                                Workflow(Initial);
                                Applicant(Individual);
                                DealType(DealType.Manual);
                            ]
                        code = Code.None
                    };
                    {
                        description = "Scenario 2"
                        criticality = High
                        testType = Postive
                        testExecutionType = TestExecutionType.Automated
                        configuration = 1
                        input = ["Test1"; "Test2"]
                        expected = [ "Yada"; "Yada" ]
                        attributes = 
                            [
                                Workflow(Initial);
                                Applicant(Individual);
                                DealType(DealType.RouteOne);
                            ]
                        code = Code.None
                    }
                ]
        };

        {
            page = {
                    area = Area.Funding
                    section = Section.Verifications
                    name = "RIC"
            }
            feature = "RIC"
            description = "Fusce nunc turpis, ornare ac nunc a, venenatis porta massa"
            criticality = High
            affects = 
                [
                    "Nulla gravida massa sit amet neque fringilla, at dapibus dui gravida"
                    "Aenean id laoreet sem"
                    "Nam in leo erat"
                    "Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos"
                ]
            configurations = 
                [
                    "Nulla mollis leo elit, et facilisis orci varius rutrum"
                    "Nulla gravida massa sit amet neque fringilla, at dapibus dui gravida"
                ]
            testScenarios = 
                [
                    {
                        description = "Scenario 2"
                        criticality = High
                        testType = Postive
                        testExecutionType = TestExecutionType.Automated
                        configuration = 1
                        input = ["Test1"; "Test2"]
                        expected = [ "Yada"; "Yada" ]
                        attributes = 
                            [
                                Workflow(Initial);
                                Applicant(Individual);
                                DealType(DealType.RouteOne);
                            ]
                        code = Code.None
                    }
                ]
        };

        {
            page = {
                    area = Area.Reports
                    section = Section.Notifications
                    name = "Report"
            }
            feature = "Report"
            description = "Fusce nunc turpis, ornare ac nunc a, venenatis porta massa"
            criticality = High
            affects = 
                [
                    "Nulla gravida massa sit amet neque fringilla, at dapibus dui gravida"
                    "Aenean id laoreet sem"
                    "Nam in leo erat"
                    "Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos"
                ]
            configurations = 
                [
                    "Nulla mollis leo elit, et facilisis orci varius rutrum"
                    "Nulla gravida massa sit amet neque fringilla, at dapibus dui gravida"
                ]
            testScenarios = 
                [
                    {
                        description = "Scenario 1"
                        criticality = High
                        testType = Postive
                        testExecutionType = TestExecutionType.Manual
                        configuration = 1
                        input = ["Test1"; "Test2"]
                        expected = [ "Yada"; "Yada" ]
                        attributes = 
                            [
                                Workflow(Initial);
                                Applicant(Individual);
                                DealType(DealType.Manual);
                            ]
                        code = Code.None
                    };
                    {
                        description = "Scenario 2"
                        criticality = High
                        testType = Postive
                        testExecutionType = TestExecutionType.Automated
                        configuration = 1
                        input = ["Test1"; "Test2"]
                        expected = [ "Yada"; "Yada" ]
                        attributes = 
                            [
                                Workflow(Initial);
                                Applicant(Individual);
                                DealType(DealType.RouteOne);
                            ]
                        code = Code.None
                    }
                ]
        };
    ]

post tests
