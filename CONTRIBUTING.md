# Contributing to ProjectV

Thank you for your interest in contributing to ProjectV!

## Workflow

ProjectV follows a **GitHub-Milestone-driven** development process:

- **Phases and milestones:** Each development phase corresponds to one GitHub Milestone (e.g., `v0.9.7`). Milestones have no fixed deadlines — they ship when ready.
- **Task tracking:** Each task is one GitHub Issue linked to the active milestone. The implementing developer/agent posts notes on the Issue summarising decisions, nuances, and progress as work proceeds.
- **Pull requests:** Every PR must link at least one Issue using `closes #XXXX` (or `Part of #XXXX` for partial work) in the PR body. Opening a PR without a corresponding Issue is not allowed — create or find the Issue first.
- **Target branch:** All PRs target `master` directly. There are no long-lived feature or integration branches.
- **Branch naming:** Use descriptive short-lived branches (e.g., `phase-1/13-docs-and-cherry-pick`). Delete the branch after the PR merges.
- **Note:** The `develop` branch was retired with Phase 1 (`v0.9.7`). `master` is the only long-lived branch on the remote.

## Branch Protection

The following rules are enforced on `master`:

- **No direct pushes.** All changes must arrive via a Pull Request.
- **Required status checks before merge:**
  - Build / ubuntu-latest (GitHub Actions `build.yml`)
  - Build / windows-latest (GitHub Actions `build.yml`)
  - CodeQL
- **Up-to-date requirement:** Branches must be up-to-date with `master` before merge (strict mode).
- **Linear history:** Squash merge or rebase merge only. Merge commits are not permitted.
- **Force-push blocked.**
- **Self-approval allowed:** As a solo maintainer, the author may merge their own PR after all required status checks pass. A required reviewer count is not enforced.

## Build and Test Commands

All commands are run from the **repo root** (the directory containing this file):

```shell
# Restore NuGet packages
dotnet restore Sources

# Build the cross-platform solution (Linux + Windows)
dotnet build Sources/ProjectV.sln

# Build the Windows-only solution (includes WPF DesktopApp; Windows only)
dotnet build Sources/ProjectV.Desktop.sln

# Run all C# tests
dotnet test Sources/ProjectV.sln

# Run F# tests (separate step — not picked up by solution-level test discovery)
dotnet test Sources/Tests/ProjectV.ContentDirectories.Tests/ProjectV.ContentDirectories.Tests.fsproj

# Check code style (must produce no changes; mirrors the CI gate)
dotnet format Sources/ProjectV.sln --severity warn --verify-no-changes
```

**SDK requirement:** .NET 10 SDK is required locally. The WPF DesktopApp (`ProjectV.Desktop.sln`) requires Windows.

## Code Style

- **EditorConfig:** `.editorconfig` at `Sources/.editorconfig` is authoritative. Key rules: UTF-8 with BOM, 4-space indent, `System.*` usings first, block-scoped namespaces.
- **Warnings as errors:** `TreatWarningsAsErrors=true` is global. Do not suppress warnings to make a build pass — fix the root cause.
- **Central Package Management (CPM):** New NuGet packages add a `<PackageVersion>` entry in `Sources/Directory.Packages.props`. Individual `.csproj` files declare `<PackageReference Include="..." />` **without** a `Version` attribute.
- **Platforms:** `x64` only. Never add `AnyCPU` configurations.
- **Run `dotnet format`** before pushing to ensure the style gate passes in CI.

## Reporting Issues

Use the Issue templates in `.github/ISSUE_TEMPLATE/`:

- **Bug report** — for regressions and incorrect behaviour.
- **Feature request** — for new capabilities.
- **Custom** — for anything that doesn't fit the above.

Link every Issue to the active GitHub Milestone before starting work.
