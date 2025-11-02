open System
open System.IO
open System.Reflection
open Raylib_cs
open GameLogic
open GameAbstractions

let loadAssembly path =
    let bytes = File.ReadAllBytes(path)
    let asm = Assembly.Load(bytes)
    let factoryType = asm.GetType("GameLogic.GameFactory")
    let createMethod = factoryType.GetMethod("Create")
    createMethod.Invoke(null, [||]) :?> IGame

[<EntryPoint>]
let main _ =
    let logicDll =
        Directory.GetFiles("GameLogic/bin/Debug", "GameLogic.dll", SearchOption.AllDirectories)
        |> Array.head
        |> Path.GetFullPath

    let mutable game = loadAssembly logicDll
    let mutable lastWrite = File.GetLastWriteTime(logicDll)

    Raylib.InitWindow(800, 600, "F# Hot Reload")
    Raylib.SetTargetFPS(60)

    while not (Raylib.WindowShouldClose() |> CBool.op_Implicit) do
        // Watch for rebuild
        let currentWrite = File.GetLastWriteTime(logicDll)

        if currentWrite > lastWrite then
            lastWrite <- currentWrite

            try
                let state = game.SaveState()
                let newGame = loadAssembly logicDll
                newGame.LoadState(state)
                game <- newGame
            with ex ->
                printfn "Reload failed: %A" ex

        let dt = Raylib.GetFrameTime()
        game.Update(dt)

        Raylib.BeginDrawing()
        Raylib.ClearBackground(Color.RayWhite)
        game.Draw()
        Raylib.EndDrawing()

    Raylib.CloseWindow()
    0
