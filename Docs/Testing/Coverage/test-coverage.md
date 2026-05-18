# ProjectV Test Coverage Inventory

**Phase 2 start status:** 2026-05-18
**Phase:** v0.9.8 — Test Coverage
**Document role:** TEST-01 deliverable; design contract for the rest of Phase 2.

## TEST-01 (verbatim from `.planning/REQUIREMENTS.md`)

> A checked-in document enumerates the critical paths across Domain (appraisal
> logic, model invariants, F# policy / activities), Application (`Shell`,
> `ShellBuilder`, `DataflowPipeline`, `Executors`), and Infrastructure (DB +
> TMDb / OMDb / Steam adapters) layers and maps each path to the tests that
> cover it.

## How downstream plans update this file

Downstream Phase 2 plans tick rows off by:

1. Flipping the row `Status` column from `planned` (or `partially covered`) to
   `covered` once the row's planned test project actually exercises the path.
2. Adding a new `Test Files` column on the right with the path(s) of the
   committed test file(s) that cover the row (e.g.
   `Sources/Tests/ProjectV.Appraisers.Tests/AppraiserTests.cs`).
3. Never deleting rows. Paths can be promoted to `tested around` if an
   architectural decision (`ARCHITECTURE.md` § "Anti-Patterns") pushes them
   out of direct test scope; the row stays as the audit trail.

Cross-references: this document is the source of truth that
[`projectv-scenario-tests-overview.md`](../Scenarios/projectv-scenario-tests-overview.md)
and [`ARCHITECTURE.md`](../../../.planning/codebase/ARCHITECTURE.md) point back to.

## Legend

