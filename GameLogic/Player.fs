module Player

open Raylib_cs
open System.Numerics

type Player =
    { mutable Pos: Vector2
      mutable Color: Color }

let init =
    { Pos = Vector2(100f, 100f)
      Color = Color.Blue }

let update (p: Player) =
    let speed = 3f

    if p.Pos.X > (Raylib.GetScreenWidth() |> float32) then
        p.Pos.X <- 0f
    else
        p.Pos.X <- p.Pos.X + speed

let draw (p: Player) =
    Raylib.DrawCircle(int p.Pos.X, int p.Pos.Y, 20f, p.Color)
