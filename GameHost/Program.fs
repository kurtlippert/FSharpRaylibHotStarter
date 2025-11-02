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

let loadGame () =
    try
        if File.Exists(dllPath) then
            printfn "Loading game from %s" dllPath

            // Unload previous context if exists
            match loadContext with
            | Some ctx ->
                try
                    ctx.Unload()
                    GC.Collect()
                    GC.WaitForPendingFinalizers()
                with e ->
                    printfn "Warning: failed to unload: %s" e.Message
            | None -> ()

            // Create new context and load assembly bytes
            let ctx = new AssemblyLoadContext(Guid.NewGuid().ToString(), isCollectible = true)

            use fs =
                new FileStream(dllPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)

            let asm = ctx.LoadFromStream(fs)

            // Find the game type and instance
            let t =
                asm.GetTypes()
                |> Array.find (fun t -> typeof<IGame>.IsAssignableFrom(t) && not t.IsInterface)

            let instance = Activator.CreateInstance(t) :?> IGame
            instance.Init()

            loadContext <- Some ctx
            game <- Some instance

            printfn "✅ Game reloaded successfully."
    with e ->
        printfn "Failed to load: %s" e.Message

let tryReloadGame () =
    let info = FileInfo(dllPath)

    if info.Exists && info.LastWriteTime > lastWriteTime then
        printfn "\n🔄 Detected DLL change — reloading..."
        lastWriteTime <- info.LastWriteTime
        loadGame ()

[<EntryPoint>]
let main _ =
    Raylib.InitWindow(800, 600, "F# Hot Reload Demo")
    Raylib.SetTargetFPS(60)

    loadGame ()
    lastWriteTime <- FileInfo(dllPath).LastWriteTime

    while not (Raylib.WindowShouldClose() |> CBool.op_Implicit) do
        tryReloadGame ()

        match game with
        | Some g ->
            g.Update()
            g.Draw()
        | None -> ()

    Raylib.CloseWindow()
    0
