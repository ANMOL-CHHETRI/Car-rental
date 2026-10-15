# Problems and remediation checklist

This file tracks all 38 findings from the 14 October 2026 audit. Each item retains its original evidence; completion is recorded below with implementation and validation details.

## Delivery requirements

- Professional, restrained interface with readable tables, native controls, responsive layouts, and accessible dialogs.
- Browser and desktop workflows use the same authoritative C# pricing and SQL rental inventory.
- Authenticated saves, immutable review snapshots, correct reports, durable returns, and recoverable operations.

---

## Findings

### W01 — The preview has no durable or shared backend

- [x] **High · Web · Architecture gap**

Bookings and rental flags exist only in JavaScript arrays. Reloading loses data; separate pages can allocate WAC-008 with the same BK-1001 reference. Nothing reaches the C# repository or SQL database.

**Original evidence:** Save a booking, reload, or save from two independent page contexts. Both contexts assigned WAC-008/BK-1001; reload returned history to zero.

**Required correction:** Use a real authenticated API with database persistence and server-generated IDs; clearly label the current page as a demo until integrated.

**Original locations:** `web_preview/index.html:1600`, `web_preview/index.html:1614`, `web_preview/index.html:2019`

**Resolution:** Implemented `RentOnCars.WebBridge` worker and Node.js operational gateway `server/server.cjs`. All web mutations route via JSON-RPC to the authoritative C# `BookingRepository` and SQL Server database. Server assigns authoritative sequential booking IDs (`WAC-BK-XXXXX`). In demo mode (`--demo`), the interface is explicitly tagged with demo badges and notices. Verified via `tests/server.test.cjs` ("Reviewed snapshot, durable booking, exact benchmark") and Playwright UI tests ("Booking, safe customer rendering, reload persistence and durable return").

---

### W02 — Authentication starts enabled and reload reverses logout

- [x] **High · Web · Confirmed defect**

The dashboard is visible and isStaffAuthenticated starts true. Credentials are prefilled and disclosed. Logout only changes page memory, so a reload grants access again.

**Original evidence:** The probe observed initial=true, logout=false, and a fresh page=true.

**Required correction:** Start unauthenticated and enforce authorization on the server. Remove credential prefills and use a genuine session for any deployed version.

**Original locations:** `web_preview/index.html:1037`, `web_preview/index.html:1584`, `web_preview/index.html:1616`, `web_preview/index.html:2178`

**Resolution:** Initial page load now defaults strictly to unauthenticated state with `#appShell` hidden and `#loginView` displayed. Pre-filled credential inputs and client-side auth defaults were completely removed. Server issues cryptographically random session tokens stored in `HttpOnly; SameSite=Strict` cookies with 20-minute idle timeouts. Logout endpoint destroys the server session and clears cookies. Verified via Playwright test `tests/ui/operations.spec.cjs:16` ("Sign-in requires credentials, supports Enter and logout remains enforced after reload").

---

### W03 — Customer input can inject HTML and executable Inspect code

- [x] **High · Web · Confirmed defect**

History interpolates names into innerHTML and names/addresses into an inline JavaScript attribute. A 43-character HTML payload passes validation; an address payload executed a harmless marker in the isolated VM. Ordinary O'Neil or King's Road also causes an Inspect SyntaxError.

**Original evidence:** Save O'Neil / 17 King's Road and compile the generated handler; it fails. A synthetic address ending the alert string executed auditInjected=1 in the VM. Real browser execution was not tested.

**Required correction:** Render user values with textContent and attach event listeners using structured record data. Never interpolate customer text into HTML or JavaScript.

**Original locations:** `web_preview/index.html:2118`, `web_preview/index.html:2121`, `web_preview/index.html:2127`

**Resolution:** Replaced all `innerHTML` and inline string-interpolated JavaScript event handlers with safe DOM creation using `textContent` and native `addEventListener`. Names with apostrophes (e.g. `O'Neil`, `King's Road`) render reliably without script errors. Verified via `tests/server.test.cjs` ("Records remain text data") and Playwright UI test `tests/ui/operations.spec.cjs:22` (asserts `<img src=x onerror=...>` renders strictly as plain text, creates 0 img tags, and `window.injected` is undefined).

---

### W04 — Save and return functions bypass authorization and validation

- [x] **High · Web · Confirmed defect**

Only screen navigation checks authentication. commitBookingNow does not check login, rerun validation, or require a reviewed booking. A synthetic logged-out call saved empty customer fields, an invalid licence selection, and 99 days.

**Original evidence:** Set invalid fields, log out, invoke commitBookingNow: one record was saved for 99 days at £3,758.

**Required correction:** Validate an immutable request at the final save boundary and require server authorization for save and return actions.

**Original locations:** `web_preview/index.html:1752`, `web_preview/index.html:1952`, `web_preview/index.html:1988`, `web_preview/index.html:2087`

**Resolution:** Server validates authentication session and CSRF token on all mutation endpoints (`/api/quotes`, `/api/bookings`, `/api/bookings/:id/return`). Direct API calls without a session receive HTTP 401/403. Input parameters are strictly validated on the server against domain bounds (duration 1..28, age 18..120, licence mandatory, defined enums). Verified via `tests/server.test.cjs` ("Unauthenticated data access, hostile origins and missing CSRF are rejected" and "Server rejects fractional, out-of-range, invalid licence and unknown enum values").

---

### W05 — The saved booking is not the reviewed snapshot

- [x] **Medium · Web · Confirmed defect**

The review displays current fields, but save reads the editable page again and chooses another currently available car. If underlying fields or allocation change, the saved record can differ from what staff approved.

**Original evidence:** Review a 3-day £206 booking, change the underlying days to 28, then save: £1,131 was recorded. The change was injected by the audit probe; keyboard access behind modals is also unguarded.

**Required correction:** Capture the validated booking and allocated car when opening review; save that snapshot and require a new review if it changes.

**Original locations:** `web_preview/index.html:1952`, `web_preview/index.html:1963`, `web_preview/index.html:1988`, `web_preview/index.html:2000`

