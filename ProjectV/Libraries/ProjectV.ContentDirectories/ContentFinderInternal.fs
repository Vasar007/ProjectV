module internal ProjectV.ContentDirectories.ContentFinderInternal

open System
open System.IO
open System.Threading.Tasks
open Acolyte.Functional
open Acolyte.Collections
open ProjectV.ContentDirectories


[<Struct>]
type internal ContentTypeInternal =
    | Movie
    | Image
    | Text

type internal ContentFinderArgumentsInternal = {
    DirectorySeq: seq<string>
    FileSeqGen: ContentModels.FileSeqGenerator
    ContentType: ContentTypeInternal
    DirectoryExceptionHandler: exn -> string -> unit
    Paging: ContentModels.PagingInfo
}

let private getPatterns contentType =
    match contentType with
        | Movie -> [ "*.mkv"; "*.mp4"; "*.flv"; "*.avi"; "*.mov"; "*.3gp" ]
        | Image -> [ "*.png"; "*.jpg"; "*.jpeg"; "*.bmp"; "*.jpe"; "*.jfif" ]
        | Text  -> [ "*.txt"; "*.md" ]

let private convertSeqGenToAsync fileSeqGen =
    match fileSeqGen with
        | ContentModels.FileSeqGenerator.Sync(generatorSync = genSync) ->
            fun arg1 arg2 -> Task.FromResult (genSync arg1 arg2)
        | ContentModels.FileSeqGenerator.Async(generatorAsync = genAsync) ->
            genAsync

let private convertToReadOnlyList files  =
    files
    |> EnumerableExtensions.ToReadOnlyList

let private transformGrouppedPairs (directoryName, files) =
    (directoryName, convertToReadOnlyList files)

let internal findContentAsync args =
    async {
        let patterns = getPatterns args.ContentType

        let (innerArgs: ContentModels.ScannerArguments) = {
            FileNamePatterns = patterns
            DirectoryExceptionHandler = args.DirectoryExceptionHandler
        }

        let fileSeqGenAsync = convertSeqGenToAsync args.FileSeqGen

        let seqResults = args.DirectorySeq
                         |> Seq.filter (isNull >> not)
                         |> Seq.map (fun directoryName -> fileSeqGenAsync directoryName innerArgs)

        let! dirResults = Task.WhenAll(seqResults) |> Async.AwaitTask

        // If count is negative, then tries get as many items as possible.
        let pagingInfo = {
            args.Paging
            with Count = if args.Paging.Count < 0 then Int32.MaxValue else args.Paging.Count
        }

        return dirResults
               // Step 1: collecting all items.
               |> Seq.collect (fun resultForOneDir -> resultForOneDir)
               // Step 2: applying paging to items.
               |> SeqEx.skipSafe pagingInfo.Offset
               |> Seq.truncate pagingInfo.Count
               // Step 3: groupping items by directory name.
               |> Seq.groupBy (Path.GetDirectoryName >> Path.GetFileName)
               // Step 4: transforming items to result object.
               |> Seq.map transformGrouppedPairs
               |> readOnlyDict
    } |> Async.StartAsTask
