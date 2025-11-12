namespace GameLogic

open Raylib_cs
open GameAbstractions
open Model
open MessagePack
open MessagePack.Resolvers
open FSharp.Core

module MsgPack =
    open System.Reflection

    let options =
        MessagePackSerializerOptions.Standard
            .WithResolver(
                CompositeResolver.Create(
                    FSharp.FSharpResolver.Instance,
                    StandardResolverAllowPrivate.Instance,
                    ContractlessStandardResolver.Instance
                )
            )
            .WithCompression
            MessagePackCompression.Lz4Block // smaller snapshots

    let rec mergeObjects (intoObj: obj) (fromObj: obj) =
        if isNull fromObj then
            ()
        else

            let t = intoObj.GetType()

            for f in t.GetFields(BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance) do
                let vSaved = f.GetValue fromObj
                let vInto = f.GetValue intoObj

                match vSaved, vInto with
                | null, _ -> () // do nothing
                | :? System.ValueType, _ -> f.SetValue(intoObj, vSaved) // overwrite scalars
                | :? string, _ -> f.SetValue(intoObj, vSaved) // overwrite strings
                | _, null -> f.SetValue(intoObj, vSaved) // missing structure: copy whole object
                | _ -> mergeObjects vInto vSaved // recursive merge

    let merge fresh saved =
        mergeObjects fresh saved
        fresh

    let serialize (model: Model) : byte[] =
        MessagePackSerializer.Serialize(model, options)

    let deserialize (bytes: byte[]) : Model =
        MessagePackSerializer.Deserialize(bytes, options)

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
                | (:? JsonObject as savedChild), (:? JsonObject as intoChild) -> merge intoChild savedChild

                // Otherwise → overwrite intoObj with a CLONE of saved value
                | _ -> intoObj[k] <- vSaved.DeepClone()

        merge freshNode savedNode
        freshNode.ToJsonString()

    let serialize (model: 'Model) =
        JsonSerializer.Serialize(model, options)

    let deserialize<'Model> (json: string) =
        JsonSerializer.Deserialize<'Model>(json, options)

type Game() =
    interface IGame with
        member _.Init maybeStateBytes =
            let fresh = Model.init ()

            let restored =
                match maybeStateBytes with
                | Some s ->
                    let saved = MsgPack.deserialize s
                    MsgPack.merge fresh saved
                | None -> fresh

            restored |> modelOverride |> MsgPack.serialize

        member _.Update stateBytes =
            let m = MsgPack.deserialize stateBytes

            Player.update m.Player
            Counter.update m.Counter

            modelOverride m |> MsgPack.serialize

        member _.Draw stateBytes =
            let m = MsgPack.deserialize stateBytes
            Raylib.BeginDrawing()
            Raylib.ClearBackground Color.RayWhite

            Player.draw m.Player
            Counter.draw m.Counter

            Raylib.EndDrawing()
