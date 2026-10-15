using System;
using System.Collections.Generic;
using System.Diagnostics;
using RentOnCars.Models;
using RentOnCars.Services;

namespace RentOnCars.Tests
{
    internal static class Program
    {
        private static int passedCount = 0;
        private static int failedCount = 0;
        private static readonly List<string> failureDetails = new List<string>();

        [STAThread]
        private static int Main()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("========================================================================");
            Console.WriteLine("            WEARECARS CAR RENTAL SYSTEM - AUTOMATED TEST SUITE          ");
            Console.WriteLine("                     University of Sunderland - CET131                  ");
            Console.WriteLine("========================================================================");
            Console.ResetColor();
            Console.WriteLine($"Execution Started: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n");

            Stopwatch sw = Stopwatch.StartNew();

            // Run Test Suites
            RunTest("Credentials - Fixed Staff Credentials (sta001 / givemethekeys123)", Test_FixedCredentials_Valid);
            RunTest("Credentials - Invalid Username Rejected", Test_Credentials_InvalidUsername);
            RunTest("Credentials - Invalid Password Rejected", Test_Credentials_InvalidPassword);
            RunTest("Credentials - Empty Inputs Rejected", Test_Credentials_EmptyInputs);

            RunTest("Validation - Required Customer Fields (First name, Address)", Test_CustomerValidation_RequiredFields);
            RunTest("Validation - Mandatory Surname Rule", Test_CustomerValidation_MandatorySurname);
            RunTest("Validation - Age Threshold (Under 18 Rejected, 18+ Allowed)", Test_CustomerValidation_AgeLimit);
            RunTest("Validation - Driving Licence Requirement (Valid Mandatory)", Test_CustomerValidation_DrivingLicence);
            RunTest("Validation - Rental Duration Boundaries (0, 1, 28, 29 days)", Test_RentalDuration_Boundaries);
            RunTest("Validation - Database Column Length Limits (First name 50, Surname 50, Address 150)", Test_DatabaseLengthLimits);

            RunTest("Pricing - Vehicle Category Surcharges (City=£0, Family=£50, Sports=£75, SUV=£65)", Test_Pricing_VehicleCategorySurcharges);
            RunTest("Pricing - Fuel Type Surcharges (Petrol=£0, Diesel=£0, Hybrid=£30, Electric=£50)", Test_Pricing_FuelTypeSurcharges);
            RunTest("Pricing - Optional Extras Independent & Combined (£10/day, £2/day)", Test_Pricing_OptionalExtras);
            RunTest("Pricing - Authoritative Assignment Benchmark (£206.00 for 3-Day SUV Hybrid)", Test_Pricing_AuthoritativeBenchmark_206);
            RunTest("Pricing - Calculation Independence from Database", Test_Pricing_IndependenceFromDatabase);
            RunTest("Pricing - Incomplete Estimates Clearly Labeled", Test_Pricing_IncompleteEstimateLabels);

            RunTest("Data Integrity - Strict Enum Parsing (No Silent Fallback)", Test_StrictEnumParsing);
            RunTest("Domain Model - RentedCarsList Operations", Test_RentedCarsListOperations);
            RunTest("Audit Logging - Flowchart Login Activity Logging", Test_AuditLogging);
            RunTest("Database Discovery - SqlLocalDB Discovery Safety", Test_SqlLocalDbDiscoverySafety);

            RunTest("Regression - Undefined numeric enums rejected", RegressionTests.NumericEnums);
            RunTest("Regression - Vehicle state and domain constraints", RegressionTests.InvalidVehicleState);
            RunTest("Regression - Allocated rate and stable GBP formatting", RegressionTests.PricingRateAndCulture);
            RunTest("Regression - Vehicle identity and clear state", RegressionTests.VehicleIdentity);
            RunTest("Regression - Filtered and sorted booking identity", RegressionTests.SelectedBookingIdentity);
            RunTest("Regression - Explicit age and licence entry", RegressionTests.LicenceIsExplicit);
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEARECARS_TEST_CONNECTION_STRING")))
            {
                RunTest("SQL - Authoritative prices, snapshots, retry and return", RegressionTests.AuthoritativePricingAndSnapshot);
                RunTest("SQL - Concurrent booking exclusion", RegressionTests.ConcurrentSave);
                RunTest("SQL - Schema constraints and transaction audit", RegressionTests.SchemaConstraintsAndAudit);
                RunTest("SQL - Staff authentication and lockout", RegressionTests.StaffAuthenticationAndLockout);
                RunTest("SQL - Verified backup, safe restore and retention preview", RegressionTests.BackupRestoreAndRetention);
            }

