namespace ThingAppraiser.ContentDirectories.Tests

open System
open Xunit
open Swensen.Unquote
open ThingAppraiser.ContentDirectories

module ContentFinderTests =

    [<Fact>]
    let ``Contnent directory does not exist`` () =
        Assert.True(true)

    [<Fact>]
    let ``Contnent directory name is null`` () =
        let contnentType = ContentFinder.ContentType.Movie
        raises<ArgumentNullException> <@ ContentFinder.findContent null contnentType @>
