module Counter

open Raylib_cs

type Counter = { mutable time: float32 }

let init () : Counter = { time = 0f }

let update (c: Counter) =
    c.time <- c.time + Raylib.GetFrameTime()

let draw (c: Counter) =
    Raylib.DrawText("time: " + (c.time |> int |> string), 10, 30, 20, Color.Black)