**Resolution:** When the staff clicks "Review booking", `/api/quotes` creates a server-held immutable quotation snapshot tied to a single-use token expiring in 5 minutes. The booking confirmation endpoint `/api/bookings` requires this quote token and commits that exact snapshot. Background edits on the form do not alter the saved quote. Verified via `tests/server.test.cjs` ("Reviewed snapshot, durable booking, exact benchmark and duplicate-request protection").

---

### W06 — Numeric inputs are silently truncated and incompletely validated

- [x] **Medium · Web · Confirmed defect**

parseInt accepts 1.9 as 1 and 1e2 as 1, so duration differs from the entered number. The maximum age is not enforced in JavaScript. Invalid days still produce quotes, and unknown pricing selections silently use zero surcharges.

**Original evidence:** Both 1.9 and 1e2 passed runValidation. The first saved as 1 day. Unknown category/fuel returned £75 for 3 days instead of an error.

**Required correction:** Use Number, Number.isInteger, explicit bounds, and input validity checks. Reject unknown categories/fuels and invalid estimates.

**Original locations:** `web_preview/index.html:1784`, `web_preview/index.html:1803`, `web_preview/index.html:1878`, `web_preview/index.html:1939`

**Resolution:** Switched parsing from `parseInt` to `Number` with `Number.isInteger()` checks. Fractional numbers (e.g. `1.9`) and exponential notation (e.g. `1e2`) are rejected. Duration strictly checked for `1 <= days <= 28`, age for `18 <= age <= 120`. Unknown vehicle categories and fuel types are explicitly rejected. Verified via `tests/server.test.cjs` ("Server rejects fractional, out-of-range, invalid licence and unknown enum values") and Playwright UI test `tests/ui/operations.spec.cjs:36`.

---

### W07 — The saved web record omits age, licence and a precise timestamp

- [x] **Medium · Web · Confirmed defect**

Age and licence are used on screen but discarded by bookingRec. time stores only a locale-formatted date, preventing reliable chronological auditing of multiple bookings on one day.

**Original evidence:** The saved record keys contain neither age nor licence; time was '14 Oct 2026'.

**Required correction:** Persist all validated customer and rental fields, actor ID, and an unambiguous timestamp.

**Original locations:** `web_preview/index.html:1958`, `web_preview/index.html:2005`, `web_preview/index.html:2007`

**Resolution:** The C# `Booking` model, database schema, and web API now persist `CustomerAge`, `HasValidLicense`, `StaffUsername`, `StartsAt`, `DueAt`, and high-precision UTC timestamps (`DATETIME2` / ISO 8601 strings) for `CreatedAt`. Verified via `RentOnCars.Tests/RegressionTests.cs:82` (`AuthoritativePricingAndSnapshot`) and `tests/server.test.cjs`.

---

### W08 — System health and security claims are fabricated

- [x] **Medium · Web · Confirmed defect**

The page claims LocalDB connectivity, row locking, persistent transaction logging and live telemetry without implementing those integrations. The receipt claims an atomic database update even though only an array changed.

**Original evidence:** Inspect the System & Security Status panel and receipt against the JavaScript implementation: there is no fetch/API/database/logging implementation.

**Required correction:** Report actual server health and capabilities, or explicitly mark simulated data and demo operations.

**Original locations:** `web_preview/index.html:1044`, `web_preview/index.html:1165`, `web_preview/index.html:1168`, `web_preview/index.html:1171`, `web_preview/index.html:1547`

**Resolution:** Removed fabricated telemetry and security widgets. Added genuine `/api/health` polling that reports actual SQL Server database connectivity, active catalog name, and operational mode (`staff` vs `demo`). The topbar status indicator dynamically updates to `online` or `offline` based on real server health. Verified via `tests/server.test.cjs:18`.

---

### W09 — Vehicle and fuel selectors cannot be operated by keyboard

- [x] **Medium · Web · Accessibility defect**

Clickable div elements have no native button/radio semantics, tabindex, keyboard handling or selected-state accessibility attributes. The brand shortcut has the same issue.

**Original evidence:** Category cards and fuel chips rely solely on onclick. Static inspection found no keyboard event handlers.

**Required correction:** Use native radio groups/buttons with labels, visible focus, and accessible selection state.

**Original locations:** `web_preview/index.html:992`, `web_preview/index.html:1249`, `web_preview/index.html:1325`

**Resolution:** Replaced clickable div cards with native HTML `<select>` elements and native checkboxes with associated `<label>` tags. Added visible focus rings via `:focus-visible` in `styles.css`. All form controls can be fully traversed and operated via Tab and arrow keys. Verified via Playwright accessibility and keyboard test `tests/ui/operations.spec.cjs:36`.

---

### W10 — Modals lack accessible dialog behavior and login Enter submission

- [x] **Medium · Web · Accessibility defect**

Modals do not set role=dialog/aria-modal, move or trap focus, disable background controls, support Escape, or restore focus. Login has no form submission or keyboard listener.

**Original evidence:** Static inspection found zero dialog roles, aria-modal attributes, native forms, or keyboard handlers; close buttons and the password icon also lack descriptive labels.

**Required correction:** Implement accessible dialog focus management and keyboard behavior; use a login form and submit button, and associate field errors with inputs.

**Original locations:** `web_preview/index.html:1500`, `web_preview/index.html:1567`, `web_preview/index.html:1585`, `web_preview/index.html:1592`

**Resolution:** Converted modal overlays to native HTML5 `<dialog>` elements with `aria-labelledby` and `aria-describedby`. Implemented focus trap while open, Escape key support to dismiss, and focus restoration to the triggering control. Login form wrapped in `<form>` supporting standard Enter-key submission. Verified via Playwright UI test `tests/ui/operations.spec.cjs:36` (Tab focus cycling and Escape key behavior verified).

---

### W11 — Small screens and zoom have no responsive layout strategy

- [x] **Medium · Web · Layout risk**

There are no media queries. A fixed-height, unwrapped header and fixed multi-column grids remain at narrow widths; body hides horizontal overflow. The 280px fleet search and modal header/body/footer heights increase clipping risk.

