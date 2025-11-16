module Counter

open Raylib_cs
open Elmish

type Model =
    { Time: float32 }

let init () =
    { Time = 0f }, Cmd.none

type Msg =
    | Tick of float32

let update msg model =
    match msg with
    | Tick dt -> { model with Time = model.Time + dt }, Cmd.none 

let draw model =
    Raylib.DrawText($"time: {int model.Time}", 10, 30, 20, Color.Black)
