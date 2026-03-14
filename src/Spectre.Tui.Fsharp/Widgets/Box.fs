namespace Spectre.Tui.FSharp.Widgets

open Spectre.Tui
open Spectre.Tui.FSharp.View

[<AutoOpen>]
module Box =
    let box = BoxWidget Spectre.Console.Color.Red

    let withBorder (box: BoxWidget) border =
        box.Border <- border
        box