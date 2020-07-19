module ProjectV.Policies.PolicyModels

open System
open Acolyte.Assertions


// You should create "PolicyId" through factory functions.
type PolicyId =
    private {
        Value: Guid
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PolicyId =

    let createPolicyId =
        { PolicyId.Value = Guid.NewGuid() }

    let wrapPolicyId (idValue: Guid) =
        idValue.ThrowIfEmpty("idValue") |> ignore
        { PolicyId.Value = idValue }

    let (|PolicyId|) (valueToMatch: PolicyId) =
        valueToMatch.Value


type PolicyName =
    private {
        Value: string
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PolicyName =

    let createPolicyName (value: string) =
        value.ThrowIfNullOrWhiteSpace("value") |> ignore
        { PolicyName.Value = value }

    let (|PolicyName|) (valueToMatch: PolicyName) =
        valueToMatch.Value


type Policy = {
    Id: PolicyId
    Name: PolicyName
    IsEnabled: bool
}
