module Player

open Raylib_cs
open System.Numerics

type Attributes =
    { mutable Str: float
      mutable Wis: float }

type Player =
    { mutable Pos: Vector2
      mutable Color: Color
      mutable Attributes: Attributes
      mutable Health: int
    }

let init () =
    { Pos = Vector2(100f, 100f)
      Color = Color.Red
      Attributes = { Str = 10; Wis = 10 }
      Health = 100
    }

let applyStaticOverrides (p: Player) =
    p.Color <- Color.Red
    p.Attributes.Str <- 12

let update (p: Player) =
    let speed = 3f
    p.Pos <- Vector2(p.Pos.X + speed, p.Pos.Y)

    if p.Pos.X > (Raylib.GetScreenWidth() |> float32) then
        p.Pos.X <- 0f

let draw (p: Player) =
    Raylib.DrawCircle(int p.Pos.X, int p.Pos.Y, 20f, p.Color)
    Raylib.DrawText($"Str: {p.Attributes.Str}", 160, 30, 16, Color.Black)
    Raylib.DrawText($"Health: {p.Health}", 250, 30, 16, Color.Black)
