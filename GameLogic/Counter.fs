module Counter

type State = { count: int }

let init () = { count = 0 }

let update state = { state with count = state.count + 1 }

// let draw state =
//     Raylib.DrawCircle((int state.X), (int state.Y), 20f, Color.Red)
