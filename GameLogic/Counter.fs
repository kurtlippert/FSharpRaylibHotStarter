namespace GameLogic

open Raylib_cs
open System.Numerics
open GameAbstractions

module Counter =
    let init (c: CounterState) =
        c.time <- 0f

    let update (c: CounterState) =
        c.time <- c.time + Raylib.GetFrameTime()

    let draw (c: CounterState) =
        Raylib.DrawText("time: " + (c.time |> int |> string), 10, 30, 20, Color.Black)
