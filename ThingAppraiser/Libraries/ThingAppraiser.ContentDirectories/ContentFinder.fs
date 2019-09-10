namespace ThingAppraiser.ContentDirectories

open System
open ThingAppraiser

module ContentFinder =

    type ContentType =
        | Movie
        | Image
        | Text

    let findContent (directoryName: string) (contentType: ContentType) =
        Throw.ifNull directoryName "directoryName"

        raise (NotImplementedException("You haven't written a test yet!"))
