namespace GameLogic

open Raylib_cs
open GameAbstractions
open Model

module Json =
    open System.Text.Json
    open System.Text.Json.Serialization
    open System.Text.Json.Nodes

    let options =
        let opts =
            JsonSerializerOptions(WriteIndented = false, PropertyNamingPolicy = JsonNamingPolicy.CamelCase)

        opts.IncludeFields <- true
        opts.Converters.Add(JsonFSharpConverter())
        opts

    let mergeJson (freshJson: string) (savedJson: string) : string =
        let freshNode = JsonNode.Parse(freshJson) :?> JsonObject
        let savedNode = JsonNode.Parse(savedJson) :?> JsonObject

        let rec merge (intoObj: JsonObject) (fromObj: JsonObject) =
            for KeyValue(k, vSaved) in fromObj do
                match vSaved, intoObj[k] with
                // Both are objects → recursively merge
                | (:? JsonObject as savedChild), (:? JsonObject as intoChild) ->
                    merge intoChild savedChild

                // Otherwise → overwrite intoObj with a CLONE of saved value
                | _ ->
                    intoObj[k] <- vSaved.DeepClone()

        merge freshNode savedNode
        freshNode.ToJsonString()

    let serialize (model: 'Model) =
        JsonSerializer.Serialize(model, options)

    let deserialize<'Model> (json: string) =
        JsonSerializer.Deserialize<'Model>(json, options)

type Game() =
    interface IGame with
        member _.Init(saved) =
            let freshJson = Json.serialize (Model.init())

            let mergedJson =
                match saved with
                | Some s -> Json.mergeJson freshJson s
                | None -> freshJson

            mergedJson
            |> Json.deserialize<Model>
            |> modelOverride
            |> Json.serialize

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
