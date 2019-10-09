module ThingAppraiser.Throw

let ifNull obj (paramName: string) =
    if isNull paramName then
        nullArg "paramName" // Replace with nameof operator which still does not compile now.

    if isNull obj then
        nullArg paramName
