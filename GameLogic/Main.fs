module Main

open Elmish


type Model =
    { Player: Player.Model
      Counter: Counter.Model }

type Msg =
    | PlayerMsg of Player.Msg
    | CounterMsg of Counter.Msg

let init () =
    let playerModel, playerCmd = Player.init ()
    let counterModel, counterCmd = Counter.init ()

    { Player = playerModel
      Counter = counterModel },
    Cmd.batch [ Cmd.map PlayerMsg playerCmd; Cmd.map CounterMsg counterCmd ]

let update msg model =
    match msg with
    | PlayerMsg playerMsg ->
        let playerModel, playerCmd = Player.update playerMsg model.Player
        { model with Player = playerModel }, Cmd.map PlayerMsg playerCmd

    | CounterMsg cmsg ->
        let counterModel, counterCmd = Counter.update cmsg model.Counter
        { model with Counter = counterModel }, Cmd.map CounterMsg counterCmd

let draw model =
    Player.draw model.Player
    Counter.draw model.Counter
