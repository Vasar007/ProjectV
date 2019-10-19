module ThingAppraiser.Throw

let private (|NotNull|_|) value = 
    if obj.ReferenceEquals(value, null) then None 
    else Some()

let ifNullValue value (paramName: string) =
    if isNull paramName then
        nullArg "paramName" // Replace with nameof operator which still does not compile now.

    match value with
        | NotNull -> ()
        | _ -> nullArg paramName

let ifNull obj (paramName: string) =
    if isNull paramName then
        nullArg "paramName" // Replace with nameof operator which still does not compile now.

    if isNull obj then
        nullArg paramName
