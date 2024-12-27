module App

open Browser.Dom
open Elmish
open Elmish.React
open Feliz
open Feliz.Recharts
open Feliz.Markdown
open Feliz.PigeonMaps
open Feliz.Router
open Fable.Core.JsInterop
open Fable.SimpleHttp
open Zanaptak.TypedCssClasses
open Feliz.UseElmish
open Feliz.SelectSearch
open Fable.Core

type Bulma = CssClasses<"https://cdnjs.cloudflare.com/ajax/libs/bulma/0.7.5/css/bulma.min.css", Naming.PascalCase>
type FA = CssClasses<"https://stackpath.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css", Naming.PascalCase>

type Highlight =
    static member inline highlight (properties: IReactProperty list) =
        Interop.reactApi.createElement(importDefault "react-highlight", createObj !!properties)

type State =
    { CurrentPath : string list
      CurrentTab: string list }

let init () =
    let path =
        match document.URL.Split('#') with
        | [| _ |] -> []
        | [| _; path |] -> path.Split('/') |> List.ofArray |> List.tail
        | _ -> []
    { CurrentPath = path
      CurrentTab = path }, Cmd.none

type Msg =
    | TabToggled of string list
    | UrlChanged of string list

let update msg state =
    match msg with
    | UrlChanged segments ->
        { state with CurrentPath = segments },
        match state.CurrentTab with
        | [ ] when segments.Length > 2 ->
            segments
            |> TabToggled
            |> Cmd.ofMsg
        | _ -> Cmd.none
    | TabToggled tabs ->
        match tabs with
        | [ ] -> { state with CurrentTab = [ ] }, Cmd.none
        | _ -> { state with CurrentTab = tabs }, Cmd.none

open Feliz.RoughViz

let fruitSales = [
    ("Oranges", 5.0)
    ("Apples", 8.2)
    ("Strawberry", 10.0)
    ("Peach", 2.0)
    ("Pineapple", 17.0)
    ("Bananas", 10.0)
    ("Mango", 6.4)
]

[<AbstractClass;Sealed; Erase>]
type StaticComponents =
    [<ReactComponent>]
    static member Header (title: string, ?className: string) =
        Html.h1 [
            if className.IsSome then prop.className className.Value
            prop.children [ Html.text title ]
        ]

    [<ReactComponent>] // not required yet - but will be for auto complete controls, ...
    static member BadToggle (isChecked: bool, onChange: bool -> unit, ?key: string) =
        Html.input [
            prop.type'.checkbox
            prop.isChecked isChecked
            prop.onChange (onChange)
        ]

[<ReactComponent>]
let Counter() =
    let (count, setCount) = React.useState 0
    Html.div [
        Html.button [
            prop.style [ style.marginRight 5 ]
            prop.onClick (fun _ -> setCount(count + 1))
            prop.text "Increment"
        ]

        Html.button [
            prop.style [ style.marginLeft 5 ]
            prop.onClick (fun _ -> setCount(count - 1))
            prop.text "Decrement"
        ]

        Html.h1 count
    ]

[<ReactComponent>]
let View() = Examples.View()

[<ReactComponent>]
let CounterWithInput (initialCount: int) =
    let (count, setCount) = React.useState initialCount
    Html.div [
        Html.button [
            prop.style [ style.marginRight 5 ]
            prop.onClick (fun _ -> setCount(count + 1))
            prop.text "Increment"
        ]

        Html.button [
            prop.style [ style.marginLeft 5 ]
            prop.onClick (fun _ -> setCount(count - 1))
            prop.text "Decrement"
        ]

        Html.h1 count
    ]

[<ReactComponent>]
let CounterWithAnonRecord (props: {| initial : int |}) =
    let (count, setCount) = React.useState props.initial
    Html.div [
        Html.button [
            prop.style [ style.marginRight 5 ]
            prop.onClick (fun _ -> setCount(count + 1))
            prop.text "Increment"
        ]

        Html.button [
            prop.style [ style.marginLeft 5 ]
            prop.onClick (fun _ -> setCount(count - 1))
            prop.text "Decrement"
        ]

        Html.h1 count
    ]

[<ReactComponent>]
let CounterWithRecord (props: Examples.CounterRecordProps) =
    let (count, setCount) = React.useState props.initial
    Html.div [
        Html.button [
            prop.style [ style.marginRight 5 ]
            prop.onClick (fun _ -> setCount(count + 1))
            prop.text "Increment"
        ]

        Html.button [
            prop.style [ style.marginLeft 5 ]
            prop.onClick (fun _ -> setCount(count - 1))
            prop.text "Decrement"
        ]

        if props.show then Html.h1 count
    ]

type MoreExamples() = 
    [<ReactComponent>]
    static member CounterWithRecord (props: Examples.CounterRecordProps, ?showAge: bool) =
        let showAge = defaultArg showAge false
        let (count, setCount) = React.useState props.initial
        Html.div [
            Html.button [
                prop.style [ style.marginRight 5 ]
                prop.onClick (fun _ -> setCount(count + 1))
                prop.text "Increment"
            ]

            Html.button [
                prop.style [ style.marginLeft 5 ]
                prop.onClick (fun _ -> setCount(count - 1))
                prop.text "Decrement"
            ]

            if showAge then Html.h1 count
        ]

    [<ReactMemoComponent>]
    static member Memoized (value: int) =
        printfn "Rendering Memoized..."
        Html.p (sprintf "Value: %i" value)

    [<ReactComponent>]
    static member Parent () =
        printfn "Rendering Parent..."
        let count, setCount = React.useState 0
        React.fragment [
            Html.button [
                prop.onClick (fun _ -> setCount(count + 1))
                prop.text "+"
            ]
            Html.p (sprintf "Count: %i" count)
            MoreExamples.Memoized 5
        ]

type KeyedCounterProps = { Key: string; Name: string }

[<ReactComponent>]
let CounterWithKeyedRecord (props: KeyedCounterProps) =
    let (count, setCount) = React.useState 0
    Html.div [
        Html.h1 count
    ]

type LowerKeyedCounterProps = { key: string; Name: string }

[<ReactComponent>]
let CounterWithLowercaseKeyedRecord (props: LowerKeyedCounterProps) =
    let (count, setCount) = React.useState 0
    Html.div [
        Html.h1 count
    ]

//[<ReactComponent(import="Hello", from="external-module")>]
//let Hello (className: string) (children: ReactElement []) = Html.none

[<ReactComponent(exportDefault=true)>]
let HelloExported (className: string) (children: ReactElement []) = Html.none

[<ReactComponent>]
let Counters(show: bool) =
    Html.div [
        Counter()
        CounterWithInput 10
        CounterWithAnonRecord {| initial = 20 |}
        CounterWithRecord { initial = 10; show = true }
        CounterWithKeyedRecord { Key = "keyA"; Name = "Counter" }
        CounterWithLowercaseKeyedRecord { key = "keyB"; Name = "Counter" }
        MoreExamples.CounterWithRecord({ initial = 10; show = true })
        MoreExamples.CounterWithRecord({ initial = 10; show = true }, true)

        //Hello "fsharp" [|
        //    Html.text "content"
        //|]
    ]

