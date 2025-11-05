namespace GameLogic

open Raylib_cs
open GameAbstractions
open Player
open Counter

module Json =
    open System.Text.Json
    open System.Text.Json.Serialization

    let options =
        let opts =
            JsonSerializerOptions(WriteIndented = false, PropertyNamingPolicy = JsonNamingPolicy.CamelCase)

        opts.IncludeFields <- true
        opts.Converters.Add(JsonFSharpConverter())
        opts

    let inline tryDeserialize<'Model> (json: string) =
        try
            let v = JsonSerializer.Deserialize<'Model>(json, options)
            if isNull (box v) then None else Some v
        with _ ->
            None

    let serialize (model: 'Model) = JsonSerializer.Serialize(model, options)

    let deserialize<'Model> (json: string) =
        JsonSerializer.Deserialize<'Model>(json, options)

// [<CLIMutable>]
type Model =
    { mutable Player: Player
      mutable Counter: Counter }

type Game() =
    interface IGame with
        member _.Init(saved: string option) =
            let fresh =
                { Player = Player.init
                  Counter = Counter.init () }

            let restored =
                match saved with
                | Some json -> Json.tryDeserialize<Model> json |> Option.defaultValue fresh
                | None -> fresh

            Json.serialize (restored)

        member _.Update(json: string) =
            let model = Json.deserialize<Model> (json)

            Player.update model.Player
            Counter.update model.Counter

            Json.serialize (model)

        member _.Draw(json: string) =
            let m = Json.deserialize<Model> (json)
            Raylib.BeginDrawing()

            Raylib.ClearBackground(Color.RayWhite)

            Player.draw (m.Player)
            Counter.draw (m.Counter)

            Raylib.EndDrawing()
