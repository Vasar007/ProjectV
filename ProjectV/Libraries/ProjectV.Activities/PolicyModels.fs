﻿module ProjectV.Activities.PolicyModels

open System
open Acolyte.Assertions


// You should create "PolicyId" through factory functions.
[<Struct>]
type PolicyId =
    private {
        Value: Guid
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PolicyId =

    let createPolicyId =
        { PolicyId.Value = Guid.NewGuid() }

    let wrapPolicyId (idValue: Guid) =
        idValue.ThrowIfEmpty((nameof idValue)) |> ignore
        { PolicyId.Value = idValue }

    let (|PolicyId|) (valueToMatch: PolicyId) =
        valueToMatch.Value


// You should create "PolicyName" through factory functions.
[<Struct>]
type PolicyName =
    private {
        Value: string
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PolicyName =

    let createPolicyName (value: string) =
        value.ThrowIfNullOrWhiteSpace((nameof value)) |> ignore
        { PolicyName.Value = value }

    let (|PolicyName|) (valueToMatch: PolicyName) =
        valueToMatch.Value


type Policy = {
    Id: PolicyId
    Name: PolicyName
    IsEnabled: bool
}
