namespace GameLogic

open Raylib_cs
open GameAbstractions
open Model
open MessagePack
open MessagePack.Resolvers
open FSharp.Core
open Main

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

    let serialize<'a> model =
        MessagePackSerializer.Serialize<'a>(model, options)

    let deserialize<'a> (bytes: byte[]) =
        MessagePackSerializer.Deserialize<'a>(bytes, options)

type Game() =
    interface IGame with
        member _.Init maybeState =
            let freshModel, _ = Main.init()

            let restored =
                match maybeState with
                | Some bytes -> MsgPack.deserialize bytes
                | None -> freshModel

            MsgPack.serialize restored

        member _.Update stateBytes =
            let model = MsgPack.deserialize<Model>(stateBytes)

            // produce a Tick msg for both children
            let dt = Raylib.GetFrameTime()
            let msgBatch = [
                PlayerMsg(Player.Msg.Travel dt)
                CounterMsg(Counter.Msg.Tick dt)
            ]

            let final =
                msgBatch
                |> List.fold (fun m msg -> fst (Main.update msg m)) model

            MsgPack.serialize final

        member _.Draw stateBytes =
            let m = MsgPack.deserialize stateBytes
            Raylib.BeginDrawing()
            Raylib.ClearBackground Color.RayWhite

            Player.draw m.Player
            Counter.draw m.Counter

            Raylib.EndDrawing()