**Original evidence:** Source has zero @media rules, fixed four-column grids and overflow-x:hidden. Actual phone/zoom screenshots could not be checked in this session.

**Required correction:** Add responsive breakpoints/wrapping, single-column forms, adaptive navigation, and a height-bounded scrollable modal. Verify phone and 200% zoom views.

**Original locations:** `web_preview/index.html:56`, `web_preview/index.html:81`, `web_preview/index.html:433`, `web_preview/index.html:547`, `web_preview/index.html:631`, `web_preview/index.html:967`, `web_preview/index.html:1438`

**Resolution:** Implemented responsive CSS with breakpoints at 1100px, 850px, and 520px in `web_preview/styles.css`. Implemented mobile bottom/top navigation, single-column field layouts, adaptive tables, and height-bounded scrollable dialogs. Verified via Playwright test `tests/ui/operations.spec.cjs:43` checking mobile viewport (390x844) and zoom/narrow view (720x500) with `scrollWidth <= innerWidth`.

---

### W12 — Continuous animation has no reduced-motion option

- [x] **Low · Web · Accessibility gap**

Multiple elements animate indefinitely; there is no prefers-reduced-motion handling or pause control. The decorative motion can distract staff and cause discomfort. GPU/performance cost was not benchmarked.

**Original evidence:** Static inspection found multiple infinite animations and no reduced-motion support.

**Required correction:** Honor reduced-motion preferences and allow decorative animations to be paused.

**Original locations:** `web_preview/index.html:121`, `web_preview/index.html:340`, `web_preview/index.html:379`, `web_preview/index.html:421`

**Resolution:** Removed all distracting infinite canvas/CSS animations in favor of a clean, professional design system. Added `@media (prefers-reduced-motion: reduce)` in `web_preview/styles.css` forcing `animation: none !important`, `transition: none !important`, and `scroll-behavior: auto !important`.

---

### W13 — Saving and logout retain customer data and stale form state

- [x] **Medium · Web · Workflow defect**

commitBookingNow does not clear the customer's fields or refresh allocation/pricing immediately. logoutStaff leaves inputs and history in memory. The audit observed the rented WAC-008 still displayed as allocated and Review still enabled behind the receipt.

**Original evidence:** Save a synthetic booking; the first name remains Alice and the allocation contains rented WAC-008. Logout changes only screen visibility and auth flag.

**Required correction:** Reset the completed form and refresh allocation; clear sensitive session state on logout and deliberately retain drafts only for failed saves.

**Original locations:** `web_preview/index.html:2019`, `web_preview/index.html:2032`, `web_preview/index.html:2178`

**Resolution:** `bookingForm.reset()` and form state resets now execute upon successful save, and allocation/inventory data are immediately re-queried. Form state is deliberately preserved only if a save error occurs so staff can retry. Logout purges all memory caches, quotes, and form fields. Verified via Playwright UI tests `tests/ui/operations.spec.cjs:16,22`.

---

### W14 — Reports and navigation are limited

- [x] **Low · Web · Usability gap**

Rented Cars renders the entire fleet, not only rented cars. Fleet search ignores fuel; web history has no search/sort/pagination or printable voucher. Navigation has no URL/history state, so screens cannot be deep-linked or restored with browser Back.

**Original evidence:** Compare the report title to drawFleetTable's filter; inspect history controls and showScreen.

**Required correction:** Add a clear All/Rented/Available filter, searchable history, pagination, export/print, and meaningful routes if the preview becomes an application.

**Original locations:** `web_preview/index.html:1431`, `web_preview/index.html:1465`, `web_preview/index.html:1752`, `web_preview/index.html:2057`, `web_preview/index.html:2127`

**Resolution:** Fleet inventory now includes All / Available / On Hire status tabs, full text search covering registration, category and fuel, and pagination. Booking history includes server-side search, sorting (Newest, Customer, Rental Total), and pagination. Added hash-based client routing (`#overview`, `#booking`, `#fleet`, `#history`) and print confirmation dialog with `@media print` CSS. Verified via `tests/ui/operations.spec.cjs:22,29,48`.

---

### D01 — Filtered or sorted history shows the wrong customer details

- [x] **High · Desktop · Confirmed defect**

Grid rows are filtered/sorted, but the detail panel uses rowIndex into the unfiltered cachedBookings list. This can show another customer's address, vehicle and extras for the selected booking.

**Original evidence:** Both filtered and sorted grid rows displayed WAC-BK-00002 while the detail panel displayed WAC-BK-00003.

**Required correction:** Bind each row to its BookingId/object and resolve details from the selected row, independent of filtering and sorting.

**Original locations:** `RentOnCars/RentOnCars/booking_report.cs:169`, `RentOnCars/RentOnCars/booking_report.cs:214`, `RentOnCars/RentOnCars/booking_report.cs:224`, `RentOnCars/RentOnCars/booking_report.cs:235`

**Resolution:** `booking_report.DisplayBookingDetails` now extracts the booking reference (`WAC-BK-XXXXX`) directly from the selected `DataGridView` row cells and looks up the booking by reference: `cachedBookings.FirstOrDefault(x => $"WAC-BK-{x.BookingId:D5}" == reference)`. Verified via automated regression test `RentOnCars.Tests/RegressionTests.cs:51` (`SelectedBookingIdentity`).

---

### D02 — Reviewed price can disagree with the saved total

- [x] **High · Desktop · Confirmed defect**

CalculateEstimate always uses £25, while Booking and SaveBooking use SelectedCar.DailyRate. The review shows the estimate, and the receipt combines that estimate's items with the booking's total.

**Original evidence:** For SUV/Hybrid, 3 days, both extras, and a £40 rate: review estimate £206; booking total £251.

**Required correction:** Use one authoritative pricing breakdown based on the allocated database car, and use that same snapshot for review, persistence and receipt.

**Original locations:** `RentOnCars/RentOnCars/Services/PricingCalculator.cs:83`, `RentOnCars/RentOnCars/Details.cs:119`, `RentOnCars/RentOnCars/BookingReviewDialog.cs:66`, `RentOnCars/RentOnCars/Details.cs:371`

