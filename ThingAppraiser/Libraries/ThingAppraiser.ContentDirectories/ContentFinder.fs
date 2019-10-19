module ThingAppraiser.ContentDirectories.ContentFinder

open System.IO
open ThingAppraiser
open ThingAppraiser.Extensions
open ThingAppraiser.ContentDirectories


type ContentType =
    | Movie = 1
    | Image = 2
    | Text  = 3

type ScannerArguments = Models.ScannerArguments

type FileSeqGenerator = Models.FileSeqGenerator

type ContentFinderArguments = {
    DirectorySeq: seq<string>
    FileSeqGen: FileSeqGenerator
    ContentType: ContentType
    DirectoryExceptionHandler: (exn -> string -> unit) option
}

let private convertContentType (contentType: ContentType) =
    match contentType with
        | ContentType.Movie -> ContentFinderInternal.ContentTypeInternal.Movie
        | ContentType.Image -> ContentFinderInternal.ContentTypeInternal.Image
        | ContentType.Text  -> ContentFinderInternal.ContentTypeInternal.Text
        | _                 -> invalidArg "contentType" ("Content type is out of range: \"" +
                                                         contentType.ToString() + "\"")

let private getFolderSeq (directoryName: string) =
    Directory.EnumerateDirectories directoryName

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

let ConvertToReadOnly (collection: seq<string * seq<string>>) =
    Throw.ifNull collection "collection"

    collection
    |> Seq.map (fun (directoryName, files) -> (directoryName,
                                               EnumerableExtensions.ToReadOnlyList files))
    |> readOnlyDict

let FindContent (args: ContentFinderArguments) =
    Throw.ifNullValue args "args"
    Throw.ifNull args.DirectorySeq "args.DirectorySeq"
    
    let exceptionHandler = match args.DirectoryExceptionHandler with
                               | Some handler -> handler
                               | None -> defaultDirectoryExceptionHandler
    
    let contentType = convertContentType args.ContentType

    let (internalArgs: ContentFinderInternal.ContentFinderArgumentsInternal) = {
        DirectorySeq = args.DirectorySeq
        FileSeqGen = args.FileSeqGen
        ContentType = contentType
        DirectoryExceptionHandler = exceptionHandler
    }
    
    ContentFinderInternal.findContent internalArgs

let FindContentForDirWith (directoryName: string) (fileSeqGen: FileSeqGenerator)
    (contentType: ContentType) =

    Throw.ifNull directoryName "directoryName"
    Throw.ifNullValue fileSeqGen "fileSeqGen"

    let args = {
        DirectorySeq = (getFolderSeq directoryName)
        FileSeqGen = fileSeqGen
        ContentType = contentType
        DirectoryExceptionHandler = None
    }

    FindContent args

let FindContentForDir (directoryName: string) (contentType: ContentType) =
    FindContentForDirWith directoryName getFileSeq contentType
