namespace GameAbstractions

type IGame =
    abstract member Init : unit -> unit
    abstract member Update : unit -> unit
    abstract member Draw : unit -> unit
