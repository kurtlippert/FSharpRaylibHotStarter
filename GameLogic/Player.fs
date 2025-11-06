module Player

open Raylib_cs
open System.Numerics

type Player =
    { mutable Pos: Vector2
      mutable Color: Color }

let init () =
    { Pos = Vector2(100f, 100f)
      Color = Color.Red }

let applyStaticOverrides (p: Player) =
    // Any static override goes here:
    p.Color <- Color.Blue

let update (p: Player) =
    let speed = 3f
    p.Pos <- Vector2(p.Pos.X + speed, p.Pos.Y)
    if p.Pos.X > (Raylib.GetScreenWidth() |> float32) then
        p.Pos.X <- 0f

let draw (p: Player) =
    Raylib.DrawCircle(int p.Pos.X, int p.Pos.Y, 20f, p.Color)
