module Player

open Raylib_cs

type State = { mutable X: float32; mutable Y: float32 }

let init () = { X = 400f; Y = 300f }

let update state =
    let speed = 3f

    if state.X > (Raylib.GetScreenWidth() |> float32) then
        state.X <- 0f
    else
        state.X <- state.X + speed

let draw state =
    Raylib.DrawCircle((int state.X), (int state.Y), 20f, Color.Red)
