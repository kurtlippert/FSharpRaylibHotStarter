namespace GameLogic

open Raylib_cs
open GameAbstractions

type SimpleGame() =
    let mutable x = 100.0f
    let mutable y = 100.0f
    let speed = 3.0f

    interface IGame with
        member _.Init() =
            x <- 100.0f
            y <- 100.0f

        member _.Update() =
            x <- x + speed
            if x > (Raylib.GetScreenWidth() |> float32) then
                x <- 0f
            // if Raylib.IsKeyDown(KeyboardKey.Right) |> CBool.op_Implicit then
            //     x <- x + speed
            // if Raylib.IsKeyDown(KeyboardKey.Left) |> CBool.op_Implicit then
            //     x <- x - speed
            // if Raylib.IsKeyDown(KeyboardKey.Down) |> CBool.op_Implicit then
            //     y <- y + speed
            // if Raylib.IsKeyDown(KeyboardKey.Up) |> CBool.op_Implicit then
            //     y <- y - speed

        member _.Draw() =
            Raylib.BeginDrawing()
            Raylib.ClearBackground(Color.RayWhite)
            Raylib.DrawCircle(int x, int y, 20.0f, Color.Blue)
            Raylib.EndDrawing()
