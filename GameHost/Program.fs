open System
open System.IO
open System.Reflection
open System.Runtime.Loader
open Raylib_cs
open GameAbstractions

let dllPath = Path.GetFullPath("GameLogic/bin/Debug/net9.0/GameLogic.dll")

let mutable lastWriteTime = DateTime.MinValue
let mutable loadContext: AssemblyLoadContext option = None
let mutable gameLogic: IGameLogic option = None

let gameState =
    { Player = { Pos = System.Numerics.Vector2(0f, 0f) }
      Enemies = []
      Counter = { time = 0f } }

let mutable lastGameTypeName = ""

let loadGame () =
    try
        if File.Exists(dllPath) then
            printfn "Loading game from %s" dllPath

            match loadContext with
            | Some ctx ->
                ctx.Unload()
                GC.Collect()
                GC.WaitForPendingFinalizers()
            | None -> ()

            let ctx = new AssemblyLoadContext(Guid.NewGuid().ToString(), isCollectible = true)
            use fs = new FileStream(dllPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            let asm = ctx.LoadFromStream(fs)

            let t = asm.GetTypes() |> Array.find (fun t -> typeof<IGameLogic>.IsAssignableFrom(t))
            let typeName = t.FullName

            let instance = Activator.CreateInstance(t) :?> IGameLogic
            let reinitNeeded = typeName <> lastGameTypeName

            lastGameTypeName <- typeName
            loadContext <- Some ctx
            gameLogic <- Some instance

            if reinitNeeded then
                printfn "🧩 Type changed (%s) — reinitializing state" typeName
                instance.Init(gameState)
            else
                printfn "♻️ Reloaded code, keeping existing state"
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
    Raylib.InitWindow(400, 300, "F# Modular Hot Reload")
    Raylib.SetTargetFPS(60)

    loadGame ()
    lastWriteTime <- FileInfo(dllPath).LastWriteTime

    while not (Raylib.WindowShouldClose() |> CBool.op_Implicit) do
        tryReloadGame ()

        match gameLogic with
        | Some g ->
            g.Update(gameState)
            g.Draw(gameState)
        | None -> ()

    Raylib.CloseWindow()
    0
