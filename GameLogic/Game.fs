namespace GameLogic

open Raylib_cs
open GameAbstractions

type ModularGame() =
    interface IGameLogic with
        member _.Init state =
            // Generally register the slices
            Player.init state.Player
            Counter.init state.Counter

            // ad-hoc items
            state.Enemies <- []

        member _.Update state =
            // generally register the slices
            Player.update state.Player
            Counter.update state.Counter

        member _.Draw state =
            Raylib.BeginDrawing()
            Raylib.ClearBackground Color.RayWhite

            Player.draw state.Player
            Counter.draw state.Counter

            Raylib.EndDrawing()
