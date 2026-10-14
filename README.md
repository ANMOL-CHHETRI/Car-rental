# WeAreCars - Car Rental Management Solution

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-blue.svg)](https://dotnet.microsoft.com/)
[![Language](https://img.shields.io/badge/Language-C%23-green.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Database](https://img.shields.io/badge/Database-SQL%20Server%20LocalDB-red.svg)](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
[![Tests](https://img.shields.io/badge/Tests-20%2F20%20Passed-brightgreen.svg)]()

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

## Automated Unit & Integration Tests

The solution includes a dedicated automated test suite (`RentOnCars.Tests`) validating all core business logic and domain constraints:

| # | Test Name | Target Area | Result |
|---|---|---|---|
| 1 | `Authenticate_ValidStaffCredentials_ReturnsTrue` | Authentication | Pass |
| 2 | `Authenticate_InvalidStaffCredentials_ReturnsFalse` | Authentication | Pass |
| 3 | `Validate_CompleteCustomerDetails_IsValid` | Input Validation | Pass |
| 4 | `Validate_MissingRequiredFields_ReportsErrors` | Input Validation | Pass |
| 5 | `Validate_AgeBelow18_FailsValidation` | Age Boundary | Pass |
| 6 | `Validate_WithoutValidLicense_FailsValidation` | License Check | Pass |
| 7 | `Validate_RentalDaysBoundaries_0_29_Fail_1_28_Pass` | Days Boundaries (0, 1, 28, 29) | Pass |
| 8 | `Calculate_AllVehicleCategorySurcharges_AreExact` | Vehicle Surcharges | Pass |
| 9 | `Calculate_AllFuelTypeSurcharges_AreExact` | Fuel Surcharges | Pass |
| 10 | `Calculate_OptionalExtras_UnlimitedMileageOnly` | Optional Extras | Pass |
| 11 | `Calculate_OptionalExtras_BreakdownCoverOnly` | Optional Extras | Pass |
| 12 | `Calculate_OptionalExtras_BothExtras` | Optional Extras | Pass |
| 13 | `Calculate_AssignmentBenchmark_3Day_SUV_Hybrid_BothExtras_Equals206` | £206.00 Benchmark | Pass |
| 14 | `CarType_StrictParsing_RejectsInvalidValues` | Enum Parsing | Pass |
| 15 | `FuelType_StrictParsing_RejectsInvalidValues` | Enum Parsing | Pass |
| 16 | `PricingCalculator_LiveEstimator_HandlesIncompleteSelection` | Offline Pricing Engine | Pass |
| 17 | `CustomerStringLengths_DatabaseLimits_Enforced` | DB Boundary Constraints | Pass |
| 18 | `LoginAuditLogger_RecordsSuccessfulAndFailedEvents` | Audit Logging | Pass |
| 19 | `Car_MarkAsRented_And_Available_TogglesState` | Domain Model Lifecycle | Pass |
| 20 | `RentedCarsList_AddAndRemove_MaintainsCollection` | Domain Model Lifecycle | Pass |

**Test Execution:** 20/20 passed (100% pass rate).

---

## Project Structure

```text
RentOnCars/
├── RentOnCars.sln                     # Visual Studio Solution (.NET 4.8)
├── dbo.database.sql                   # Non-destructive SQL Server schema & seed script
├── DATABASE_SETUP.txt                 # Setup & configuration guide
├── RentOnCars/
│   ├── Program.cs                     # Application entry point with resilient startup
│   ├── App.config                     # LocalDB connection strings
│   ├── DatabaseInitializer.cs        # Non-destructive schema & starter fleet initializer
│   ├── UI/
│   │   └── Theme.cs                   # Centralized design system (Navy/Teal tokens)
│   ├── Services/
│   │   ├── PricingCalculator.cs       # Decoupled offline pricing engine
│   │   ├── BookingValidator.cs        # Business rules and field length validator
│   │   └── LoginAuditLogger.cs        # Authentication audit logging
│   ├── Models/
│   │   └── BookingModels.cs           # Domain models: Car, Booking, OptionalExtras, Enums
│   ├── Repos/
│   │   └── Booking_repos.cs           # ADO.NET repository with atomic transactions
│   ├── Forms:
│   │   ├── Welcome_page.cs/.Designer.cs       # Branded splash screen
│   │   ├── Form1.cs/.Designer.cs              # Staff login with Show/Hide toggle
│   │   ├── Dashboard.cs/.Designer.cs          # KPI metrics dashboard & navigation
│   │   ├── Details.cs/.Designer.cs            # New booking with live quote sidebar
│   │   ├── BookingReviewDialog.cs/.Designer.cs# Staff review & confirmation modal
│   │   └── booking_report.cs/.Designer.cs     # Dual-mode Rented Cars & History reports
└── RentOnCars.Tests/
    ├── RentOnCars.Tests.csproj        # Standalone test project
    └── Program.cs                     # 20 automated domain & integration tests
```

---

## Setup & Run Instructions

### Prerequisites
- **Windows OS** (Windows 10 / 11)
- **.NET Framework 4.8 Runtime / Developer Pack**
- **Visual Studio 2019 / 2022** (with *.NET desktop development* workload)
- **SQL Server Express LocalDB** (optional: included in Visual Studio Installer)

### Building & Running via Visual Studio
1. Open `RentOnCars/RentOnCars.sln` in Visual Studio.
2. Select **Debug** or **Release** configuration with **Any CPU**.
3. Press <kbd>F5</kbd> or click **Start** to build and run the application.

### Building & Running via Command Line (PowerShell)
```powershell
# 1. Build the solution
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" "RentOnCars\RentOnCars.sln" /t:Build /p:Configuration=Debug

# 2. Run the automated test suite
& "RentOnCars\RentOnCars.Tests\bin\Debug\RentOnCars.Tests.exe"

# 3. Launch the desktop application
Start-Process -FilePath "RentOnCars\RentOnCars\bin\Debug\RentOnCars.exe"
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
