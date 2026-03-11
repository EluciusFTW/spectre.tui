open Elmish
open System

type Msg =
    | InputMsg of Input.Msg
    | LogicMsg of Logic.Msg
    | Exit

type Model = { LogicModel: Logic.Model; ExitEvent: Threading.ManualResetEventSlim }

let exitEvent = new Threading.ManualResetEventSlim false;
let init () = { LogicModel = { Count = 0; }; ExitEvent = exitEvent }, []

let update msg model =
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

open Spectre.Tui;

let shrink (rectangle: Rectangle) w h =
    rectangle.Inflate(Size (-1*w, -1*h))

let getInner (rectangle: Rectangle) =
    shrink rectangle 1 1

let view (renderer: Renderer) model dispatch =
    renderer.Draw(fun ctx elapsed->
        let vp = ctx.Viewport
        let box = Rectangle(0, 0, vp.Width, vp.Height)
        let boxes = model.LogicModel.Count
        for i in [0..boxes] do
            ctx.Render(BoxWidget Spectre.Console.Color.Red, shrink box i i)

        ctx.Render(Text (LineExtensions.FromString $"Current Count: {model.LogicModel.Count}"), shrink vp (boxes + 1) (boxes+1))
    )

let logTrace msg model subs =
    eprintfn "Msg: %A" msg
    eprintfn "Model: %A" model
    eprintfn "Subs: %A" subs

let noLog _ __ ___ = ()

Console.Clear ()
let terminal = Terminal.Create ()
let renderer = Renderer terminal
renderer.SetTargetFps 144

Program.mkProgram init update (view renderer)
|> Input.withKeyListener InputMsg
|> Program.withTrace noLog
|> Program.run

exitEvent.Wait()