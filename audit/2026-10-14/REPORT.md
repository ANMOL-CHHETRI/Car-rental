# Car rental inspection — 14 October 2026

**38 findings: 11 High, 22 Medium, 5 Low.** The browser preview is a simulation with no durable/shared backend. The desktop application builds, and its existing 20 tests pass, but focused probes reproduced security, pricing, validation and reporting failures. The current project is suitable for a constrained assignment/demo and needs the changes below before operational use.

High means a security, data-loss, pricing, customer-record or core-workflow issue. Medium means incorrect behavior, integrity/reliability risk, accessibility failure, or a substantial production gap. Low means narrower usability, maintenance or presentation issues. Production limitations and archived-file hazards are labeled separately from active defects.

## Scope and evidence

- Inspected the active web page, Windows Forms code and designers, domain/services, ADO.NET repository, SQL setup/initializer, project/build configuration, original tests, README/setup documentation, and archived source.
- Built RentOnCars.sln in Debug successfully; all 20 existing tests passed.
- Executed the original web JavaScript inside an isolated Node VM with a minimal DOM stub. This verifies script behavior and generated markup/handlers; it does not emulate real browser layout, focus, native input constraints or screen-reader behavior.
- Ran extra C# probes and real SQL integration checks against a dedicated temporary LocalDB instance using synthetic records. The audit catalog and instance were removed afterward. No existing user database was used.
- The concurrency safeguard passed: of two simultaneous saves for the same car, one succeeded and one was rejected. Parameterized SQL and transactional booking/extras/vehicle updates are present; this audit does not claim SQL injection or a missing desktop double-booking lock.
- Browser controls are unavailable in this session. Mobile/zoom layout and native window lifetime findings are based on code; no screenshot, assistive-technology or end-to-end visible-window verification was performed.
- Audit results describe the inspected repository, not deployed systems or external infrastructure. This is a broad audit, not proof that no additional defects exist.

## Recommended order

1. Address W01–W04 before treating the web page as a working rental system: durable backend, authorization, safe rendering and save-boundary validation.
2. Fix D01, D02 and B01: correct selected-record mapping, a single reviewed pricing snapshot, and pricing from the locked database car.
3. Implement B02 and D03/D04: durable returns, clean application exit, and non-blocking database work.
4. Add regression/integration coverage, then improve schema/history integrity, accessible controls, responsive layouts and operational recovery.

## Findings

### 1. W01 — The preview has no durable or shared backend

**High · Web · Architecture gap · Evidence: Runtime probe**

Bookings and rental flags exist only in JavaScript arrays. Reloading loses data; separate pages can allocate WAC-008 with the same BK-1001 reference. Nothing reaches the C# repository or SQL database.

**Evidence/reproduction:** Save a booking, reload, or save from two independent page contexts. Both contexts assigned WAC-008/BK-1001; reload returned history to zero.

**Suggested correction:** Use a real authenticated API with database persistence and server-generated IDs; clearly label the current page as a demo until integrated.

**Source:** [web_preview/index.html:1600](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1600), [web_preview/index.html:1614](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1614), [web_preview/index.html:2019](C:/Users/beepl/dev/Car-rental/web_preview/index.html:2019)

### 2. W02 — Authentication starts enabled and reload reverses logout

**High · Web · Confirmed defect · Evidence: Runtime probe**

The dashboard is visible and isStaffAuthenticated starts true. Credentials are prefilled and disclosed. Logout only changes page memory, so a reload grants access again.

**Evidence/reproduction:** The probe observed initial=true, logout=false, and a fresh page=true.

**Suggested correction:** Start unauthenticated and enforce authorization on the server. Remove credential prefills and use a genuine session for any deployed version.

**Source:** [web_preview/index.html:1037](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1037), [web_preview/index.html:1584](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1584), [web_preview/index.html:1616](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1616), [web_preview/index.html:2178](C:/Users/beepl/dev/Car-rental/web_preview/index.html:2178)

### 3. W03 — Customer input can inject HTML and executable Inspect code

**High · Web · Confirmed defect · Evidence: Runtime probe + source**

History interpolates names into innerHTML and names/addresses into an inline JavaScript attribute. A 43-character HTML payload passes validation; an address payload executed a harmless marker in the isolated VM. Ordinary O'Neil or King's Road also causes an Inspect SyntaxError.

**Evidence/reproduction:** Save O'Neil / 17 King's Road and compile the generated handler; it fails. A synthetic address ending the alert string executed auditInjected=1 in the VM. Real browser execution was not tested.

**Suggested correction:** Render user values with textContent and attach event listeners using structured record data. Never interpolate customer text into HTML or JavaScript.

**Source:** [web_preview/index.html:2118](C:/Users/beepl/dev/Car-rental/web_preview/index.html:2118), [web_preview/index.html:2121](C:/Users/beepl/dev/Car-rental/web_preview/index.html:2121), [web_preview/index.html:2127](C:/Users/beepl/dev/Car-rental/web_preview/index.html:2127)

### 4. W04 — Save and return functions bypass authorization and validation

**High · Web · Confirmed defect · Evidence: Runtime probe**

Only screen navigation checks authentication. commitBookingNow does not check login, rerun validation, or require a reviewed booking. A synthetic logged-out call saved empty customer fields, an invalid licence selection, and 99 days.

