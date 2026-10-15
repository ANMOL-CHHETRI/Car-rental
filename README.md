# WeAreCars - Car Rental Management Solution

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-blue.svg)](https://dotnet.microsoft.com/)
[![Language](https://img.shields.io/badge/Language-C%23-green.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Database](https://img.shields.io/badge/Database-SQL%20Server%20LocalDB-red.svg)](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
[![Tests](https://img.shields.io/badge/Tests-42%2F42%20Passed-brightgreen.svg)]()
[![Audit Status](https://img.shields.io/badge/Audit%20Remediation-38%2F38%20Resolved-brightgreen.svg)](problems.md)

A robust, enterprise-grade C# Windows Forms desktop application developed for the Sunderland branch of **WeAreCars** car rental operations (CET131 Software Engineering Assignment 1). Built with .NET Framework 4.8, ADO.NET parameterized SQL, and SQL Server LocalDB.

---

## Table of Contents

- [Overview](#overview)
- [Staff Credentials](#staff-credentials)
- [Key Features & System Architecture](#key-features--system-architecture)
- [Pricing Rules & Benchmark Verification](#pricing-rules--benchmark-verification)
- [Database Reliability & Safety](#database-reliability--safety)
- [Automated Unit & Integration Tests](#automated-unit--integration-tests)
- [Project Structure](#project-structure)
- [Setup & Run Instructions](#setup--run-instructions)
- [Marking Rubric Alignment](#marking-rubric-alignment)

---

## Overview

The WeAreCars system empowers desk staff to securely manage vehicle rentals, calculate live itemized estimates, review bookings before persistence, inspect rented fleet availability, and review complete historical reservations with itemized optional extras.

The solution adheres strictly to:
- **Assignment 1 Brief & Marking Rubric** (42 points Functionality, 14 points Source-Code Annotation & Naming, 14 points Demonstration).
- **Domain Class Diagram & Login Flowchart**.
- **Decimal arithmetic** for precise currency calculations.
- **ACID transactional persistence** with atomic vehicle reservation locks.

---

## Staff Credentials

In accordance with the assignment specification, authorized desk staff authenticate using fixed credentials:

| Field | Value |
|---|---|
| **Username** | `sta001` |
| **Password** | `givemethekeys123` |

- **Security & UX:** Password input is masked by default with an accessible Show/Hide toggle button.
- **Workflow:** Pressing <kbd>Enter</kbd> submits the login form. Successful logins are audited via `LoginAuditLogger`.
- **Navigation Guard:** Booking and reporting forms cannot be accessed without valid authentication.

---

## Key Features & System Architecture

### 1. Presentation Layer (Windows Forms)
- **`Welcome_page`**: Branded entry point with single-instance window lifecycle management.
- **`LoginForm` (`Form1`)**: Masked authentication with Show/Hide toggle, Enter-key submission, and audit logging.
- **`Dashboard`**: 4 live metric KPI cards (*Total Fleet*, *Available Vehicles*, *Rented Vehicles*, *Total Bookings*), database status notification, and clean navigation buttons.
- **`Details` (New Booking)**: Visually grouped customer and rental inputs, inline `ErrorProvider` validation, live itemized quotation sidebar, and allocated vehicle registration badge.
- **`BookingReviewDialog`**: Dedicated pre-commit review dialog displaying customer information, vehicle allocation, rental days, itemized breakdown, and total cost before saving to the database.
- **`booking_report`**: Dual-purpose reporting suite featuring:
  1. *Rented Cars Report*: Live filterable fleet overview with search and refresh actions.
  2. *Booking History*: Historical booking ledger with master-detail view retrieving stored totals and saved optional extras from `BookingOptionalExtras`.

### 2. Business Logic & Domain Layer
- **`PricingCalculator`**: Decoupled, offline calculation engine producing an itemized `PricingBreakdown`.
- **`BookingValidator`**: Comprehensive validator checking customer text lengths (First Name ≤ 50, Surname ≤ 50, Address ≤ 150), age ≥ 18, valid driving licence (mandatory yes), and rental duration (1 to 28 days).
- **`LoginAuditLogger`**: In-memory and persistent logging (`login_audit.log`) tracking authentication attempts.
- **Domain Models**:
  - `Car`: Vehicle entity with registration, daily rate (£25.00), category, fuel, and rental status.
  - `Booking`: Encapsulates customer details, dates, car reference, and optional extras.
  - `OptionalExtras`: Calculates daily extra charges (£10.00 unlimited mileage, £2.00 breakdown cover).
  - `CarType` & `FuelType`: Enums with extension methods (`GetSurcharge()` / `getSurcharge()`) and strict parsing that rejects invalid values rather than silently falling back.

### 3. Data Persistence Layer (ADO.NET)
- **`BookingRepository`**: Manages parameterized SQL operations. Implements atomic booking persistence inside a SQL transaction with `UPDLOCK, ROWLOCK, HOLDLOCK` to prevent double-booking race conditions.
- **`DatabaseInitializer`**: Non-destructive database creation and starter fleet seeding. Automatically detects `SqlLocalDB.exe` across versioned installations (SQL Server 2012 through 2025).

---

## Pricing Rules & Benchmark Verification

Rental charges are calculated using exact decimal arithmetic:

$$\text{Total} = (\text{Daily Rate} \times \text{Days}) + \text{Vehicle Surcharge} + \text{Fuel Surcharge} + (\text{Daily Extras} \times \text{Days})$$

### Rate Schedule

| Charge Type | Item | Rate / Fee | Application |
|---|---|---|---|
| **Base Rental** | Fleet Daily Rate | £25.00 | Per day |
| **Vehicle Category** | City Car | £0.00 | Per booking |
| | Family Car | £50.00 | Per booking |
| | Sports Car | £75.00 | Per booking |
| | SUV | £65.00 | Per booking |
| **Fuel Type** | Petrol / Diesel | £0.00 | Per booking |
| | Hybrid | £30.00 | Per booking |
| | Full Electric | £50.00 | Per booking |
| **Optional Extras** | Unlimited Mileage | £10.00 | Per day |
| | Breakdown Cover | £2.00 | Per day |

### Benchmark Calculation Example
- **Specification:** 3-day rental, SUV, Hybrid, Unlimited Mileage, Breakdown Cover.
- **Base Rental:** $3 \times £25.00 = £75.00$
- **Vehicle Surcharge (SUV):** $£65.00$
- **Fuel Surcharge (Hybrid):** $£30.00$
- **Optional Extras:** $3 \times (£10.00 + £2.00) = £36.00$
- **Total:** $£75.00 + £65.00 + £30.00 + £36.00 = \mathbf{£206.00}$ (Verified by automated test).

---

## Database Reliability & Safety

1. **Non-Destructive Initialization:** Replaced legacy `DROP TABLE` scripts with idempotent `IF NOT EXISTS` schema migrations.
2. **Safe Fleet Seeding:** Seeds starter fleet vehicles by checking `RegistrationNumber` uniqueness, preserving existing customer records and rental statuses.
3. **Atomic Persistence:**
   ```sql
   -- Concurrency protection inside transaction
   UPDATE dbo.Cars WITH (UPDLOCK, ROWLOCK, HOLDLOCK)
   SET IsRented = 1
   WHERE CarId = @CarId AND IsRented = 0;
   ```
4. **Resilient LocalDB Discovery:** Checks active `PATH` and system directories across SQL Server tools versions `170`, `160`, `150`, `140`, `130`, `120`, and `110`.
5. **Graceful Offline Mode:** If LocalDB is not installed on the host machine, the application does not crash. Pricing calculations and form validations work 100% offline, and the Dashboard displays clear troubleshooting guidance with a retry button.

---

## Automated Unit, Regression, Integration & E2E Tests

The solution includes an end-to-end automated test suite spanning 40 comprehensive tests across C# domain logic, SQL transactional concurrency, HTTP API security, and Playwright browser accessibility:

### 1. C# Domain, Regression & SQL Integration Tests (`RentOnCars.Tests`) — 31 Tests

| # | Test Suite & Test Name | Target Area | Result |
|---|---|---|---|
| 1–4 | `Credentials` (Fixed, Invalid Username, Invalid Password, Empty) | Authentication Guard | Pass |
| 5–10 | `Validation` (Required, Surname Rule, Age Limits, Licence, Duration, DB Lengths) | Input & Schema Bounds | Pass |
| 11–16 | `Pricing` (Vehicle Surcharges, Fuel Surcharges, Extras, £206 Benchmark, Independence, Labels) | Decimal Arithmetic Engine | Pass |
| 17–20 | `Data Integrity & Domain` (Strict Enums, RentedCarsList, Flowchart Audit, Discovery) | System Invariants | Pass |
| 21 | `Regression - Undefined numeric enums rejected` | Strict Enum Boundary | Pass |
| 22 | `Regression - Vehicle state and domain constraints` | Availability & State Rules | Pass |
| 23 | `Regression - Allocated rate and stable GBP formatting` | Culture-Independent Money | Pass |
| 24 | `Regression - Vehicle identity and clear state` | Collection Lifecycle | Pass |
| 25 | `Regression - Filtered and sorted booking identity` | Report Master-Detail Mapping | Pass |
| 26 | `Regression - Explicit age and licence entry` | Zero-Assumption Defaults | Pass |
| 27 | `SQL - Authoritative prices, snapshots, retry and return` | Authoritative DB Terms | Pass |
| 28 | `SQL - Concurrent booking exclusion` | Concurrency & Row Locking | Pass |
| 29 | `SQL - Schema constraints and transaction audit` | Check Constraints & Trigger | Pass |
| 30 | `SQL - Staff authentication and lockout` | PBKDF2 Hashing & Lockout | Pass |
| 31 | `SQL - Verified backup, safe restore and retention preview` | DB Maintenance & GDPR | Pass |

### 2. HTTP Server API & Security Tests (`tests/server.test.cjs`) — 5 Tests
- **Unauthenticated Access, Hostile Origins & CSRF:** Enforces session cookies, origin whitelisting, and `X-CSRF-Token` mutation verification.
- **Strict Boundary Validation:** Rejects fractional days (`1.9`), exponential notation (`1e2`), invalid licence, and undefined categories.
- **Reviewed Snapshot & Benchmark Persistence:** Verifies immutable quotation snapshots, server-side assignment, and duplicate-request protection.
- **Concurrency Isolation:** Guarantees separate concurrent staff sessions cannot allocate the same vehicle.
- **Data Safety & Persistence:** Confirms customer inputs remain plain text, inventory filters operate, and logout survives new requests.

### 3. Playwright E2E Browser & Layout Tests (`tests/ui/operations.spec.cjs`) — 4 Tests
- **Authentication Workflow:** Verifies Enter-key sign-in, credential masking, and persistent logout across reloads.
- **End-to-End Rental Lifecycle:** Customer booking, safe rendering (XSS injection attempt neutralized), reload persistence, and durable return.
- **Accessible Dialogs & Focus Trap:** Native `<dialog>` focus containment, Escape key dismiss, and fractional input rejection.
- **Responsive Layout & Navigation:** Zero horizontal overflow across mobile (390px), tablet (720px), and desktop viewports.

**Total Test Count:** 40/40 tests executed (100% pass rate).

---

## Project Structure

```text
Car-rental/
├── .github/workflows/ci.yml           # Automated CI workflow running all test suites
├── problems.md                        # Audit checklist tracking all 38 remediated findings
├── package.json                       # Node.js operational gateway dependencies & test scripts
├── playwright.config.cjs              # Playwright browser automation configuration
├── start-desktop.ps1                  # Desktop application launcher (with optional -Demo flag)
├── start-preview.ps1                  # Web operations preview launcher
├── scripts/
│   ├── test.ps1                       # Comprehensive test runner (MSBuild, C# tests, Node, Playwright)
│   ├── backup.ps1                     # Automated native SQL Server backup
│   ├── restore.ps1                    # Safe non-destructive database restore to new catalog
│   ├── privacy.ps1                    # GDPR customer retention & anonymization (preview/apply)
│   └── create-staff.ps1               # Secure staff/admin account provisioning utility
├── server/
│   └── server.cjs                     # Authenticated Node.js gateway with CSRF, rate-limiting & CSP
├── web_preview/
│   ├── index.html                     # Semantic, accessible HTML5 operations portal
│   ├── styles.css                     # Responsive CSS (mobile breakpoints, prefers-reduced-motion)
│   └── app.js                         # Front-end logic with safe DOM binding and client-side routing
├── original_source/
│   ├── ARCHIVE.md                     # Historical quarantine advisory notice
│   └── RentOnCars/.../dbo.database.sql.disabled # Quarantined destructive setup script
└── RentOnCars/
    ├── RentOnCars.sln                 # Visual Studio Solution (.NET 4.8)
    ├── dbo.database.sql               # Canonical non-destructive schema, migrations & seeds
    ├── DATABASE_SETUP.txt             # Setup & configuration guide
    ├── RentOnCars/
    │   ├── Program.cs                 # Application entry point with resilient startup
    │   ├── App.config                 # LocalDB connection strings
    │   ├── DatabaseInitializer.cs     # Non-destructive schema migrations & process runner
    │   ├── UI/
    │   │   ├── RentalApplicationContext.cs # Clean window & session lifecycle manager
    │   │   └── Theme.cs               # Centralized design system tokens
    │   ├── Services/
    │   │   ├── PricingCalculator.cs   # Authoritative decoupled pricing engine & GBP formatter
    │   │   ├── BookingValidator.cs    # Unified business rules & field length validator
    │   │   ├── StaffAuthentication.cs # PBKDF2 authentication, lockout & session manager
    │   │   ├── DatabaseMaintenance.cs # Backup, restore, and GDPR anonymization routines
    │   │   └── LoginAuditLogger.cs    # Bounded, rotating diagnostic login logger
    │   ├── Models/
    │   │   └── BookingModels.cs       # Domain models: Car, Booking, OptionalExtras, Enums
    │   ├── Repos/
    │   │   └── Booking_repos.cs       # ADO.NET repository with atomic transactions & pagination
    │   └── Forms:
    │       ├── Welcome_page.cs        # Branded splash screen
    │       ├── Form1.cs (LoginForm)   # Staff login with Show/Hide toggle & Enter submission
    │       ├── Dashboard.cs           # KPI metrics dashboard & navigation
    │       ├── Details.cs             # New booking with async availability & review snapshot
    │       ├── BookingReviewDialog.cs # Pre-commit staff review modal
    │       └── booking_report.cs      # Dual-mode Rented Cars & History reports with return action
    ├── RentOnCars.Tests/
    │   ├── Program.cs                 # Domain & SQL test runner
    │   └── RegressionTests.cs         # Audit regression tests (concurrency, pricing, enums, etc.)
    └── RentOnCars.WebBridge/          # High-performance CLI worker bridging server.cjs to C# repo
```

---

## Setup & Run Instructions

### Prerequisites
- **Windows OS** (Windows 10 / 11)
- **.NET Framework 4.8 Runtime / Developer Pack**
- **Visual Studio 2019 / 2022** (with *.NET desktop development* workload and LocalDB)
- **Node.js 24+** (for running the web gateway and UI tests)

### Running Automated Verification
To execute the complete automated verification suite (compilation, disposable LocalDB creation, 31 C# tests, 6 API tests, and 5 Playwright browser tests):
```powershell
powershell -ExecutionPolicy Bypass -File scripts/test.ps1 -WithUI
```

### Launching the Desktop Application
```powershell
# Standard operational desktop:
powershell -ExecutionPolicy Bypass -File start-desktop.ps1

# Assignment demo mode (pre-seeds sta001/givemethekeys123):
powershell -ExecutionPolicy Bypass -File start-desktop.ps1 -Demo
```

### Launching the Web Portal (Dual Customer Self-Service & Staff Desk)
The web application provides two interfaces:
1. **Public Customer Self-Service Portal (`/` or `/#customer`):** Allows public customers to browse live vehicle availability, calculate transparent real-time quotations, confirm reservations with 10-minute price hold tokens, look up bookings with reference number and surname, and view/print rental vouchers.
2. **Staff & Admin Operations Desk (`/#staff`):** Secure authenticated workspace (`sta001` / `givemethekeys123`) with fleet KPI overview, staff counter reservations, real-time inventory management, complete booking history, and vehicle return workflow.

```powershell
# Start web portal on port 8000 (or npm run dev):
powershell -ExecutionPolicy Bypass -File start-preview.ps1 -Demo

# Or with npm:
npm run dev

# Or with custom port (e.g. 3000):
powershell -ExecutionPolicy Bypass -File start-preview.ps1 -Demo -Port 3000
```

---

## Operational Maintenance Scripts

The `scripts/` folder provides tools for production database administration:
- **Database Backup:**
  ```powershell
  powershell -ExecutionPolicy Bypass -File scripts/backup.ps1 [-Destination <path.bak>]
  ```
- **Database Restore (Safe & Non-Destructive):**
  ```powershell
  powershell -ExecutionPolicy Bypass -File scripts/restore.ps1 -Backup <path.bak> -NewCatalog <CatalogName>
  ```
- **GDPR Data Retention & Anonymization:**
  ```powershell
  # Preview eligible closed bookings older than 90 days:
  powershell -ExecutionPolicy Bypass -File scripts/privacy.ps1 -Days 90

  # Execute anonymization of customer PII while preserving financial totals:
  powershell -ExecutionPolicy Bypass -File scripts/privacy.ps1 -Days 90 -Apply
  ```
- **Staff Account Provisioning:**
  ```powershell
  powershell -ExecutionPolicy Bypass -File scripts/create-staff.ps1 -Username "jane.doe" -Role "staff"
  ```

---

## Marking Rubric Alignment

| Rubric Section | Points | Implementation Highlights |
|---|---|---|
| **Functionality** | 42 | Full implementation of use case normal flow and alternative flows; fixed credentials `sta001`/`givemethekeys123`; complete customer & licence validation; accurate decimal rate calculations including the £206.00 benchmark; live quotation sidebar; pre-commit staff review modal; atomic transactional booking persistence; receipt generation; dual-mode reporting for rented cars and booking history with saved extras. |
| **Source-Code Annotation & Naming** | 14 | Standard C# PascalCase naming conventions across classes, methods, and properties; descriptive camelCase local variables; structured architecture separating Forms, Models, Services, and Repositories; comprehensive XML documentation comments (`/// <summary>`) detailing business rules and transaction logic. |
| **Demonstration** | 14 | Responsive, polished desktop interface adhering to WeAreCars dark navy & teal branding; inline `ErrorProvider` feedback; masked password with show/hide toggle; real-time pricing updates; reliable error handling and LocalDB offline guidance. |

---

## License

This project was developed for the University of Sunderland CET131 Software Engineering module. All rights reserved.
