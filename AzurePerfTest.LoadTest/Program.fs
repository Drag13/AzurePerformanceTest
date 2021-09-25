open NBomber
open NBomber.Contracts
open NBomber.FSharp
open NBomber.Plugins.Http.FSharp
open System

let getFromArgsOrDefault (parser: (string)-> 'a) (args:string[]) (position:int) (defaultValue:'a) = 
    if args.Length > position then parser(args.[position]) else defaultValue

let getStringFromArgs = getFromArgsOrDefault (fun (x)->x)
let getIntFromArgs = getFromArgsOrDefault (fun x-> Convert.ToInt32 x)

[<EntryPoint>]
let main argv =
    let url = getStringFromArgs argv 0 "https://azperf.azurewebsites.net/"
    let rate = getIntFromArgs argv 1 1
    let loadingTimeSeconds = getIntFromArgs argv 2 60
    printfn "Bombing %s for %d rps during %d seconds" url rate loadingTimeSeconds
    
    let step = Step.create("index.html", 
                            timeout = seconds 10,
                            clientFactory = HttpClientFactory.create(), 
                            execute = fun context -> 
                            Http.createRequest "GET" url
                            |> Http.withHeader "Accept" "text/html"
                            |> Http.send context)

    let scenario = 
        Scenario.create "simple_http" [step]       
        |> Scenario.withWarmUpDuration(seconds 5)
        |> Scenario.withLoadSimulations [
                   InjectPerSec(rate = rate, during = seconds loadingTimeSeconds)
               ]    

    NBomberRunner.registerScenario scenario
    |> NBomberRunner.run
    |> ignore

    0 // return an integer exit 
    