**Evidence/reproduction:** Set invalid fields, log out, invoke commitBookingNow: one record was saved for 99 days at £3,758.

**Suggested correction:** Validate an immutable request at the final save boundary and require server authorization for save and return actions.

**Source:** [web_preview/index.html:1752](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1752), [web_preview/index.html:1952](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1952), [web_preview/index.html:1988](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1988), [web_preview/index.html:2087](C:/Users/beepl/dev/Car-rental/web_preview/index.html:2087)

### 5. W05 — The saved booking is not the reviewed snapshot

**Medium · Web · Confirmed defect · Evidence: Runtime probe**

The review displays current fields, but save reads the editable page again and chooses another currently available car. If underlying fields or allocation change, the saved record can differ from what staff approved.

**Evidence/reproduction:** Review a 3-day £206 booking, change the underlying days to 28, then save: £1,131 was recorded. The change was injected by the audit probe; keyboard access behind modals is also unguarded.

**Suggested correction:** Capture the validated booking and allocated car when opening review; save that snapshot and require a new review if it changes.

**Source:** [web_preview/index.html:1952](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1952), [web_preview/index.html:1963](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1963), [web_preview/index.html:1988](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1988), [web_preview/index.html:2000](C:/Users/beepl/dev/Car-rental/web_preview/index.html:2000)

### 6. W06 — Numeric inputs are silently truncated and incompletely validated

**Medium · Web · Confirmed defect · Evidence: Runtime probe**

parseInt accepts 1.9 as 1 and 1e2 as 1, so duration differs from the entered number. The maximum age is not enforced in JavaScript. Invalid days still produce quotes, and unknown pricing selections silently use zero surcharges.

**Evidence/reproduction:** Both 1.9 and 1e2 passed runValidation. The first saved as 1 day. Unknown category/fuel returned £75 for 3 days instead of an error.

**Suggested correction:** Use Number, Number.isInteger, explicit bounds, and input validity checks. Reject unknown categories/fuels and invalid estimates.

**Source:** [web_preview/index.html:1784](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1784), [web_preview/index.html:1803](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1803), [web_preview/index.html:1878](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1878), [web_preview/index.html:1939](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1939)

### 7. W07 — The saved web record omits age, licence and a precise timestamp

**Medium · Web · Confirmed defect · Evidence: Runtime probe**

Age and licence are used on screen but discarded by bookingRec. time stores only a locale-formatted date, preventing reliable chronological auditing of multiple bookings on one day.

**Evidence/reproduction:** The saved record keys contain neither age nor licence; time was '14 Oct 2026'.

**Suggested correction:** Persist all validated customer and rental fields, actor ID, and an unambiguous timestamp.

**Source:** [web_preview/index.html:1958](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1958), [web_preview/index.html:2005](C:/Users/beepl/dev/Car-rental/web_preview/index.html:2005), [web_preview/index.html:2007](C:/Users/beepl/dev/Car-rental/web_preview/index.html:2007)

### 8. W08 — System health and security claims are fabricated

**Medium · Web · Confirmed defect · Evidence: Source**

The page claims LocalDB connectivity, row locking, persistent transaction logging and live telemetry without implementing those integrations. The receipt claims an atomic database update even though only an array changed.

**Evidence/reproduction:** Inspect the System & Security Status panel and receipt against the JavaScript implementation: there is no fetch/API/database/logging implementation.

**Suggested correction:** Report actual server health and capabilities, or explicitly mark simulated data and demo operations.

**Source:** [web_preview/index.html:1044](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1044), [web_preview/index.html:1165](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1165), [web_preview/index.html:1168](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1168), [web_preview/index.html:1171](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1171), [web_preview/index.html:1547](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1547)

### 9. W09 — Vehicle and fuel selectors cannot be operated by keyboard

**Medium · Web · Accessibility defect · Evidence: Source**

Clickable div elements have no native button/radio semantics, tabindex, keyboard handling or selected-state accessibility attributes. The brand shortcut has the same issue.

**Evidence/reproduction:** Category cards and fuel chips rely solely on onclick. Static inspection found no keyboard event handlers.

**Suggested correction:** Use native radio groups/buttons with labels, visible focus, and accessible selection state.

**Source:** [web_preview/index.html:992](C:/Users/beepl/dev/Car-rental/web_preview/index.html:992), [web_preview/index.html:1249](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1249), [web_preview/index.html:1325](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1325)

### 10. W10 — Modals lack accessible dialog behavior and login Enter submission

**Medium · Web · Accessibility defect · Evidence: Source**

Modals do not set role=dialog/aria-modal, move or trap focus, disable background controls, support Escape, or restore focus. Login has no form submission or keyboard listener.

**Evidence/reproduction:** Static inspection found zero dialog roles, aria-modal attributes, native forms, or keyboard handlers; close buttons and the password icon also lack descriptive labels.

**Suggested correction:** Implement accessible dialog focus management and keyboard behavior; use a login form and submit button, and associate field errors with inputs.

**Source:** [web_preview/index.html:1500](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1500), [web_preview/index.html:1567](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1567), [web_preview/index.html:1585](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1585), [web_preview/index.html:1592](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1592)

### 11. W11 — Small screens and zoom have no responsive layout strategy

