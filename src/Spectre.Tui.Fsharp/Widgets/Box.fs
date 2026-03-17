namespace Spectre.Tui.FSharp.Widgets

open Spectre.Tui

[<AutoOpen>]
module Box =
    let box = BoxWidget Spectre.Console.Color.Red

    let withBorder (box: BoxWidget) border =
        box.Border <- border
        box