module Player

open Raylib_cs
open System.Numerics
open Elmish

type Attributes = { Str: float; Wis: float }

type Model =
    { Pos: Vector2
      Color: Color
      Attributes: Attributes
      Health: int }

let init () =
    { Pos = Vector2(100f, 100f)
      Color = Color.Blue
      Attributes = { Str = 10; Wis = 10 }
      Health = 100 },
    Cmd.none

type Msg = 
  | Travel of float32
  | ChangeColor of Color

// let applyStaticOverrides (p: Model) =
//     p.Color <- Color.Red
//     p.Attributes.Str <- 12

let update msg model =
    match msg with
    | Travel dt ->
        let speed = 100f * dt
        let mutable x = model.Pos.X + speed

        if x > float32 (Raylib.GetScreenWidth()) then
            x <- 0f

        { model with
            Pos = Vector2(x, model.Pos.Y) },
        Cmd.none
    | ChangeColor color ->
      { model with Color = color }, Cmd.none

let draw model =
    Raylib.DrawCircle(int model.Pos.X, int model.Pos.Y, 20f, model.Color)
    Raylib.DrawText($"Str: {model.Attributes.Str}", 160, 30, 16, Color.Black)
    Raylib.DrawText($"Health: {model.Health}", 250, 30, 16, Color.Black)