**Medium · Web · Layout risk · Evidence: Source inference**

There are no media queries. A fixed-height, unwrapped header and fixed multi-column grids remain at narrow widths; body hides horizontal overflow. The 280px fleet search and modal header/body/footer heights increase clipping risk.

**Evidence/reproduction:** Source has zero @media rules, fixed four-column grids and overflow-x:hidden. Actual phone/zoom screenshots could not be checked in this session.

**Suggested correction:** Add responsive breakpoints/wrapping, single-column forms, adaptive navigation, and a height-bounded scrollable modal. Verify phone and 200% zoom views.

**Source:** [web_preview/index.html:56](C:/Users/beepl/dev/Car-rental/web_preview/index.html:56), [web_preview/index.html:81](C:/Users/beepl/dev/Car-rental/web_preview/index.html:81), [web_preview/index.html:433](C:/Users/beepl/dev/Car-rental/web_preview/index.html:433), [web_preview/index.html:547](C:/Users/beepl/dev/Car-rental/web_preview/index.html:547), [web_preview/index.html:631](C:/Users/beepl/dev/Car-rental/web_preview/index.html:631), [web_preview/index.html:967](C:/Users/beepl/dev/Car-rental/web_preview/index.html:967), [web_preview/index.html:1438](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1438)

### 12. W12 — Continuous animation has no reduced-motion option

**Low · Web · Accessibility gap · Evidence: Source**

Multiple elements animate indefinitely; there is no prefers-reduced-motion handling or pause control. The decorative motion can distract staff and cause discomfort. GPU/performance cost was not benchmarked.

**Evidence/reproduction:** Static inspection found multiple infinite animations and no reduced-motion support.

**Suggested correction:** Honor reduced-motion preferences and allow decorative animations to be paused.

**Source:** [web_preview/index.html:121](C:/Users/beepl/dev/Car-rental/web_preview/index.html:121), [web_preview/index.html:340](C:/Users/beepl/dev/Car-rental/web_preview/index.html:340), [web_preview/index.html:379](C:/Users/beepl/dev/Car-rental/web_preview/index.html:379), [web_preview/index.html:421](C:/Users/beepl/dev/Car-rental/web_preview/index.html:421)

### 13. W13 — Saving and logout retain customer data and stale form state

**Medium · Web · Workflow defect · Evidence: Runtime probe + source**

commitBookingNow does not clear the customer's fields or refresh allocation/pricing immediately. logoutStaff leaves inputs and history in memory. The audit observed the rented WAC-008 still displayed as allocated and Review still enabled behind the receipt.

**Evidence/reproduction:** Save a synthetic booking; the first name remains Alice and the allocation contains rented WAC-008. Logout changes only screen visibility and auth flag.

**Suggested correction:** Reset the completed form and refresh allocation; clear sensitive session state on logout and deliberately retain drafts only for failed saves.

**Source:** [web_preview/index.html:2019](C:/Users/beepl/dev/Car-rental/web_preview/index.html:2019), [web_preview/index.html:2032](C:/Users/beepl/dev/Car-rental/web_preview/index.html:2032), [web_preview/index.html:2178](C:/Users/beepl/dev/Car-rental/web_preview/index.html:2178)

### 14. W14 — Reports and navigation are limited

**Low · Web · Usability gap · Evidence: Source**

Rented Cars renders the entire fleet, not only rented cars. Fleet search ignores fuel; web history has no search/sort/pagination or printable voucher. Navigation has no URL/history state, so screens cannot be deep-linked or restored with browser Back.

**Evidence/reproduction:** Compare the report title to drawFleetTable's filter; inspect history controls and showScreen.

**Suggested correction:** Add a clear All/Rented/Available filter, searchable history, pagination, export/print, and meaningful routes if the preview becomes an application.

**Source:** [web_preview/index.html:1431](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1431), [web_preview/index.html:1465](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1465), [web_preview/index.html:1752](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1752), [web_preview/index.html:2057](C:/Users/beepl/dev/Car-rental/web_preview/index.html:2057), [web_preview/index.html:2127](C:/Users/beepl/dev/Car-rental/web_preview/index.html:2127)

### 15. D01 — Filtered or sorted history shows the wrong customer details

**High · Desktop · Confirmed defect · Evidence: Runtime probe**

Grid rows are filtered/sorted, but the detail panel uses rowIndex into the unfiltered cachedBookings list. This can show another customer's address, vehicle and extras for the selected booking.

**Evidence/reproduction:** Both filtered and sorted grid rows displayed WAC-BK-00002 while the detail panel displayed WAC-BK-00003.

**Suggested correction:** Bind each row to its BookingId/object and resolve details from the selected row, independent of filtering and sorting.

**Source:** [RentOnCars/RentOnCars/booking_report.cs:169](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/booking_report.cs:169), [RentOnCars/RentOnCars/booking_report.cs:214](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/booking_report.cs:214), [RentOnCars/RentOnCars/booking_report.cs:224](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/booking_report.cs:224), [RentOnCars/RentOnCars/booking_report.cs:235](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/booking_report.cs:235)

### 16. D02 — Reviewed price can disagree with the saved total

**High · Desktop · Confirmed defect · Evidence: Runtime probe + source**

CalculateEstimate always uses £25, while Booking and SaveBooking use SelectedCar.DailyRate. The review shows the estimate, and the receipt combines that estimate's items with the booking's total.

