module Model

type Model =
    { mutable Player: Player.Player
      mutable Counter: Counter.Counter }

let init () =
    { Player = Player.init ()
      Counter = Counter.init () }

// This is the important part:
// Applies static values from code INTO the live model (no state loss)
let modelOverride (m: Model) =
    Player.applyStaticOverrides m.Player
    Counter.applyStaticOverrides m.Counter
    m