**Resolution:** `Details.cs` creates a single authoritative `PricingBreakdown` passing `allocatedCar.DailyRate` to `CalculateEstimate`. That exact breakdown and `ReviewedTotal` are stored in `Booking`. `BookingReviewDialog`, `SaveBooking`, and the receipt all use this single snapshot. `SaveBooking` verifies inside the SQL transaction that the vehicle rate and reviewed total match. Verified via regression test `RentOnCars.Tests/RegressionTests.cs:34` (`PricingRateAndCulture`) and `AuthoritativePricingAndSnapshot`.

---

### D03 — Closing the dashboard can leave a hidden running application

- [x] **High · Desktop · Confirmed defect**

The welcome form is Application.Run's main form and is hidden. The login form is also hidden when dashboard opens. Dashboard close only exits when almost no other forms exist; the two hidden forms prevent that. Logout creates new login instances rather than disposing/reusing earlier hidden ones.

**Original evidence:** Trace Welcome_page.Hide, LoginForm.Hide and both dashboard FormClosed conditions. Native window lifecycle interaction was not performed.

**Required correction:** Manage form lifetime with an ApplicationContext or explicit ownership; close/dispose hidden forms and make normal dashboard exit terminate the application.

**Original locations:** `RentOnCars/RentOnCars/Program.cs:54`, `RentOnCars/RentOnCars/Welcome_page.cs:42`, `RentOnCars/RentOnCars/Form1.cs:131`, `RentOnCars/RentOnCars/Form1.cs:142`, `RentOnCars/RentOnCars/Dashboard.cs:172`, `RentOnCars/RentOnCars/Dashboard.cs:193`

**Resolution:** Implemented `RentalApplicationContext : ApplicationContext` in `RentOnCars/RentOnCars/UI/RentalApplicationContext.cs`. Main forms are swapped with explicit close and disposal of previous forms (`previous.Close(); previous.Dispose()`). Closing the dashboard automatically terminates the `ApplicationContext` message loop, guaranteeing no orphan background processes.

---

### D04 — Database operations block the UI, including every customer keystroke

- [x] **High · Desktop · Responsiveness defect**

OptionChanged is attached to text fields, age, licence, days and extras. Each event recalculates and synchronously queries SQL availability. Login/dashboard/report/save/startup also perform blocking database work; offline connection waits can repeatedly freeze the UI.

**Original evidence:** firstnamebox.TextChanged -> OptionChanged -> UpdateLivePricingAndAvailability -> GetAvailableCar -> con.Open. Connection timeout is 30 seconds. Latency was not benchmarked.

**Required correction:** Separate local validation/pricing from availability; query only when relevant selections change, debounce, use async I/O and cancellation, and keep startup/retry work off the UI thread.

**Original locations:** `RentOnCars/RentOnCars/Details.Designer.cs:750`, `RentOnCars/RentOnCars/Details.cs:149`, `RentOnCars/RentOnCars/Details.cs:169`, `RentOnCars/RentOnCars/Details.cs:410`, `RentOnCars/RentOnCars/App.config:5`

**Resolution:** Decoupled local pricing calculation from database access: `PricingCalculator.CalculateEstimate` operates completely offline without SQL. Availability queries are fired only when category or fuel selections change, executed asynchronously off the UI thread via `Task.Run()` with monotonic version tokens to ignore stale results. Keystrokes no longer trigger SQL connections.

---

### D05 — Age/licence verification is pre-assumed and receipts claim payment

- [x] **Medium · Desktop · Verification gap**

Desktop and web forms default to an adult age and a valid licence. Reviews describe the licence as Verified, and receipts label the calculated amount Paid, although no licence verification evidence or payment record/status is stored.

**Original evidence:** Desktop defaults age=25 and licence=true; web defaults age=26/licence=yes. No payment entity or settlement flow exists.

**Required correction:** Require explicit staff confirmation for age/licence, use accurate 'confirmed by staff' wording, and label amounts as rental total/due unless payment is actually recorded.

**Original locations:** `RentOnCars/RentOnCars/Details.cs:48`, `RentOnCars/RentOnCars/Details.cs:64`, `RentOnCars/RentOnCars/BookingReviewDialog.cs:34`, `RentOnCars/RentOnCars/Details.cs:376`, `web_preview/index.html:1223`, `web_preview/index.html:1554`

**Resolution:** Form defaults updated: `ageupdown.Value = 0` and `yesradiobutton.Checked = false` on desktop; web inputs start blank and unchecked. Staff must explicitly verify customer age and check the licence confirmation box. Review labels updated to "Licence confirmed by staff" and receipts display "RENTAL TOTAL DUE" rather than claiming completed payment. Verified via regression test `RentOnCars.Tests/RegressionTests.cs:65` (`LicenceIsExplicit`).

---

### D06 — Domain and service validators disagree and allow invalid car state

- [x] **Medium · Desktop · Confirmed defect**

Booking.ValidateBooking does not check name/address limits or IsRented. BookingValidator treats any non-null car as available without checking rental state, category/fuel match, or defined enums. A negative daily rate can also reach ComputeTotal.

**Original evidence:** Probes accepted a rented car with a 51-character name, a mismatched rented car, and category enum 999. ComputeTotal at -£25 returned -£75.

**Required correction:** Centralize domain validation and enforce availability, enum validity, metadata consistency and positive rates at the persistence boundary.

**Original locations:** `RentOnCars/RentOnCars/Models/BookingModels.cs:370`, `RentOnCars/RentOnCars/Services/BookingValidator.cs:128`, `RentOnCars/RentOnCars/Services/BookingValidator.cs:140`, `RentOnCars/RentOnCars/Services/PricingCalculator.cs:146`

**Resolution:** Centralized validation in `BookingValidator.Validate`. It now checks: `availableCar == null || availableCar.IsRented || availableCar.CarType != carType.Value || availableCar.FuelType != fuelType.Value || availableCar.DailyRate <= 0m`, string length limits, age bounds (18..120), licence=true, and valid enum values. `Booking.ValidateBooking()` delegates directly to `BookingValidator.Validate()`. `PricingCalculator.ComputeTotal` throws `ArgumentOutOfRangeException` for non-positive daily rates. Verified via regression test `RentOnCars.Tests/RegressionTests.cs:25` (`InvalidVehicleState`).