**Evidence/reproduction:** For SUV/Hybrid, 3 days, both extras, and a £40 rate: review estimate £206; booking total £251.

**Suggested correction:** Use one authoritative pricing breakdown based on the allocated database car, and use that same snapshot for review, persistence and receipt.

**Source:** [RentOnCars/RentOnCars/Services/PricingCalculator.cs:83](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Services/PricingCalculator.cs:83), [RentOnCars/RentOnCars/Details.cs:119](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Details.cs:119), [RentOnCars/RentOnCars/BookingReviewDialog.cs:66](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/BookingReviewDialog.cs:66), [RentOnCars/RentOnCars/Details.cs:371](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Details.cs:371)

### 17. D03 — Closing the dashboard can leave a hidden running application

**High · Desktop · Confirmed defect · Evidence: Source**

The welcome form is Application.Run's main form and is hidden. The login form is also hidden when dashboard opens. Dashboard close only exits when almost no other forms exist; the two hidden forms prevent that. Logout creates new login instances rather than disposing/reusing earlier hidden ones.

**Evidence/reproduction:** Trace Welcome_page.Hide, LoginForm.Hide and both dashboard FormClosed conditions. Native window lifecycle interaction was not performed.

**Suggested correction:** Manage form lifetime with an ApplicationContext or explicit ownership; close/dispose hidden forms and make normal dashboard exit terminate the application.

**Source:** [RentOnCars/RentOnCars/Program.cs:54](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Program.cs:54), [RentOnCars/RentOnCars/Welcome_page.cs:42](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Welcome_page.cs:42), [RentOnCars/RentOnCars/Form1.cs:131](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Form1.cs:131), [RentOnCars/RentOnCars/Form1.cs:142](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Form1.cs:142), [RentOnCars/RentOnCars/Dashboard.cs:172](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Dashboard.cs:172), [RentOnCars/RentOnCars/Dashboard.cs:193](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Dashboard.cs:193)

### 18. D04 — Database operations block the UI, including every customer keystroke

**High · Desktop · Responsiveness defect · Evidence: Source**

OptionChanged is attached to text fields, age, licence, days and extras. Each event recalculates and synchronously queries SQL availability. Login/dashboard/report/save/startup also perform blocking database work; offline connection waits can repeatedly freeze the UI.

**Evidence/reproduction:** firstnamebox.TextChanged -> OptionChanged -> UpdateLivePricingAndAvailability -> GetAvailableCar -> con.Open. Connection timeout is 30 seconds. Latency was not benchmarked.

**Suggested correction:** Separate local validation/pricing from availability; query only when relevant selections change, debounce, use async I/O and cancellation, and keep startup/retry work off the UI thread.

**Source:** [RentOnCars/RentOnCars/Details.Designer.cs:750](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Details.Designer.cs:750), [RentOnCars/RentOnCars/Details.cs:149](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Details.cs:149), [RentOnCars/RentOnCars/Details.cs:169](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Details.cs:169), [RentOnCars/RentOnCars/Details.cs:410](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Details.cs:410), [RentOnCars/RentOnCars/App.config:5](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/App.config:5)

### 19. D05 — Age/licence verification is pre-assumed and receipts claim payment

**Medium · Desktop · Verification gap · Evidence: Source + runtime probe**

Desktop and web forms default to an adult age and a valid licence. Reviews describe the licence as Verified, and receipts label the calculated amount Paid, although no licence verification evidence or payment record/status is stored.

**Evidence/reproduction:** Desktop defaults age=25 and licence=true; web defaults age=26/licence=yes. No payment entity or settlement flow exists.

**Suggested correction:** Require explicit staff confirmation for age/licence, use accurate 'confirmed by staff' wording, and label amounts as rental total/due unless payment is actually recorded.

**Source:** [RentOnCars/RentOnCars/Details.cs:48](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Details.cs:48), [RentOnCars/RentOnCars/Details.cs:64](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Details.cs:64), [RentOnCars/RentOnCars/BookingReviewDialog.cs:34](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/BookingReviewDialog.cs:34), [RentOnCars/RentOnCars/Details.cs:376](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Details.cs:376), [web_preview/index.html:1223](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1223), [web_preview/index.html:1554](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1554)

### 20. D06 — Domain and service validators disagree and allow invalid car state

**Medium · Desktop · Confirmed defect · Evidence: Runtime probe**

Booking.ValidateBooking does not check name/address limits or IsRented. BookingValidator treats any non-null car as available without checking rental state, category/fuel match, or defined enums. A negative daily rate can also reach ComputeTotal.

**Evidence/reproduction:** Probes accepted a rented car with a 51-character name, a mismatched rented car, and category enum 999. ComputeTotal at -£25 returned -£75.

**Suggested correction:** Centralize domain validation and enforce availability, enum validity, metadata consistency and positive rates at the persistence boundary.

**Source:** [RentOnCars/RentOnCars/Models/BookingModels.cs:370](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Models/BookingModels.cs:370), [RentOnCars/RentOnCars/Services/BookingValidator.cs:128](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Services/BookingValidator.cs:128), [RentOnCars/RentOnCars/Services/BookingValidator.cs:140](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Services/BookingValidator.cs:140), [RentOnCars/RentOnCars/Services/PricingCalculator.cs:146](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Services/PricingCalculator.cs:146)

