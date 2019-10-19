module ThingAppraiser.ContentDirectories.Tests.ContentFinderTests

open System
open System.IO
open Xunit
open Swensen.Unquote
open ThingAppraiser.ContentDirectories


[<Fact>]
let ``Content directory path does not exist`` () =
    let contentType = ContentFinder.ContentType.Movie

    raises<DirectoryNotFoundException> <@ ContentFinder.FindContentForDir @"Z:\abc" contentType @>

[<Fact>]
let ``Content directory path contains invalid disk label`` () =
    let contentType = ContentFinder.ContentType.Movie

    raises<IOException> <@ ContentFinder.FindContentForDir @"ZaZ:\abc" contentType @>

[<Fact>]
let ``Content directory path contains invalid characters`` () =
    let contentType = ContentFinder.ContentType.Movie

    raises<IOException> <@ ContentFinder.FindContentForDir "a<b>c?" contentType @>

[<Fact>]
let ``Content directory path is too long`` () =
    let contentType = ContentFinder.ContentType.Movie
    let path = String.replicate 10_000 "qwerty"

    raises<IOException> <@ ContentFinder.FindContentForDir path contentType @>

[<Fact>]
let ``Content directory path is empty`` () =
    let contentType = ContentFinder.ContentType.Movie

    raises<ArgumentException> <@ ContentFinder.FindContentForDir String.Empty contentType @>

[<Fact>]
let ``Content type is out of range`` () =
    let (contentType: ContentFinder.ContentType) = enum -1

    raises<ArgumentException> <@ ContentFinder.FindContentForDir "." contentType @>

[<Fact>]
let ``Content directory path is null for "FindContentForDir" call`` () =
    let contentType = ContentFinder.ContentType.Movie

    raises<ArgumentNullException> <@ ContentFinder.FindContentForDir null contentType @>

[<Fact>]
let ``Directory sequence is null for "FindContentForDirWith" call`` () =
    let contentType = ContentFinder.ContentType.Movie
    let f = fun (_: string) (_: ContentFinder.ScannerArguments) -> Seq.empty<string>

    raises<ArgumentNullException>
        <@ ContentFinder.FindContentForDirWith null f contentType @>

[<Fact>]
let ``Directory sequence is null for "FindContent" call`` () =
    let contentType = ContentFinder.ContentType.Movie
    let f = fun (_: string) (_: ContentFinder.ScannerArguments) -> Seq.empty<string>

    let (args: ContentFinder.ContentFinderArguments) = {
        DirectorySeq = null
        FileSeqGen = f
        ContentType = contentType
        DirectoryExceptionHandler = None
    }

    raises<ArgumentNullException> <@ ContentFinder.FindContent args @>
