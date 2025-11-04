namespace GameAbstractions

open System.Numerics

type PlayerState = { mutable Pos: Vector2 }
type EnemyState = { mutable Pos: Vector2 }
type CounterState = { mutable time: float32 }

type GameState = {
    mutable Player: PlayerState
    mutable Enemies: EnemyState list
    mutable Counter: CounterState
}

type IGameLogic =
    abstract member Init : GameState -> unit
    abstract member Update : GameState -> unit
    abstract member Draw : GameState -> unit