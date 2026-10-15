using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RentOnCars.Models;
using RentOnCars.Repos;
using RentOnCars.Services;

namespace RentOnCars.Tests
{
    internal static class RegressionTests
    {
        private static void Check(bool value,string message) { if(!value)throw new Exception(message); }
        private static void Reject<T>(Action action) where T:Exception
        { try{action();}catch(T){return;}throw new Exception("Expected rejection: "+typeof(T).Name); }
        public static void NumericEnums()
        { Reject<FormatException>(()=>VehicleEnumExtensions.ParseCarType("999"));Reject<FormatException>(()=>VehicleEnumExtensions.ParseFuelType("-1")); }
        public static void InvalidVehicleState()
        {
            var rented=new Car(1,"TEST-1",CarType.CityCar,FuelType.Petrol,true);
            Check(!BookingValidator.Validate("Test","Customer","Test address",26,true,3,CarType.CityCar,FuelType.Petrol,rented).IsValid,"Rented car accepted");
            rented.IsRented=false;
            Check(!BookingValidator.Validate("Test","Customer","Test address",26,true,3,CarType.SUV,FuelType.Hybrid,rented).IsValid,"Mismatched vehicle accepted");
            Check(!BookingValidator.Validate("Test","Customer","Test address",121,true,3,CarType.CityCar,FuelType.Petrol,rented).IsValid,"Invalid maximum age accepted");
            var booking=ValidBooking(rented);booking.CustomerFirstName=new string('a',51);Check(!booking.ValidateBooking(),"Domain length limit bypassed");
        }
        public static void PricingRateAndCulture()
        {
            var estimate=PricingCalculator.CalculateEstimate(3,CarType.SUV,FuelType.Hybrid,true,true,40m);
            Check(estimate.GrandTotal==251m,"Allocated daily rate ignored");
            Reject<ArgumentOutOfRangeException>(()=>PricingCalculator.ComputeTotal(3,CarType.CityCar,FuelType.Petrol,false,false,-25m));
            CultureInfo before=CultureInfo.CurrentCulture;
            try {CultureInfo.CurrentCulture=CultureInfo.GetCultureInfo("fr-FR");Check(estimate.FormattedGrandTotal=="£251.00","GBP formatting depends on machine culture");}
            finally {CultureInfo.CurrentCulture=before;}
            Check(!PricingCalculator.CalculateEstimate(0,CarType.CityCar,FuelType.Petrol,false,false).IsComplete,"Invalid duration marked complete");
        }
        public static void VehicleIdentity()
        {
            var list=new RentedCarsList();var car=new Car(1,"TEST-1",CarType.CityCar,FuelType.Petrol);list.AddRentedCar(car);list.AddRentedCar(new Car(1,"TEST-1",CarType.CityCar,FuelType.Petrol));
            Check(list.Count==1,"Duplicate vehicle identity accepted");list.Clear();Check(!car.IsRented,"Clear left vehicle rented");
        }
        private static T Field<T>(object instance,string name)=>(T)instance.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(instance);
        private static void Call(object instance,string name,params object[] args)=>instance.GetType().GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(instance,args);
        public static void SelectedBookingIdentity()
        {
            using(var form=new booking_report(false))
            {
                var first=ValidBooking(new Car(1,"TEST-1",CarType.CityCar,FuelType.Petrol));first.BookingId=3;first.CustomerFirstName="Zulu";
                var second=ValidBooking(new Car(2,"TEST-2",CarType.CityCar,FuelType.Petrol));second.BookingId=2;second.CustomerFirstName="Alpha";
                form.GetType().GetField("cachedBookings",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(form,new List<Booking>{first,second});
                var grid=Field<DataGridView>(form,"dataGridView1");grid.BindingContext=new BindingContext();
                Field<TextBox>(form,"searchBox").Text="Alpha";Call(form,"ApplyBookingHistoryFilter");
                Check(Field<Label>(form,"detailBookingRefLabel").Text.Contains("00002"),"Filtered detail belongs to another customer");
                Field<TextBox>(form,"searchBox").Text="";Call(form,"ApplyBookingHistoryFilter");grid.Sort(grid.Columns["Customer Name"],ListSortDirection.Ascending);Call(form,"DisplayBookingDetails",0);
                Check(Field<Label>(form,"detailBookingRefLabel").Text.Contains("00002"),"Sorted detail belongs to another customer");
            }
        }
        public static void LicenceIsExplicit()
        {
            Check(!new Booking().HasValidLicense,"A licence was implicitly verified");
            using(var form=new Details())
            {Check(!Field<RadioButton>(form,"yesradiobutton").Checked,"Form pre-confirmed licence");Check(Field<NumericUpDown>(form,"ageupdown").Value==0,"Form pre-assumed customer age");}
        }
        private static Booking ValidBooking(Car car)=>new Booking {SelectedCar=car,CarId=car.CarId,CustomerFirstName="Synthetic",CustomerSurname="Test",CustomerAddress="Synthetic test address",CustomerAge=26,HasValidLicense=true,NumberOfDays=3,StaffUsername="sta001",Extras=new OptionalExtras(true,true)};
        private static void Sql(string connection,string sql) {using(var con=new SqlConnection(connection)){con.Open();using(var cmd=new SqlCommand(sql,con))cmd.ExecuteNonQuery();}}
        private static BookingRepository Fixture()
        {
            string connection=Environment.GetEnvironmentVariable("WEARECARS_TEST_CONNECTION_STRING");
            if(string.IsNullOrEmpty(connection))throw new Exception("Dedicated test connection is required");
            var b=new SqlConnectionStringBuilder(connection);
            if(!b.InitialCatalog.StartsWith("WeAreCarsTest_",StringComparison.Ordinal)||!b.DataSource.Contains("WeAreCarsTest_"))throw new Exception("Refusing a non-test database");
            var result=DatabaseInitializer.Initialize(connection);if(!result.IsSuccessful)throw new Exception(result.Message);
            return new BookingRepository(connection);
        }
        public static void AuthoritativePricingAndSnapshot()
        {
            var repo=Fixture();var car=repo.GetAvailableCar(CarType.CityCar,FuelType.Petrol);
            var spoof=ValidBooking(new Car(car.CarId,car.RegistrationNumber,CarType.SUV,FuelType.Hybrid,false,1m));
            Reject<InvalidOperationException>(()=>repo.SaveBooking(spoof));
            var valid=ValidBooking(car);valid.ReviewedTotal=111m;int id=repo.SaveBooking(valid);
            string connection=Environment.GetEnvironmentVariable("WEARECARS_TEST_CONNECTION_STRING");
            Sql(connection,"UPDATE dbo.Cars SET DailyRate=99 WHERE CarId="+car.CarId);
            var saved=repo.GetBooking(id);Check(saved.SelectedCar.DailyRate==25m&&saved.TotalCost==111m&&saved.Pricing.GrandTotal==111m,"Historic pricing changed with current fleet rate");
            Check(saved.CustomerAge==26&&saved.HasValidLicense&&saved.StaffUsername=="sta001"&&saved.DueAt.HasValue,"Required booking metadata missing");
            Check(repo.SaveBooking(valid)==id,"Retried request duplicated a booking");
            repo.ReturnBooking(id,"sta001");Check(repo.GetBooking(id).Status=="Returned"&&!repo.GetCar(car.CarId).IsRented,"Return did not atomically close rental");
        }
        public static void ConcurrentSave()
        {
            var repo=Fixture();var car=repo.GetAvailableCar(CarType.SUV,FuelType.Hybrid);int accepted=0,rejected=0;
            Parallel.For(0,2,i=>{try{repo.SaveBooking(ValidBooking(new Car(car.CarId,car.RegistrationNumber,car.CarType,car.FuelType,false,car.DailyRate)));Interlocked.Increment(ref accepted);}catch(InvalidOperationException){Interlocked.Increment(ref rejected);}});
            Check(accepted==1&&rejected==1,"Concurrent double booking was not prevented");repo.ReturnBooking(repo.GetActiveBookingId(car.CarId),"sta001");
        }
        public static void SchemaConstraintsAndAudit()
        {
            var repo=Fixture();string connection=Environment.GetEnvironmentVariable("WEARECARS_TEST_CONNECTION_STRING");
            Reject<SqlException>(()=>Sql(connection,"INSERT dbo.Bookings(CarId,CustomerFirstName,CustomerSurname,CustomerAddress,CustomerAge,HasValidLicense,NumberOfDays,CarType,FuelType,TotalCost) VALUES(1,'','','',26,0,3,'Unknown','Unknown',0)"));
            var car=repo.GetAvailableCar(CarType.FamilyCar,FuelType.Diesel);var b=ValidBooking(car);int id=repo.SaveBooking(b);
            Reject<SqlException>(()=>Sql(connection,"INSERT dbo.BookingOptionalExtras(BookingId,ExtraName,DailyPrice,NumberOfDays) VALUES("+id+",'Unlimited Mileage',10,3)"));
            Reject<SqlException>(()=>Sql(connection,"INSERT dbo.BookingOptionalExtras(BookingId,ExtraName,DailyPrice,NumberOfDays) VALUES("+id+",'Unknown',10,3)"));
            Sql(connection,"DELETE FROM dbo.BookingOptionalExtras WHERE BookingId="+id+" AND ExtraName='Breakdown Cover'");
            Reject<SqlException>(()=>Sql(connection,"INSERT dbo.BookingOptionalExtras(BookingId,ExtraName,DailyPrice,NumberOfDays) VALUES("+id+",'Breakdown Cover',2,28)"));
            using(var con=new SqlConnection(connection)){con.Open();using(var cmd=new SqlCommand("SELECT COUNT(*) FROM dbo.AuditEvents WHERE BookingId="+id+" AND Action='BookingCreated'",con))Check(Convert.ToInt32(cmd.ExecuteScalar())==1,"Transaction audit is missing");}
            repo.ReturnBooking(id,"sta001");
        }
        public static void StaffAuthenticationAndLockout()
        {
            Fixture();string connection=Environment.GetEnvironmentVariable("WEARECARS_TEST_CONNECTION_STRING");
            Check(StaffAuthentication.Authenticate(connection,"sta001","givemethekeys123")!=null,"Seeded test staff cannot authenticate");
            for(int i=0;i<5;i++)Check(StaffAuthentication.Authenticate(connection,"sta001","wrong-password")==null,"Wrong password accepted");
            Check(StaffAuthentication.Authenticate(connection,"sta001","givemethekeys123")==null,"Lockout bypassed");
            Sql(connection,"UPDATE dbo.StaffAccounts SET FailedAttempts=0,LockedUntil=NULL WHERE Username='sta001'");
        }
        public static void BackupRestoreAndRetention()
        {
            var repo=Fixture();string connection=Environment.GetEnvironmentVariable("WEARECARS_TEST_CONNECTION_STRING");
            var car=repo.GetAvailableCar(CarType.FamilyCar,FuelType.Petrol);var booking=ValidBooking(car);int id=repo.SaveBooking(booking);
            repo.ReturnBooking(id,"sta001");Sql(connection,"UPDATE dbo.Bookings SET ReturnedAt=DATEADD(day,-10,SYSUTCDATETIME()) WHERE BookingId="+id);
            int eligible=DatabaseMaintenance.AnonymizeClosedRecords(connection,5,false);
            Check(eligible>=1&&!repo.GetBooking(id).IsAnonymized,"Retention preview changed customer records");
            decimal total=repo.GetBooking(id).TotalCost;
            DatabaseMaintenance.AnonymizeClosedRecords(connection,5,true);
            Check(repo.GetBooking(id).IsAnonymized&&repo.GetBooking(id).TotalCost==total,"Retention did not preserve financial records");
            var builder=new SqlConnectionStringBuilder(connection);string clone=builder.InitialCatalog+"_Restore";
            string backup=System.IO.Path.Combine(System.IO.Path.GetTempPath(),builder.InitialCatalog+".bak");
            try
            {
                DatabaseMaintenance.Backup(connection,backup);
                Reject<ArgumentException>(()=>DatabaseMaintenance.Backup(connection,backup));
                DatabaseMaintenance.RestoreToNewCatalog(connection,backup,clone);
                var restored=new SqlConnectionStringBuilder(connection){InitialCatalog=clone};
                Check(new BookingRepository(restored.ConnectionString).GetBooking(id).TotalCost==total,"Restored booking differs from backup");
                Reject<InvalidOperationException>(()=>DatabaseMaintenance.RestoreToNewCatalog(connection,backup,clone));
            }
            finally {if(System.IO.File.Exists(backup))System.IO.File.Delete(backup);}
        }
    }
}
