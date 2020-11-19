namespace Feliz.Styles

open Fable.Core

[<Erase>]
type grid =
    static member inline span(value: string) : IGridSpan = unbox("span " + value)
    static member inline span(value: string, count: int) : IGridSpan = unbox("span " + value + " " + (unbox<string> count))
    static member inline span(value: int) : IGridSpan = unbox("span " + (unbox<string> value))

    static member inline namedLine(value: string) : IGridTemplateItem = unbox ("[" + value + "]")
    static member inline namedLines(value: string[]) : IGridTemplateItem = unbox ("[" + (String.concat " " value) + "]")
    static member inline namedLines(value: string list) : IGridTemplateItem = unbox ("[" + (String.concat " " value) + "]")
    static member inline templateWidth(value: ICssUnit) : IGridTemplateItem = unbox value