### 21. D07 — Strict enum parsers accept undefined numeric enum values

**Medium · Desktop · Confirmed defect · Evidence: Runtime probe**

Enum.TryParse accepts numeric strings such as 999 without Enum.IsDefined. The methods therefore return undefined categories/fuels despite claiming strict validation; later surcharge calculation can throw.

**Evidence/reproduction:** ParseCarType("999") and ParseFuelType("999") both returned enum integer 999.

**Suggested correction:** Require Enum.IsDefined or an explicit whitelist after parsing; cover numeric and combined enum strings in tests.

**Source:** [RentOnCars/RentOnCars/Models/BookingModels.cs:155](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Models/BookingModels.cs:155), [RentOnCars/RentOnCars/Models/BookingModels.cs:186](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Models/BookingModels.cs:186)

### 22. D08 — RentedCarsList identifies objects instead of vehicles

**Low · Desktop · Confirmed defect · Evidence: Runtime probe**

Contains uses reference identity, so two objects for the same CarId are both added. Clear empties the list without marking cars available. DisplayRentedCars is an empty method.

**Evidence/reproduction:** Two objects with CarId=1 yielded Count=2; Clear left the original car IsRented=true.

**Suggested correction:** Use stable vehicle identity, define Clear's state behavior, and implement or remove the empty display method.

**Source:** [RentOnCars/RentOnCars/Models/BookingModels.cs:437](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Models/BookingModels.cs:437), [RentOnCars/RentOnCars/Models/BookingModels.cs:459](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Models/BookingModels.cs:459), [RentOnCars/RentOnCars/Models/BookingModels.cs:475](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Models/BookingModels.cs:475)

### 23. D09 — GBP formatting depends on the computer's current culture

**Low · Desktop · Localization defect · Evidence: Runtime probe**

Financial strings prefix £ but use the current culture's decimal separator. The existing benchmark test assumes a period, making behavior and tests locale-dependent.

**Evidence/reproduction:** Under fr-FR, FormattedGrandTotal produced £206,00 instead of £206.00.

**Suggested correction:** Format GBP consistently with an explicit intended culture, and keep persisted values culture-independent.

**Source:** [RentOnCars/RentOnCars/Services/PricingCalculator.cs:40](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Services/PricingCalculator.cs:40), [RentOnCars/RentOnCars.Tests/Program.cs:329](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars.Tests/Program.cs:329)

### 24. B01 — SaveBooking trusts caller-supplied rate, category and fuel

**High · Backend / DB · Confirmed defect · Evidence: SQL integration probe**

The repository calculates TotalCost before locking the car. The lock SELECT retrieves authoritative metadata but only checks whether a row exists. Booking insertion still uses the caller's SelectedCar values, allowing stale or altered metadata to be recorded.

**Evidence/reproduction:** In an isolated SQL database, a real CityCar/Petrol at £25 was passed as SUV/Hybrid at £1. SaveBooking recorded SUV and £134 rather than the real vehicle's £111 with both extras.

**Suggested correction:** Read and validate authoritative car metadata inside the transaction, calculate pricing from that row, and reject or re-review changed terms.

**Source:** [RentOnCars/RentOnCars/Repos/Booking_repos.cs:339](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Repos/Booking_repos.cs:339), [RentOnCars/RentOnCars/Repos/Booking_repos.cs:360](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Repos/Booking_repos.cs:360), [RentOnCars/RentOnCars/Repos/Booking_repos.cs:370](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Repos/Booking_repos.cs:370), [RentOnCars/RentOnCars/Repos/Booking_repos.cs:402](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Repos/Booking_repos.cs:402)

### 25. B02 — There is no durable rental return or booking lifecycle

**High · Backend / DB · Workflow gap · Evidence: Source + runtime probe**

The desktop repository only changes cars from available to rented. No return/cancel/complete operation exists, so normal bookings eventually exhaust the seeded fleet. Schema has no hire start/end, actual return date or booking status; web Return just flips a local boolean without closing a booking.

**Evidence/reproduction:** Search repository operations: no check-in method. The web return probe kept history without status or returnedAt.

**Suggested correction:** Add transactional return/cancellation with actor/time, booking status and rental dates; update availability from active rentals and record returns against the specific booking.

**Source:** [RentOnCars/RentOnCars/Repos/Booking_repos.cs:427](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Repos/Booking_repos.cs:427), [RentOnCars/dbo.database.sql:45](C:/Users/beepl/dev/Car-rental/RentOnCars/dbo.database.sql:45), [web_preview/index.html:2087](C:/Users/beepl/dev/Car-rental/web_preview/index.html:2087)

### 26. B03 — Historical item prices are not preserved or faithfully retrieved

**Medium · Backend / DB · Confirmed defect · Evidence: SQL integration probe**

History joins the current car DailyRate. Optional extras retrieval loads only ExtraName and reconstructs prices using current constants, ignoring saved DailyPrice and NumberOfDays. Stored TotalCost survives, but the historical breakdown is not reproducible.

**Evidence/reproduction:** After changing the audit car rate to £99 and saved extra rates to £123, history read base=£99 and extras/day=£12 while retaining total £134.