            sw.Stop();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("========================================================================");
            Console.WriteLine("                             TEST SUMMARY                               ");
            Console.WriteLine("========================================================================");
            Console.ResetColor();

            Console.WriteLine($"Total Tests Executed : {passedCount + failedCount}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Passed               : {passedCount}");
            Console.ResetColor();

            if (failedCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed               : {failedCount}");
                Console.ResetColor();
                Console.WriteLine("\nFailures Detail:");
                foreach (string f in failureDetails)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" • " + f);
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("ALL TESTS PASSED SUCCESSFULLY! (100% Pass Rate)");
                Console.ResetColor();
            }

            Console.WriteLine($"Total Execution Time : {sw.ElapsedMilliseconds} ms");
            Console.WriteLine("========================================================================\n");

            return failedCount == 0 ? 0 : 1;
        }

        private static void RunTest(string testName, Action testMethod)
        {
            try
            {
                testMethod();
                passedCount++;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[PASS] ");
                Console.ResetColor();
                Console.WriteLine(testName);
            }
            catch (Exception ex)
            {
                failedCount++;
                string msg = $"{testName} -> {ex.Message}";
                failureDetails.Add(msg);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[FAIL] ");
                Console.ResetColor();
                Console.WriteLine(msg);
            }
        }

        private static void AssertTrue(bool condition, string message)
        {
            if (!condition)
                throw new Exception($"Assertion failed: {message}");
        }

        private static void AssertEqual<T>(T expected, T actual, string message)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                throw new Exception($"Assertion failed: {message}. Expected: {expected}, Actual: {actual}");
        }

        #region Credentials Tests

        private static void Test_FixedCredentials_Valid()
        {
            LoginForm form = new LoginForm();
            bool result = form.ValidateCredentials("sta001", "givemethekeys123");
            AssertTrue(result, "Fixed staff credentials 'sta001' and 'givemethekeys123' must be accepted.");
        }

        private static void Test_Credentials_InvalidUsername()
        {
            LoginForm form = new LoginForm();
            bool result = form.ValidateCredentials("admin", "givemethekeys123");
            AssertTrue(!result, "Username other than 'sta001' must be rejected.");
        }

        private static void Test_Credentials_InvalidPassword()
        {
            LoginForm form = new LoginForm();
            bool result = form.ValidateCredentials("sta001", "wrongpass");
            AssertTrue(!result, "Password other than 'givemethekeys123' must be rejected.");
        }

        private static void Test_Credentials_EmptyInputs()
        {
            LoginForm form = new LoginForm();
            AssertTrue(!form.ValidateCredentials("", "givemethekeys123"), "Blank username must be rejected.");
            AssertTrue(!form.ValidateCredentials("sta001", ""), "Blank password must be rejected.");
            AssertTrue(!form.ValidateCredentials("", ""), "Both blank must be rejected.");
        }

        #endregion

        #region Validation Tests

        private static void Test_CustomerValidation_RequiredFields()
        {
            Car car = new Car(1, "WAC-001", CarType.CityCar, FuelType.Petrol);

            // Missing first name
            var r1 = BookingValidator.Validate("", "Smith", "10 High St", 25, true, 3, CarType.CityCar, FuelType.Petrol, car);
            AssertTrue(!r1.IsValid, "Missing first name must fail validation.");
            AssertTrue(r1.FieldErrors.ContainsKey("FirstName"), "Error provider must contain FirstName key.");

            // Missing address
            var r2 = BookingValidator.Validate("John", "Smith", "", 25, true, 3, CarType.CityCar, FuelType.Petrol, car);
            AssertTrue(!r2.IsValid, "Missing address must fail validation.");
            AssertTrue(r2.FieldErrors.ContainsKey("Address"), "Error provider must contain Address key.");
        }

        private static void Test_CustomerValidation_MandatorySurname()
        {
            Car car = new Car(1, "WAC-001", CarType.CityCar, FuelType.Petrol);

            // Missing surname
            var resEmpty = BookingValidator.Validate("John", "", "10 High St", 25, true, 3, CarType.CityCar, FuelType.Petrol, car);
            AssertTrue(!resEmpty.IsValid, "Missing surname must fail validation according to mandatory surname rule.");
            AssertTrue(resEmpty.FieldErrors.ContainsKey("Surname"), "Error provider must report Surname error.");

            // Whitespace surname
            var resSpace = BookingValidator.Validate("John", "   ", "10 High St", 25, true, 3, CarType.CityCar, FuelType.Petrol, car);
            AssertTrue(!resSpace.IsValid, "Whitespace surname must fail validation.");
        }

        private static void Test_CustomerValidation_AgeLimit()
        {
            Car car = new Car(1, "WAC-001", CarType.CityCar, FuelType.Petrol);

            // Under 18
            var r17 = BookingValidator.Validate("John", "Smith", "10 High St", 17, true, 3, CarType.CityCar, FuelType.Petrol, car);
            AssertTrue(!r17.IsValid, "Customer age 17 must be rejected.");
            AssertTrue(r17.FieldErrors.ContainsKey("Age"), "Error provider must flag Age error.");

            // Exactly 18
            var r18 = BookingValidator.Validate("John", "Smith", "10 High St", 18, true, 3, CarType.CityCar, FuelType.Petrol, car);
            AssertTrue(r18.IsValid, "Customer age 18 must be accepted.");

            // Age 45
            var r45 = BookingValidator.Validate("John", "Smith", "10 High St", 45, true, 3, CarType.CityCar, FuelType.Petrol, car);
            AssertTrue(r45.IsValid, "Customer age 45 must be accepted.");
        }

        private static void Test_CustomerValidation_DrivingLicence()
        {
            Car car = new Car(1, "WAC-001", CarType.CityCar, FuelType.Petrol);

            // Licence = false
            var rNo = BookingValidator.Validate("John", "Smith", "10 High St", 25, false, 3, CarType.CityCar, FuelType.Petrol, car);
            AssertTrue(!rNo.IsValid, "Customer without valid licence cannot hire vehicle.");
            AssertTrue(rNo.FieldErrors.ContainsKey("License"), "Error provider must flag License.");

            // Licence = true
            var rYes = BookingValidator.Validate("John", "Smith", "10 High St", 25, true, 3, CarType.CityCar, FuelType.Petrol, car);
            AssertTrue(rYes.IsValid, "Valid licence must pass validation.");
        }

        private static void Test_RentalDuration_Boundaries()
        {
            Car car = new Car(1, "WAC-001", CarType.CityCar, FuelType.Petrol);

            // 0 days (invalid)
            var r0 = BookingValidator.Validate("John", "Smith", "10 High St", 25, true, 0, CarType.CityCar, FuelType.Petrol, car);
            AssertTrue(!r0.IsValid, "0 rental days must be rejected.");

            // 1 day (valid lower boundary)
            var r1 = BookingValidator.Validate("John", "Smith", "10 High St", 25, true, 1, CarType.CityCar, FuelType.Petrol, car);
            AssertTrue(r1.IsValid, "1 rental day must be accepted.");

            // 28 days (valid upper boundary)
            var r28 = BookingValidator.Validate("John", "Smith", "10 High St", 25, true, 28, CarType.CityCar, FuelType.Petrol, car);
            AssertTrue(r28.IsValid, "28 rental days must be accepted.");

            // 29 days (invalid upper boundary)
            var r29 = BookingValidator.Validate("John", "Smith", "10 High St", 25, true, 29, CarType.CityCar, FuelType.Petrol, car);
            AssertTrue(!r29.IsValid, "29 rental days must be rejected.");
        }

        private static void Test_DatabaseLengthLimits()
        {
            Car car = new Car(1, "WAC-001", CarType.CityCar, FuelType.Petrol);

            // First name > 50 characters
            string longFirst = new string('A', 51);
            var r1 = BookingValidator.Validate(longFirst, "Smith", "10 High St", 25, true, 3, CarType.CityCar, FuelType.Petrol, car);
            AssertTrue(!r1.IsValid, "First name > 50 chars must fail database schema length check.");

            // Surname > 50 characters
            string longLast = new string('B', 51);
            var r2 = BookingValidator.Validate("John", longLast, "10 High St", 25, true, 3, CarType.CityCar, FuelType.Petrol, car);
            AssertTrue(!r2.IsValid, "Surname > 50 chars must fail database schema length check.");

            // Address > 150 characters
            string longAddr = new string('C', 151);
            var r3 = BookingValidator.Validate("John", "Smith", longAddr, 25, true, 3, CarType.CityCar, FuelType.Petrol, car);
            AssertTrue(!r3.IsValid, "Address > 150 chars must fail database schema length check.");
        }

        #endregion

        #region Pricing Tests

        private static void Test_Pricing_VehicleCategorySurcharges()
        {
            AssertEqual(0m, CarType.CityCar.GetSurcharge(), "City car must have £0 surcharge.");
            AssertEqual(50m, CarType.FamilyCar.GetSurcharge(), "Family car must have £50 surcharge.");
            AssertEqual(75m, CarType.SportsCar.GetSurcharge(), "Sports car must have £75 surcharge.");
            AssertEqual(65m, CarType.SUV.GetSurcharge(), "SUV must have £65 surcharge.");
        }

        private static void Test_Pricing_FuelTypeSurcharges()
        {
            AssertEqual(0m, FuelType.Petrol.GetSurcharge(), "Petrol must have £0 surcharge.");
            AssertEqual(0m, FuelType.Diesel.GetSurcharge(), "Diesel must have £0 surcharge.");
            AssertEqual(30m, FuelType.Hybrid.GetSurcharge(), "Hybrid must have £30 surcharge.");
            AssertEqual(50m, FuelType.FullElectric.GetSurcharge(), "Full electric must have £50 surcharge.");
        }

        private static void Test_Pricing_OptionalExtras()
        {
            // 5 days hire
            int days = 5;

            // None
            decimal none = PricingCalculator.ComputeTotal(days, CarType.CityCar, FuelType.Petrol, false, false);
            AssertEqual(125m, none, "5 days base rent @ £25 with no extras should be £125.00.");

            // Unlimited mileage only (£10/day)
            decimal um = PricingCalculator.ComputeTotal(days, CarType.CityCar, FuelType.Petrol, true, false);
            AssertEqual(125m + (10m * 5), um, "5 days with Unlimited Mileage should add £50.00.");

            // Breakdown cover only (£2/day)
            decimal bc = PricingCalculator.ComputeTotal(days, CarType.CityCar, FuelType.Petrol, false, true);
            AssertEqual(125m + (2m * 5), bc, "5 days with Breakdown Cover should add £10.00.");

            // Both extras (£12/day)
            decimal both = PricingCalculator.ComputeTotal(days, CarType.CityCar, FuelType.Petrol, true, true);
            AssertEqual(125m + (12m * 5), both, "5 days with both extras should add £60.00.");
        }

        private static void Test_Pricing_AuthoritativeBenchmark_206()
        {
            // Requirement from brief:
            // "For example, a three-day SUV hybrid rental with both extras must total £206."
            // Breakdown:
            // Base rental: 3 days * £25 = £75.00
            // SUV surcharge: £65.00
            // Hybrid surcharge: £30.00
            // Extras: (£10 + £2) * 3 days = £36.00
            // Total: 75 + 65 + 30 + 36 = £206.00

            decimal total = PricingCalculator.ComputeTotal(
                rentalDays: 3,
                carType: CarType.SUV,
                fuelType: FuelType.Hybrid,
                unlimitedMileage: true,
                breakdownCover: true);

            AssertEqual(206.00m, total, "Three-day SUV Hybrid rental with both extras must equal exactly £206.00.");

            // Also test via PricingCalculator.CalculateEstimate
            PricingBreakdown breakdown = PricingCalculator.CalculateEstimate(3, CarType.SUV, FuelType.Hybrid, true, true);
            AssertEqual(206.00m, breakdown.GrandTotal, "Live breakdown GrandTotal must equal £206.00.");
            AssertEqual("£206.00", breakdown.FormattedGrandTotal, "Formatted total must be £206.00.");
            AssertTrue(breakdown.IsComplete, "Estimate must be complete.");
        }

        private static void Test_Pricing_IndependenceFromDatabase()
        {
            // Calculate 10 different estimates without initializing database
            for (int d = 1; d <= 5; d++)
            {
                var est = PricingCalculator.CalculateEstimate(d, CarType.SportsCar, FuelType.FullElectric, true, false);
                AssertTrue(est.GrandTotal > 0, "Estimate calculated independently of database.");
            }
        }

        private static void Test_Pricing_IncompleteEstimateLabels()
        {
            // Missing category
            var b1 = PricingCalculator.CalculateEstimate(3, null, FuelType.Petrol, false, false);
            AssertTrue(!b1.IsComplete, "Estimate without category must be flagged incomplete.");
            AssertTrue(b1.StatusMessage.Contains("Select car category"), "Status must prompt for category.");

            // Missing fuel
            var b2 = PricingCalculator.CalculateEstimate(3, CarType.SUV, null, false, false);
            AssertTrue(!b2.IsComplete, "Estimate without fuel must be flagged incomplete.");
            AssertTrue(b2.StatusMessage.Contains("Select fuel type"), "Status must prompt for fuel.");

            // Missing both
            var b3 = PricingCalculator.CalculateEstimate(3, null, null, false, false);
            AssertTrue(!b3.IsComplete, "Estimate without category and fuel must be flagged incomplete.");
        }

        #endregion

        #region Domain & Infrastructure Tests

        private static void Test_StrictEnumParsing()
        {
            // Valid categories
            AssertEqual(CarType.CityCar, VehicleEnumExtensions.ParseCarType("CityCar"), "CityCar parsed");
            AssertEqual(CarType.FamilyCar, VehicleEnumExtensions.ParseCarType("Family Car"), "Family Car parsed");
            AssertEqual(CarType.SportsCar, VehicleEnumExtensions.ParseCarType("Sports car"), "Sports car parsed");
            AssertEqual(CarType.SUV, VehicleEnumExtensions.ParseCarType("SUV"), "SUV parsed");

            // Valid fuels
            AssertEqual(FuelType.Petrol, VehicleEnumExtensions.ParseFuelType("Petrol"), "Petrol parsed");
            AssertEqual(FuelType.Diesel, VehicleEnumExtensions.ParseFuelType("Diesel"), "Diesel parsed");
            AssertEqual(FuelType.Hybrid, VehicleEnumExtensions.ParseFuelType("Hybrid"), "Hybrid parsed");
            AssertEqual(FuelType.FullElectric, VehicleEnumExtensions.ParseFuelType("Full electric"), "Full electric parsed");

            // Invalid category must throw FormatException rather than silently falling back
            bool caughtCar = false;
            try
            {
                VehicleEnumExtensions.ParseCarType("SpaceRocket");
            }
            catch (FormatException)
            {
                caughtCar = true;
            }
            AssertTrue(caughtCar, "Invalid car category string must throw FormatException without silent fallback.");

            // Invalid fuel must throw FormatException
            bool caughtFuel = false;
            try
            {
                VehicleEnumExtensions.ParseFuelType("Plutonium");
            }
            catch (FormatException)
            {
                caughtFuel = true;
            }
            AssertTrue(caughtFuel, "Invalid fuel type string must throw FormatException without silent fallback.");
        }

        private static void Test_RentedCarsListOperations()
        {
            RentedCarsList list = new RentedCarsList();
            AssertEqual(0, list.Count, "Initial rented cars list must be empty.");

            Car car1 = new Car(1, "WAC-001", CarType.CityCar, FuelType.Petrol);
            Car car2 = new Car(2, "WAC-002", CarType.SUV, FuelType.Hybrid);

            list.AddRentedCar(car1);
            AssertEqual(1, list.Count, "Rented cars count after add must be 1.");
            AssertTrue(car1.IsRented, "Car must be marked as rented.");

            list.AddRentedCar(car2);
            AssertEqual(2, list.Count, "Rented cars count after second add must be 2.");

            list.RemoveRentedCar(car1);
            AssertEqual(1, list.Count, "Rented cars count after remove must be 1.");
            AssertTrue(!car1.IsRented, "Removed car must be marked as available.");

            list.DisplayRentedCars(); // Method from UML class diagram executes without error
        }

        private static void Test_AuditLogging()
        {
            int before = LoginAuditLogger.Entries.Count;
            LoginAuditLogger.LogSuccess("sta001");
            AssertEqual(before + 1, LoginAuditLogger.Entries.Count, "Login audit entry must be appended.");

            var latest = LoginAuditLogger.Entries[LoginAuditLogger.Entries.Count - 1];
            AssertTrue(latest.Success, "Audit entry must record success.");
            AssertEqual("sta001", latest.Username, "Audit entry must record username sta001.");
        }

        private static void Test_SqlLocalDbDiscoverySafety()
        {
            string exe = DatabaseInitializer.FindSqlLocalDbExecutable();
            // If found, it must be an actual existing file on disk; never a fake path
            if (exe != null)
            {
                AssertTrue(System.IO.File.Exists(exe), "Discovered SqlLocalDB executable must exist on disk.");
            }
            // If null, it gracefully indicated not found rather than throwing
            AssertTrue(true, "Discovery executed safely without crash.");
        }

        #endregion
    }
}
