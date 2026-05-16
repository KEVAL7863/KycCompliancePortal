# KYC & Compliance Onboarding Portal

A multi-tier customer-onboarding backend for banks: every customer is **risk-scored**,
**screened against an AML / sanctions watchlist**, and every action is written to an
**immutable audit trail**. Built to mirror the kind of RegTech product TSS Consultancy
(TrackWizz) builds.

> **Stack:** ASP.NET Core 9 Web API · C# · Entity Framework Core (SQLite) · JWT auth ·
> BCrypt · xUnit + FluentAssertions + Moq · Clean layered architecture

---

## Why this project

TSS / TrackWizz builds KYC / AML / regulatory-compliance software. This project
deliberately demonstrates every skill from their job description:

| TSS requirement | Where it lives here |
|---|---|
| .NET / C# / ASP.NET Core Web API | whole solution |
| Multi-tier application | Core → Application → Infrastructure → Api |
| OOP & clean design | entities, interfaces, dependency inversion |
| SQL & data modelling | EF Core `AppDbContext`, indexes, relations |
| Unit testing (xUnit) | `tests/` — 48 tests, TDD-style |
| TDD / automated testing | pure `RiskScoringService` & `AmlScreeningService` |
| Data structures & complexity | `LevenshteinDistance` (O(m·n) time, O(min) space) |
| Security | JWT auth, BCrypt hashing, role-based access |
| Multi-tenancy | every row carries a `TenantId`, isolated per bank |

---

## Architecture (Clean / layered)

```
KycCompliancePortal.Core            ← entities, enums, domain models, interfaces (no dependencies)
KycCompliancePortal.Application     ← pure business logic: risk engine, AML screening, algorithms
KycCompliancePortal.Infrastructure  ← EF Core, JWT, BCrypt, audit logger, DB seeder
KycCompliancePortal.Api             ← controllers, auth, Swagger, DI wiring
KycCompliancePortal.Tests           ← xUnit tests for the Application layer
```

Dependencies point **inward only** (Api → Infrastructure → Application → Core).
The business logic has no database or framework dependency, which is exactly why
it can be unit tested in isolation.

---

## Run it

```bash
# from the KycCompliancePortal folder
dotnet test          # run all 48 unit tests
dotnet run --project src/KycCompliancePortal.Api --launch-profile http
```

Then open Swagger: **http://localhost:5164/swagger**

The SQLite database (`kyc.db`) is created and seeded automatically on first run.

### Seeded accounts

| Role | Email | Password |
|---|---|---|
| Admin | `keval@bank001.com` | `Admin@123` |
| Compliance Officer | `hardik@bank001.com` | `Officer@123` |

---

## API

| Method | Endpoint | Access |
|---|---|---|
| POST | `/api/auth/register` | anonymous |
| POST | `/api/auth/login` | anonymous → returns JWT |
| POST | `/api/customers` | any authenticated user (runs risk + AML) |
| GET | `/api/customers` | ComplianceOfficer / Admin |
| GET | `/api/customers/{id}` | authenticated, tenant-scoped |
| POST | `/api/customers/{id}/approve` | ComplianceOfficer / Admin |
| POST | `/api/customers/{id}/reject` | ComplianceOfficer / Admin |

On customer creation: a high risk level **or** any watchlist hit forces the status
to `UnderReview` so a human compliance officer must decide.

---

## How the risk engine works

Pure, deterministic, rule-based scoring (0–100), bucketed Low / Medium / High:

| Factor | Points |
|---|---|
| High-risk jurisdiction (Iran, North Korea, …) | +40 |
| Politically Exposed Person (PEP) | +30 |
| Activity with no declared income / volume ≫ income | +20 |
| Very high monthly transaction volume | +15 |
| Young customer with activity | +10 |

`< 30` → Low, `30–69` → Medium, `≥ 70` → High.

## How AML screening works

1. Normalize both names (lower-case, strip punctuation, collapse spaces).
2. Exact match → score 100.
3. Otherwise best **Levenshtein** similarity; ≥ 85 → fuzzy hit.

Levenshtein uses the rolling-array optimization: **O(m·n)** time but only
**O(min(m, n))** space instead of the full matrix.

---

## Tests

48 xUnit tests (FluentAssertions, `[Theory]`/`[InlineData]`, an in-memory test
double for the watchlist). They cover the risk rules and their boundaries, the
edit-distance algorithm against textbook values, name normalization, and AML
exact / fuzzy / threshold behaviour — written test-first (TDD).

```
Passed!  - Failed: 0, Passed: 48, Skipped: 0
```

---