**Suggested correction:** Store a booking-time pricing snapshot and read the persisted item rates/quantities rather than current fleet rates/constants.

**Source:** [RentOnCars/RentOnCars/Repos/Booking_repos.cs:219](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Repos/Booking_repos.cs:219), [RentOnCars/RentOnCars/Repos/Booking_repos.cs:251](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Repos/Booking_repos.cs:251), [RentOnCars/RentOnCars/Repos/Booking_repos.cs:287](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Repos/Booking_repos.cs:287), [RentOnCars/dbo.database.sql:79](C:/Users/beepl/dev/Car-rental/RentOnCars/dbo.database.sql:79)

### 27. B04 — Database constraints permit invalid booking and extra records

**Medium · Backend / DB · Integrity gap · Evidence: SQL integration probe**

Bookings allows blank strings, licence=false, unknown category/fuel and zero totals. Extras has no unique booking/name constraint or consistency rule tying days to the booking, permitting duplicates and inconsistent quantities. Repository validation covers some, but alternate writers can violate invariants.

**Evidence/reproduction:** Isolated direct SQL inserts accepted blank fields, invalid licence, unknown selections, zero total and a duplicate 28-day extra for a 3-day booking.

**Suggested correction:** Enforce critical invariants in schema and write procedures: nonblank fields, defined enums, licence requirement, unique extras and consistent quantities.

**Source:** [RentOnCars/dbo.database.sql:50](C:/Users/beepl/dev/Car-rental/RentOnCars/dbo.database.sql:50), [RentOnCars/dbo.database.sql:55](C:/Users/beepl/dev/Car-rental/RentOnCars/dbo.database.sql:55), [RentOnCars/dbo.database.sql:58](C:/Users/beepl/dev/Car-rental/RentOnCars/dbo.database.sql:58), [RentOnCars/dbo.database.sql:74](C:/Users/beepl/dev/Car-rental/RentOnCars/dbo.database.sql:74)

### 28. B05 — Initialization creates missing tables but does not migrate or validate existing schema

**Medium · Backend / DB · Maintenance gap · Evidence: Source**

IF OBJECT_ID IS NULL skips existing tables entirely. There is no schema version, ALTER migration or compatibility check. An old/incompatible schema can be declared initialized; failures later appear as database-offline errors.

**Evidence/reproduction:** Review CreateTables and Initialize: success depends on statements running, not a schema compatibility check. No actual old-schema upgrade was tested.

**Suggested correction:** Add versioned migrations and schema compatibility checks; distinguish connection failure from incompatible schema.

**Source:** [RentOnCars/RentOnCars/DatabaseInitializer.cs:62](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/DatabaseInitializer.cs:62), [RentOnCars/RentOnCars/DatabaseInitializer.cs:70](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/DatabaseInitializer.cs:70), [RentOnCars/RentOnCars/DatabaseInitializer.cs:237](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/DatabaseInitializer.cs:237), [RentOnCars/RentOnCars/DatabaseInitializer.cs:254](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/DatabaseInitializer.cs:254)

### 29. B06 — A custom database catalog is ignored during database creation

**Medium · Backend / DB · Configuration defect · Evidence: Source**

EnsureDatabaseExists switches to master but tests/creates the hardcoded RentOnCarsDB rather than the configured InitialCatalog. It also manages MSSQLLocalDB even for a custom server. This breaks an otherwise supported custom-catalog configuration and produces misleading guidance.

**Evidence/reproduction:** Set Initial Catalog to another database: initialization still creates RentOnCarsDB and then attempts the configured database. This path was reviewed, not executed against a user database.

**Suggested correction:** Use the configured server/catalog consistently; only manage LocalDB when it is the selected provider/instance.

**Source:** [RentOnCars/RentOnCars/DatabaseInitializer.cs:28](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/DatabaseInitializer.cs:28), [RentOnCars/RentOnCars/DatabaseInitializer.cs:57](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/DatabaseInitializer.cs:57), [RentOnCars/RentOnCars/DatabaseInitializer.cs:211](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/DatabaseInitializer.cs:211), [RentOnCars/RentOnCars/DatabaseInitializer.cs:224](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/DatabaseInitializer.cs:224), [RentOnCars/DATABASE_SETUP.txt:25](C:/Users/beepl/dev/Car-rental/RentOnCars/DATABASE_SETUP.txt:25)

### 30. B07 — The LocalDB process timeout does not bound execution

**Medium · Backend / DB · Reliability defect · Evidence: Source**

ReadToEnd is called on redirected streams before WaitForExit(15000), so a hung process can block before the timeout is reached. The wait result and exit code are ignored; timed-out children are not terminated. Sequential stream reading also risks blocking.

**Evidence/reproduction:** RunSqlLocalDb reads stdout and stderr synchronously before its timed wait. A hung child was not deliberately created.

**Suggested correction:** Read both streams concurrently, enforce a real process deadline, inspect exit codes and handle timed-out children explicitly.

**Source:** [RentOnCars/RentOnCars/DatabaseInitializer.cs:192](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/DatabaseInitializer.cs:192), [RentOnCars/RentOnCars/DatabaseInitializer.cs:197](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/DatabaseInitializer.cs:197), [RentOnCars/RentOnCars/DatabaseInitializer.cs:199](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/DatabaseInitializer.cs:199)

