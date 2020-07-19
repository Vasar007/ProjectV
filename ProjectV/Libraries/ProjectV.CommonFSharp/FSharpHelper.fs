module ProjectV.CommonFSharp.FSharpHelper


let CreateOption<'T when 'T : null> (value: 'T) =
    if obj.ReferenceEquals(value, null) then None
    else Some(value)
