open PuppeteerSharp

[<EntryPoint>]
let main (argv: string array) : int =
    let fetcher = BrowserFetcher()

    let info =
        fetcher.DownloadAsync "848005" |> Async.AwaitTask |> Async.RunSynchronously

    let exe = fetcher.GetExecutablePath info.Revision

    let path = System.IO.Path.GetFullPath "../public-tests"

    Puppeteer.runTestsWithConfig
        {| Arguments = [| "--no-sandbox" |]
           ExecutablePath = exe |}
        path
    |> Async.RunSynchronously
