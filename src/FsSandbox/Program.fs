open Elmish
open System
open FsSandbox
open FsSandbox.ListWidget

type Msg =
    | InputMsg of Input.Msg
    | LogicMsg of Logic.Msg
    | ListMsg of ListWidget.Msg
    | Exit

type Model =
    { LogicModel: Logic.Model
      ListModel: ListWidget.Model
      ExitEvent: Threading.ManualResetEventSlim }

let exitEvent = new Threading.ManualResetEventSlim false

let init () =
    { LogicModel = { Count = 0 }
      ListModel = { index = 0 }
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
            | ConsoleKey.UpArrow -> model, Cmd.ofMsg (ListMsg(Up))
            | ConsoleKey.DownArrow -> model, Cmd.ofMsg (ListMsg(Down))
            | ConsoleKey.Q -> model, Cmd.ofMsg Exit
            | _ -> model, Cmd.none
    | LogicMsg logicMsg ->
        let logicModel, command = Logic.update logicMsg model.LogicModel
        { model with LogicModel = logicModel }, command
    | ListMsg listMsg ->
        match listMsg with
        | Up -> { model with ListModel = { index = model.ListModel.index - 1 } }, []
        | Down -> { model with ListModel = { index = model.ListModel.index + 1 } }, []
    | Exit ->
        model.ExitEvent.Set()
        model, []

open Spectre.Tui
open Spectre.Tui.FSharp.View
open Spectre.Tui.FSharp.Widgets

let view (renderer: Renderer) model dispatch =
    renderer.Draw(fun ctx elapsed ->
        let vp = ctx.Viewport
        let count = model.LogicModel.Count

        for i in [ 0..count ] do
            RenderContextExtensions.Render(ctx, box, shrink vp i i)

        RenderContextExtensions.Render(
            ctx,
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