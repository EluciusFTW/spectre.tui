open Elmish
open System

type Msg =
    | InputMsg of Input.Msg
    | LogicMsg of Logic.Msg
    | Exit

type Model = { LogicModel: Logic.Model; ExitEvent: Threading.ManualResetEventSlim }

let exitEvent = new System.Threading.ManualResetEventSlim(false);
let init () = { LogicModel = { Count = 0; }; ExitEvent = exitEvent }, []

let update msg model =
    Console.Clear ()
    match msg with
    | InputMsg inputMsg ->
        match inputMsg with
        | Input.KeyPressed key ->
            match key.Key with
            | ConsoleKey.D1 -> model, Cmd.ofMsg (LogicMsg (Logic.Increment 1))
            | ConsoleKey.D5 -> model, Cmd.ofMsg (LogicMsg (Logic.Increment 5))
            | ConsoleKey.D2 -> model, Cmd.ofMsg (LogicMsg (Logic.Increment 2))
            | ConsoleKey.Q -> model, Cmd.ofMsg Exit
            | _ -> model, Cmd.none
    | LogicMsg logicMsg ->
        let logicModel, command = Logic.update logicMsg model.LogicModel
        { model with LogicModel = logicModel }, command
    | Exit -> model.ExitEvent.Set()
              model, []

let view model dispatch =
    printfn "---------------------------------"
    printfn "Count: %d" model.LogicModel.Count

let logTrace msg model subs =
    eprintfn "Msg: %A" msg
    eprintfn "Model: %A" model
    eprintfn "Subs: %A" subs

let noLog _ __ ___ = ()

open Spectre.Tui

Console.Clear ()
let terminal = Terminal.Create ()
let renderer = Renderer terminal
renderer.SetTargetFps 144

Program.mkProgram init update view
|> Input.withKeyListener InputMsg
|> Program.withTrace noLog
|> Program.run

exitEvent.Wait()