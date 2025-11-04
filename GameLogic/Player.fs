namespace GameLogic

open Raylib_cs
open System.Numerics
open GameAbstractions

module Player =
    let init (p: PlayerState) =
        p.Pos <- Vector2(100f, 100f)

    let update (p: PlayerState) =
        let speed = 3f

        if p.Pos.X > (Raylib.GetScreenWidth() |> float32) then
            p.Pos.X <- 0f
        else
            p.Pos.X <- p.Pos.X + speed

    let draw (p: PlayerState) =
        Raylib.DrawCircle(int p.Pos.X, int p.Pos.Y, 20f, Color.Blue)