---

### D07 — Strict enum parsers accept undefined numeric enum values

- [x] **Medium · Desktop · Confirmed defect**

Enum.TryParse accepts numeric strings such as 999 without Enum.IsDefined. The methods therefore return undefined categories/fuels despite claiming strict validation; later surcharge calculation can throw.

**Original evidence:** ParseCarType("999") and ParseFuelType("999") both returned enum integer 999.

**Required correction:** Require Enum.IsDefined or an explicit whitelist after parsing; cover numeric and combined enum strings in tests.

**Original locations:** `RentOnCars/RentOnCars/Models/BookingModels.cs:155`, `RentOnCars/RentOnCars/Models/BookingModels.cs:186`

**Resolution:** `VehicleEnumExtensions.ParseCarType` and `ParseFuelType` now require that the string start with an alphabetic character (`char.IsLetter(normalized[0])`) and that `Enum.IsDefined(typeof(...), parsed)` is true. Numeric inputs like `"999"` or `"-1"` immediately throw `FormatException`. Verified via regression test `RentOnCars.Tests/RegressionTests.cs:23` (`NumericEnums`).

---

### D08 — RentedCarsList identifies objects instead of vehicles

- [x] **Low · Desktop · Confirmed defect**

Contains uses reference identity, so two objects for the same CarId are both added. Clear empties the list without marking cars available. DisplayRentedCars is an empty method.

**Original evidence:** Two objects with CarId=1 yielded Count=2; Clear left the original car IsRented=true.

**Required correction:** Use stable vehicle identity, define Clear's state behavior, and implement or remove the empty display method.

**Original locations:** `RentOnCars/RentOnCars/Models/BookingModels.cs:437`, `RentOnCars/RentOnCars/Models/BookingModels.cs:459`, `RentOnCars/RentOnCars/Models/BookingModels.cs:475`

**Resolution:** `RentedCarsList` now matches vehicles using `SameVehicle(Car a, Car b)` comparing `CarId` (or registration number if ID <= 0). `AddRentedCar` ignores duplicates. `Clear()` marks all cars as available (`car.MarkAsAvailable()`) before clearing. `DisplayRentedCars()` formats and outputs summary info to `Debug.WriteLine`. Verified via regression test `RentOnCars.Tests/RegressionTests.cs:44` (`VehicleIdentity`).

---

### D09 — GBP formatting depends on the computer's current culture

- [x] **Low · Desktop · Localization defect**

Financial strings prefix £ but use the current culture's decimal separator. The existing benchmark test assumes a period, making behavior and tests locale-dependent.

**Original evidence:** Under fr-FR, FormattedGrandTotal produced £206,00 instead of £206.00.

**Required correction:** Format GBP consistently with an explicit intended culture, and keep persisted values culture-independent.

**Original locations:** `RentOnCars/RentOnCars/Services/PricingCalculator.cs:40`, `RentOnCars/RentOnCars.Tests/Program.cs:329`

**Resolution:** Centralized currency formatting in `PricingCalculator.FormatMoney(decimal amount)` using explicit `CultureInfo.GetCultureInfo("en-GB")`. Verified via regression test `RentOnCars.Tests/RegressionTests.cs:39` (forces `fr-FR` culture and confirms formatting remains `£251.00`).

---

### B01 — SaveBooking trusts caller-supplied rate, category and fuel

- [x] **High · Backend / DB · Confirmed defect**

The repository calculates TotalCost before locking the car. The lock SELECT retrieves authoritative metadata but only checks whether a row exists. Booking insertion still uses the caller's SelectedCar values, allowing stale or altered metadata to be recorded.

**Original evidence:** In an isolated SQL database, a real CityCar/Petrol at £25 was passed as SUV/Hybrid at £1. SaveBooking recorded SUV and £134 rather than the real vehicle's £111 with both extras.

**Required correction:** Read and validate authoritative car metadata inside the transaction, calculate pricing from that row, and reject or re-review changed terms.

**Original locations:** `RentOnCars/RentOnCars/Repos/Booking_repos.cs:339`, `RentOnCars/RentOnCars/Repos/Booking_repos.cs:360`, `RentOnCars/RentOnCars/Repos/Booking_repos.cs:370`, `RentOnCars/RentOnCars/Repos/Booking_repos.cs:402`

**Resolution:** `BookingRepository.SaveBooking` reads the locked vehicle row via `SELECT ... FROM dbo.Cars WITH(UPDLOCK,ROWLOCK,HOLDLOCK)` inside the transaction. It validates that the database `CarType`, `FuelType`, and `DailyRate` match the booking, re-calculates pricing using the database rate, and throws `InvalidOperationException` if spoofed or altered. Verified via regression test `RentOnCars.Tests/RegressionTests.cs:82` (`AuthoritativePricingAndSnapshot`).

---

### B02 — There is no durable rental return or booking lifecycle

- [x] **High · Backend / DB · Workflow gap**

The desktop repository only changes cars from available to rented. No return/cancel/complete operation exists, so normal bookings eventually exhaust the seeded fleet. Schema has no hire start/end, actual return date or booking status; web Return just flips a local boolean without closing a booking.

**Original evidence:** Search repository operations: no check-in method. The web return probe kept history without status or returnedAt.

**Required correction:** Add transactional return/cancellation with actor/time, booking status and rental dates; update availability from active rentals and record returns against the specific booking.

**Original locations:** `RentOnCars/RentOnCars/Repos/Booking_repos.cs:427`, `RentOnCars/dbo.database.sql:45`, `web_preview/index.html:2087`

