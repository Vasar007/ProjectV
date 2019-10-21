module ThingAppraiser.ContentDirectories.ContentModels

open System.Threading.Tasks


type ContentType =
    | Movie = 1
    | Image = 2
    | Text  = 3

type ScannerArguments = {
    FileNamePatterns: list<string>
    DirectoryExceptionHandler: exn -> string -> unit
}

type FileSeqGenerator =
    | Sync of generatorSync: (string -> ScannerArguments -> seq<string>)
    | Async of generatorAsync: (string -> ScannerArguments -> Task<seq<string>>)

// TODO: add option to specify paging (offset + count) for results of content finder.
type ContentFinderArguments = {
    DirectorySeq: seq<string>
    FileSeqGen: FileSeqGenerator
    ContentType: ContentType
    DirectoryExceptionHandler: (exn -> string -> unit) option
}
