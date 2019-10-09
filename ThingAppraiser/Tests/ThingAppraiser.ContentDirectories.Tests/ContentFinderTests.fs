module ThingAppraiser.ContentDirectories.Tests.ContentFinderTests

open System
open Xunit
open Swensen.Unquote
open ThingAppraiser.ContentDirectories


[<Fact>]
let ``Contnent directory does not exist`` () =
    Assert.True(true)

[<Fact>]
let ``Content directory name is null for "findContentForDir" call`` () =
    let contentType = ContentFinder.ContentType.Movie

    raises<ArgumentNullException> <@ ContentFinder.findContentForDir null contentType @>

[<Fact>]
let ``Directory sequence is null for "findContentForDirectoryWith" call`` () =
    let contentType = ContentFinder.ContentType.Movie
    let f = fun (_: string) (_: ContentFinder.ScannerArguments) -> Seq.empty<string>
    
    raises<ArgumentNullException>
        <@ ContentFinder.findContentForDirectoryWith null f contentType @>

[<Fact>]
let ``Directory sequence is null for "findContent" call`` () =
    let contentType = ContentFinder.ContentType.Movie
    let f = fun (_: string) (_: ContentFinder.ScannerArguments) -> Seq.empty<string>

    let (args: ContentFinder.ContentFinderArguments) = {
        DirectorySeq = null
        FileSeqGen = f
        ContentType = contentType
        DirectoryExceptionHandler = None
    }

    raises<ArgumentNullException> <@ ContentFinder.findContent args @>