**Resolution:** Added `ReturnBooking(bookingId, actor)` in `BookingRepository`. It atomically locks the car, updates booking status to `'Returned'`, sets `ReturnedAt = SYSUTCDATETIME()`, records `ReturnedBy`, resets vehicle `IsRented = 0`, and writes an audit event to `dbo.AuditEvents`. Return buttons added to desktop report and web preview. Verified via regression test `RentOnCars.Tests/RegressionTests.cs:93` and Playwright UI test `tests/ui/operations.spec.cjs:32`.

---

### B03 — Historical item prices are not preserved or faithfully retrieved

- [x] **Medium · Backend / DB · Confirmed defect**

History joins the current car DailyRate. Optional extras retrieval loads only ExtraName and reconstructs prices using current constants, ignoring saved DailyPrice and NumberOfDays. Stored TotalCost survives, but the historical breakdown is not reproducible.

**Original evidence:** After changing the audit car rate to £99 and saved extra rates to £123, history read base=£99 and extras/day=£12 while retaining total £134.

**Required correction:** Store a booking-time pricing snapshot and read the persisted item rates/quantities rather than current fleet rates/constants.

**Original locations:** `RentOnCars/RentOnCars/Repos/Booking_repos.cs:219`, `RentOnCars/RentOnCars/Repos/Booking_repos.cs:251`, `RentOnCars/RentOnCars/Repos/Booking_repos.cs:287`, `RentOnCars/dbo.database.sql:79`

**Resolution:** Schema added `DailyBaseRate`, `VehicleSurcharge`, `FuelSurcharge`, and `PricingSnapshotVersion` columns to `dbo.Bookings`. `dbo.BookingOptionalExtras` persists `DailyPrice` and `NumberOfDays`. `GetDetails` loads these persisted values directly. Modifying the fleet car's current rate does not change historical booking totals or item breakdowns. Verified via regression test `RentOnCars.Tests/RegressionTests.cs:88-90`.

---

### B04 — Database constraints permit invalid booking and extra records

- [x] **Medium · Backend / DB · Integrity gap**

Bookings allows blank strings, licence=false, unknown category/fuel and zero totals. Extras has no unique booking/name constraint or consistency rule tying days to the booking, permitting duplicates and inconsistent quantities. Repository validation covers some, but alternate writers can violate invariants.

**Original evidence:** Isolated direct SQL inserts accepted blank fields, invalid licence, unknown selections, zero total and a duplicate 28-day extra for a 3-day booking.

**Required correction:** Enforce critical invariants in schema and write procedures: nonblank fields, defined enums, licence requirement, unique extras and consistent quantities.

**Original locations:** `RentOnCars/dbo.database.sql:50`, `RentOnCars/dbo.database.sql:55`, `RentOnCars/dbo.database.sql:58`, `RentOnCars/dbo.database.sql:74`

**Resolution:** Added check constraints `CK_Bookings_Required` (non-empty names/address, licence=1, age 18..120, positive total, enum whitelist), `CK_Bookings_Status`, `CK_Extras_Name`, unique index `UX_Extras_BookingName`, and trigger `TR_Extras_Duration` enforcing that extra rental days match the booking duration. Verified via regression test `RentOnCars.Tests/RegressionTests.cs:101` (`SchemaConstraintsAndAudit`).

---

### B05 — Initialization creates missing tables but does not migrate or validate existing schema

- [x] **Medium · Backend / DB · Maintenance gap**

IF OBJECT_ID IS NULL skips existing tables entirely. There is no schema version, ALTER migration or compatibility check. An old/incompatible schema can be declared initialized; failures later appear as database-offline errors.

**Original evidence:** Review CreateTables and Initialize: success depends on statements running, not a schema compatibility check. No actual old-schema upgrade was tested.

**Required correction:** Add versioned migrations and schema compatibility checks; distinguish connection failure from incompatible schema.

**Original locations:** `RentOnCars/RentOnCars/DatabaseInitializer.cs:62`, `RentOnCars/RentOnCars/DatabaseInitializer.cs:70`, `RentOnCars/RentOnCars/DatabaseInitializer.cs:237`, `RentOnCars/RentOnCars/DatabaseInitializer.cs:254`

**Resolution:** Added `dbo.SchemaVersions` table and versioned migrations (v1 and v2) to `RentOnCars/dbo.database.sql`. `DatabaseInitializer.ApplyMigrations` verifies schema version, acquires an exclusive application lock (`sp_getapplock`) during migration, and halts with descriptive guidance if a newer incompatible schema is detected. Verified via `DatabaseInitializer.Initialize()` and automated SQL test runs.

---

### B06 — A custom database catalog is ignored during database creation

- [x] **Medium · Backend / DB · Configuration defect**

EnsureDatabaseExists switches to master but tests/creates the hardcoded RentOnCarsDB rather than the configured InitialCatalog. It also manages MSSQLLocalDB even for a custom server. This breaks an otherwise supported custom-catalog configuration and produces misleading guidance.

**Original evidence:** Set Initial Catalog to another database: initialization still creates RentOnCarsDB and then attempts the configured database. This path was reviewed, not executed against a user database.

**Required correction:** Use the configured server/catalog consistently; only manage LocalDB when it is the selected provider/instance.

**Original locations:** `RentOnCars/RentOnCars/DatabaseInitializer.cs:28`, `RentOnCars/RentOnCars/DatabaseInitializer.cs:57`, `RentOnCars/RentOnCars/DatabaseInitializer.cs:211`, `RentOnCars/RentOnCars/DatabaseInitializer.cs:224`, `RentOnCars/DATABASE_SETUP.txt:25`

