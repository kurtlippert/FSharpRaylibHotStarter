namespace GameAbstractions

type IGame =
    abstract member Update: float32 -> unit
    abstract member Draw: unit -> unit
    abstract member SaveState: unit -> obj
    abstract member LoadState: obj -> unit
