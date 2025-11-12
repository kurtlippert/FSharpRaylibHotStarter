namespace GameAbstractions

type IGame =
    abstract member Init: byte[] option -> byte[]
    abstract member Update: byte[] -> byte[]
    abstract member Draw: byte[] -> unit