[<ReactComponent>]
let CountersWithConditionals (show: bool) (more: int) =
    Html.div [|
        Counter()
        CounterWithInput 10
        CounterWithAnonRecord {| initial = 30 |}
    |]

// ok
[<ReactComponent>]
let MyComponent (label: string, value: string, onChange: string -> Unit) =
  Html.textf "Value: %s" value
// ok
[<ReactComponent>]
let MyComponent4 (value: string, onChange: string -> Unit) =
  React.fragment [
    MyComponent ("Name", value, onChange)
  ]


[<ReactComponent>]
let MyComponent2 (value: string, onChange: string -> Unit) =
  MyComponent ("Name", value, onChange)

let myComponent2 = MyComponent2("", fun _ -> ())

let counterCaller = React.functionComponent(fun () -> Counters(true))

let partiallyAppied = CountersWithConditionals true

let withMore = partiallyAppied 42

let appliedComponents = Html.div [
    StaticComponents.Header("title")
    StaticComponents.Header("title", "className")
    StaticComponents.Header(className="first arg", title="second")
    StaticComponents.BadToggle(isChecked=true, onChange=ignore)
    StaticComponents.BadToggle(isChecked=true, onChange=ignore)
    StaticComponents.BadToggle(isChecked=true, onChange=ignore, key="Hello")
]

let roughBarChart = React.functionComponent(fun () ->
    RoughViz.barChart [
        barChart.title "Fruit Sales"
        barChart.data fruitSales
        barChart.roughness 3
        barChart.color color.skyBlue
        barChart.stroke color.darkCyan
        barChart.axisFontSize 18
        barChart.fillStyle.crossHatch
        barChart.highlight color.lightGreen
    ])

let roughHorizontalBarChart = React.functionComponent(fun () ->
    RoughViz.horizontalBarChart [
        barChart.title "Fruit Sales"
        barChart.data fruitSales
        barChart.roughness 3
        barChart.color color.skyBlue
        barChart.stroke color.darkCyan
        barChart.axisFontSize 18
        barChart.fillStyle.crossHatch
        barChart.highlight color.lightGreen
    ])

let roughPieChart = React.functionComponent(fun () ->
    RoughViz.pieChart [
        pieChart.title "Fruit Sales"
        pieChart.data fruitSales
        pieChart.roughness 3
        pieChart.fillStyle.crossHatch
        pieChart.highlight color.lightGreen
    ])

let basicDropdown = React.functionComponent(fun () ->
    let (selectedValue, setSelectedValue) = React.useState<string option>(None)

    Html.div [
        prop.style [ style.width 400 ]
        prop.children [
            SelectSearch.selectSearch [
                selectSearch.placeholder "Select a language"
                selectSearch.onChange (fun value -> setSelectedValue(Some value))
                selectSearch.options [
                    { value = "en-GB"; name = "English"; disabled = false }
                    { value = "fr-FR"; name = "French"; disabled = false }
                    { value = "nl-NL"; name = "Dutch"; disabled = false }
                ]
            ]

            match selectedValue with
            | None -> Html.h3 "No value selected"
            | Some value -> Html.h3 (sprintf "Selected value '%s'" value)
        ]
    ])

let searchabeDropdown = React.functionComponent(fun () ->
    let (selectedValue, setSelectedValue) = React.useState<string option>(None)

    Html.div [
        prop.style [ style.width 400 ]
        prop.children [
            SelectSearch.selectSearch [
                selectSearch.placeholder "Select a language"
                selectSearch.search true
                selectSearch.onChange (fun value -> setSelectedValue(Some value))
                selectSearch.options [
                    { value = "en-GB"; name = "English"; disabled = false }
                    { value = "fr-FR"; name = "French"; disabled = false }
                    { value = "nl-NL"; name = "Dutch"; disabled = false }
                ]
            ]

            match selectedValue with
            | None -> Html.h3 "No value selected"
            | Some value -> Html.h3 (sprintf "Selected value '%s'" value)
        ]
    ])

let addedDisbaledValuesInDropdown = React.functionComponent(fun () ->
    let (selectedValue, setSelectedValue) = React.useState<string option>(None)

    Html.div [
        prop.style [ style.width 400 ]
        prop.children [
            SelectSearch.selectSearch [
                selectSearch.placeholder "Select a language"
                selectSearch.search true
                selectSearch.onChange (fun value -> setSelectedValue(Some value))
                selectSearch.options [
                    { value = "en-GB"; name = "English"; disabled = true }
                    { value = "fr-FR"; name = "French"; disabled = false }
                    { value = "nl-NL"; name = "Dutch"; disabled = false }
                ]
            ]

            match selectedValue with
            | None -> Html.h3 "No value selected"
            | Some value -> Html.h3 (sprintf "Selected value '%s'" value)
        ]
    ])

let customizedSearchInDropdown = React.functionComponent(fun () ->
    let (selectedValue, setSelectedValue) = React.useState<string option>(None)

    Html.div [
        prop.style [ style.width 400 ]
        prop.children [
            SelectSearch.selectSearch [
                selectSearch.placeholder "Select a language"
                selectSearch.search true
                selectSearch.onChange (fun value -> setSelectedValue(Some value))
                selectSearch.filterOptions (fun item searchQuery -> not item.disabled && item.name.StartsWith searchQuery)
                selectSearch.options [
                    { value = "en-GB"; name = "English"; disabled = true }
                    { value = "fr-FR"; name = "French"; disabled = false }
                    { value = "nl-NL"; name = "Dutch"; disabled = false }
                ]
            ]

            match selectedValue with
            | None -> Html.h3 "No value selected"
            | Some value -> Html.h3 (sprintf "Selected value '%s'" value)
        ]
    ])

let selectMultipleValuesFromDropDown = React.functionComponent(fun () ->
    let (selectedValues, setSelectedValues) = React.useState<string list> [ ]

    Html.div [
        prop.style [ style.width 400 ]
        prop.children [
            match selectedValues with
            | [ ] -> Html.h3 "No values selected"
            | values -> Html.h3 (sprintf "Selected values [%s]" (String.concat ", " values))

            SelectSearch.selectSearch [
                selectSearch.value selectedValues
                selectSearch.placeholder "Select a language"
                selectSearch.multiple true
                selectSearch.printOptions.onFocus
                selectSearch.onChange (fun values -> setSelectedValues values)
                selectSearch.options [
                    { value = "en-GB"; name = "English"; disabled = false }
                    { value = "fr-FR"; name = "French"; disabled = false }
                    { value = "nl-NL"; name = "Dutch"; disabled = false }
                ]
            ]
        ]
    ])

