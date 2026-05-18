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

## Workflow & Field Conventions

ProjectV uses a structured issue→branch→work→pr→review→merge workflow with GitHub field discipline for traceability.

### Standard workflow

1. **Issue first.** Every meaningful change starts as a GitHub Issue. Use the templates under `.github/ISSUE_TEMPLATE/`:
   - `bug_report.md` for defects.
   - `feature_request.md` for new functionality.
   - `custom.md` for governance/process/tooling work.
2. **Branch from `master`.** Feature branch naming: `<type>/<short-description>` (e.g., `fix/ci-format-path`, `refactor/rename-sources`, `phase-N/NN-short-name` for multi-plan phase work).
3. **Work in commits.** Atomic commits per logical change. Conventional-commit prefixes preferred (`fix:`, `feat:`, `refactor:`, `chore:`, `docs:`, `ci:`, `phase(NN-MM):` for phase plans).
4. **Open the PR.** PR body must include `closes #<issue-number>` (or `refs #<n>` for related-but-not-closing). Fill in the PR template's test plan.
5. **Apply the field convention** (see below) on every PR and Issue.
6. **Review.** External contributors require a review. Owner/maintainer (Vasar007) may bypass review for low-risk batches (e.g., dependency-only updates inside a planned phase) using `--admin`, but should request review by default and prefer the long-lived feature-branch model below for multi-plan phases.
7. **Squash-merge.** Default: `gh pr merge <PR#> --squash --delete-branch=false` (preserves remote branches for post-mortem). Use `--admin` only when explicitly bypassing a pre-existing CI hiccup unrelated to the PR's own gate.

### GitHub field convention

Every Issue and PR should have the following fields set:

| Field | Purpose | Example |
|-------|---------|---------|
| **Assignee** | Owner of the work. | `Vasar007` (default). |
| **Labels** | Categorization. Always include one `type:` and one `area:` label minimum. Add `status:` labels as work progresses. | `type: Code Maintenance` + `area: Dependencies` + `area: Tests`. |
| **Milestone** | Release scope. Use the next open milestone (e.g., `v0.9.8`) for in-flight work; closed milestones for already-released work (assigned via API where `gh` CLI refuses closed milestones). | `v0.9.8`. |
| **Project** | Long-running project board for cross-milestone tracking. | `ProjectV v1.0.0` (number 2). |
| **Development** | GitHub auto-links via `closes #<n>` / `fixes #<n>` keywords in PR body. No manual action needed when keywords are used correctly. | Implicit via `closes #330` in PR body. |
| **Relationships (optional)** | Use parent/sub-issue links when an issue is decomposed into multiple sub-issues. | Sub-issue under a phase tracking issue. |

`gh` commands to apply the convention:

```shell
# PR
gh pr edit <PR#> \
  --add-assignee Vasar007 \
  --milestone <vX.Y.Z> \
  --add-label "type: Code Maintenance" \
  --add-label "area: <area>"
gh project item-add 2 --owner Vasar007 --url <pr-url>

# Issue
gh issue edit <#> \
  --add-assignee Vasar007 \
  --milestone <vX.Y.Z> \
  --add-label "type: <type>" \
  --add-label "area: <area>"
gh project item-add 2 --owner Vasar007 --url <issue-url>
```

For closed milestones, `gh` CLI refuses — fall back to direct API:

```shell
gh api -X PATCH repos/Vasar007/ProjectV/issues/<#> -F milestone=<numeric-milestone-id>
```

### Long-lived feature branch for multi-plan phases (Phase 1+ model)

For large multi-plan phases (e.g., Phase 1 dependency upgrades), the workflow uses a single long-lived feature branch per phase with commits per plan/task:

1. Phase opens with a tracking Issue and a `.planning/phases/NN-<name>/` directory holding `PLAN.md` files per task.
2. A single feature branch `phase-N/<name>` is cut from `master`.
3. Each plan's work commits to that branch (typically per-plan PRs squash-merged into the long branch, OR commits applied directly on the branch — owner's choice).
4. At end of phase, owner reviews the cumulative diff and merges to `master`.
5. Release tag + GitHub Release marks phase closure.

This model lets the maintainer review the whole phase in one pass instead of one micro-PR at a time. Per-plan PRs targeting `master` directly (the model Phase 1 actually used) is also acceptable when each plan is independently verifiable.
