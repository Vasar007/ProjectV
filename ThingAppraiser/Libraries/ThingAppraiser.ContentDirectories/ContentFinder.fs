namespace ThingAppraiser.ContentDirectories

open System

module ContentFinder =
    type ContentType =
        | Movie
        | Image
        | Text

    let findContent (directoryName: string) (contentType: ContentType) =
        if isNull directoryName then raise (ArgumentNullException("Directory name should not be null."))

        raise (NotImplementedException("You haven't written a test yet!"))