let groupedOptionsInDropdown = React.functionComponent(fun () ->
    Html.div [
        prop.style [ style.width 400 ]
        prop.children [
            SelectSearch.selectSearch [
                selectSearch.placeholder "Choose food category"
                selectSearch.search true
                selectSearch.options [
                    SelectOption.Group {
                        name = "Asian"
                        items = [
                            { value = "sushi"; name = "Sushi"; disabled = false }
                            { value = "ramen"; name = "Ramen"; disabled = false }
                        ]
                    }

                    SelectOption.Group {
                        name = "Italian"
                        items = [
                            { value = "pasta"; name = "Pasta"; disabled = false }
                            { value = "pizza"; name = "Pizza"; disabled = false }
                        ]
                    }
                ]
            ]
        ]
    ])

let customizedButtonsInDropdown = React.functionComponent(fun () ->
    let imageUrlByValue = function
    | "sushi" -> "https://upload.wikimedia.org/wikipedia/commons/thumb/9/9f/California_Sushi_%2826571101885%29.jpg/1200px-California_Sushi_%2826571101885%29.jpg"
    | "ramen" -> "https://www.24kitchen.nl/files/styles/social_media_share/public/2019-02/24K_Job%26Perry_HIGHRes_Snelle_Japanse_Ramen_met_misogehakt_en_een_eitje_L_NOLOGO-1.jpg?itok=YD85hSCT"
    | "pasta" -> "https://www.24kitchen.nl/files/styles/social_media_share/public/2018-09/Screen%20Shot%202018-09-11%20at%2016.46.22.png?itok=5uoIHWGg"
    |  _ -> "https://www.oetker.nl/Recipe/Recipes/oetker.nl/nl-nl/miscallaneous/image-thumb__97330__RecipeDetailsLightBox/pizza-caprese.jpg"

    Html.div [
        prop.style [ style.width 400 ]
        prop.children [
            SelectSearch.selectSearch [
                selectSearch.placeholder "Choose food category"
                selectSearch.search true
                selectSearch.options [
                    SelectOption.Group {
                        name = "Asian"
                        items = [
                            { value = "sushi"; name = "Sushi"; disabled = false }
                            { value = "ramen"; name = "Ramen"; disabled = false }
                        ]
                    }

                    SelectOption.Group {
                        name = "Italian"
                        items = [
                            { value = "pasta"; name = "Pasta"; disabled = false }
                            { value = "pizza"; name = "Pizza"; disabled = false }
                        ]
                    }
                ]

                selectSearch.renderOption (fun properties ->
                    Browser.Dom.console.log(properties)
                    Html.button [
                        yield! properties.attributes
                        prop.className properties.className
                        prop.children [
                            Html.img [
                                prop.style [
                                    style.marginRight 10
                                    style.borderRadius 16
                                ]
                                prop.height 32
                                prop.width 38
                                prop.src (imageUrlByValue properties.option.value)
                            ]

                            Html.span properties.option.name
                        ]
                    ]
                )
            ]
        ]
    ])

let dynamicRoughChart = React.functionComponent(fun () ->
    let (data, setData) = React.useStateWithUpdater [
        ("point1", 70.0)
        ("point2", 40.0)
        ("point3", 65.0)
    ]

    let roughness, setRoughness = React.useState 3

    let title, setTitle = React.useState "Random Data Points"

    let addDataPoint() = setData <| fun previousState ->
        let pointCount = List.length previousState
        let pointLabel = "point" + string (pointCount + 1)
        let nextPoint = (pointLabel, System.Random().NextDouble() * 100.0)
        List.append previousState [ nextPoint ]

    let barClicked (pointIndex: int) =
        let (label, value) = List.item pointIndex data
        setTitle (sprintf "Clicked %s: %f" label value)

    Html.div [

        Html.button [
            prop.className "button is-primary"
            prop.onClick (fun _ -> addDataPoint())
            prop.text "Add Datapoint"
        ]

        Html.h3 (sprintf "Roughness: %d" roughness)

        Html.input [
            prop.className "input"
            prop.type'.range
            prop.min 1
            prop.max 10
            prop.valueOrDefault roughness
            prop.onChange (fun (value: string) -> try setRoughness(int value) with | _ -> ())
            prop.style [ style.marginBottom 20 ]
        ]

        RoughViz.barChart [
            barChart.title title
            barChart.data data
            barChart.roughness roughness
            barChart.color color.skyBlue
            barChart.stroke color.darkCyan
            barChart.axisFontSize 18
            barChart.fillStyle.crossHatch
            barChart.highlight color.lightGreen
            barChart.barClicked (fun index -> barClicked index)
        ]
    ])

