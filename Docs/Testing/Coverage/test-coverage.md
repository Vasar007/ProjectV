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

| Path | Component | Planned Test Project | Test Type | Status | Test Files |
|------|-----------|----------------------|-----------|--------|------------|
| `Appraiser<BasicInfo>.GetRatings` — property defaults, null-arg, 1/3/N items | `ProjectV.Appraisers` | `ProjectV.Appraisers.Tests` | Unit | covered | `Sources/Tests/ProjectV.Appraisers.Tests/AppraiserTests.cs` |
| `Appraiser<TmdbMovieInfo>` + `TmdbCommonAppraisal` — movie-common rating computation accuracy (planner's `MovieCommonAppraiser` row) | `ProjectV.Appraisers` | `ProjectV.Appraisers.Tests` | Unit | covered | `Sources/Tests/ProjectV.Appraisers.Tests/AppraisersExtensions/MovieCommonAppraiserTests.cs` |
| `Appraiser<BasicInfo>` + `BasicAppraisalNormalized` — movie-normalized rating computation accuracy (planner's `MovieNormalizedAppraiser` row) | `ProjectV.Appraisers` | `ProjectV.Appraisers.Tests` | Unit | covered | `Sources/Tests/ProjectV.Appraisers.Tests/AppraisersExtensions/MovieNormalizedAppraiserTests.cs` |
| `Appraiser<SteamGameInfo>` + Steam/`Omdb` appraisals — game-common / game-normalized rating computation accuracy | `ProjectV.Appraisers` | `ProjectV.Appraisers.Tests` | Unit | planned | — |
| `AppraisersManager` — add/remove appraisers, `CreateFlow()` shape | `ProjectV.Appraisers` | `ProjectV.Appraisers.Tests` | Unit | covered | `Sources/Tests/ProjectV.Appraisers.Tests/AppraisersExtensions/AppraisersManagerTests.cs` |
| `BasicInfo` model invariants + Newtonsoft.Json round-trip | `ProjectV.Models` | `ProjectV.Models.Tests` | Unit | covered | `Sources/Tests/ProjectV.Models.Tests/Data/BasicInfoInvariantsTests.cs`, `Sources/Tests/ProjectV.Common.Tests/ModelSerializationTests.cs` |
| `MovieInfo`, `GameInfo` model invariants + JSON round-trip | `ProjectV.Models` | `ProjectV.Models.Tests` | Unit | planned | — |
| Custom exception types (`CannotGetTmdbConfigException`, etc.) — 3-ctor convention | `ProjectV.Models` | `ProjectV.Models.Tests` | Unit | covered | `Sources/Tests/ProjectV.Models.Tests/Exceptions/CannotGetTmdbConfigExceptionTests.cs`, `Sources/Tests/ProjectV.Models.Tests/Exceptions/CommonExceptionsTestSuite.cs` |
| `UserId`, `JobId` value-object behavior — `Create`, `Parse`, `None`, `Wrap`, `TryParse`, `IsSpecified` | `ProjectV.Models` | `ProjectV.Models.Tests` | Unit | covered | `Sources/Tests/ProjectV.Models.Tests/ValueObjects/UserIdTests.cs`, `Sources/Tests/ProjectV.Models.Tests/ValueObjects/JobIdTests.cs` |
| `ProjectV.Activities.PolicyModels` — retry policy construction | `ProjectV.Activities` | `ProjectV.Activities.Tests` (F# or C# wrapper) | Unit | planned | — |
| `ProjectV.ContentDirectories.ContentFinder` — guard clauses on bad paths | `ProjectV.ContentDirectories` | `ProjectV.ContentDirectories.Tests` | Unit (F#) | covered | `Sources/Tests/ProjectV.ContentDirectories.Tests/ContentFinderTests.fs` |

## Application Layer

| Path | Component | Planned Test Project | Test Type | Status | Test Files |
|------|-----------|----------------------|-----------|--------|------------|
| `Shell.Run` — success path, error path (`ServiceStatus.Error`), output-error path | `ProjectV.Core` | `ProjectV.Core.Tests` | Unit (mocked managers) | tested around — see 02-05-SUMMARY § "Deviations" for the Gridsum.DataflowEx empty-pipeline blocker. Shell's constructor null-guards (5 args), property surface, `Dispose` idempotency, and the `CreateBuilderDirector` static factory ARE covered at Unit; full `Run` branch coverage is deferred to a future integration plan (Phase 3 E2E or 02-10 JWT integration). | `Sources/Tests/ProjectV.Core.Tests/ShellTests.cs` |
| `ShellBuilderFromXDocument` — builds Shell from minimal valid XDocument | `ProjectV.Core` | `ProjectV.Core.Tests` | Unit | covered (ctor null-guard, missing-root guard, minimal-config happy path, GetResult-before-build guard, Reset, BuildMessageHandler missing-element error path) | `Sources/Tests/ProjectV.Core.Tests/ShellBuilders/ShellBuilderFromXDocumentTests.cs` |
| `ShellBuilderDirector` — director invokes all 4 builder steps in order | `ProjectV.Core` | `ProjectV.Core.Tests` | Unit | covered (ctor null-guard, ctor happy path, ChangeShellBuilder null-guard, MakeShell invokes all 7 steps, MakeShell invokes them in declared order, MakeShell dispatches to replaced builder) | `Sources/Tests/ProjectV.Core.Tests/ShellBuilders/ShellBuilderDirectorTests.cs` |
| `DataflowPipeline.Execute` — stages connected, data flows end-to-end | `ProjectV.DataPipeline` | `ProjectV.DataPipeline.Tests` | Integration (real dataflow, mocked `ICrawler`/`IAppraiser`) | covered (single-item happy path through the four real Gridsum.DataflowEx stages with substitute `ICrawler` + `IAppraiser` leaves; the production `DataflowPipeline.Execute(string)` uses the single-arg `InputtersFlow.ProcessAsync(...)` overload that deadlocks on terminal pipelines, so the integration test drives the same wiring via the two-arg `ProcessAsync(..., completeFlowOnFinish: true)` — see `02-06-SUMMARY.md` § "Deviations §1") | `Sources/Tests/ProjectV.DataPipeline.Tests/DataflowPipelineTests.cs` |
| `InputtersFlow` — deduplication of repeated input items + `MinWordLength > 2` length filter | `ProjectV.DataPipeline` | `ProjectV.DataPipeline.Tests` | Integration (real Gridsum.DataflowEx block; reflection probe on the private `FilterInputData` predicate to avoid the predicated-link completion deadlock — see `02-06-SUMMARY.md` § "Deviations §2") | covered (dedup branch — unique items pass, duplicates filtered; length branch — `Length > MinWordLength (2)` survivors only; happy-path end-to-end smoke with no filtering) | `Sources/Tests/ProjectV.DataPipeline.Tests/InputtersFlowTests.cs` |
| `CrawlersManager.TryGetResponse` — rethrows original exception on child-crawler failure | `ProjectV.Crawlers` | `ProjectV.Crawlers.Tests` | Unit | covered (rethrow assertion via reflection on the private method with a throwing `ICrawler` substitute). The `_logger.Error(...)` side-effect is NOT directly substituted in this Unit suite because the logger is a `private static readonly` field initialised via `LoggerFactory.CreateLoggerFor<CrawlersManager>()` — see `02-06-SUMMARY.md` § "Deviations §3". Also covers constructor + `Add` null-guard + `Remove` happy path. | `Sources/Tests/ProjectV.Crawlers.Tests/CrawlersManagerTests.cs` |
| `InputManager.CreateFlow` — returns non-null `InputtersFlow` for empty + populated registrations and empty storage-name fallback | `ProjectV.InputProcessing` | `ProjectV.InputProcessing.Tests` | Unit | covered (CreateFlow non-null with/without registered inputters; empty storage-name → default fallback; ctor null/whitespace guard; Add null-guard; Remove round-trip) | `Sources/Tests/ProjectV.InputProcessing.Tests/InputManagerTests.cs` |
| `OutputManager.CreateFlow` — returns non-null `OutputtersFlow` for empty + populated registrations and empty storage-name fallback | `ProjectV.OutputProcessing` | `ProjectV.OutputProcessing.Tests` | Unit | covered (CreateFlow non-null with/without registered outputters; empty storage-name → default fallback; ctor null/whitespace guard; Add null-guard; Remove round-trip) | `Sources/Tests/ProjectV.OutputProcessing.Tests/OutputManagerTests.cs` |
| `SimpleExecutor.ExecuteAsync()` — parameterless overload throws `NotImplementedException` | `ProjectV.Executors` | `ProjectV.Executors.Tests` | Unit | covered (tested around — anti-pattern documented in `ARCHITECTURE.md` § "Anti-Patterns" and `02-CONTEXT.md` § "Deferred Ideas"; the test asserts the CURRENT throw behaviour. Also covers ctor null-guard on `jobInfo`, `ArgumentOutOfRangeException` on non-positive `executionsNumber`, and happy-path property exposure.) | `Sources/Tests/ProjectV.Executors.Tests/SimpleExecutorTests.cs` |
| `CommunicationServiceClient.LoginAsync` — happy path + 401 auth failure | `ProjectV.Core` | `ProjectV.Core.Tests` | Unit (NSubstitute IHttpClientFactory + FakeHttpMessageHandler) | covered (200 → `Result.Ok<TokenResponse>`; 401 → `Result.Error<ErrorResponse>`; null-arg guard). `StartJobAsync` happy path deferred to integration — see `02-05-SUMMARY.md` Deviations §3 (the token-cache pre-flight + refresh-on-unauthorized policy chain requires real composition to exercise meaningfully). | `Sources/Tests/ProjectV.Core.Tests/Net/CommunicationServiceClientTests.cs` |
| `AddHttpClientWithOptions` + Polly retry policy wiring — retry fires on transient HTTP error | `ProjectV.Core` | `ProjectV.Core.Tests` | Unit (FakeHttpMessageHandler DelegatingHandler) | covered (503 → 503 → 503 → 200 with `RetryCountOnFailed=3` → 4 invocations; always-503 → 1 + N retries; first-call-200 → 1 invocation) | `Sources/Tests/ProjectV.Core.Tests/Net/HttpClientPollyPolicyTests.cs` |

## Infrastructure Layer

| Path | Component | Planned Test Project | Test Type | Status | Test Files |
|------|-----------|----------------------|-----------|--------|------------|
| `DatabaseJobInfoService.AddJobAsync` / `GetJobAsync` / `UpdateJobAsync` — round-trip | `ProjectV.DataAccessLayer` | `ProjectV.DataAccessLayer.Tests` | Integration (Testcontainers) | planned | — |
| `DatabaseUserInfoService.AddUserAsync` / `GetUserAsync` | `ProjectV.DataAccessLayer` | `ProjectV.DataAccessLayer.Tests` | Integration (Testcontainers) | planned | — |
| `DatabaseRefreshTokenInfoService.AddTokenAsync` / expiry behavior | `ProjectV.DataAccessLayer` | `ProjectV.DataAccessLayer.Tests` | Integration (Testcontainers) | planned | — |
| `ProjectVDbContext` schema — tables exist, constraints enforced | `ProjectV.DataAccessLayer` | `ProjectV.DataAccessLayer.Tests` | Integration (Testcontainers) | planned | — |
| `TmdbClient.TrySearchMovieAsync` / `GetConfigAsync` — search success, empty-result envelope, configuration fetch (`GetMovieAsync` does NOT exist in the production wrapper — Rule 1 deviation from the 02-08 plan wording, recorded in `02-08-SUMMARY.md` § "Deviations §1") | `ProjectV.TmdbService` | `ProjectV.TmdbService.Tests` | Contract (WireMock) | covered (3 tests exercise the real `TMDbLib` HTTP pipeline against WireMock-served recorded JSON; redirection seam: `new TmdbClient(apiKey, useSsl: false, baseUrl: WireMockHostPort)` via `InternalsVisibleTo` per `02-08-SUMMARY.md` § "Deviations §2") | `Sources/Tests/ProjectV.TmdbService.Tests/TmdbContractTests.cs` |
| `OmdbClient.TryGetItemByTitleAsync` — success response, false-response swallowed | `ProjectV.OmdbService` | `ProjectV.OmdbService.Tests` | Contract (WireMock) | covered (2 tests exercise the real `OMDbApiNet` HTTP pipeline against WireMock-served recorded JSON; redirection seam: `HttpClient.DefaultProxy = new WebProxy(WireMock.Url)` because OMDbApiNet 1.3.0 hardcodes `BaseUrl` as a `const` field — see `02-08-SUMMARY.md` § "Deviations §3") | `Sources/Tests/ProjectV.OmdbService.Tests/OmdbContractTests.cs` |
| `SteamApiClient.GetAppListAsync` / `TryGetSteamAppAsync` | `ProjectV.SteamService` | `ProjectV.SteamService.Tests` | Contract (WireMock) | covered (2 tests exercise the real `SteamWebApiLib` HTTP pipeline against WireMock-served recorded JSON; redirection seam: reflection-replace the wrapper's `_steamApiClient` with an SDK instance built from a `SteamApiConfig` whose `SteamPoweredBaseUrl` + `SteamStoreBaseUrl` point at WireMock — see `02-08-SUMMARY.md` § "Deviations §4") | `Sources/Tests/ProjectV.SteamService.Tests/SteamContractTests.cs` |
| CommunicationWebService — `POST /api/v1/Requests` with valid JWT → 200 | `ProjectV.CommunicationWebService` | `ProjectV.CommunicationWebService.Tests` | Integration (WebApplicationFactory) | planned | — |
| CommunicationWebService — `POST /api/v1/Requests` without JWT → 401 | `ProjectV.CommunicationWebService` | `ProjectV.CommunicationWebService.Tests` | Integration (WebApplicationFactory) | planned | — |
| CommunicationWebService — `POST /api/v1/Users/Login` — valid credentials → JWT issued | `ProjectV.CommunicationWebService` | `ProjectV.CommunicationWebService.Tests` | Integration (WebApplicationFactory) | planned | — |
| TelegramBotWebService webhook — `POST /api/v1/Update` with valid Update payload → 200 | `ProjectV.TelegramBotWebService` | `ProjectV.TelegramBotWebService.Tests` | Integration (WebApplicationFactory) | planned | — |
| TelegramBotWebService polling — `PoolingProcessor` processes a fixed Update sequence | `ProjectV.TelegramBotWebService` | `ProjectV.TelegramBotWebService.Tests` | Integration (WebApplicationFactory) | planned | — |
| ProcessingWebService — `POST /api/v1/Processing` smoke test (config + pipeline construction) | `ProjectV.ProcessingWebService` | `ProjectV.ProcessingWebService.Tests` | Integration (WebApplicationFactory, WireMock) | planned | — |

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
