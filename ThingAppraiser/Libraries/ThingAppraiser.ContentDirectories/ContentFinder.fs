module ThingAppraiser.ContentDirectories.ContentFinder

open System.IO
open ThingAppraiser
open ThingAppraiser.Extensions
open ThingAppraiser.ContentDirectories


let private convertContentType (contentType: ContentModels.ContentType) =
    match contentType with
        | ContentModels.ContentType.Movie -> ContentFinderInternal.ContentTypeInternal.Movie
        | ContentModels.ContentType.Image -> ContentFinderInternal.ContentTypeInternal.Image
        | ContentModels.ContentType.Text  -> ContentFinderInternal.ContentTypeInternal.Text
        | _                               -> invalidArg "contentType"
                                                        ("Content type is out of range: \"" +
                                                         contentType.ToString() + "\".")

let private defaultDirectoryExceptionHandler (_: exn) (_: string) =
    ()

let private getFolderSeq (directoryName: string) =
    Directory.EnumerateDirectories directoryName

let private getFileSeqAsync (directoryName: string) (args: ContentModels.ScannerArguments) =
    async {
        return seq {
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
    } |> Async.StartAsTask

let ConvertToReadOnly (collection: seq<string * seq<string>>) =
    Throw.ifNull collection "collection"

    collection
    |> Seq.map (fun (directoryName, files) -> (directoryName,
                                               EnumerableExtensions.ToReadOnlyList files))
    |> readOnlyDict

let FindContentAsync (args: ContentModels.ContentFinderArguments) =
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

    ContentFinderInternal.findContentAsync internalArgs

let FindContentForDirWithAsync (directoryName: string) (fileSeqGen: ContentModels.FileSeqGenerator)
    (contentType: ContentModels.ContentType) =

    Throw.ifNull directoryName "directoryName"
    Throw.ifNullValue fileSeqGen "fileSeqGen"

    let (args: ContentModels.ContentFinderArguments) = {
        DirectorySeq = (getFolderSeq directoryName)
        FileSeqGen = fileSeqGen
        ContentType = contentType
        DirectoryExceptionHandler = None
    }

    FindContentAsync args

let FindContentForDirAsync (directoryName: string) (contentType: ContentModels.ContentType) =
    let fileSeqGen = ContentModels.FileSeqGenerator.Async(getFileSeqAsync)
    FindContentForDirWithAsync directoryName fileSeqGen contentType
