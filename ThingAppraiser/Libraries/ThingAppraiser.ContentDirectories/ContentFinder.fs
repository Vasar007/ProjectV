module ThingAppraiser.ContentDirectories.ContentFinder

open System.IO
open ThingAppraiser


type ContentType =
    | Movie = 1
    | Image = 2
    | Text = 3

type ScannerArguments = {
    FileNamePatterns: list<string>
    DirectoryExceptionHandler: exn -> string -> unit
}

type FileSeqGenerator = string -> ScannerArguments -> seq<string>

type ContentFinderArguments = {
    DirectorySeq: seq<string>
    FileSeqGen: FileSeqGenerator
    ContentType: ContentType
    DirectoryExceptionHandler: (exn -> string -> unit) option
}

let private getFolderSeq (directoryName: string) =
    Directory.EnumerateDirectories(directoryName)

let private getPatterns (contentType: ContentType) =
    match contentType with
        | ContentType.Movie -> [ "*.mkv"; "*.mp4"; "*.flv"; "*.avi"; "*.mov"; "*.3gp" ]
        | ContentType.Image -> [ "*.png"; "*.jpg"; "*.jpeg"; "*.bmp"; "*.jpe"; "*.jfif" ]
        | ContentType.Text -> [ "*.txt"; "*.md" ]
        | _ -> invalidArg "contentType" ("Content type is out of range: " +
                                            contentType.ToString())

let private getFileSeq (directoryName: string) (args: ScannerArguments) =
    seq {
        for fileNamePattern in args.FileNamePatterns do
            let fileNames =
                try
                    Directory.EnumerateFiles(directoryName, fileNamePattern,
                                                SearchOption.AllDirectories)
                with ex ->
                    args.DirectoryExceptionHandler ex directoryName
                    Seq.empty

            yield! fileNames
    }

let private defaultDirectoryExceptionHandler (_: exn) (_: string) =
    ()

let convertToReadOnly (collection: seq<string * seq<string>>) =
    collection
    |> Seq.map (fun (directoryName, files) -> (directoryName, Seq.toList files))
    |> readOnlyDict

let findContent (args: ContentFinderArguments) =
    Throw.ifNull args.DirectorySeq "args.DirectorySeq"

    let patterns = getPatterns args.ContentType

    let exceptionHandler = match args.DirectoryExceptionHandler with
                                | Some handler -> handler
                                | None -> defaultDirectoryExceptionHandler

    let innerArgs = {
        FileNamePatterns = patterns
        DirectoryExceptionHandler = exceptionHandler
    }

    args.DirectorySeq
    |> Seq.filter (isNull >> not)
    |> Seq.collect (fun directoryName -> args.FileSeqGen directoryName innerArgs)
    |> Seq.groupBy (Path.GetDirectoryName >> Path.GetFileName)

let findContentForDirectoryWith (directoryName: string) (fileSeqGen: FileSeqGenerator)
    (contentType: ContentType) =
    
    Throw.ifNull directoryName "directoryName"

    let args = {
        DirectorySeq = (getFolderSeq directoryName)
        FileSeqGen = fileSeqGen
        ContentType = contentType
        DirectoryExceptionHandler = None
    }

    findContent args

let findContentForDir (directoryName: string) (contentType: ContentType) =
    findContentForDirectoryWith directoryName (getFileSeq) contentType
