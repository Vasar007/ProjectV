namespace ThingAppraiser

open System

module Throw =
    let ifNull obj (paramName: string) =
        if isNull paramName then
            nullArg "paramName"

        if isNull obj then
            nullArg paramName
