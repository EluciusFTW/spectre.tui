namespace Spectre.Tui.FSharp.Widgets

open Spectre.Tui

[<AutoOpen>]
module Box =

    let box color =
        BoxWidget color

    let withBorder (box: BoxWidget) border =
        box.Border <- border
        box