namespace GameLogic

open Raylib_cs
open GameAbstractions

type SimpleGame() =
    let mutable x = 100.0f
    let mutable z = 100.0f
    let speed = 3.0f
    let mutable time = 0f

    interface IGame with
        member _.Init() =
            x <- 100.0f
            z <- 100.0f
            time <- 0f

        member _.Update() =
            x <- x + speed
            if x > (Raylib.GetScreenWidth() |> float32) then
                x <- 0f

            time <- time + Raylib.GetFrameTime()

        member _.Draw() =
            Raylib.BeginDrawing()
            Raylib.ClearBackground Color.RayWhite

            Raylib.DrawText("time: " + (time |> int |> string), 10, 30, 20, Color.Black)
            Raylib.DrawCircle(int x, int z, 20.0f, Color.Blue)

            Raylib.EndDrawing()