### 31. B08 — History reads extras and bookings without a consistent snapshot

**Medium · Backend / DB · Consistency risk · Evidence: Source inference**

GetDetails first loads all extras, then all bookings using a separate statement with no read transaction. A booking committed between those reads can appear without its saved extras until the next refresh.

**Evidence/reproduction:** A commit after extrasSql completes but before the bookings SELECT is the problematic ordering. The timing window was not stress-tested.

**Suggested correction:** Retrieve bookings and extras in one query or use an appropriate consistent read transaction/snapshot.

**Source:** [RentOnCars/RentOnCars/Repos/Booking_repos.cs:218](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Repos/Booking_repos.cs:218), [RentOnCars/RentOnCars/Repos/Booking_repos.cs:245](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Repos/Booking_repos.cs:245), [RentOnCars/RentOnCars/Repos/Booking_repos.cs:264](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Repos/Booking_repos.cs:264)

### 32. B09 — Reports load all records and keep stale data on refresh errors

**Medium · Backend / DB · Scalability gap · Evidence: Source**

GetDetails scans all bookings and all extras; filtering is in memory and recreates grids. The schema has no explicit supporting indexes for availability/filter/FK lookups. A refresh error displays an error label while leaving the old grid/details in place. Ten demo cars conceal the scale issue.

**Evidence/reproduction:** Inspect unbounded SELECTs and report readData catch. Performance at large volumes was not measured.

**Suggested correction:** Add database-side filtering/pagination and suitable indexes; label retained data as stale or clear it on load failure.

**Source:** [RentOnCars/RentOnCars/Repos/Booking_repos.cs:219](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Repos/Booking_repos.cs:219), [RentOnCars/RentOnCars/Repos/Booking_repos.cs:246](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Repos/Booking_repos.cs:246), [RentOnCars/RentOnCars/booking_report.cs:82](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/booking_report.cs:82), [RentOnCars/RentOnCars/booking_report.cs:158](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/booking_report.cs:158), [RentOnCars/dbo.database.sql:45](C:/Users/beepl/dev/Car-rental/RentOnCars/dbo.database.sql:45)

### 33. R01 — The shared fixed credential is a demo gate, not production staff security

**Medium · Security / Operations · Production limitation · Evidence: Source**

The required assignment credential is embedded in source/documentation. There are no individual accounts, roles, lockout, session expiry or persisted staff actor for bookings. Forms/repository also have no authenticated-user context. This is assignment-aligned, but inadequate for an operational multi-user service.

**Evidence/reproduction:** Form1 defines public credential constants; Booking and schema have no staff actor field. This is not a claim of a reachable remote endpoint.

**Suggested correction:** Keep the required credential for the assignment/demo only; production needs individual identities, server-enforced permissions and actor-linked records.

**Source:** [RentOnCars/RentOnCars/Form1.cs:21](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Form1.cs:21), [RentOnCars/RentOnCars/Form1.cs:53](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Form1.cs:53), [RentOnCars/RentOnCars/Models/BookingModels.cs:333](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Models/BookingModels.cs:333), [RentOnCars/dbo.database.sql:45](C:/Users/beepl/dev/Car-rental/RentOnCars/dbo.database.sql:45)

### 34. R02 — Audit logging is incomplete and can fail silently

**Medium · Security / Operations · Reliability gap · Evidence: Source**

Only login events are logged; bookings/returns lack transaction audit entries or staff attribution. LoginAuditLogger writes beside the executable, ignores all write failures, keeps an unbounded in-memory list and has no rotation/tamper protection. A read-only installation can silently lose disk audit records.

**Evidence/reproduction:** Inspect File.AppendAllText and empty catch, plus SaveBooking without audit calls. Disk permission failures were not injected.

**Suggested correction:** Write structured actor-linked audit events to a supported writable store, surface failures, and define retention/rotation.

**Source:** [RentOnCars/RentOnCars/Services/LoginAuditLogger.cs:30](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Services/LoginAuditLogger.cs:30), [RentOnCars/RentOnCars/Services/LoginAuditLogger.cs:73](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Services/LoginAuditLogger.cs:73), [RentOnCars/RentOnCars/Services/LoginAuditLogger.cs:81](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Services/LoginAuditLogger.cs:81), [RentOnCars/RentOnCars/Services/LoginAuditLogger.cs:84](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Services/LoginAuditLogger.cs:84), [RentOnCars/RentOnCars/Repos/Booking_repos.cs:445](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Repos/Booking_repos.cs:445)

### 35. R03 — The 20 passing tests omit the workflows where serious bugs occur

**Medium · Tests / Delivery · Coverage gap · Evidence: Build + test run + source**

Existing tests cover credentials, local validation, pricing, enums, an in-memory collection, logging and executable discovery. They never test SaveBooking/GetDetails with SQL, concurrent saves, filtered/sorted report selection, frontend injection, persistence, keyboard flows or window lifetime. No CI workflow is tracked.

**Evidence/reproduction:** The Debug solution built and all 20 original tests passed despite the reproduced data/security/report failures. New audit probes demonstrate the missing coverage.

**Suggested correction:** Add targeted regression tests for the verified failures and isolated database integration tests; run them in CI with required environment setup.

