# Contributing to ProjectV

Thank you for your interest in contributing to ProjectV!

## Workflow

ProjectV follows a **GitHub-Milestone-driven** development process:

- **Phases and milestones:** Each development phase corresponds to one GitHub Milestone (e.g., `v0.9.8`). Milestones have no fixed deadlines — they ship when ready.
- **Task tracking:** Each task is one GitHub Issue linked to the active milestone. The implementing developer/agent posts notes on the Issue summarising decisions, nuances, and progress as work proceeds.
- **Pull requests:** Every PR must link at least one Issue using `closes #XXXX` (or `Part of #XXXX` for partial work) in the PR body. Opening a PR without a corresponding Issue is not allowed — create or find the Issue first.
- **Target branch:** From Phase 2 onward, all plan work targets a long-lived feature branch per phase (see below). The feature branch is merged to `master` at phase end via `/gsd-ship`. Phase 1 used per-plan PRs directly to `master` — that exception is closed.
- **Branch naming:** Feature branches: `phase-NN/<short-name>`. Short-lived per-plan worktree branches: `phase-NN/<short-name>/<plan-short>`. Delete worktree branches after they merge back to the phase branch.
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
2. **Branch from `master` (phase start).** At the start of each phase, cut a long-lived feature branch: `git checkout -b phase-NN/<short-name>`. All plan work for that phase commits to this branch.
3. **Work in commits.** Atomic commits per logical change. Conventional-commit prefixes preferred (`fix:`, `feat:`, `refactor:`, `chore:`, `docs:`, `ci:`, `phase(NN-MM):` for phase plans).
4. **Open the PR (at phase end).** PR body must include `closes #<issue-number>` (or `refs #<n>` for related-but-not-closing). Fill in the PR template's test plan. The user runs `/gsd-ship` to create the PR from the feature branch to `master`.
5. **Apply the field convention** (see below) on every PR and Issue.
6. **Review.** The reviewer on a feature-branch-to-master PR must be `Vasar007` — this is required (see field convention below). Per-plan commits into the feature branch are not PRs, so no reviewer is needed there.
7. **Squash-merge.** Default: `gh pr merge <PR#> --squash --delete-branch=false` (preserves remote branches for post-mortem).

### GitHub field convention

Every Issue and PR **must** have the following fields set (mandatory, not recommended):

| Field | Purpose | Example |
|-------|---------|---------|
| **Assignee** | Owner of the work. | `Vasar007` (default and required). |
| **Labels** | Categorization. Always include one `type:` and one `area:` label minimum. Add `status:` labels as work progresses. | `type: Code Maintenance` + `area: Dependencies` + `area: Tests`. |
| **Milestone** | Release scope. Use the next open milestone (e.g., `v0.9.8`) for in-flight work; closed milestones for already-released work (assigned via API where `gh` CLI refuses closed milestones). | `v0.9.8`. |
| **Project** | Long-running project board for cross-milestone tracking. | `ProjectV v1.0.0` (number 2). |
| **Reviewers** | Required on every feature-branch-to-master PR (i.e., the `/gsd-ship` PR at phase end). | `Vasar007`. Not needed for per-plan commits into the feature branch (those are not PRs). |
| **Development** | GitHub auto-links via `closes #<n>` / `fixes #<n>` keywords in PR body. No manual action needed when keywords are used correctly. | Implicit via `closes #330` in PR body. |
| **Relationships (optional)** | Use parent/sub-issue links when an issue is decomposed into multiple sub-issues. | Sub-issue under a phase tracking issue. |

`gh` commands to apply the convention:

```shell
# PR
gh pr edit <PR#> \
  --add-assignee Vasar007 \
  --milestone <vX.Y.Z> \
  --add-label "type: Code Maintenance" \
  --add-label "area: <area>" \
  --add-reviewer Vasar007
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

### Long-lived feature branch per phase (default from Phase 2 onward)

Starting with Phase 2, one long-lived feature branch per phase is the **default model**. Per-plan PRs directly to `master` are reserved for exceptional cases explicitly approved by the repository owner (Phase 1 used that exception because .NET 10 SDK availability was uncertain and each upgrade needed independent validation before continuing — that risk is now resolved).

**Mechanics:**

1. At phase start, branch from `master`: `git checkout -b phase-NN/<short-name>`.
2. All plan work commits to that branch. Per-plan can be one commit or several; the executor decides.
3. If a plan needs an isolated worktree (e.g., a sweeping rename), the worktree branch is created from the **phase feature branch**, not from `master`. After the plan's commits land back on the phase branch (merge or rebase), the worktree branch is deleted; the phase branch continues.
4. When the phase is complete — all plans done, builds green, SUMMARY.md files written — the **user** calls `/gsd-ship`. This creates the PR from the feature branch to `master`. The user reviews the full cumulative diff and merges or requests changes.
5. Do **not** open per-plan PRs into `master` mid-phase. Do **not** squash-merge per-plan worktree branches into `master` before the phase is complete.
6. Review bypass (`gh pr merge --admin`) is **off by default**. Use only for genuine pre-existing CI hiccups that the user has explicitly acknowledged.

### Issue Comment Lifecycle

Every Issue created for a GSD plan gets three categories of comments. This mirrors the two-comment protocol in `dev-essentials:feature-pipeline` (Task Comment #1 + Task Comment #2) and adds a closing summary so the issue tracker stands on its own as a record (independent of `.planning/`, which is gitignored).

1. **Initial plan comment** — posted _when work begins on the issue_ (immediately before the executor starts writing code). Contents: one-line restatement of the objective, 2–4 bullets on the planned approach, key constraints/risks, PR or branch name where the work will land, "implementation starts now", footer `(comment posted by Claude Code at <UTC timestamp>)`.

2. **Follow-up comment(s)** — posted _only when the plan changes mid-execution_ (scope expansion, new sub-tasks discovered, blocker, approach pivot). Contents: what changed, why, new approach, updated PR/branch info, footer.

3. **Closing summary** — posted _when the issue closes_ (whether by `closes #N` PR keyword or `gh issue close`). Contents: what was actually done (1–3 sentences), PR number(s) + merge SHA(s), the path of the corresponding `.planning/phases/NN-*/MM-SUMMARY.md` if relevant, any deferred work, footer `(closing summary by Claude Code at <UTC timestamp>)` or `(retroactive summary ...)` for backfills.

Post via `gh issue comment <n> --repo Vasar007/ProjectV --body-file <path>` (use `--body-file` for multi-line markdown bodies — they don't survive shell-quoting reliably).
