module Counter

open Raylib_cs

type Counter =
    { mutable time: float32 }

let init () : Counter =
    { time = 0f }

let applyStaticOverrides (_: Counter) =
    () // no static overrides right now

let update (c: Counter) =
    c.time <- c.time + Raylib.GetFrameTime()

let draw (c: Counter) =
    Raylib.DrawText($"time: {int c.time}", 10, 30, 20, Color.Black)