let samples = [
    "feliz-elmish-counter", Examples.ElmishCounter.app()
    "simple-components", Examples.ReactComponents.simple
    "multiple-state-variables", Examples.multipleStateVariables()
    "hover-animations", Examples.animationSample
    "working-with-dates", Examples.dateInputsExample()
    "portals", Examples.portalsSample
    "stateful-counter", Examples.ReactComponents.counter()
    "effectful-tab-counter", DelayedComponent.render {| load = Examples.effectfulTabCounter |}
    "effectful-async", DelayedComponent.render {| load = Examples.asyncEffect |}
    "effectful-async-once", DelayedComponent.render {| load = Examples.asyncEffectOnce |}
    "effectful-user-id", DelayedComponent.render {| load = Examples.effectfulUserId |}
    "effectful-timer", DelayedComponent.render {| load = Examples.timer |}
    "effectful-usecancellationtoken", DelayedComponent.render {| load = Examples.TokenCancellation.render |}
    "static-html", Examples.staticHtml()
    "static-markup", Examples.staticMarkup()
    "basic-select-search", basicDropdown()
    "searchable-dropdown", searchabeDropdown()
    "dropdown-with-disabled-values", addedDisbaledValuesInDropdown()
    "customized-search-in-dropdown", customizedSearchInDropdown()
    "multiple-values-from-dropdown", selectMultipleValuesFromDropDown()
    "grouped-options-in-dropdown", groupedOptionsInDropdown()
    "customized-buttons-in-dropdown", customizedButtonsInDropdown()
    "strict-mode", DelayedComponent.render {| load = Examples.strictModeExample |}
    "recharts-main", Samples.Recharts.AreaCharts.Main.chart()
    "recharts-area-simpleareachart", Samples.Recharts.AreaCharts.SimpleAreaChart.chart()
    "recharts-area-stackedareachart", Samples.Recharts.AreaCharts.StackedAreaChart.chart()
    "recharts-area-tinyareachart", Samples.Recharts.AreaCharts.TinyAreaChart.chart()
    "recharts-area-optionalvalues", Samples.Recharts.AreaCharts.OptionalValues.chart()
    "recharts-area-synchronized", Samples.Recharts.AreaCharts.SynchronizedAreaChart.chart()
    "recharts-area-fillbyvalue", Samples.Recharts.AreaCharts.AreaChartFillByValue.chart()
    "recharts-line-simplelinechart", Samples.Recharts.LineCharts.SimpleLineChart.chart()
    "recharts-line-responsivefullwidth", Samples.Recharts.LineCharts.ResponsiveFullWidth.chart()
    "recharts-area-responsefullwidth", Samples.Recharts.AreaCharts.ResponsiveFullWidth.chart()
    "recharts-bar-simplebarchart", Samples.Recharts.BarCharts.SimpleBarChart.chart()
    "recharts-bar-stackedbarchart", Samples.Recharts.BarCharts.StackedBarChart.chart()
    "recharts-bar-mixbarchart", Samples.Recharts.BarCharts.MixBarChart.chart()
    "recharts-bar-tinybarchart", Samples.Recharts.BarCharts.TinyBarChart.chart()
    "recharts-line-customizedlabellinechart", Samples.Recharts.LineCharts.CustomizedLabelLineChart.chart()
    "recharts-line-optionalvalues", Samples.Recharts.LineCharts.OptionalValues.chart()
    "recharts-bar-positiveandnagative", Samples.Recharts.BarCharts.PostiveAndNegative.chart()
    "recharts-line-biaxial", Samples.Recharts.LineCharts.BiaxialLineChart.chart()
    "recharts-pie-twolevel", Samples.Recharts.PieCharts.TwoLevelPieChart.chart()
    "recharts-pie-straightangle", Samples.Recharts.PieCharts.StraightAngle.chart()
    "recharts-pie-customizedlabelpiechart", Samples.Recharts.PieCharts.CustomizedLabelPieChart.chart()
    "recharts-radar-simpleradarchart", Samples.Recharts.RadarCharts.SimpleRadarChart.chart()
    "recharts-scatter-simplescatterchart", Samples.Recharts.ScatterCharts.SimpleScatterChart.chart()
    "recharts-scatter-scatterchartwithlabels", Samples.Recharts.ScatterCharts.ScatterChartWithLabels.chart()
    "pigeonmaps-map-basic", Samples.PigeonMaps.Main.pigeonMap
    "pigeonmaps-map-cities", Samples.PigeonMaps.DynamicMarkers.citiesMap()
    "pigeonmaps-map-popover-hover", Samples.PigeonMaps.MarkerOverlaysOnHover.citiesMap()
    "pigeonmaps-map-popover", Samples.PigeonMaps.MarkerOverlays.citiesMap()
    "pigoenmaps-map-closebutton", Samples.PigeonMaps.MarkerWithCloseButton.citiesMap()
    "pigeonmaps-map-stamenterrain", Samples.PigeonMaps.CustomProviders.pigeonMap
    "popover-basic-sample", Samples.Popover.Basic.sample
    "focus-input-example", Examples.focusInputExample()
    "forward-ref-example", Examples.forwardRefParent()
    "use-imperative-handle", Examples.forwardRefImperativeParent()
    "code-splitting", DelayedComponent.render {| load = Examples.codeSplitting |}
    "code-splitting-delayed", DelayedComponent.render {| load = Examples.codeSplittingDelayed |}
    "use-state-lazy", DelayedComponent.render {| load = Examples.useStateNormalVsLazy |}
    "use-deferred", DelayedComponent.render {| load = UseDeferredExamples.basicDeferred |}
    "deferred-form", DelayedComponent.render {| load = UseDeferredExamples.loginForm |}
    "use-deferred-v2", DelayedComponent.render {|  load = UseDeferredExamples.basicDeferredV2 |}
    "parallel-deferred", DelayedComponent.render {| load = UseDeferredExamples.parallelDeferred |}
    "use-elmish-basic", DelayedComponent.render {| load = UseElmishExamples.counter |}
    "use-elmish-combined", DelayedComponent.render {| load = UseElmishExamples.counterCombined |}
    "use-media-query", DelayedComponent.render {| load = UseMediaQueryExamples.useMediaQueryExample |}
    "use-responsive", DelayedComponent.render {| load = UseMediaQueryExamples.useResponsiveExample |}
    "use-responsive-custom", DelayedComponent.render {| load = UseMediaQueryExamples.useResponsiveCustomBreakpointsExample |}
    "rough-bar-chart", roughBarChart()
    "rough-horizontal-bar-chart", roughHorizontalBarChart()
    "rough-pie-chart", roughPieChart()
    "dynamic-rough-chart", dynamicRoughChart()
    "delay-simple", DelayedComponent.render {| load = DelayExamples.simpleDelay |}
    "delay-fallback", DelayedComponent.render {| load = DelayExamples.delayWithCustomFallback |}
    "delay-nested", DelayedComponent.render {| load = DelayExamples.nestedDelays |}
    "delay-suspense", DelayedComponent.render {| load = DelayExamples.delaySuspense |}
    "delay-template", DelayedComponent.render {| load = DelayExamples.renderDelayTemplate |}
    "delay-suspense-template", DelayedComponent.render {| load = DelayExamples.renderDelaySuspenseTemplate |}
]

let githubPath (rawPath: string) =
    let parts = rawPath.Split('/')
    if parts.Length > 5
    then sprintf "http://www.github.com/%s/%s" parts.[3] parts.[4]
    else rawPath

let centeredSpinner =
    Html.div [
        prop.style [
            style.textAlign.center
            style.marginLeft length.auto
            style.marginRight length.auto
            style.marginTop 50
        ]
        prop.children [
            Html.li [
                prop.className [
                    FA.Fa
                    FA.FaRefresh
                    FA.FaSpin
                    FA.Fa3X
                ]
            ]
        ]
    ]

let renderMarkdown = React.functionComponent(fun (input: {| path: string; content: string |}) ->
    Html.div [
        prop.className [ Bulma.Content; "scrollbar" ]
        prop.style [
            style.width (length.percent 100)
            style.padding (0,20)
        ]
        prop.children [
            if input.path.StartsWith "https://raw.githubusercontent.com" then
                Html.h2 [
                    Html.i [ prop.className [ FA.Fa; FA.FaGithub ] ]
                    Html.anchor [
                        prop.style [ style.marginLeft 10; style.color.lightGreen ]
                        prop.href (githubPath input.path)
                        prop.text "View on Github"
                    ]
                ]

            Markdown.markdown [
                markdown.children input.content
                markdown.escapeHtml false
            ]
        ]
    ])

module MarkdownLoader =

    type State =
        | Initial
        | Loading
        | Errored of string
        | LoadedMarkdown of content: string

    type Msg =
        | StartLoading of path: string list
        | Loaded of Result<string, int * string>

    let init (path: string list) = Initial, Cmd.ofMsg (StartLoading path)

    let resolvePath = function
    | [ one: string ] when one.StartsWith "http" -> one
    | segments -> String.concat "/" segments

    let update (msg: Msg) (state: State) =
        match msg with
        | StartLoading path ->
            let loadMarkdownAsync() = async {
                let! (statusCode, responseText) = Http.get (resolvePath path)
                if statusCode = 200
                then return Loaded (Ok responseText)
                else return Loaded (Error (statusCode, responseText))
            }

            Loading, Cmd.OfAsync.perform loadMarkdownAsync () id

        | Loaded (Ok content) ->
            State.LoadedMarkdown content, Cmd.none

        | Loaded (Error (status, _)) ->
            State.LoadedMarkdown (sprintf "Status %d: could not load markdown" status), Cmd.none

    let load' = React.functionComponent("LoadMarkdown", fun (props: {| path: string list |}) ->
        let (state, dispatch) = React.useElmish(init props.path, update, [| box props.path |])
        match state with
        | Initial -> Html.none
        | Loading -> centeredSpinner
        | LoadedMarkdown content -> renderMarkdown {| path = (resolvePath props.path); content = content |}
        | Errored error ->
            Html.h1 [
                prop.style [ style.color.crimson ]
                prop.text error
            ]
    )

    let load (path: string list) = load' {| path = path |}

