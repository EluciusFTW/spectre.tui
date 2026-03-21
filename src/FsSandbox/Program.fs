open Elmish
open System

type Msg =
    | InputMsg of Input.Msg
    | LogicMsg of Logic.Msg
    | Exit

type Model =
    { LogicModel: Logic.Model
      ExitEvent: Threading.ManualResetEventSlim }

let exitEvent = new Threading.ManualResetEventSlim false

let init () =
    { LogicModel = { Count = 0 }
      ExitEvent = exitEvent },
    []

let update msg model =
    match msg with
    | InputMsg inputMsg ->
        match inputMsg with
        | Input.KeyPressed key ->
            match key.Key with
            | ConsoleKey.D1 -> model, Cmd.ofMsg (LogicMsg(Logic.Increment 1))
            | ConsoleKey.D5 -> model, Cmd.ofMsg (LogicMsg(Logic.Increment 5))
            | ConsoleKey.D2 -> model, Cmd.ofMsg (LogicMsg(Logic.Increment 2))
            | ConsoleKey.Q -> model, Cmd.ofMsg Exit
            | _ -> model, Cmd.none
    | LogicMsg logicMsg ->
        let logicModel, command = Logic.update logicMsg model.LogicModel
        { model with LogicModel = logicModel }, command
    | Exit ->
        model.ExitEvent.Set()
        model, []

open Spectre.Tui
open Spectre.Tui.FSharp.View
open Spectre.Tui.FSharp.Widgets
open Spectre.Tui.FSharp.Layout

let layout =
    layout "main"
    |> splitHorizontally [|
        layout "left";
        layout "right"
        |> splitVertically [|
            layout "one";
            layout "two" |> hide;
            layout "three"
        |]
    |]

let widgets = [
    "green", box Spectre.Console.Color.Green
    "red", box Spectre.Console.Color.Red
]



let view (renderer: Renderer) model dispatch =
    renderer.Draw(fun ctx elapsed ->

        for i in [ 0..count ] do
            ctx.Render(box, shrink vp i i)

        ctx.Render(
            Text(LineExtensions.FromString $"Current Count: {model.LogicModel.Count}"),
            shrink vp (count + 1) (count + 1)
        ))

let logTrace msg model subs =
    eprintfn "Msg: %A" msg
    eprintfn "Model: %A" model
    eprintfn "Subs: %A" subs

let noLog _ __ ___ = ()

Console.Clear()
let terminal = Terminal.Create()
let renderer = Renderer terminal
renderer.SetTargetFps 144

Program.mkProgram init update (view renderer)
|> Input.withKeyListener InputMsg
|> Program.withTrace noLog
|> Program.run

exitEvent.Wait()