**Source:** [RentOnCars/RentOnCars.Tests/Program.cs:27](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars.Tests/Program.cs:27), [RentOnCars/RentOnCars.Tests/Program.cs:47](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars.Tests/Program.cs:47), [RentOnCars/RentOnCars.Tests/RentOnCars.Tests.csproj:10](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars.Tests/RentOnCars.Tests.csproj:10)

### 36. R04 — The archived SQL setup script deletes all rental data

**High · Tests / Delivery · Archive hazard · Evidence: Source**

original_source retains a setup script that drops extras, bookings and cars in RentOnCarsDB. It is not called by the active solution, but executing the wrong setup file would erase the operational tables.

**Evidence/reproduction:** The archived script contains DROP TABLE at lines 20, 24 and 28. It was not executed.

**Suggested correction:** Clearly quarantine historical source and remove destructive setup instructions from any runnable/distributed path; add prominent warnings and safe scripts.

**Source:** [original_source/RentOnCars/RentOnCars/dbo.database.sql:19](C:/Users/beepl/dev/Car-rental/original_source/RentOnCars/RentOnCars/dbo.database.sql:19), [original_source/RentOnCars/RentOnCars/dbo.database.sql:23](C:/Users/beepl/dev/Car-rental/original_source/RentOnCars/RentOnCars/dbo.database.sql:23), [original_source/RentOnCars/RentOnCars/dbo.database.sql:27](C:/Users/beepl/dev/Car-rental/original_source/RentOnCars/RentOnCars/dbo.database.sql:27)

### 37. R05 — Business rules and source artifacts are duplicated and documentation overstates coverage

**Low · Tests / Delivery · Maintenance gap · Evidence: Source**

Pricing is repeated in JavaScript, PricingCalculator, model methods and OptionalExtras; schema/seeds are duplicated in code and SQL. Original source and an enhanced ZIP create multiple plausible entry points. README's named test list does not match the actual runner and calls table creation 'schema migrations'. It does not explain running or limiting the web preview.

**Evidence/reproduction:** Compare tariffRules, ComputeTotal, Booking.CalculateTotalCost, SQL setup and README test table to Program.cs.

**Suggested correction:** Centralize authoritative rules/migrations, document active entry points and preview limitations, and keep test claims generated from actual runs.

**Source:** [web_preview/index.html:1623](C:/Users/beepl/dev/Car-rental/web_preview/index.html:1623), [RentOnCars/RentOnCars/Services/PricingCalculator.cs:57](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Services/PricingCalculator.cs:57), [RentOnCars/RentOnCars/Models/BookingModels.cs:256](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Models/BookingModels.cs:256), [RentOnCars/RentOnCars/Models/BookingModels.cs:394](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/Models/BookingModels.cs:394), [README.md:114](C:/Users/beepl/dev/Car-rental/README.md:114), [README.md:147](C:/Users/beepl/dev/Car-rental/README.md:147)

### 38. R06 — There is no documented deployment, backup/recovery or customer-data lifecycle

**Medium · Security / Operations · Production limitation · Evidence: Source**

Default per-user LocalDB creates separate inventories across machines/Windows users. The repo has no backup/restore procedure, packaged deployment, production environment configuration, customer-data retention/deletion policy or application-level access model for records. Windows/.NET Framework are required for the desktop.

**Evidence/reproduction:** Review App.config, schema and setup guide. OS/database protections may exist outside this repo; they were not assessed.

**Suggested correction:** Define a shared deployment architecture if required, least-privilege database access, tested backups/restores, and explicit customer-data retention and access rules.

**Source:** [RentOnCars/RentOnCars/App.config:5](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/App.config:5), [RentOnCars/DATABASE_SETUP.txt:6](C:/Users/beepl/dev/Car-rental/RentOnCars/DATABASE_SETUP.txt:6), [RentOnCars/RentOnCars/RentOnCars.csproj:12](C:/Users/beepl/dev/Car-rental/RentOnCars/RentOnCars/RentOnCars.csproj:12), [RentOnCars/dbo.database.sql:50](C:/Users/beepl/dev/Car-rental/RentOnCars/dbo.database.sql:50)

## Reproducible audit artifacts

- [Structured findings](C:/Users/beepl/dev/Car-rental/audit/2026-10-14/findings.json)
- [Web probes](C:/Users/beepl/dev/Car-rental/audit/2026-10-14/web-probes.cjs) and [web results](C:/Users/beepl/dev/Car-rental/audit/2026-10-14/web-probe-results.json)
- [Backend probes](C:/Users/beepl/dev/Car-rental/audit/2026-10-14/backend-probes.cs) and [backend results](C:/Users/beepl/dev/Car-rental/audit/2026-10-14/backend-probe-results.txt)

Web probes: run `node audit/2026-10-14/web-probes.cjs` from the workspace root. They create only a local result file.

Backend probes require building the active solution, compiling the probe against RentOnCars.exe with Windows Forms/Data/Configuration references, and a **dedicated disposable LocalDB instance named CarRentalAudit20261014**. They create a synthetic catalog of the same name, refuse to overwrite an existing catalog, and remove that audit-created catalog when complete. Supply the current SQL script path and output text path as the two executable arguments. Do not point this harness at an existing operational database.

Application source was not changed. Only audit artifacts were added; build outputs/logs are ignored by the repository.