// A collapsable nested menu for the sidebar
// keeps internal state on whether the items should be visible or not based on the collapsed state
let nestedMenuList' = React.functionComponent(fun (input: {| state: State; name: string; basePath: string list; elems: (string list -> Fable.React.ReactElement) list; dispatch: Msg -> unit |}) ->
    let collapsed =
        match input.state.CurrentTab with
        | [ ] -> false
        | _ ->
            input.basePath
            |> List.indexed
            |> List.forall (fun (i, segment) ->
                List.tryItem i input.state.CurrentTab
                |> Option.map ((=) segment)
                |> Option.defaultValue false)

    Html.li [
        Html.anchor [
            prop.className Bulma.IsUnselectable
            prop.onClick <| fun _ ->
                match collapsed with
                | true -> input.dispatch <| TabToggled (input.basePath |> List.rev |> List.tail |> List.rev)
                | false -> input.dispatch <| TabToggled input.basePath
            prop.children [
                Html.i [
                    prop.style [ style.marginRight 10 ]
                    prop.className [
                        FA.Fa
                        if not collapsed
                        then FA.FaAngleDown
                        else FA.FaAngleUp
                    ]
                ]

                Html.span input.name
            ]
        ]

        Html.ul [
            prop.className Bulma.MenuList
            prop.style [
                if not collapsed
                then style.display.none
            ]

            prop.children (input.elems |> List.map (fun f -> f input.basePath))
        ]
    ])

// top level label
let menuLabel' = React.functionComponent (fun (input: {| content: string |}) ->
    Html.p [
        prop.className [ Bulma.MenuLabel; Bulma.IsUnselectable ]
        prop.text input.content
    ])

// top level menu
let menuList' = React.functionComponent(fun (input: {| items: Fable.React.ReactElement list |}) ->
    Html.ul [
        prop.className Bulma.MenuList
        prop.style [ style.width (length.percent 95) ]
        prop.children input.items
    ])

let menuItem' = React.functionComponent(fun (input: {| currentPath: string list; name: string; path: string list |}) ->
    Html.li [
        Html.anchor [
            prop.className [
                if input.currentPath = input.path then Bulma.IsActive
                if input.currentPath = input.path then Bulma.HasBackgroundPrimary
            ]
            prop.text input.name
            prop.href (sprintf "#/%s" (String.concat "/" input.path))
        ]
    ])

let menuLabel (content: string) =
    menuLabel' {| content = content |}

let menuList (items: Fable.React.ReactElement list) =
    menuList' {| items = items |}

