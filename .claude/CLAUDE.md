# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**ProjectV** — auto-evaluates "things" (movies, games, books) by aggregating ratings from popular databases (OMDb, TMDb, Steam). Surfaces include a WPF desktop app, console app, ASP.NET Core web services, and a Telegram bot.

## Repo Layout Quirk

The solution lives **one level down** from the repo root: `ProjectV/ProjectV.sln`. All build/test/`dotnet` commands must be run from `ProjectV/` (or with explicit paths). Everything in the table below assumes you are at the repo root.

| Path                       | What lives there                                                                            |
|----------------------------|---------------------------------------------------------------------------------------------|
| `ProjectV/Applications/`   | `ProjectV.ConsoleApp`, `ProjectV.DesktopApp` (WPF)                                          |
| `ProjectV/Libraries/`      | Core C# libs + F# libs (`*.fsproj`) + `ExternalServices/`                                   |
| `ProjectV/WebServices/`    | ASP.NET Core services (Communication, Configuration, Processing, TelegramBot, CommonWebApi) |
| `ProjectV/Tests/`          | C# tests (xUnit) + F# tests (`ProjectV.ContentDirectories.Tests`)                           |
| `ProjectV/Resources/`      | Static CSV/icon assets used by the apps                                                     |

## Build & Test

```shell
dotnet restore ProjectV
dotnet build   ProjectV/ProjectV.sln                                    # full solution
dotnet build   ProjectV/Libraries/ProjectV.Core/ProjectV.Core.csproj    # single project
dotnet test    ProjectV/ProjectV.sln                                    # all .NET tests
dotnet test    ProjectV/ProjectV.sln --filter "FullyQualifiedName~MyTestMethod"
# F# tests run separately (the .fsproj test discovery does not pick them up from the .sln):
dotnet test    ProjectV/Tests/ProjectV.ContentDirectories.Tests/ProjectV.ContentDirectories.Tests.fsproj
```

- **Solution platforms**: `x64` and `Linux x64`. There is **no** `Any CPU` / `AnyCPU` configuration.
- **Linux builds skip `ProjectV.DesktopApp`** — it targets `net8.0-windows` and uses WPF. The `Linux x64` solution configuration excludes it; do not "fix" missing project references on Linux by adding it back.
- **Windows-only PR #283 / commit `ce4690d` background**: marking a project `Skip` on Linux is not enough because the SDK still imports the project; that's why DesktopApp is excluded at the solution-configuration level on `Linux x64`.

## Stack & Versions

- **TFMs**: `net8.0` (libs/web/tests/console), `net8.0-windows` (desktop only). Defined in `ProjectV/Directory.Build.props`.
- **Languages**: C# `12.0`, F# `8.0`.
- `Nullable` enabled, `TreatWarningsAsErrors=true`, `Deterministic=true`. Do **not** suppress warnings to make a build pass — fix the root cause.
- **Central Package Management** (`Microsoft.Build.CentralPackageVersions`): add/bump NuGet versions in `ProjectV/Directory.Packages.props`. Individual `.csproj` files declare `<PackageReference Include="..." />` **without** a `Version` attribute.
- **NuGet feed**: `nuget.org` only (`ProjectV/NuGet.Config`). No private feeds.
- **Code style**: `ProjectV/.editorconfig` is authoritative. UTF-8 with BOM, 4-space indent, system usings first.

## Test Stack

- C# tests: **xUnit** + **FluentAssertions** + **Moq**.
- F# tests: **xUnit** + **Unquote** in `.fsproj`.
- The AppVeyor pipeline runs F# tests in a separate `dotnet test` step (`after_test:`); mirror that locally when validating F# code.

## Branch & PR Model

- `master` — release branch. PRs from this repo's contributors target it via `develop`.
- `develop` — integration branch. Dependabot opens PRs against `develop` (see `.github/dependabot.yml`).
- For new work, branch from `master` (or `develop` if continuing in-flight integration work) and follow the PR template at `.github/pull_request_template.md` — it requires linking an issue and a test plan.

## CI

- **AppVeyor** (`appveyor.yml`) — Visual Studio 2022 image, `x64` only, builds `Debug` and `Release`, runs xUnit `**/*.Tests.dll` plus the F# tests in `after_test`. Treats build warnings as errors via the project settings.
- **GitHub CodeQL** (`.github/workflows/codeql-analysis.yml`) — C# analysis on pushes/PRs to `master` and weekly schedule. Runs on `windows-latest` because of WPF.
- There is no Linux CI yet; verify Linux builds manually if you touch cross-platform code paths.

## Skills That Apply Here

This is a C# repo, so the personal-marketplace `dotnet-backend-dev` skills apply (XML docs, conventions hub, exception/wrapper/config/test creation). The repo does **not** use NSwag, EF Core migrations to Cosmos DB, Mapperly, or Clean-Architecture-with-DDD layering — apply only the skills that match what you actually see in the code, and don't force foreign patterns (e.g. don't introduce CQRS or domain aggregates where none exist).

## Things Not To Do

- Don't add `Version="..."` to `<PackageReference>` in `.csproj` — it breaks CPM.
- Don't add `AnyCPU` to project/solution configurations.
- Don't include `ProjectV.DesktopApp` in `Linux x64` builds.
- Don't commit anything under `.planning/`, `.claude/settings.local.json`, `CLAUDE.local.md`, `settings.local.json`, `Notes/`, `binary-tools/`, or `tasks/` — they are gitignored on purpose.

