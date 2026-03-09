open Elmish
open System

type Model = { Count: int; ShouldExit: bool }

type Msg =
    | KeyPressed of ConsoleKeyInfo
    | Increment of int
    | Decrement
    | Exit

let init () = { Count = 0; ShouldExit = false }, []

let update msg model =
    match msg with
    | KeyPressed key ->
        match key.Key with
        | ConsoleKey.D1 -> model, Cmd.ofMsg (Increment 1)
        | ConsoleKey.D5 -> model, Cmd.ofMsg (Increment 5)
        | ConsoleKey.D2 -> model, Cmd.ofMsg (Increment 2)
        | ConsoleKey.Q -> model, Cmd.ofMsg (Exit)
        | _ -> model, Cmd.none
    | Increment n -> { model with Count = model.Count + n }, []
    | Decrement -> { model with Count = model.Count - 1 }, []
    | Exit -> model, Cmd.ofMsg (Increment 7)

let view model dispatch =
    printfn "Count: %d" model.Count
    printfn "[+] increment | [-] decrement | [q] quit"

let keyListener (model: Model) : Sub<Msg> =
    let sub dispatch =
        let cts = new System.Threading.CancellationTokenSource()

        let rec loop () =
            async {
                let key = Console.ReadKey(true)
                dispatch (KeyPressed key)
                if not model.ShouldExit then do! loop () else cts.Cancel()
            }

        Async.Start(loop (), cts.Token)

        { new System.IDisposable with
            member _.Dispose() = cts.Cancel() }

    if model.ShouldExit then [] else [ [ "keyListener" ], sub ]

open Spectre.Tui

use terminal = Terminal.Create()
let renderer = new Renderer(terminal)
renderer.SetTargetFps(144) |> ignore

Program.mkProgram init update view
|> Program.withSubscription keyListener
|> Program.withTrace (fun msg model _ ->
    eprintfn "Msg: %A" msg
    eprintfn "Model: %A" model)
|> Program.run