let allItems = React.functionComponent(fun (input: {| state: State; dispatch: Msg -> unit |} ) ->
    let dispatch = React.useCallback(input.dispatch, [||])

    let menuItem (name: string) (basePath: string list) =
        menuItem'
            {| currentPath = input.state.CurrentPath
               name = name
               path = basePath |}

    let nestedMenuItem (name: string) (extendedPath: string list) (basePath: string list) =
        let path = basePath @ extendedPath
        menuItem'
            {| currentPath = input.state.CurrentPath
               name = name
               path = path |}

    let nestedMenuList (name: string) (basePath: string list) (items: (string list -> Fable.React.ReactElement) list) =
        nestedMenuList'
            {| state = input.state
               name = name
               basePath = basePath
               elems = items
               dispatch = dispatch |}

    let subNestedMenuList (name: string) (basePath: string list) (items: (string list -> Fable.React.ReactElement) list) (addedBasePath: string list) =
        nestedMenuList'
            {| state = input.state
               name = name
               basePath = (addedBasePath @ basePath)
               elems = items
               dispatch = dispatch |}

    Html.div [
        prop.className "scrollbar"
        prop.children [
            menuList [
                menuItem "Overview" [ ]
                menuItem "Installation" [ Urls.Feliz; Urls.Installation ]
                menuItem "Project Template" [ Urls.Feliz; Urls.ProjectTemplate ]
                menuItem "Syntax" [ Urls.Feliz; Urls.Syntax ]
                menuItem "React API Support" [ Urls.Feliz; Urls.ReactApiSupport ]
                menuItem "Type-Safe Styling" [ Urls.Feliz; Urls.TypeSafeStyling ]
                menuItem "Type-Safe CSS" [ Urls.Feliz; Urls.TypeSafeCss ]
                menuItem "Using JSX" [ Urls.Feliz; Urls.UsingJsx ]
                menuItem "Use with Elmish" [ Urls.Feliz; Urls.UseWithElmish ]
                menuItem "React Elmish Components" [ Urls.Hooks; Urls.UseElmish ]
                menuItem "Contributing" [ Urls.Feliz; Urls.Contributing ]
                nestedMenuList "React Components" [ Urls.Feliz; Urls.React ] [
                    nestedMenuItem "Stateless Components" [ Urls.StatelessComponents ]
                    nestedMenuItem "Not Just Functions" [ Urls.NotJustFunctions ]
                    nestedMenuItem "Stateful Components" [ Urls.StatefulComponents ]
                    nestedMenuItem "Components with Effects" [ Urls.EffectfulComponents ]
                    nestedMenuItem "Subscriptions with Effects" [ Urls.SubscriptionsWithEffects ]
                    nestedMenuItem "Context Propagation" [ Urls.ContextPropagation ]
                    nestedMenuItem "Portals" [ Urls.Portals ]
                    nestedMenuItem "Hover Animations" [ Urls.HoverAnimations ]
                    nestedMenuItem "Working With Dates" [ Urls.WorkingWithDates ]
                    nestedMenuItem "Using References" [ Urls.Refs ]
                    nestedMenuItem "Common Pitfalls" [ Urls.CommonPitfalls ]
                    nestedMenuItem "Render Static Html" [ Urls.RenderStaticHtml ]
                    nestedMenuItem "Strict Mode" [ Urls.StrictMode ]
                    nestedMenuItem "Code Splitting" [ Urls.CodeSplitting ]
                    nestedMenuItem "Aliasing props" [ Urls.Aliasing ]
                ]
                menuLabel "Ecosystem"
                nestedMenuList "UI Frameworks" [ Urls.UIFrameworks ] [
                    nestedMenuItem "Feliz.Bulma" [ Urls.Bulma ]
                    nestedMenuItem "Feliz.MaterialUI" [ Urls.Mui ]
                    nestedMenuItem "Feliz.DaisyUI" [ Urls.Daisy ]
                ]

                nestedMenuList "Hooks" [ Urls.Hooks ] [
                    nestedMenuItem "Feliz.UseDeferred" [ Urls.UseDeferred ]
                    nestedMenuItem "Feliz.UseElmish" [ Urls.UseElmish ]
                    nestedMenuItem "Feliz.UseListener" [ Urls.UseListener ]
                    nestedMenuItem "Feliz.UseMediaQuery" [ Urls.UseMediaQuery ]
                    nestedMenuItem "Feliz.UseWorker" [ Urls.UseWorker ]
                ]

                nestedMenuList "Components" [ Urls.Components ] [
                    nestedMenuItem "Feliz.Delay" [ Urls.Delay ]
                    nestedMenuItem "Feliz.Popover" [ Urls.Popover ]
                    nestedMenuItem "Feliz.SelectSearch" [ Urls.SelectSearch ]
                    nestedMenuItem "Feliz.Kawaii" [ Urls.Kawaii ]
                    nestedMenuItem "Feliz.Router" [ Urls.Router ]
                ]

                nestedMenuList "Visualizations" [ Urls.Visualizations ] [
                    nestedMenuItem "Feliz.PigeonMaps" [ Urls.PigeonMaps ]
                    nestedMenuItem "Feliz.Plotly" [ Urls.Plotly ]
                    nestedMenuItem "Feliz.Recharts" [ Urls.Recharts ]
                    nestedMenuItem "Feliz.RoughViz" [ Urls.RoughViz ]
                ]

                nestedMenuList "Testing" [ Urls.Testing ] [
                    subNestedMenuList "Frameworks" [ Urls.Frameworks ] [
                        nestedMenuItem "Fable.Jester" [ Urls.Jest ]
                        nestedMenuItem "Fable.Mocha" [ Urls.Mocha ]
                    ]
                    subNestedMenuList "Utilities" [ Urls.Utilities ] [
                        nestedMenuItem "Fable.FastCheck" [ Urls.FastCheck ]
                        nestedMenuItem "Fable.ReactTestingLibrary" [ Urls.RTL ]
                    ]
                ]

                nestedMenuList "Misc" [ Urls.Misc ] [
                    nestedMenuItem "Felizia" [ Urls.Felizia ]
                    nestedMenuItem "Feliz.Recoil" [ Urls.Recoil ]
                    nestedMenuItem "Fable.SignalR" [ Urls.SignalR ]
                    nestedMenuItem "Feliz.SweetAlert" [ Urls.SweetAlert ]
                    nestedMenuItem "Feliz.ViewEngine" [ Urls.ViewEngine ]
                ]
                menuLabel "Other Docs"
                nestedMenuList "Feliz.PigeonMaps" [ Urls.PigeonMaps ] [
                    nestedMenuItem "Overview" [ Urls.Overview ]
                    nestedMenuItem "Installation" [ Urls.Installation ]
                ]
                nestedMenuList "Feliz.Recharts" [ Urls.Recharts ] [
                    nestedMenuItem "Overview" [ Urls.Overview ]
                    nestedMenuItem "Installation" [ Urls.Installation ]
                    subNestedMenuList "Line Charts" [ Urls.LineCharts ] [
                        nestedMenuItem "Simple Line Chart" [ Urls.SimpleLineChart ]
                        nestedMenuItem "Responsive Full Width" [ Urls.ResponsiveFullWidth ]
                        nestedMenuItem "Customized Label" [ Urls.CustomizedLabelLineChart ]
                        nestedMenuItem "Optional Values" [ Urls.LineChartOptionalValues ]
                        nestedMenuItem "Biaxial Line Chart" [ Urls.BiaxialLineChart ]
                    ]
                    subNestedMenuList "Bar Charts" [ Urls.BarCharts ] [
                        nestedMenuItem "Simple Bar Chart" [ Urls.SimpleBarChart ]
                        nestedMenuItem "Tiny Bar Chart" [ Urls.TinyBarChart ]
                        nestedMenuItem "Stacked Bar Chart" [ Urls.StackedBarChart ]
                        nestedMenuItem "Mix Bar Chart" [ Urls.MixBarChart ]
                        nestedMenuItem "Positive And Negative" [ Urls.PositiveAndNegative ]
                    ]
                    subNestedMenuList "Area Charts" [ Urls.AreaCharts ] [
                        nestedMenuItem "Simple Area Chart" [ Urls.SimpleAreaChart ]
                        nestedMenuItem "Stacked Area Chart" [ Urls.StackedAreaChart ]
                        nestedMenuItem "Tiny Area Chart" [ Urls.TinyAreaChart ]
                        nestedMenuItem "Responsive Full Width" [ Urls.ResponsiveFullWidth ]
                        nestedMenuItem "Optional Values" [ Urls.AreaChartOptionalValues ]
                        nestedMenuItem "Synchronized Charts" [ Urls.SynchronizedAreaChart ]
                        nestedMenuItem "AreaChartFillByValue" [ Urls.AreaChartFillByValue ]
                    ]
                    subNestedMenuList "Pie Charts" [ Urls.PieCharts ] [
                        nestedMenuItem "Two Level Pie Chart" [ Urls.TwoLevelPieChart ]
                        nestedMenuItem "Straight Angle Pie Chart" [ Urls.StraightAngle ]
                        nestedMenuItem "Customized Label Pie Chart" [ Urls.CustomizedLabelPieChart ]
                    ]
                    subNestedMenuList "Radar Charts" [ Urls.RadarCharts ] [
                        nestedMenuItem "Simple Radar Chart" [ Urls.SimpleRadarChart ]
                    ]
                    subNestedMenuList "Scatter Charts" [ Urls.ScatterCharts ] [
                        nestedMenuItem "Simple Scatter Chart" [ Urls.SimpleScatterChart ]
                        nestedMenuItem "Scatter Chart With Labels" [ Urls.ScatterChartWithLabels ]
                    ]
                ]
            ]
        ]
    ])

let sidebar = React.functionComponent(fun (input: {| state: State; dispatch: Msg -> unit |}) ->
    let dispatch = React.useCallback(input.dispatch, [||])

    // the actual nav bar
    Html.aside [
        prop.className Bulma.Menu
        prop.style [
            style.width (length.perc 100)
        ]
        prop.children [
            menuLabel "Feliz"
            allItems {| state = input.state; dispatch = dispatch |}
        ]
    ])

let readme = sprintf "https://raw.githubusercontent.com/%s/%s/master/README.md"

