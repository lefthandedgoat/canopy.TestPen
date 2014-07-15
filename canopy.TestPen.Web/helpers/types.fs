module types

open FSharp.Data

type TestCases = JsonProvider<"""{"cases":[{"page":{"area":{"Case":"None","Fields":[]},"section":{"Case":"None","Fields":[]},"name":"NAME"},"bulletPoints":[{"Item1":"Bullet Point 1","Item2":{"Case":"None","Fields":[]}},{"Item1":"Bullet Point 2","Item2":{"Case":"None","Fields":[]}}],"status":{"Case":"None","Fields":[]},"tShirtSize":{"Case":"None","Fields":[]},"feature":"FEATURE","description":"DESCRIPTION","criticality":{"Case":"None","Fields":[]},"affects":["AFFECTS 1","AFFECTS 2"],"configurations":["CONFIGURATION 1","CONFIGURATION 2"],"testScenarios":[{"description":"SCENARIO 1 DESCRIPTION","criticality":{"Case":"None","Fields":[]},"testType":{"Case":"None","Fields":[]},"testExecutionType":{"Case":"None","Fields":[]},"configuration":1,"inputs":["INPUTS 1","INPUTS 2"],"steps":["STEPS 1","STEPS 2"],"expected":["EXPECTS 1","EXPECTS 2"],"code":{"Case":"None","Fields":[]},"attributes":[{"Case":"Workflow","Fields":[{"Case":"Initial","Fields":[]}]},{"Case":"DealType","Fields":[{"Case":"Manual","Fields":[]}]}]},{"description":"SCENARIO 2 DESCRIPTION","criticality":{"Case":"None","Fields":[]},"testType":{"Case":"None","Fields":[]},"testExecutionType":{"Case":"None","Fields":[]},"configuration":1,"inputs":["INPUTS 1","INPUTS 2"],"steps":["STEPS 1","STEPS 2"],"expected":["EXPECTS 1","EXPECTS 2"],"code":{"Case":"None","Fields":[]},"attributes":[{"Case":"Workflow","Fields":[{"Case":"None","Fields":[]}]},{"Case":"DealType","Fields":[{"Case":"None","Fields":[]}]}]}],"documentation":"http://www.google.com"}],"reportResults":[{"id":1,"label":"S","count":68},{"id":1,"label":"M","count":44},{"id":1,"label":"XS","count":11},{"id":1,"label":"XL","count":10},{"id":1,"label":"None","count":9},{"id":1,"label":"L","count":8},{"id":1,"label":"XXL","count":0},{"id":2,"label":"Ready","count":89},{"id":2,"label":"TestCase","count":41},{"id":2,"label":"None","count":9},{"id":2,"label":"Done","count":8},{"id":2,"label":"Bullet","count":3},{"id":2,"label":"Automation","count":0},{"id":3,"label":"None","count":1488},{"id":3,"label":"Automated","count":92},{"id":3,"label":"Manual","count":29},{"id":3,"label":"Bulk","count":0},{"id":4,"label":"total","count":1609}]}""">

type getCasesResult = { 
    CaseId : int
    Area : string
    Section : string
    Name : string
    Criticality : string
    Pass : int
    Fail : int
    Skip : int
    None : int 
    Total: int
    Percent : decimal
}

type getPSFNResult = { Pass : int; Fail : int; Skip : int; None : int }