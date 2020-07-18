module ProjectV.ContentDirectories.ContentModels

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

type PagingInfo = {
    Offset: int32
    Count: int32
}

let internal defaultPagingInfo = {
    Offset = 0
    Count = -1
}

// TODO: add filter spec to order and filtering items.
type ContentFinderArguments = {
    DirectorySeq: seq<string>
    FileSeqGen: FileSeqGenerator
    ContentType: ContentType
    DirectoryExceptionHandler: option<(exn -> string -> unit)>
    Paging: option<PagingInfo>
}

let CreateOption<'T when 'T : null> (value: 'T) =
    if obj.ReferenceEquals(value, null) then None
    else Some(value)