let reactExamples (currentPath: string list) =
    match currentPath with
    | [ Urls.Standalone ] -> [ "Standalone.md" ]
    | [ Urls.StatelessComponents ] -> [ "StatelessComponents.md" ]
    | [ Urls.NotJustFunctions ] -> [ "NotJustFunctions.md" ]
    | [ Urls.StatefulComponents ] -> [ "StatefulComponents.md" ]
    | [ Urls.EffectfulComponents ] -> [ "EffectfulComponents.md" ]
    | [ Urls.SubscriptionsWithEffects ] -> [ "SubscriptionsWithEffects.md" ]
    | [ Urls.ContextPropagation ] -> [ "ContextPropagation.md" ]
    | [ Urls.Portals ] -> [ "Portals.md" ]
    | [ Urls.HoverAnimations ] -> [ "HoverAnimations.md" ]
    | [ Urls.Refs ] -> [ "UsingReferences.md" ]
    | [ Urls.Components ] -> [ "Components.md" ]
    | [ Urls.CommonPitfalls ] -> [ "CommonPitfalls.md" ]
    | [ Urls.RenderStaticHtml ] -> [ "RenderStaticHtml.md" ]
    | [ Urls.StrictMode ] -> [ "StrictMode.md" ]
    | [ Urls.CodeSplitting ] -> [ "CodeSplitting.md" ]
    | [ Urls.WorkingWithDates ] -> [ "WorkingWithDates.md" ]
    | [ Urls.Aliasing ] -> [ "AliasingProp.md" ]
    | _ -> []
    |> fun path -> [ Urls.React ] @ path

let rechartsExamples (currentPath: string list) =
    match currentPath with
    | [ Urls.Overview ] -> [ "README.md" ]
    | [ Urls.Installation ] -> [ "Installation.md" ]
    | Urls.AreaCharts :: rest ->
        match rest with
        | [ Urls.SimpleAreaChart ] -> [ "SimpleAreaChart.md" ]
        | [ Urls.StackedAreaChart ] -> [ "StackedAreaChart.md" ]
        | [ Urls.TinyAreaChart ] -> [ "TinyAreaChart.md" ]
        | [ Urls.ResponsiveFullWidth ] -> [ "ResponsiveFullWidth.md" ]
        | [ Urls.AreaChartOptionalValues ] -> [ "OptionalValues.md" ]
        | [ Urls.SynchronizedAreaChart ] -> [ "SynchronizedAreaChart.md" ]
        | [ Urls.AreaChartFillByValue ] -> [ "AreaChartFillByValue.md" ]
        | _ -> []
        |> List.append [ Urls.AreaCharts ]
    | Urls.LineCharts :: rest ->
        match rest with
        | [ Urls.SimpleLineChart ] -> [ "SimpleLineChart.md" ]
        | [ Urls.ResponsiveFullWidth ] -> [ "ResponsiveFullWidth.md" ]
        | [ Urls.CustomizedLabelLineChart ] -> [ "CustomizedLabelLineChart.md" ]
        | [ Urls.LineChartOptionalValues ] -> [ "OptionalValues.md" ]
        | [ Urls.BiaxialLineChart ] -> [ "BiaxialLineChart.md" ]
        | _ -> []
        |> List.append [ Urls.LineCharts ]
    | Urls.BarCharts :: rest ->
        match rest with
        | [ Urls.SimpleBarChart ] -> [ "SimpleBarChart.md" ]
        | [ Urls.StackedBarChart ] -> [ "StackedBarChart.md" ]
        | [ Urls.MixBarChart ] -> [ "MixBarChart.md" ]
        | [ Urls.TinyBarChart ] -> [ "TinyBarChart.md" ]
        | [ Urls.PositiveAndNegative ] -> [ "PositiveAndNegative.md" ]
        | _ -> []
        |> List.append [ Urls.BarCharts ]
    | Urls.PieCharts :: rest ->
        match rest with
        | [ Urls.TwoLevelPieChart ] -> [ "TwoLevelPieChart.md" ]
        | [ Urls.StraightAngle ] -> [ "StraightAngle.md" ]
        | [ Urls.CustomizedLabelPieChart ] -> [ "CustomizedLabelPieChart.md" ]
        | _ -> []
        |> List.append [ Urls.PieCharts ]
    | Urls.RadarCharts :: rest ->
        match rest with
        | [ Urls.SimpleRadarChart ] -> [ "SimpleRadarChart.md" ]
        | _ -> []
        |> List.append [ Urls.RadarCharts ]
    | Urls.ScatterCharts :: rest ->
        match rest with
        | [ Urls.SimpleScatterChart ] -> [ "SimpleScatterChart.md" ]
        | [ Urls.ScatterChartWithLabels ] -> [ "ScatterChartWithLabels.md" ]
        | _ -> []
        |> List.append [ Urls.ScatterCharts ]
    | _ -> []
    |> fun path -> [ Urls.Recharts ] @ path

let (|PathPrefix|) (segments: string list) (path: string list) =
    if path.Length > segments.Length then
        match List.splitAt segments.Length path with
        | start,end' when start = segments -> Some end'
        | _ -> None
    else None

let loadOrSegment (origPath: string list) (basePath: string list) (path: string list) =
    if path |> List.isEmpty then
        Html.div [ for segment in origPath -> Html.p segment ]
    else basePath @ path |> lazyView MarkdownLoader.load

