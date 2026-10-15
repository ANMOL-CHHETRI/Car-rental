using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using RentOnCars;
using RentOnCars.Models;
using RentOnCars.Repos;
using RentOnCars.Services;

class BackendAuditProbes
{
    const string Catalog = "CarRentalAudit20261014";
    const string Server = @"(localdb)\CarRentalAudit20261014";
    static readonly string Conn = "Data Source=" + Server + ";Initial Catalog=" + Catalog + ";Integrated Security=True;Connect Timeout=5";
    static readonly List<string> Results = new List<string>();
    static void Record(string message) { Results.Add(message); Console.WriteLine(message); }
    static void Probe(string name, Action test) { try { test(); } catch(Exception e) { Record(name + " ERROR: " + e.GetBaseException().Message); } }
    static T Field<T>(object o, string name) { return (T)o.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(o); }
    static void Invoke(object o, string name, params object[] args) { o.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(o, args); }
    static Booking BookingFor(Car car, string name = "Audit") { return new Booking { CustomerFirstName = name, CustomerSurname = "Probe", CustomerAddress = "Synthetic test address", CustomerAge = 26, HasValidLicense = true, NumberOfDays = 3, CarId = car.CarId, SelectedCar = car, Extras = new OptionalExtras(true, true) }; }
    static void Sql(string sql) { using(var c = new SqlConnection(Conn)) { c.Open(); using(var cmd = new SqlCommand(sql, c)) cmd.ExecuteNonQuery(); } }
    static void Prepare(string sqlPath) {
        var master = "Data Source=" + Server + ";Initial Catalog=master;Integrated Security=True;Connect Timeout=5";
        using(var c = new SqlConnection(master)) {
            c.Open(); using(var cmd = new SqlCommand("IF DB_ID(N'" + Catalog + "') IS NOT NULL THROW 50000, 'Audit catalog already exists; refusing to overwrite.', 1; CREATE DATABASE [" + Catalog + "];", c)) cmd.ExecuteNonQuery();
        }
        string sql = File.ReadAllText(sqlPath).Replace("RentOnCarsDB", Catalog);
        using(var c = new SqlConnection(Conn)) {
            c.Open(); foreach(string batch in Regex.Split(sql, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
                if(!string.IsNullOrWhiteSpace(batch)) using(var cmd = new SqlCommand(batch, c)) cmd.ExecuteNonQuery();
        }
    }
    [STAThread]
    static void Main(string[] args) {
        Probe("Enum parsing", () => Record("Numeric enum 999 accepted: car=" + (int)VehicleEnumExtensions.ParseCarType("999") + ", fuel=" + (int)VehicleEnumExtensions.ParseFuelType("999")));
        Probe("Validation", () => {
            var rented = new Car(1, "AUDIT-1", CarType.CityCar, FuelType.Petrol, true);
            var b = BookingFor(rented); b.CustomerFirstName = new string('A', 51);
            Record("Domain ValidateBooking accepts rented car and 51-character name: " + b.ValidateBooking());
            Record("Service validator accepts rented/wrong-category vehicle: " + BookingValidator.Validate("Audit", "Probe", "Synthetic", 26, true, 3, CarType.SUV, FuelType.Hybrid, rented).IsValid);
            Record("Service validator accepts undefined category enum: " + BookingValidator.Validate("Audit", "Probe", "Synthetic", 26, true, 3, (CarType)999, FuelType.Petrol, rented).IsValid);
            var list = new RentedCarsList(); list.AddRentedCar(rented); list.AddRentedCar(new Car(1, "AUDIT-1", CarType.CityCar, FuelType.Petrol));
            Record("RentedCarsList count for same vehicle ID in different objects: " + list.Count); list.Clear(); Record("RentedCarsList.Clear leaves original car rented: " + rented.IsRented);
        });
        Probe("Estimator", () => {
            var b = BookingFor(new Car(1, "AUDIT-1", CarType.SUV, FuelType.Hybrid, false, 40m));
            var estimate = PricingCalculator.CalculateEstimate(3, CarType.SUV, FuelType.Hybrid, true, true);
            Record("At daily rate 40: reviewed estimate=" + estimate.GrandTotal + ", booking total=" + b.CalculateTotalCost());
            Record("Zero-day estimate marked complete: " + PricingCalculator.CalculateEstimate(0, CarType.CityCar, FuelType.Petrol, false, false).IsComplete);
            Record("Negative daily rate accepted by ComputeTotal: " + PricingCalculator.ComputeTotal(3, CarType.CityCar, FuelType.Petrol, false, false, -25m));
            var old = CultureInfo.CurrentCulture; try { CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR"); Record("GBP formatting under fr-FR: " + estimate.FormattedGrandTotal); } finally { CultureInfo.CurrentCulture = old; }
        });
        Probe("Report filter", () => {
            using(var report = new booking_report(false)) {
                var a = BookingFor(new Car(1,"AUDIT-1",CarType.CityCar,FuelType.Petrol), "Zulu"); a.BookingId=3;
                var b = BookingFor(new Car(2,"AUDIT-2",CarType.SUV,FuelType.Hybrid), "Alpha"); b.BookingId=2;
                report.GetType().GetField("cachedBookings", BindingFlags.Instance|BindingFlags.NonPublic).SetValue(report, new List<Booking>{a,b});
                var grid=Field<DataGridView>(report,"dataGridView1"); grid.BindingContext = new BindingContext();
                Field<TextBox>(report,"searchBox").Text="Alpha"; Invoke(report,"ApplyBookingHistoryFilter");
                Record("Filtered row="+grid.Rows[0].Cells["Booking ID"].Value+", detail="+Field<Label>(report,"detailBookingRefLabel").Text);
                Field<TextBox>(report,"searchBox").Text=""; Invoke(report,"ApplyBookingHistoryFilter"); grid.Sort(grid.Columns["Customer Name"],ListSortDirection.Ascending);
                Invoke(report,"DisplayBookingDetails",0);
                Record("Sorted row="+grid.Rows[0].Cells["Booking ID"].Value+", detail="+Field<Label>(report,"detailBookingRefLabel").Text);
            }
        });
        bool prepared=false;
        try {
            Prepare(args[0]); prepared=true; Record("Isolated SQL database prepared using current setup script.");
            var repo=new BookingRepository(Conn);
            Probe("Authoritative car fields", () => {
                var actual=repo.GetAvailableCar(CarType.CityCar,FuelType.Petrol); var spoof=new Car(actual.CarId,actual.RegistrationNumber,CarType.SUV,FuelType.Hybrid,false,1m);
                var b=BookingFor(spoof); repo.SaveBooking(b);
                Record("Database vehicle CityCar/Petrol @25 saved using caller SUV/Hybrid @1: total="+b.TotalCost+", category="+repo.GetDetails()[0].SelectedCar.CarType);
            });
            Probe("Historical rate snapshot", () => {
                var b=repo.GetDetails()[0]; Sql("UPDATE dbo.Cars SET DailyRate=99 WHERE CarId="+b.SelectedCar.CarId+"; UPDATE dbo.BookingOptionalExtras SET DailyPrice=123 WHERE BookingId="+b.BookingId);
                var read=repo.GetDetails()[0]; Record("History after rate edits: stored total="+read.TotalCost+", reconstructed base rate="+read.SelectedCar.DailyRate+", extras daily="+read.Extras.GetDailyExtrasTotal());
            });
            Probe("Concurrency safeguard", () => {
                var car=repo.GetAvailableCar(CarType.SUV,FuelType.Hybrid); int successes=0, failures=0;
                Parallel.For(0,2,i=>{try { new BookingRepository(Conn).SaveBooking(BookingFor(new Car(car.CarId,car.RegistrationNumber,car.CarType,car.FuelType)));System.Threading.Interlocked.Increment(ref successes);}catch(InvalidOperationException){System.Threading.Interlocked.Increment(ref failures);} });
                Record("Concurrent saves for same car: successes="+successes+", rejected="+failures);
            });
            Probe("Database constraints", () => {
                Sql("INSERT INTO dbo.Bookings (CarId,CustomerFirstName,CustomerSurname,CustomerAddress,CustomerAge,HasValidLicense,NumberOfDays,CarType,FuelType,TotalCost) VALUES (1,N'',N'',N'',26,0,3,N'Unknown',N'Unknown',0)");
                Record("Database accepted blank customer fields, invalid licence, unknown category/fuel and zero total via direct SQL.");
                Sql("INSERT INTO dbo.BookingOptionalExtras (BookingId,ExtraName,DailyPrice,NumberOfDays) SELECT TOP 1 BookingId,N'Unlimited Mileage',10,28 FROM dbo.Bookings ORDER BY BookingId");
                Record("Database accepted duplicate extra with rental days inconsistent with booking.");
            });
        } catch(Exception e) { Record("SQL integration unavailable: "+e.GetBaseException().Message); }
        finally {
            if(prepared) { using(var c=new SqlConnection("Data Source="+Server+";Initial Catalog=master;Integrated Security=True;Connect Timeout=5")) { c.Open(); using(var cmd=new SqlCommand("ALTER DATABASE ["+Catalog+"] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE ["+Catalog+"];",c)) cmd.ExecuteNonQuery(); } Record("Removed only the audit-created catalog from the dedicated audit instance."); }
            File.WriteAllLines(args[1],Results);
        }
    }
}
