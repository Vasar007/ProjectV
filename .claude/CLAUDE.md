# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**ProjectV** — auto-evaluates "things" (movies, games, books) by aggregating ratings from popular databases (OMDb, TMDb, Steam). Surfaces include a WPF desktop app, console app, ASP.NET Core web services, and a Telegram bot.

## Repo Layout

The solution lives under `Sources/` at the repo root: `Sources/ProjectV.sln`. All build/test/`dotnet` commands reference this path. Everything in the table below assumes you are at the repo root.

| Path                       | What lives there                                                                            |
|----------------------------|---------------------------------------------------------------------------------------------|
| `Sources/Applications/`    | `ProjectV.ConsoleApp`, `ProjectV.DesktopApp` (WPF)                                          |
| `Sources/Libraries/`       | Core C# libs + F# libs (`*.fsproj`) + `ExternalServices/`                                   |
| `Sources/WebServices/`     | ASP.NET Core services (Communication, Configuration, Processing, TelegramBot, CommonWebApi) |
| `Sources/Tests/`           | C# tests (xUnit) + F# tests (`ProjectV.ContentDirectories.Tests`)                           |
| `Sources/Resources/`       | Static CSV/icon assets used by the apps                                                     |

## Build & Test

```shell
dotnet restore Sources
dotnet build   Sources/ProjectV.sln                                    # full solution
dotnet build   Sources/Libraries/ProjectV.Core/ProjectV.Core.csproj    # single project
dotnet test    Sources/ProjectV.sln                                    # all .NET tests
dotnet test    Sources/ProjectV.sln --filter "FullyQualifiedName~MyTestMethod"
# F# tests run separately (the .fsproj test discovery does not pick them up from the .sln):
dotnet test    Sources/Tests/ProjectV.ContentDirectories.Tests/ProjectV.ContentDirectories.Tests.fsproj
```

- **Solution platforms**: `x64` and `Linux x64`. There is **no** `Any CPU` / `AnyCPU` configuration.
- **Linux builds skip `ProjectV.DesktopApp`** — it targets `net10.0-windows` and uses WPF. The `Linux x64` solution configuration excludes it; do not "fix" missing project references on Linux by adding it back.
- **Windows-only PR #283 / commit `ce4690d` background**: marking a project `Skip` on Linux is not enough because the SDK still imports the project; that's why DesktopApp is excluded at the solution-configuration level on `Linux x64`.

## Stack & Versions

- **TFMs**: `net10.0` (libs/web/tests/console), `net10.0-windows` (desktop only). Defined in `Sources/Directory.Build.props`.
- **Languages**: C# `12.0`, F# `8.0`.
- `Nullable` enabled, `TreatWarningsAsErrors=true`, `Deterministic=true`. Do **not** suppress warnings to make a build pass — fix the root cause.
- **Central Package Management** (`Microsoft.Build.CentralPackageVersions`): add/bump NuGet versions in `Sources/Directory.Packages.props`. Individual `.csproj` files declare `<PackageReference Include="..." />` **without** a `Version` attribute.
- **NuGet feed**: `nuget.org` only (`Sources/NuGet.Config`). No private feeds.
- **Code style**: `Sources/.editorconfig` is authoritative. UTF-8 with BOM, 4-space indent, system usings first.

## Test Stack

- C# tests: **xUnit** + **AwesomeAssertions** + **NSubstitute**.
- F# tests: **xUnit** + **Unquote** in `.fsproj`.
- Run F# tests separately with `--no-build` after a full solution build (the `.fsproj` test discovery does not pick them up from the `.sln`).

## Branch & PR Model

- `master` — the only long-lived branch. All PRs target `master` directly.
- `develop` was retired with Phase 1 (`v0.9.7`).
- For new work, branch from `master` and follow the PR template at `.github/pull_request_template.md` — it requires linking an issue and a test plan.

## CI

- **GitHub Actions `build.yml`** — Linux + Windows matrix (`ubuntu-latest`, `windows-latest`). Each job: restore → build `ProjectV.sln` → format check (`dotnet format Sources/ProjectV.sln --severity warn --verify-no-changes`) → C# tests (ubuntu only) → F# tests (ubuntu only, `--no-build`). Windows job additionally builds `ProjectV.Desktop.sln`.
- **GitHub CodeQL** (`.github/workflows/codeql-analysis.yml`) — C# analysis on pushes/PRs to `master` and weekly schedule. Runs on `windows-latest` because of WPF.
- AppVeyor was retired in Phase 1 (`v0.9.7`) — `appveyor.yml` has been deleted.

## Skills That Apply Here

This is a C# repo, so the personal-marketplace `dotnet-backend-dev` skills apply (XML docs, conventions hub, exception/wrapper/config/test creation). The repo does **not** use NSwag, EF Core migrations to Cosmos DB, Mapperly, or Clean-Architecture-with-DDD layering — apply only the skills that match what you actually see in the code, and don't force foreign patterns (e.g. don't introduce CQRS or domain aggregates where none exist).

## Things Not To Do

- Don't add `Version="..."` to `<PackageReference>` in `.csproj` — it breaks CPM.
- Don't add `AnyCPU` to project/solution configurations.
- Don't include `ProjectV.DesktopApp` in `Linux x64` builds.
- Don't commit anything under `.planning/`, `.claude/settings.local.json`, `CLAUDE.local.md`, `settings.local.json`, `Notes/`, `binary-tools/`, or `tasks/` — they are gitignored on purpose.

