module ThingAppraiser.ContentDirectories.Tests.ContentFinderTests

open System
open System.IO
open Xunit
open Swensen.Unquote
open ThingAppraiser.ContentDirectories


[<Fact>]
let ``Content directory path does not exist`` () =
    let contentType = ContentModels.ContentType.Movie

    raises<DirectoryNotFoundException>
        <@ ContentFinder.FindContentForDirAsync @"Z:\abc" contentType @>

[<Fact>]
let ``Content directory path contains invalid disk label`` () =
    let contentType = ContentModels.ContentType.Movie

    raises<IOException> <@ ContentFinder.FindContentForDirAsync @"ZaZ:\abc" contentType @>

[<Fact>]
let ``Content directory path contains invalid characters`` () =
    let contentType = ContentModels.ContentType.Movie

    raises<IOException> <@ ContentFinder.FindContentForDirAsync "a<b>c?" contentType @>

[<Fact>]
let ``Content directory path is too long and invalid`` () =
    let contentType = ContentModels.ContentType.Movie
    let path = String.replicate 10_000 "qwerty"

    raises<IOException> <@ ContentFinder.FindContentForDirAsync path contentType @>

[<Fact>]
let ``Content directory path is empty`` () =
    let contentType = ContentModels.ContentType.Movie

    raises<ArgumentException> <@ ContentFinder.FindContentForDirAsync String.Empty contentType @>

[<Fact>]
let ``Content type is out of range`` () =
    let (contentType: ContentModels.ContentType) = enum -1

    raises<ArgumentException> <@ ContentFinder.FindContentForDirAsync "." contentType @>

[<Fact>]
let ``Content directory path is null for "FindContentForDirAsync" call`` () =
    let contentType = ContentModels.ContentType.Movie

    raises<ArgumentNullException> <@ ContentFinder.FindContentForDirAsync null contentType @>

[<Fact>]
let ``Directory sequence is null for "FindContentForDirWithAsync" call`` () =
    let contentType = ContentModels.ContentType.Movie
    let f = fun (_: string) (_: ContentModels.ScannerArguments) -> Seq.empty<string>

    let seqGen = ContentModels.FileSeqGenerator.Sync(f)

    raises<ArgumentNullException>
        <@ ContentFinder.FindContentForDirWithAsync null seqGen contentType @>

[<Fact>]
let ``Directory sequence is null for "FindContentAsync" call`` () =
    let contentType = ContentModels.ContentType.Movie
    let f = fun (_: string) (_: ContentModels.ScannerArguments) -> Seq.empty<string>

    let (args: ContentModels.ContentFinderArguments) = {
        DirectorySeq = null
        FileSeqGen = ContentModels.FileSeqGenerator.Sync(f)
        ContentType = contentType
        DirectoryExceptionHandler = None
        Paging = None
    }

    raises<ArgumentNullException> <@ ContentFinder.FindContentAsync args @>