| Column | Meaning |
|--------|---------|
| `Path` | The critical-path entry point — class/method/scenario being verified. |
| `Component` | The production library or web service that owns the path. |
| `Planned Test Project` | The canonical `ProjectV.<Library>.Tests` project that will hold the test(s). Names follow D-01 (one test project per production library) and D-02 (`ProjectV.Tests.Shared` for shared infrastructure). |
| `Test Type` | `Unit` (NSubstitute-mocked collaborators, AwesomeAssertions on return) / `Integration` (real composition, real Testcontainers Postgres, real EF Core) / `Contract` (WireMock.Net HTTP stubs fed from recorded JSON fixtures) / `Unit (F#)` (Unquote quoted-expression assertions, F# stack stays as-is). |
| `Status` | `planned` (no covering test yet) / `partially covered` (some coverage exists, more needed) / `covered` (verified by a committed test, see Test Files) / `tested around` (path is verified through a higher-level path; ARCHITECTURE.md anti-pattern means we test what's there, not what we wish were there). |

### Status vocabulary

- `planned` — no covering test in the repo today.
- `partially covered` — at least one test exists; the remaining shape is named in the row notes.
- `partially covered (skip resolved in 02-01)` — the historical `[Fact(Skip = "…")]` blocker on the `BasicInfo` JSON round-trip in `ProjectV.Common.Tests` was removed during the 02-01 retrofit. Row stays `partially covered` because the broader model-invariants surface ports to `ProjectV.Models.Tests` per row.
- `covered` — a committed test file under `Sources/Tests/ProjectV.<Library>.Tests/` exercises the path; the test-file path is listed in the `Test Files` column.
- `tested around` — the path is exercised indirectly through a higher-level integration test because an architectural anti-pattern blocks direct unit testing (see ARCHITECTURE.md § "Anti-Patterns" — `Shell` references concrete plugin assemblies; `SimpleExecutor.ExecuteAsync()` is a `NotImplementedException` stub; `ServiceRequestProcessor.CreateExecutorAsync` rebuilds the pipeline per request).

## Trait conventions

Every C# test class declares one `Category` trait. Integration tests that
depend on a Docker daemon (Testcontainers) add the `RequiresDocker` trait too.
F# tests skip the `Category` trait — they run as their own named CI stage via
the explicit `fsproj` invocation per D-23.

- Unit tests: `[Trait("Category","Unit")]`
- Integration tests: `[Trait("Category","Integration")]` and (when Testcontainers is involved) `[Trait("RequiresDocker","true")]`
- Contract tests: `[Trait("Category","Contract")]`
- F# tests: no `Category` trait — run separately in CI (`Test (F#)` stage, D-23).

## Domain Layer

| Path | Component | Planned Test Project | Test Type | Status |
|------|-----------|----------------------|-----------|--------|
| `Appraiser<BasicInfo>.GetRatings` — property defaults, null-arg, 1/3/N items | `ProjectV.Appraisers` | `ProjectV.Appraisers.Tests` | Unit | partially covered (retrofit + extend) |
| `MovieCommonAppraiser`, `MovieNormalizedAppraiser`, `GameCommonAppraiser`, `GameNormalizedAppraiser` — rating computation accuracy | `ProjectV.Appraisers` | `ProjectV.Appraisers.Tests` | Unit | planned |
| `AppraisersManager` — add/remove appraisers, `CreateFlow()` shape | `ProjectV.Appraisers` | `ProjectV.Appraisers.Tests` | Unit | planned |
| `BasicInfo`, `MovieInfo`, `GameInfo` model invariants + JSON round-trip | `ProjectV.Models` | `ProjectV.Common.Tests` | Unit | partially covered (skip resolved in 02-01) |
| Custom exception types (`CannotGetTmdbConfigException`, etc.) — 3-ctor convention | `ProjectV.Models` | `ProjectV.Models.Tests` | Unit | planned |
| `UserId`, `JobId` value-object behavior — `Create`, `Parse`, `None` | `ProjectV.Models` | `ProjectV.Models.Tests` | Unit | planned |
| `ProjectV.Activities.PolicyModels` — retry policy construction | `ProjectV.Activities` | `ProjectV.Activities.Tests` (F# or C# wrapper) | Unit | planned |
| `ProjectV.ContentDirectories.ContentFinder` — guard clauses on bad paths | `ProjectV.ContentDirectories` | `ProjectV.ContentDirectories.Tests` | Unit (F#) | covered |

## Application Layer

| Path | Component | Planned Test Project | Test Type | Status |
|------|-----------|----------------------|-----------|--------|
| `Shell.Run` — success path, error path (`ServiceStatus.Error`), output-error path | `ProjectV.Core` | `ProjectV.Core.Tests` | Unit (mocked managers) | planned (tested around — `Shell` references concrete plugin assemblies, see `ARCHITECTURE.md` § "Anti-Patterns") |
| `ShellBuilderFromXDocument` — builds Shell from minimal valid XDocument | `ProjectV.Core` | `ProjectV.Core.Tests` | Unit | planned |
| `ShellBuilderDirector` — director invokes all 4 builder steps in order | `ProjectV.Core` | `ProjectV.Core.Tests` | Unit | planned |
| `DataflowPipeline.Execute` — stages connected, data flows end-to-end | `ProjectV.DataPipeline` | `ProjectV.DataPipeline.Tests` | Integration (real dataflow, mocked `ICrawler`/`IAppraiser`) | planned |
| `InputtersFlow` — deduplication of repeated input items | `ProjectV.DataPipeline` | `ProjectV.DataPipeline.Tests` | Unit | planned |
| `CrawlersManager.TryGetResponse` — logs + rethrows on exception | `ProjectV.Crawlers` | `ProjectV.Crawlers.Tests` | Unit | planned |
| `InputManager`, `OutputManager` — `CreateFlow()` returns non-null | `ProjectV.InputProcessing`, `ProjectV.OutputProcessing` | `ProjectV.InputProcessing.Tests`, `ProjectV.OutputProcessing.Tests` | Unit | planned |
| `SimpleExecutor.ExecuteAsync()` — parameterless overload throws `NotImplementedException` | `ProjectV.Executors` | `ProjectV.Executors.Tests` | Unit | planned (tested around — current behaviour is a `NotImplementedException` stub, see `ARCHITECTURE.md` § "Anti-Patterns") |
| `CommunicationServiceClient.LoginAsync` + `StartJobAsync` — happy path + auth failure | `ProjectV.Core` | `ProjectV.Core.Tests` | Unit (WireMock HTTP or NSubstitute factory) | planned |
| `AddHttpClientWithOptions` + Polly retry policy wiring — retry fires on transient HTTP error | `ProjectV.Core` | `ProjectV.Core.Tests` | Unit (WireMock transient-error fixture) | planned |

## Infrastructure Layer

| Path | Component | Planned Test Project | Test Type | Status |
|------|-----------|----------------------|-----------|--------|
| `DatabaseJobInfoService.AddJobAsync` / `GetJobAsync` / `UpdateJobAsync` — round-trip | `ProjectV.DataAccessLayer` | `ProjectV.DataAccessLayer.Tests` | Integration (Testcontainers) | planned |
| `DatabaseUserInfoService.AddUserAsync` / `GetUserAsync` | `ProjectV.DataAccessLayer` | `ProjectV.DataAccessLayer.Tests` | Integration (Testcontainers) | planned |
| `DatabaseRefreshTokenInfoService.AddTokenAsync` / expiry behavior | `ProjectV.DataAccessLayer` | `ProjectV.DataAccessLayer.Tests` | Integration (Testcontainers) | planned |
| `ProjectVDbContext` schema — tables exist, constraints enforced | `ProjectV.DataAccessLayer` | `ProjectV.DataAccessLayer.Tests` | Integration (Testcontainers) | planned |
| `TmdbClient.GetMovieAsync` — success response, not-found, config-fetch | `ProjectV.TmdbService` | `ProjectV.TmdbService.Tests` | Contract (WireMock) | planned |
| `OmdbClient.TryGetItemByTitleAsync` — success response, false-response swallowed | `ProjectV.OmdbService` | `ProjectV.OmdbService.Tests` | Contract (WireMock) | planned |
| `SteamApiClient.GetAppListAsync` / `TryGetSteamAppAsync` | `ProjectV.SteamService` | `ProjectV.SteamService.Tests` | Contract (WireMock) | planned |
| CommunicationWebService — `POST /api/v1/Requests` with valid JWT → 200 | `ProjectV.CommunicationWebService` | `ProjectV.CommunicationWebService.Tests` | Integration (WebApplicationFactory) | planned |
| CommunicationWebService — `POST /api/v1/Requests` without JWT → 401 | `ProjectV.CommunicationWebService` | `ProjectV.CommunicationWebService.Tests` | Integration (WebApplicationFactory) | planned |
| CommunicationWebService — `POST /api/v1/Users/Login` — valid credentials → JWT issued | `ProjectV.CommunicationWebService` | `ProjectV.CommunicationWebService.Tests` | Integration (WebApplicationFactory) | planned |
| TelegramBotWebService webhook — `POST /api/v1/Update` with valid Update payload → 200 | `ProjectV.TelegramBotWebService` | `ProjectV.TelegramBotWebService.Tests` | Integration (WebApplicationFactory) | planned |
| TelegramBotWebService polling — `PoolingProcessor` processes a fixed Update sequence | `ProjectV.TelegramBotWebService` | `ProjectV.TelegramBotWebService.Tests` | Integration (WebApplicationFactory) | planned |
| ProcessingWebService — `POST /api/v1/Processing` smoke test (config + pipeline construction) | `ProjectV.ProcessingWebService` | `ProjectV.ProcessingWebService.Tests` | Integration (WebApplicationFactory, WireMock) | planned |

## Maintenance

Downstream Phase 2 plans update this document in step with the test files they
add:

- When a row's covering test file lands, flip the `Status` from `planned`
  (or `partially covered`) to `covered` and append a `Test Files` column on
  the right of that table containing the repo-relative path(s) to the test
  file(s) that exercise the row.
- Never delete rows. If an architectural change pushes a path out of direct
  test scope, promote it to `tested around` and add a one-sentence note
  pointing at the higher-level test that now exercises it (or at the
  `ARCHITECTURE.md` § "Anti-Patterns" entry that explains the indirection).
- New critical paths discovered mid-phase are added as new rows under the
  matching layer section — keep the table header stable so the diff stays
  reviewable.

## Cross-references

- [`Docs/Testing/Scenarios/projectv-scenario-tests-overview.md`](../Scenarios/projectv-scenario-tests-overview.md) — scenario-test architecture, mermaid diagram, and conventions for the `Scenarios/` subdirectory rows above.
- [`.planning/codebase/ARCHITECTURE.md`](../../../.planning/codebase/ARCHITECTURE.md) — Component Responsibilities, Data Flow, and Anti-Patterns that the `tested around` rows reference.
- [`.planning/codebase/INTEGRATIONS.md`](../../../.planning/codebase/INTEGRATIONS.md) — TMDb / OMDb / Steam / Telegram wiring that the Contract / Integration rows verify.
- [`.planning/REQUIREMENTS.md`](../../../.planning/REQUIREMENTS.md) — Phase 2 requirements TEST-01..TEST-06.
