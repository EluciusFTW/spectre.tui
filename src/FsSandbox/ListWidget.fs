module FsSandbox.ListWidget

open Spectre.Tui
open Spectre.Tui.FSharp.Widgets
open Spectre.Console

type Model = { index: int }

type Msg =
    | Up
    | Down

let view (renderer: Renderer) (model: Model) dispatch =
    renderer.Draw(fun ctx elapsed ->
        let vp = ctx.Viewport

        let items = [
            ListItem "F# Elmish"
            ListItem "Spectre.Tui"
            ListItem "List Widget"
            ListItem "Interactive"
            ListItem "Sandbox"
        ]

        let listW =
            listWidget items
            |> selectedIndex model.index
            |> withHighlightSymbol (LineExtensions.FromString("> ", Style(Color.Blue)))
            |> withWrapAround true

        let listArea = Rectangle(vp.X + 2, vp.Y + 2, 30, 10)
        RenderContextExtensions.Render(ctx, listW, listArea)

        let info = $"Selected Index: {model.index}"
        RenderContextExtensions
            .Render(ctx, Text(LineExtensions.FromString(info, Style(Color.Green))), Rectangle(vp.X + 2, vp.Y + 13, 30, 1))
    )
