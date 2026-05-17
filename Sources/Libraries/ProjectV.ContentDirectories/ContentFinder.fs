module ProjectV.ContentDirectories.ContentFinder

open System.IO
open Acolyte.Assertions
open Acolyte.Linq
open ProjectV.ContentDirectories


let private convertContentType contentType =
    match contentType with
        | ContentModels.ContentType.Movie -> ContentFinderInternal.ContentTypeInternal.Movie
        | ContentModels.ContentType.Image -> ContentFinderInternal.ContentTypeInternal.Image
        | ContentModels.ContentType.Text  -> ContentFinderInternal.ContentTypeInternal.Text
        | _                               -> invalidArg (nameof contentType)
                                                        $"Content type is out of range: \"{contentType.ToString()}\"."

let private defaultDirectoryExceptionHandler (_: exn) (_: string) =
    ()

let private getFolderSeq directoryName =
    Directory.EnumerateDirectories directoryName

let private getFileSeqAsync directoryName (args: ContentModels.ScannerArguments) =
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
        } |> EnumerableExtensions.ToReadOnlyList :> seq<string> // Execute enumeration of files.
    } |> Async.StartAsTask

let FindContentAsync (args: ContentModels.ContentFinderArguments) =
    args.ThrowIfNullValue((nameof args), false) |> ignore
    args.DirectorySeq.ThrowIfNull((nameof args.DirectorySeq)) |> ignore
    
    let exceptionHandler = match args.DirectoryExceptionHandler with
                               | Some handler -> handler
                               | None         -> defaultDirectoryExceptionHandler

    let contentType = convertContentType args.ContentType

    let paging = match args.Paging with
                     | Some pagingInfo -> pagingInfo
                     | None            -> ContentModels.defaultPagingInfo

    let (internalArgs: ContentFinderInternal.ContentFinderArgumentsInternal) = {
        DirectorySeq = args.DirectorySeq
        FileSeqGen = args.FileSeqGen
        ContentType = contentType
        DirectoryExceptionHandler = exceptionHandler
        Paging = paging
    }

    ContentFinderInternal.findContentAsync internalArgs

let FindContentForDirWithAsync directoryName fileSeqGen contentType pagingInfo =
    directoryName.ThrowIfNull((nameof directoryName)) |> ignore
    fileSeqGen.ThrowIfNullValue((nameof fileSeqGen), false) |> ignore

    let (args: ContentModels.ContentFinderArguments) = {
        DirectorySeq = (getFolderSeq directoryName)
        FileSeqGen = fileSeqGen
        ContentType = contentType
        DirectoryExceptionHandler = None
        Paging = pagingInfo
    }

    FindContentAsync args

let FindContentForDirAsync directoryName contentType =
    let fileSeqGen = ContentModels.FileSeqGenerator.Async(getFileSeqAsync)
    FindContentForDirWithAsync directoryName fileSeqGen contentType None

let FindContentForDirWithPagingAsync directoryName contentType pagingInfo =
    let fileSeqGen = ContentModels.FileSeqGenerator.Async(getFileSeqAsync)
    FindContentForDirWithAsync directoryName fileSeqGen contentType pagingInfo
