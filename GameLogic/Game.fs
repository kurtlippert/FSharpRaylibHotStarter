namespace GameLogic

open Raylib_cs
open GameAbstractions
open Model

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

    let serialize (model: 'Model) =
        JsonSerializer.Serialize(model, options)

    let deserialize<'Model> (json: string) =
        JsonSerializer.Deserialize<'Model>(json, options)

type Game() =
    interface IGame with
        member _.Init(saved) =
            let model =
                match saved |> Option.bind Json.tryDeserialize with
                | Some m -> m
                | None -> Model.init ()

            modelOverride model |> Json.serialize

        member _.Update(json) =
            let m = Json.deserialize<Model> (json)

            Player.update m.Player
            Counter.update m.Counter

            modelOverride m |> Json.serialize

        member _.Draw(json) =
            let m = Json.deserialize<Model> json
            Raylib.BeginDrawing()
            Raylib.ClearBackground(Color.RayWhite)

            Player.draw m.Player
            Counter.draw m.Counter

            Raylib.EndDrawing()
