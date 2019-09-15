namespace ThingAppraiser.ContentDirectories

open System
open System.IO
open ThingAppraiser

module ContentFinder =

    [<Struct>]
    type ContentType =
        | Movie
        | Image
        | Text

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
            | Movie -> [ "*.mkv"; "*.mp4"; "*.flv"; "*.avi"; "*.mov"; "*.3gp" ]
            | Image -> [ "*.png"; "*.jpg"; "*.jpeg"; "*.bmp"; "*.jpe"; "*.jfif" ]
            | Text -> [ "*.txt"; "*.md" ]

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
        |> Seq.map Path.GetFileName
        |> Seq.iter (printfn "%s")

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
