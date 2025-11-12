open System
open System.IO
open System.Reflection
open System.Runtime.Loader
open Raylib_cs
open GameAbstractions

let dllPath = Path.GetFullPath("GameLogic/bin/Debug/net9.0/GameLogic.dll")

let mutable lastWriteTime = DateTime.MinValue
let mutable loadContext: AssemblyLoadContext option = None
let mutable game: IGame option = None
let mutable stateBytes: byte[] option = None

let loadGame () =
    try
        if File.Exists(dllPath) then
            printfn "Loading game from %s" dllPath

            // unload previous context if present
            match loadContext with
            | Some ctx ->
                ctx.Unload()
                GC.Collect()
                GC.WaitForPendingFinalizers()
            | None -> ()

            let ctx = new AssemblyLoadContext(Guid.NewGuid().ToString(), isCollectible = true)
            use fs = new FileStream(dllPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            let asm = ctx.LoadFromStream(fs)

            // find type implementing IGame
            let gameType =
                asm.GetTypes()
                |> Array.find (fun t -> typeof<IGame>.IsAssignableFrom(t))

            let instance = Activator.CreateInstance(gameType) :?> IGame

            let restoredState =
                match stateBytes with
                | Some bytes ->
                    printfn "Restoring game state from bytes..."
                    instance.Init(Some bytes)
                | None ->
                    printfn "Initializing new game state..."
                    instance.Init(None)

            game <- Some instance
            stateBytes <- Some restoredState
            loadContext <- Some ctx
            printfn "✅ Game reloaded successfully."
    with e ->
        printfn "❌ Failed to load: %s" e.Message

let tryReloadGame () =
    let info = FileInfo(dllPath)
    if info.Exists && info.LastWriteTime > lastWriteTime then
        printfn "\n🔄 Detected DLL change — reloading..."
        lastWriteTime <- info.LastWriteTime
        loadGame ()

[<EntryPoint>]
let main _ =
    Raylib.InitWindow(400, 300, "F# Hot Reload")
    Raylib.SetTargetFPS(60)

    loadGame ()
    lastWriteTime <- FileInfo(dllPath).LastWriteTime

    while not (Raylib.WindowShouldClose() |> CBool.op_Implicit) do
        tryReloadGame ()

        match game, stateBytes with
        | Some g, Some s ->
            let newState = g.Update(s)
            g.Draw(newState)
            stateBytes <- Some newState
        | _ -> ()

    Raylib.CloseWindow()
    0