let content = React.functionComponent(fun (input: {| state: State; dispatch: Msg -> unit |}) ->
    let loadOrSegment = loadOrSegment input.state.CurrentPath

    match input.state.CurrentPath with
    | [ ] -> lazyView MarkdownLoader.load [ "Feliz"; "README.md" ]
    | PathPrefix [ Urls.Feliz ] (Some res) ->
        match res with
        | [ Urls.Overview ] -> [ "README.md" ]
        | [ Urls.ProjectTemplate ] -> [ "ProjectTemplate.md" ]
        | [ Urls.Installation ] -> [ "Installation.md" ]
        | [ Urls.UseWithElmish ] -> [ "UseWithElmish.md" ]
        | [ Urls.UsingJsx ] -> [ "UsingJsx.md" ]
        | [ Urls.Contributing ] -> [ "Contributing.md" ]
        | [ Urls.Syntax ] -> [ "Syntax.md" ]
        | [ Urls.ReactApiSupport ] -> [ "ReactApiSupport.md" ]
        | [ Urls.TypeSafeStyling ] -> [ "TypeSafeStyling.md" ]
        | [ Urls.TypeSafeCss ] -> [ "TypeSafeCss.md" ]
        | [ Urls.ConditionalStyling ] -> [ "ConditionalStyling.md" ]
        | PathPrefix [ Urls.React ] (Some res) -> reactExamples res
        | _ -> []
        |> loadOrSegment [ Urls.Feliz ]
    | PathPrefix [ Urls.UIFrameworks ] (Some res) ->
        match res with
        | [ Urls.Bulma ] -> lazyView MarkdownLoader.load [ readme "Dzoukr" "Feliz.Bulma" ]
        | [ Urls.Mui ] -> lazyView MarkdownLoader.load [ readme "cmeeren" "Feliz.MaterialUI" ]
        | [ Urls.Daisy ] -> lazyView MarkdownLoader.load [ readme "Dzoukr" "Feliz.DaisyUI" ]
        | _ -> Html.div [ for segment in input.state.CurrentPath -> Html.p segment ]
    | PathPrefix [ Urls.Hooks ] (Some res) ->
        match res with
        | [ Urls.UseDeferred ] -> lazyView MarkdownLoader.load [ "Feliz.UseDeferred"; "Index.md" ]
        | [ Urls.UseElmish ] -> lazyView MarkdownLoader.load [ "Feliz.UseElmish"; "Index.md" ]
        | [ Urls.UseListener ] -> lazyView MarkdownLoader.load [ readme "Shmew" "Feliz.UseListener" ]
        | [ Urls.UseMediaQuery ] -> lazyView MarkdownLoader.load [ "Feliz.UseMediaQuery"; "Index.md" ]
        | [ Urls.UseWorker ] -> lazyView MarkdownLoader.load [ readme "Shmew" "Feliz.UseWorker" ]
        | _ -> Html.div [ for segment in input.state.CurrentPath -> Html.p segment ]
    | PathPrefix [ Urls.Components ] (Some res) ->
        match res with
        | [ Urls.Delay ] -> lazyView MarkdownLoader.load [ "Feliz.Delay"; "Index.md" ]
        | [ Urls.Popover ] -> lazyView MarkdownLoader.load [ "Popover"; "README.md" ]
        | [ Urls.SelectSearch ] -> lazyView MarkdownLoader.load [ "SelectSearch";"README.md" ]
        | [ Urls.Kawaii ] -> lazyView MarkdownLoader.load [ "Feliz.Kawaii"; "README.md" ]
        | [ Urls.Router ] -> lazyView MarkdownLoader.load [ readme "Zaid-Ajaj" "Feliz.Router" ]
        | _ -> Html.div [ for segment in input.state.CurrentPath -> Html.p segment ]
    | PathPrefix [ Urls.Visualizations ] (Some res) ->
        match res with
        | [ Urls.PigeonMaps ] -> lazyView MarkdownLoader.load [ "PigeonMaps"; "README.md" ]
        | [ Urls.Plotly ] -> lazyView MarkdownLoader.load [ readme "Shmew" "Feliz.Plotly" ]
        | [ Urls.Recharts ] -> lazyView MarkdownLoader.load [ "Recharts"; "README.md" ]
        | [ Urls.RoughViz ] -> lazyView MarkdownLoader.load [ "RoughViz"; "Index.md" ]
        | _ -> Html.div [ for segment in input.state.CurrentPath -> Html.p segment ]
    | PathPrefix [ Urls.Testing ] (Some res) ->
        match res with
        | PathPrefix [ Urls.Frameworks ] (Some res) ->
            match res with
            | [ Urls.Jest ] -> lazyView MarkdownLoader.load [ readme "Shmew" "Fable.Jester" ]
            | [ Urls.Mocha ] -> lazyView MarkdownLoader.load [ readme "Zaid-Ajaj" "Fable.Mocha" ]
            | _ -> Html.div [ for segment in input.state.CurrentPath -> Html.p segment ]
        | PathPrefix [ Urls.Utilities ] (Some res) ->
            match res with
            | [ Urls.RTL ] -> lazyView MarkdownLoader.load [ "https://raw.githubusercontent.com/Shmew/Fable.Jester/master/src/Fable.ReactTestingLibrary/README.md" ]
            | [ Urls.FastCheck ] -> lazyView MarkdownLoader.load [ "https://raw.githubusercontent.com/Shmew/Fable.Jester/master/src/Fable.FastCheck/README.md" ]
            | _ -> Html.div [ for segment in input.state.CurrentPath -> Html.p segment ]
        | _ -> Html.div [ for segment in input.state.CurrentPath -> Html.p segment ]
    | PathPrefix [ Urls.Misc ] (Some res) ->
        match res with
        | [ Urls.Felizia ] -> lazyView  MarkdownLoader.load [ readme "dbrattli" "Felizia" ]
        | [ Urls.Recoil ] -> lazyView MarkdownLoader.load [ readme "Shmew" "Feliz.Recoil" ]
        | [ Urls.SignalR ] -> lazyView MarkdownLoader.load [ readme "Shmew" "Fable.SignalR" ]
        | [ Urls.SweetAlert ] -> lazyView MarkdownLoader.load [ readme "Shmew" "Feliz.SweetAlert" ]
        | [ Urls.ViewEngine ] -> lazyView  MarkdownLoader.load [ readme "dbrattli" "Feliz.ViewEngine" ]
        | _ -> Html.div [ for segment in input.state.CurrentPath -> Html.p segment ]
    | PathPrefix [ Urls.Recharts ] (Some res) -> rechartsExamples res |> loadOrSegment []
    | PathPrefix [ Urls.PigeonMaps ] (Some res) ->
        match res with
        | [ Urls.Overview ] -> [ "README.md" ]
        | [ Urls.Installation ] -> [ "Installation.md" ]
        | _ -> []
        |> loadOrSegment [ Urls.PigeonMaps ]
    | PathPrefix [ Urls.Tests ] (Some res) ->
        match res with
        | [ Urls.CallbackRef ] -> Tests.runCallbackTests()
        | [ Urls.FileUpload ] -> Tests.fileUpload()
        | [ Urls.ForwardRef ] -> Examples.forwardRefParent()
        | [ Urls.KeyboardKey ] -> Tests.keyboardKey()
        | [ Urls.Refs ] -> Examples.focusInputExample()
        | _ -> React.fragment [ for segment in input.state.CurrentPath -> Html.p segment ]
    | segments -> React.fragment [ for segment in segments -> Html.p segment ])

let main = React.functionComponent(fun (input: {| state: State; dispatch: Msg -> unit |}) ->
    let dispatch = React.useCallback(input.dispatch, [||])

    Html.div [
        prop.className [ Bulma.Tile; Bulma.IsAncestor ]
        prop.children [
            Html.div [
                prop.className [ Bulma.Tile; Bulma.Is2 ]
                prop.children [ sidebar {| state = input.state; dispatch = dispatch |} ]
            ]

            Html.div [
                prop.className [ Bulma.Tile; Bulma.Is10 ]
                prop.style [ style.paddingTop 30 ]
                prop.children [ content {| state = input.state; dispatch = dispatch |} ]
            ]
        ]
    ])

let render' = React.functionComponent(fun (input: {| state: State; dispatch: Msg -> unit |}) ->
    let dispatch = React.useCallback(input.dispatch, [||])

    let application =
        Html.div [
            prop.style [
                style.padding 30
            ]
            prop.children [ main {| state = input.state; dispatch = dispatch |} ]
        ]

    React.router [
        router.onUrlChanged (UrlChanged >> dispatch)
        router.children application
    ])

let render (state: State) dispatch = render' {| state = state; dispatch = dispatch |}

Program.mkProgram init update render
|> Program.withReactSynchronous "root"
|> Program.withConsoleTrace
|> Program.run
