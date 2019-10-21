module internal ThingAppraiser.ContentDirectories.ContentFinderInternal

open System.IO
open System.Threading.Tasks
open ThingAppraiser.ContentDirectories
open ThingAppraiser.Extensions


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

let private convertToReadOnlyList files =
    EnumerableExtensions.ToReadOnlyList files

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
        return dirResults
               |> Seq.collect (fun resultForOneDir -> resultForOneDir)
               |> Seq.groupBy (Path.GetDirectoryName >> Path.GetFileName)
               |> Seq.map (fun (directoryName, files) -> (directoryName, convertToReadOnlyList files))
               |> readOnlyDict
    } |> Async.StartAsTask
