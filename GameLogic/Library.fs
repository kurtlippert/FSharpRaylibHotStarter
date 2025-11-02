namespace GameLogic

open Raylib_cs
open System.Numerics
open GameAbstractions

type SimpleGame() =
    let mutable pos = Vector2(400.0f, 300.0f)
    let speed = 100.0f

    interface IGame with
        member _.Update(dt) =
            pos.X <- pos.X + speed * dt

            if pos.X > (Raylib.GetScreenWidth() |> float32) then
                pos.X <- 0f

        member _.Draw() =
            Raylib.DrawCircleV(pos, 30.0f, Color.Blue)

        member _.SaveState() = box pos

        member _.LoadState(state: obj) = pos <- unbox state

module GameFactory =
    let Create () : IGame = SimpleGame() :> IGame
