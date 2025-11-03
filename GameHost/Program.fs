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
let mutable savedState: Map<string, obj> = Map.empty

/// Reflectively copy public instance fields from one object to a Map
let captureState (instance: obj) : Map<string, obj> =
    let t = instance.GetType()

    t.GetFields(BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    |> Array.fold (fun acc f -> acc.Add(f.Name, f.GetValue(instance))) Map.empty

/// Apply stored values to matching fields on new instance
let restoreState (instance: obj) (state: Map<string, obj>) =
    let t = instance.GetType()

    for f in t.GetFields(BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.NonPublic) do
        match state.TryFind(f.Name) with
        | Some v when f.FieldType.IsAssignableFrom(v.GetType()) ->
            try
                f.SetValue(instance, v)
            with _ ->
                ()
        | _ -> ()

    instance

let loadGame () =
    try
        if File.Exists(dllPath) then
            printfn "Loading game from %s" dllPath

            // Unload previous context if exists
            match loadContext with
            | Some ctx ->
                match game with
                | Some g -> savedState <- captureState g
                | None -> ()

                ctx.Unload()
                GC.Collect()
                GC.WaitForPendingFinalizers()
                printfn "Unloaded previous assembly."
            | None -> ()

            // Create new context and load fresh assembly
            let ctx = new AssemblyLoadContext(Guid.NewGuid().ToString(), isCollectible = true)

            use fs =
                new FileStream(dllPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)

            let asm = ctx.LoadFromStream(fs)

            // Find the IGame implementation
            let t =
                asm.GetTypes()
                |> Array.find (fun t -> typeof<IGame>.IsAssignableFrom(t) && not t.IsInterface && not t.IsAbstract)

            let instance = Activator.CreateInstance(t) :?> IGame

            // Attempt to restore previous state
            if not savedState.IsEmpty then
                printfn "Restoring previous state..."
                restoreState instance savedState |> ignore
            else
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