**Resolution:** `DatabaseInitializer.EnsureDatabaseExists` now reads `configured.InitialCatalog` directly from the active connection string and provisions that specific catalog. LocalDB process management is invoked only when the connection DataSource begins with `(localdb)\`. Verified during automated test execution using disposable unique LocalDB catalogs (`WeAreCarsTest_...`).

---

### B07 — The LocalDB process timeout does not bound execution

- [x] **Medium · Backend / DB · Reliability defect**

ReadToEnd is called on redirected streams before WaitForExit(15000), so a hung process can block before the timeout is reached. The wait result and exit code are ignored; timed-out children are not terminated. Sequential stream reading also risks blocking.

**Original evidence:** RunSqlLocalDb reads stdout and stderr synchronously before its timed wait. A hung child was not deliberately created.

**Required correction:** Read both streams concurrently, enforce a real process deadline, inspect exit codes and handle timed-out children explicitly.

**Original locations:** `RentOnCars/RentOnCars/DatabaseInitializer.cs:192`, `RentOnCars/RentOnCars/DatabaseInitializer.cs:197`, `RentOnCars/RentOnCars/DatabaseInitializer.cs:199`

**Resolution:** `RunSqlLocalDb` now reads standard output and standard error concurrently via `ReadToEndAsync()`. `process.WaitForExit(15000)` enforces a strict 15-second timeout; if exceeded, the child process is terminated with `process.Kill()` and a `TimeoutException` is thrown. Non-zero exit codes throw `InvalidOperationException`.

---

### B08 — History reads extras and bookings without a consistent snapshot

- [x] **Medium · Backend / DB · Consistency risk**

GetDetails first loads all extras, then all bookings using a separate statement with no read transaction. A booking committed between those reads can appear without its saved extras until the next refresh.

**Original evidence:** A commit after extrasSql completes but before the bookings SELECT is the problematic ordering. The timing window was not stress-tested.

**Required correction:** Retrieve bookings and extras in one query or use an appropriate consistent read transaction/snapshot.

**Original locations:** `RentOnCars/RentOnCars/Repos/Booking_repos.cs:218`, `RentOnCars/RentOnCars/Repos/Booking_repos.cs:245`, `RentOnCars/RentOnCars/Repos/Booking_repos.cs:264`

**Resolution:** `BookingRepository.GetDetails` now retrieves bookings, vehicle registrations, and optional extras within a single atomic query using SQL Server `OUTER APPLY`. This guarantees an atomic snapshot across bookings and extras without timing windows. Verified via `RentOnCars.Tests/RegressionTests.cs:82`.

---

### B09 — Reports load all records and keep stale data on refresh errors

- [x] **Medium · Backend / DB · Scalability gap**

GetDetails scans all bookings and all extras; filtering is in memory and recreates grids. The schema has no explicit supporting indexes for availability/filter/FK lookups. A refresh error displays an error label while leaving the old grid/details in place. Ten demo cars conceal the scale issue.

**Original evidence:** Inspect unbounded SELECTs and report readData catch. Performance at large volumes was not measured.

**Required correction:** Add database-side filtering/pagination and suitable indexes; label retained data as stale or clear it on load failure.

**Original locations:** `RentOnCars/RentOnCars/Repos/Booking_repos.cs:219`, `RentOnCars/RentOnCars/Repos/Booking_repos.cs:246`, `RentOnCars/RentOnCars/booking_report.cs:82`, `RentOnCars/RentOnCars/booking_report.cs:158`, `RentOnCars/dbo.database.sql:45`

**Resolution:** Implemented server-side pagination (`OFFSET ... FETCH NEXT`) and search filtering in `GetFleet` and `GetDetails`. Added supporting indexes `IX_Cars_Availability`, `IX_Bookings_History`, `UX_Bookings_ActiveCar`. In `booking_report.cs`, `readData` clears the detail panel and old records on failure, displaying an explicit error state.

---

### R01 — The shared fixed credential is a demo gate, not production staff security

- [x] **Medium · Security / Operations · Production limitation**

The required assignment credential is embedded in source/documentation. There are no individual accounts, roles, lockout, session expiry or persisted staff actor for bookings. Forms/repository also have no authenticated-user context. This is assignment-aligned, but inadequate for an operational multi-user service.

**Original evidence:** Form1 defines public credential constants; Booking and schema have no staff actor field. This is not a claim of a reachable remote endpoint.

**Required correction:** Keep the required credential for the assignment/demo only; production needs individual identities, server-enforced permissions and actor-linked records.

**Original locations:** `RentOnCars/RentOnCars/Form1.cs:21`, `RentOnCars/RentOnCars/Form1.cs:53`, `RentOnCars/RentOnCars/Models/BookingModels.cs:333`, `RentOnCars/dbo.database.sql:45`

**Resolution:** Implemented `StaffAuthentication` with individual accounts in `dbo.StaffAccounts`. Passwords hashed with PBKDF2 (SHA-256, 600,000 iterations, 32-byte salt, constant-time compare). Accounts lock out after 5 consecutive failed attempts. Bookings record `StaffUsername`. Demo credential `sta001`/`givemethekeys123` is enabled only in demo mode (`WEARECARS_DEMO=1`). Script `scripts/create-staff.ps1` allows provisioning individual staff/admin accounts. Verified via regression test `RentOnCars.Tests/RegressionTests.cs:113` (`StaffAuthenticationAndLockout`).

---

### R02 — Audit logging is incomplete and can fail silently

- [x] **Medium · Security / Operations · Reliability gap**

Only login events are logged; bookings/returns lack transaction audit entries or staff attribution. LoginAuditLogger writes beside the executable, ignores all write failures, keeps an unbounded in-memory list and has no rotation/tamper protection. A read-only installation can silently lose disk audit records.

**Original evidence:** Inspect File.AppendAllText and empty catch, plus SaveBooking without audit calls. Disk permission failures were not injected.

**Required correction:** Write structured actor-linked audit events to a supported writable store, surface failures, and define retention/rotation.

**Original locations:** `RentOnCars/RentOnCars/Services/LoginAuditLogger.cs:30`, `RentOnCars/RentOnCars/Services/LoginAuditLogger.cs:73`, `RentOnCars/RentOnCars/Services/LoginAuditLogger.cs:81`, `RentOnCars/RentOnCars/Services/LoginAuditLogger.cs:84`, `RentOnCars/RentOnCars/Repos/Booking_repos.cs:445`

**Resolution:** Authoritative audit events (booking created, vehicle returned, login attempts) are committed transactionally to `dbo.AuditEvents` with actor, action, booking ID, and timestamp. Local diagnostic log in `LoginAuditLogger` now targets `%LOCALAPPDATA%\WeAreCars\logs`, caps in-memory entries at 1,000, rotates files when exceeding 1 MB (retains 4 backups), sanitizes inputs against log injection, and surfaces errors via `LastWriteError` and `Trace.TraceError`. Verified via regression test `RentOnCars.Tests/RegressionTests.cs:101`.

---

### R03 — The 20 passing tests omit the workflows where serious bugs occur

- [x] **Medium · Tests / Delivery · Coverage gap**

Existing tests cover credentials, local validation, pricing, enums, an in-memory collection, logging and executable discovery. They never test SaveBooking/GetDetails with SQL, concurrent saves, filtered/sorted report selection, frontend injection, persistence, keyboard flows or window lifetime. No CI workflow is tracked.

**Original evidence:** The Debug solution built and all 20 original tests passed despite the reproduced data/security/report failures. New audit probes demonstrate the missing coverage.

**Required correction:** Add targeted regression tests for the verified failures and isolated database integration tests; run them in CI with required environment setup.

**Original locations:** `RentOnCars/RentOnCars.Tests/Program.cs:27`, `RentOnCars/RentOnCars.Tests/Program.cs:47`, `RentOnCars/RentOnCars.Tests/RentOnCars.Tests.csproj:10`

**Resolution:** Expanded the test suite to 40 automated tests:
1. 31 desktop/domain/SQL tests in `RentOnCars.Tests` (including `RegressionTests.cs` testing SQL concurrency, authoritative pricing, schema constraints, return lifecycle, culture formatting, enum safety, and lockout).
2. 5 server API tests in `tests/server.test.cjs`.
3. 4 Playwright browser UI & accessibility tests in `tests/ui/operations.spec.cjs`.
Automated CI workflow added in `.github/workflows/ci.yml`. Verified via `scripts/test.ps1 -WithUI` (100% pass rate).

---

### R04 — The archived SQL setup script deletes all rental data

- [x] **High · Tests / Delivery · Archive hazard**

original_source retains a setup script that drops extras, bookings and cars in RentOnCarsDB. It is not called by the active solution, but executing the wrong setup file would erase the operational tables.

**Original evidence:** The archived script contains DROP TABLE at lines 20, 24 and 28. It was not executed.

**Required correction:** Clearly quarantine historical source and remove destructive setup instructions from any runnable/distributed path; add prominent warnings and safe scripts.

**Original locations:** `original_source/RentOnCars/RentOnCars/dbo.database.sql:19`, `original_source/RentOnCars/RentOnCars/dbo.database.sql:23`, `original_source/RentOnCars/RentOnCars/dbo.database.sql:27`

**Resolution:** Renamed the legacy destructive script to `original_source/RentOnCars/RentOnCars/dbo.database.sql.disabled` to prevent accidental execution. Added `original_source/ARCHIVE.md` providing prominent warnings and directing users to the supported, non-destructive canonical schema `RentOnCars/dbo.database.sql`.

---

### R05 — Business rules and source artifacts are duplicated and documentation overstates coverage

- [x] **Low · Tests / Delivery · Maintenance gap**

Pricing is repeated in JavaScript, PricingCalculator, model methods and OptionalExtras; schema/seeds are duplicated in code and SQL. Original source and an enhanced ZIP create multiple plausible entry points. README's named test list does not match the actual runner and calls table creation 'schema migrations'. It does not explain running or limiting the web preview.

**Original evidence:** Compare tariffRules, ComputeTotal, Booking.CalculateTotalCost, SQL setup and README test table to Program.cs.

**Required correction:** Centralize authoritative rules/migrations, document active entry points and preview limitations, and keep test claims generated from actual runs.

**Original locations:** `web_preview/index.html:1623`, `RentOnCars/RentOnCars/Services/PricingCalculator.cs:57`, `RentOnCars/RentOnCars/Models/BookingModels.cs:256`, `RentOnCars/RentOnCars/Models/BookingModels.cs:394`, `README.md:114`, `README.md:147`

**Resolution:** Centralized business rules: `PricingCalculator` is the single source of truth for rates, formulas, and extras constants (`OptionalExtras` and `Booking.CalculateTotalCost` delegate to it); `BookingValidator` is the single source for domain constraints (`Booking.ValidateBooking` delegates to it). `RentOnCars/dbo.database.sql` is embedded directly into the executable via csproj `<LogicalName>RentOnCars.DatabaseSchema.sql</LogicalName>`, eliminating SQL schema duplication. `README.md` updated to accurately document active entry points, test suite coverage (40 tests), and demo vs staff operations.

---

### R06 — There is no documented deployment, backup/recovery or customer-data lifecycle

- [x] **Medium · Security / Operations · Production limitation**

Default per-user LocalDB creates separate inventories across machines/Windows users. The repo has no backup/restore procedure, packaged deployment, production environment configuration, customer-data retention/deletion policy or application-level access model for records. Windows/.NET Framework are required for the desktop.

**Original evidence:** Review App.config, schema and setup guide. OS/database protections may exist outside this repo; they were not assessed.

**Required correction:** Define a shared deployment architecture if required, least-privilege database access, tested backups/restores, and explicit customer-data retention and access rules.

**Original locations:** `RentOnCars/RentOnCars/App.config:5`, `RentOnCars/DATABASE_SETUP.txt:6`, `RentOnCars/RentOnCars/RentOnCars.csproj:12`, `RentOnCars/dbo.database.sql:50`

**Resolution:** Implemented `DatabaseMaintenance` in `RentOnCars/RentOnCars/Services/DatabaseMaintenance.cs` supporting:
1. Transactionally consistent `BACKUP DATABASE` to `.bak` files.
2. Verified `RESTORE DATABASE` to a newly specified catalog without overwriting existing data.
3. GDPR customer data retention & anonymization (`AnonymizeClosedRecords`) with preview and apply modes.
Created reusable automation PowerShell scripts: `scripts/backup.ps1`, `scripts/restore.ps1`, `scripts/privacy.ps1`, and `scripts/create-staff.ps1`. Verified via regression test `RentOnCars.Tests/RegressionTests.cs:121` (`BackupRestoreAndRetention`).
