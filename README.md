# Raylib & FSharp Hot-Reload Starter

## Glossary
| Term | Meaning | Example
|-------|---------|---
| **Model** | Whole game state | `Model.fs`
| **Slice** | Piece of game state | `Player.fs`

## Build
`dotnet build`

## Run
`dotnet run --project GameHost`

## Watch
`cd GameLogic && dotnet watch build`  

## Tips
- Watch in one terminal window, run in another


## Improvements
Are there any? Currently wondering about:
- [x] Avoiding re-inits on model slice changes (mainly additions)
- [ ] Deprecating `modelOverride` (why is it necessary?)
- [ ] Avoiding `|> CBool.op_Implicit` (maybe not worth a util...)
