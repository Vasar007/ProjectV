module ThingAppraiser.ContentDirectories.Models


type ScannerArguments = {
    FileNamePatterns: list<string>
    DirectoryExceptionHandler: exn -> string -> unit
}

type FileSeqGenerator = string -> ScannerArguments -> seq<string>
