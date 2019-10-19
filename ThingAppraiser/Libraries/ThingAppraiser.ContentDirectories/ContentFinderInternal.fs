module internal ThingAppraiser.ContentDirectories.ContentFinderInternal

open System.IO
open ThingAppraiser.ContentDirectories


[<Struct>]
type internal ContentTypeInternal =
    | Movie
    | Image
    | Text

type internal ContentFinderArgumentsInternal = {
    DirectorySeq: seq<string>
    FileSeqGen: Models.FileSeqGenerator
    ContentType: ContentTypeInternal
    DirectoryExceptionHandler: exn -> string -> unit
}

let private getPatterns (contentType: ContentTypeInternal) =
    match contentType with
        | Movie -> [ "*.mkv"; "*.mp4"; "*.flv"; "*.avi"; "*.mov"; "*.3gp" ]
        | Image -> [ "*.png"; "*.jpg"; "*.jpeg"; "*.bmp"; "*.jpe"; "*.jfif" ]
        | Text  -> [ "*.txt"; "*.md" ]

let internal findContent (args: ContentFinderArgumentsInternal) =
    let patterns = getPatterns args.ContentType

    let (innerArgs: Models.ScannerArguments) = {
        FileNamePatterns = patterns
        DirectoryExceptionHandler = args.DirectoryExceptionHandler
    }

    args.DirectorySeq
    |> Seq.filter (isNull >> not)
    |> Seq.collect (fun directoryName -> args.FileSeqGen directoryName innerArgs)
    |> Seq.groupBy (Path.GetDirectoryName >> Path.GetFileName)
