namespace GameAbstractions

type IGame =
    abstract member Init: string option -> string
    abstract member Update: string -> string
    abstract member Draw: string -> unit
