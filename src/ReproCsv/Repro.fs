module ReproCsv.Repro

// Bug repro from http://stackoverflow.com/a/42472769

open System
open System.Diagnostics
open System.IO

let clock =
  let sw = Stopwatch ()
  sw.Start ()
  fun () ->
    sw.ElapsedMilliseconds

let time a =
  let before  = clock ()
  let v       = a ()
  let after   = clock ()
  after - before, v

let generateDataSet () =
  let random            = Random 19740531

  let firstDate         = DateTime(1970, 1, 1)

  let randomInt     ()  = random.Next () |> int64 |> (+) 10000000000L |> string
  let randomDate    ()  = (firstDate + (random.Next () |> float |> TimeSpan.FromSeconds)).ToString("s")
  let randomString  ()  = 
    let inline valid ch =
      match ch with
      | '"'
      | '\\'  -> ' '
      | _     -> ch
    let c   = random.Next () % 16
    let g i =
      if i = 0 || i = c + 1 then '"'
      else 32 + random.Next() % (127 - 32) |> char |> valid
    Array.init (c + 2) g |> String

  let columns =
    [|
      "Id"          , randomInt
      "ForeignId"   , randomInt
      "BirthDate"   , randomDate
      "OtherDate"   , randomDate
      "FirstName"   , randomString
      "LastName"    , randomString
    |]

  use sw      = new StreamWriter ("perf.csv")
  let headers = columns |> Array.map fst |> String.concat ";"
  sw.WriteLine headers
  for i = 0 to 700000 do
    let values = columns |> Array.map (fun (_, f) -> f ()) |> String.concat ";"
    sw.WriteLine values

open FSharp.Data

[<Literal>]
let sample = """Id;ForeignId;BirthDate;OtherDate;FirstName;LastName
11795679844;10287417237;2028-09-14T20:33:17;1993-07-21T17:03:25;",xS@ %aY)N*})Z";"ZP~;"
11127366946;11466785219;2028-02-22T08:39:57;2026-01-24T05:07:53;"H-/QA(";"g8}J?k~"
"""

type CsvWithDefaultCulture  = CsvProvider<sample, ";">
type CsvWithExplicitCulture = CsvProvider<sample, ";", Culture="en-US">

let readDataWithDefaultCulture () =
  use streamReader  = new StreamReader ("perf.csv")
  let csvFile       = CsvWithDefaultCulture.Load streamReader
  let length        = csvFile.Rows |> Seq.length
  printfn "%A" length

let readDataWithExplicitCulture () =
  use streamReader  = new StreamReader ("perf.csv")
  let csvFile       = CsvWithExplicitCulture.Load streamReader
  let length        = csvFile.Rows |> Seq.length
  printfn "%A" length

[<EntryPoint>]
let main argv = 
  Environment.CurrentDirectory <- AppDomain.CurrentDomain.BaseDirectory

  printfn "Generating dataset..."
  let ms, _ = time generateDataSet
  printfn "  took %d ms" ms

  printfn "Reading dataset with default culture..."
  let ms, _ = time readDataWithDefaultCulture
  printfn "  took %d ms" ms

  printfn "Reading dataset with explicitly-specified culture..."
  let ms, _ = time readDataWithExplicitCulture
  printfn "  took %d ms" ms

  0
