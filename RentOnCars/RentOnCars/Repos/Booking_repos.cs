using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using RentOnCars.Models;
using RentOnCars.Services;

namespace RentOnCars.Repos
{
    public class DashboardStats
    {
        public int TotalVehicles { get; set; }
        public int AvailableVehicles { get; set; }
        public int RentedVehicles { get; set; }
        public int TotalBookings { get; set; }
        public int ActiveBookings { get; set; }
    }
    public class BookingPage
    {
        public List<Booking> Items { get; set; } = new List<Booking>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
    public class FleetPage
    {
        public List<Car> Items { get; set; } = new List<Car>();
        public int Total { get; set; }
    }

    public class BookingRepository
    {
        private readonly string connectionString;
        public BookingRepository() : this(DatabaseInitializer.ConnectionString) { }
        public BookingRepository(string customConnectionString)
        {
            var builder = new SqlConnectionStringBuilder(customConnectionString ?? throw new ArgumentNullException(nameof(customConnectionString)));
            builder.ConnectTimeout = Math.Min(builder.ConnectTimeout, 5);
            connectionString = builder.ConnectionString;
        }
        private SqlConnection Open() { var con = new SqlConnection(connectionString); con.Open(); return con; }
        public bool TestConnection(out string error)
        {
            try { using (var con = Open()) using (var cmd = new SqlCommand("SELECT MAX(Version) FROM dbo.SchemaVersions", con))
                if (Convert.ToInt32(cmd.ExecuteScalar()) != 2) throw new InvalidOperationException("Database schema is incompatible. Run database preparation.");
                error = null; return true; }
            catch (Exception ex) { error = ex.Message; return false; }
        }
        public DashboardStats GetDashboardStats()
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(@"SELECT COUNT(*),SUM(CASE WHEN IsRented=0 THEN 1 ELSE 0 END),SUM(CASE WHEN IsRented=1 THEN 1 ELSE 0 END),
(SELECT COUNT(*) FROM dbo.Bookings),(SELECT COUNT(*) FROM dbo.Bookings WHERE Status='Active') FROM dbo.Cars", con))
            using (var rd = cmd.ExecuteReader())
            {
                rd.Read(); return new DashboardStats { TotalVehicles=rd.GetInt32(0),AvailableVehicles=rd.IsDBNull(1)?0:rd.GetInt32(1),
                    RentedVehicles=rd.IsDBNull(2)?0:rd.GetInt32(2),TotalBookings=rd.GetInt32(3),ActiveBookings=rd.GetInt32(4) };
            }
        }
        private static Car ReadCar(SqlDataReader rd) => new Car {
            CarId=Convert.ToInt32(rd["CarId"]),RegistrationNumber=Convert.ToString(rd["RegistrationNumber"]),
            IsRented=Convert.ToBoolean(rd["IsRented"]),DailyRate=Convert.ToDecimal(rd["DailyRate"]),
            CarType=VehicleEnumExtensions.ParseCarType(Convert.ToString(rd["CarType"])),FuelType=VehicleEnumExtensions.ParseFuelType(Convert.ToString(rd["FuelType"]))
        };
        public Car GetAvailableCar(CarType category, FuelType fuel)
        {
            if (!Enum.IsDefined(typeof(CarType),category) || !Enum.IsDefined(typeof(FuelType),fuel)) throw new ArgumentException("Invalid category or fuel.");
            using (var con=Open())
            using (var cmd=new SqlCommand("SELECT TOP 1 * FROM dbo.Cars WHERE IsRented=0 AND CarType=@cat AND FuelType=@fuel ORDER BY CarId",con))
            {
                cmd.Parameters.Add("@cat",SqlDbType.NVarChar,30).Value=category.ToString(); cmd.Parameters.Add("@fuel",SqlDbType.NVarChar,30).Value=fuel.ToString();
                using (var rd=cmd.ExecuteReader()) return rd.Read()?ReadCar(rd):null;
            }
        }
        public Car GetCar(int id)
        {
            using (var con=Open()) using (var cmd=new SqlCommand("SELECT * FROM dbo.Cars WHERE CarId=@id",con))
            { cmd.Parameters.Add("@id",SqlDbType.Int).Value=id; using (var rd=cmd.ExecuteReader()) return rd.Read()?ReadCar(rd):null; }
        }
        private static string SearchTerm(string query) => "%" + (query ?? "").Trim().Replace("[","[[]").Replace("%","[%]").Replace("_","[_]") + "%";
        public FleetPage GetFleet(string query="",string status="all",int page=1,int pageSize=25)
        {
            page=Math.Max(1,page); pageSize=Math.Max(1,Math.Min(100,pageSize));
            using (var con=Open())
            using (var cmd=new SqlCommand(@"SELECT *,COUNT(*) OVER() AS RecordCount FROM dbo.Cars
WHERE (RegistrationNumber LIKE @q OR CarType LIKE @q OR FuelType LIKE @q)
AND (@status='all' OR (@status='rented' AND IsRented=1) OR (@status='available' AND IsRented=0))
ORDER BY CarId OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY",con))
            {
                cmd.Parameters.Add("@q",SqlDbType.NVarChar,410).Value=SearchTerm(query); cmd.Parameters.Add("@status",SqlDbType.NVarChar,20).Value=status;
                cmd.Parameters.Add("@skip",SqlDbType.Int).Value=(page-1)*pageSize; cmd.Parameters.Add("@take",SqlDbType.Int).Value=pageSize;
                var result=new FleetPage();
                using(var rd=cmd.ExecuteReader()) while(rd.Read()) { result.Total=Convert.ToInt32(rd["RecordCount"]); result.Items.Add(ReadCar(rd)); }
                return result;
            }
        }
        public int GetAvailableCarsCount(CarType category,FuelType fuel)
        {
            using(var con=Open())using(var cmd=new SqlCommand("SELECT COUNT(*) FROM dbo.Cars WHERE IsRented=0 AND CarType=@category AND FuelType=@fuel",con))
            {cmd.Parameters.Add("@category",SqlDbType.NVarChar,30).Value=category.ToString();cmd.Parameters.Add("@fuel",SqlDbType.NVarChar,30).Value=fuel.ToString();return Convert.ToInt32(cmd.ExecuteScalar());}
        }
        public List<Car> GetRentedCars() => GetFleet("", "rented",1,100).Items;
        public List<Booking> GetDetails() => GetDetails("",1,50).Items;
        public Booking GetBooking(int id) => GetDetails("",1,1,id).Items.FirstOrDefault();
        public BookingPage GetDetails(string query,int page=1,int pageSize=25,int bookingId=0,string sort="newest")
        {
            page=Math.Max(1,page); pageSize=Math.Max(1,Math.Min(100,pageSize));
            string order=sort=="customer"?"ORDER BY b.CustomerSurname ASC,b.CustomerFirstName ASC,b.BookingId DESC":sort=="total"?"ORDER BY b.TotalCost DESC,b.BookingId DESC":"ORDER BY b.BookingId DESC";
            using(var con=Open())
            using(var cmd=new SqlCommand(@"WITH selected AS (
 SELECT b.*,c.RegistrationNumber,c.IsRented,COUNT(*) OVER() AS RecordCount FROM dbo.Bookings b JOIN dbo.Cars c ON c.CarId=b.CarId
 WHERE (@id=0 OR b.BookingId=@id) AND (b.CustomerFirstName+' '+b.CustomerSurname LIKE @q OR c.RegistrationNumber LIKE @q
 OR 'WAC-BK-'+RIGHT('00000'+CAST(b.BookingId AS varchar(12)),5) LIKE @q OR b.CustomerAddress LIKE @q)
 ORDER BY b.BookingId DESC OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY)
SELECT s.*,e.MileageRate,e.CoverRate FROM selected s OUTER APPLY(
 SELECT MAX(CASE WHEN ExtraName='Unlimited Mileage' THEN DailyPrice END) MileageRate,
 MAX(CASE WHEN ExtraName='Breakdown Cover' THEN DailyPrice END) CoverRate
 FROM dbo.BookingOptionalExtras WHERE BookingId=s.BookingId) e ORDER BY s.BookingId DESC".Replace("ORDER BY b.BookingId DESC",order).Replace("ORDER BY s.BookingId DESC",order.Replace("b.","s.")),con))
            {
                cmd.Parameters.Add("@q",SqlDbType.NVarChar,410).Value=SearchTerm(query); cmd.Parameters.Add("@id",SqlDbType.Int).Value=bookingId;
                cmd.Parameters.Add("@skip",SqlDbType.Int).Value=(page-1)*pageSize; cmd.Parameters.Add("@take",SqlDbType.Int).Value=pageSize;
                var result=new BookingPage { Page=page,PageSize=pageSize };
                using(var rd=cmd.ExecuteReader()) while(rd.Read())
                {
                    var extras=new OptionalExtras { UnlimitedMileage=rd["MileageRate"]!=DBNull.Value,BreakdownCover=rd["CoverRate"]!=DBNull.Value,
                        MileageRate=rd["MileageRate"]==DBNull.Value?0:Convert.ToDecimal(rd["MileageRate"]),
                        CoverRate=rd["CoverRate"]==DBNull.Value?0:Convert.ToDecimal(rd["CoverRate"]) };
                    var car=new Car { CarId=Convert.ToInt32(rd["CarId"]),RegistrationNumber=Convert.ToString(rd["RegistrationNumber"]),
                        IsRented=Convert.ToBoolean(rd["IsRented"]),DailyRate=rd["DailyBaseRate"]==DBNull.Value?25:Convert.ToDecimal(rd["DailyBaseRate"]),
                        CarType=VehicleEnumExtensions.ParseCarType(Convert.ToString(rd["CarType"])),FuelType=VehicleEnumExtensions.ParseFuelType(Convert.ToString(rd["FuelType"])) };
                    int days=Convert.ToInt32(rd["NumberOfDays"]);
                    var pricing=new PricingBreakdown { RentalDays=days,DailyBaseRate=car.DailyRate,SelectedCarType=car.CarType,SelectedFuelType=car.FuelType,
                        CarTypeSurcharge=rd["VehicleSurcharge"]==DBNull.Value?0:Convert.ToDecimal(rd["VehicleSurcharge"]),
                        FuelTypeSurcharge=rd["FuelSurcharge"]==DBNull.Value?0:Convert.ToDecimal(rd["FuelSurcharge"]),
                        UnlimitedMileageSelected=extras.UnlimitedMileage,BreakdownCoverSelected=extras.BreakdownCover,
                        UnlimitedMileageRate=extras.MileageRate,BreakdownCoverRate=extras.CoverRate,IsComplete=true };
                    result.Items.Add(new Booking { BookingId=Convert.ToInt32(rd["BookingId"]),CarId=car.CarId,SelectedCar=car,
                        CustomerFirstName=Convert.ToString(rd["CustomerFirstName"]),CustomerSurname=Convert.ToString(rd["CustomerSurname"]),
                        CustomerAddress=Convert.ToString(rd["CustomerAddress"]),CustomerAge=Convert.ToInt32(rd["CustomerAge"]),
                        HasValidLicense=Convert.ToBoolean(rd["HasValidLicense"]),NumberOfDays=days,TotalCost=Convert.ToDecimal(rd["TotalCost"]),
                        CreatedAt=DateTime.SpecifyKind(Convert.ToDateTime(rd["CreatedAt"]),DateTimeKind.Utc),Extras=extras,Pricing=pricing,
                        Status=Convert.ToString(rd["Status"]),StaffUsername=Convert.ToString(rd["StaffUsername"]),ReturnedBy=Convert.ToString(rd["ReturnedBy"]),
                        StartsAt=NullableDate(rd["StartsAt"]),DueAt=NullableDate(rd["DueAt"]),ReturnedAt=NullableDate(rd["ReturnedAt"]),
                        IsAnonymized=Convert.ToBoolean(rd["IsAnonymized"]),PricingSnapshotVersion=Convert.ToInt32(rd["PricingSnapshotVersion"]),RequestId=Convert.ToString(rd["RequestId"]) });
                    result.Total=Convert.ToInt32(rd["RecordCount"]);
                }
                return result;
            }
        }
        private static DateTime? NullableDate(object value) => value==DBNull.Value?(DateTime?)null:DateTime.SpecifyKind(Convert.ToDateTime(value),DateTimeKind.Utc);
        public bool IsStaffActive(string actor)
        {
            using(var con=Open())
            using(var cmd=new SqlCommand("SELECT COUNT(*) FROM dbo.StaffAccounts WHERE Username=@actor AND IsActive=1 AND (IsDemo=0 OR @demo=1)",con))
            {cmd.Parameters.Add("@actor",SqlDbType.NVarChar,50).Value=actor;cmd.Parameters.Add("@demo",SqlDbType.Bit).Value=Environment.GetEnvironmentVariable("WEARECARS_DEMO")=="1";return Convert.ToInt32(cmd.ExecuteScalar())==1;}
        }
        private static void RequireActor(SqlConnection con,SqlTransaction tx,string actor)
        {
            if(string.IsNullOrWhiteSpace(actor)||actor.Length>50) throw new UnauthorizedAccessException("A signed-in staff account is required.");
            using(var cmd=new SqlCommand("SELECT COUNT(*) FROM dbo.StaffAccounts WHERE Username=@actor AND IsActive=1 AND (IsDemo=0 OR @demo=1)",con,tx))
            { cmd.Parameters.Add("@actor",SqlDbType.NVarChar,50).Value=actor; cmd.Parameters.Add("@demo",SqlDbType.Bit).Value=Environment.GetEnvironmentVariable("WEARECARS_DEMO")=="1"; if(Convert.ToInt32(cmd.ExecuteScalar())!=1) throw new UnauthorizedAccessException("Staff account is unavailable."); }
        }
        private static void Audit(SqlConnection con,SqlTransaction tx,string actor,string action,int bookingId,string detail)
        {
            using(var cmd=new SqlCommand("INSERT dbo.AuditEvents(Actor,Action,BookingId,Detail) VALUES(@actor,@action,@id,@detail)",con,tx))
            { cmd.Parameters.Add("@actor",SqlDbType.NVarChar,50).Value=actor;cmd.Parameters.Add("@action",SqlDbType.NVarChar,30).Value=action;
              cmd.Parameters.Add("@id",SqlDbType.Int).Value=bookingId;cmd.Parameters.Add("@detail",SqlDbType.NVarChar,300).Value=detail;cmd.ExecuteNonQuery(); }
        }
        private static void Money(SqlCommand cmd,string key,decimal value)
        { var parameter=cmd.Parameters.Add(key,SqlDbType.Decimal);parameter.Precision=10;parameter.Scale=2;parameter.Value=value; }

        public int SaveBooking(Booking booking)
        {
            if(booking==null||booking.SelectedCar==null) throw new ArgumentException("A booking and allocated vehicle are required.");
            if(string.IsNullOrWhiteSpace(booking.RequestId)||booking.RequestId.Length>64) throw new ArgumentException("A valid request ID is required.");
            using(var con=Open()) using(var tx=con.BeginTransaction())
            {
                try
                {
                    RequireActor(con,tx,booking.StaffUsername);
                    using(var cmd=new SqlCommand("SELECT BookingId,StaffUsername FROM dbo.Bookings WITH(UPDLOCK,HOLDLOCK) WHERE RequestId=@request",con,tx))
                    {
                        cmd.Parameters.Add("@request",SqlDbType.NVarChar,64).Value=booking.RequestId;
                        using(var rd=cmd.ExecuteReader()) if(rd.Read())
                        {
                            if(rd.GetString(1)!=booking.StaffUsername) throw new InvalidOperationException("Request reference belongs to another staff member.");
                            int existing=rd.GetInt32(0); rd.Close(); tx.Commit(); booking.BookingId=existing; return existing;
                        }
                    }
                    Car actual;
                    using(var cmd=new SqlCommand("SELECT * FROM dbo.Cars WITH(UPDLOCK,ROWLOCK,HOLDLOCK) WHERE CarId=@id AND IsRented=0",con,tx))
                    {
                        cmd.Parameters.Add("@id",SqlDbType.Int).Value=booking.SelectedCar.CarId;
                        using(var rd=cmd.ExecuteReader()) { if(!rd.Read()) throw new InvalidOperationException("This vehicle is no longer available. Choose another vehicle and review again."); actual=ReadCar(rd); }
                    }
                    if(actual.CarType!=booking.SelectedCar.CarType||actual.FuelType!=booking.SelectedCar.FuelType||actual.DailyRate!=booking.SelectedCar.DailyRate)
                        throw new InvalidOperationException("Vehicle terms changed. Refresh the quotation and review it again.");
                    var validation=BookingValidator.Validate(booking.CustomerFirstName,booking.CustomerSurname,booking.CustomerAddress,booking.CustomerAge,
                        booking.HasValidLicense,booking.NumberOfDays,actual.CarType,actual.FuelType,actual);
                    if(!validation.IsValid) throw new ArgumentException(validation.BuildSummary());
                    var pricing=PricingCalculator.CalculateEstimate(booking.NumberOfDays,actual.CarType,actual.FuelType,
                        booking.Extras!=null&&booking.Extras.UnlimitedMileage,booking.Extras!=null&&booking.Extras.BreakdownCover,actual.DailyRate);
                    if(booking.ReviewedTotal.HasValue&&booking.ReviewedTotal.Value!=pricing.GrandTotal) throw new InvalidOperationException("The quotation changed. Review the new total before saving.");
                    DateTime start=DateTime.UtcNow;
                    using(var cmd=new SqlCommand(@"INSERT dbo.Bookings(CarId,CustomerFirstName,CustomerSurname,CustomerAddress,CustomerAge,HasValidLicense,NumberOfDays,
CarType,FuelType,TotalCost,DailyBaseRate,VehicleSurcharge,FuelSurcharge,PricingSnapshotVersion,StaffUsername,StartsAt,DueAt,Status,RequestId)
VALUES(@car,@first,@surname,@address,@age,1,@days,@category,@fuel,@total,@rate,@vehicleFee,@fuelFee,1,@staff,@start,@due,'Active',@request);
SELECT CAST(SCOPE_IDENTITY() AS int)",con,tx))
                    {
                        cmd.Parameters.Add("@car",SqlDbType.Int).Value=actual.CarId;cmd.Parameters.Add("@first",SqlDbType.NVarChar,50).Value=booking.CustomerFirstName.Trim();
                        cmd.Parameters.Add("@surname",SqlDbType.NVarChar,50).Value=booking.CustomerSurname.Trim();cmd.Parameters.Add("@address",SqlDbType.NVarChar,150).Value=booking.CustomerAddress.Trim();
                        cmd.Parameters.Add("@age",SqlDbType.Int).Value=booking.CustomerAge;cmd.Parameters.Add("@days",SqlDbType.Int).Value=booking.NumberOfDays;
                        cmd.Parameters.Add("@category",SqlDbType.NVarChar,30).Value=actual.CarType.ToString();cmd.Parameters.Add("@fuel",SqlDbType.NVarChar,30).Value=actual.FuelType.ToString();
                        Money(cmd,"@total",pricing.GrandTotal);Money(cmd,"@rate",actual.DailyRate);Money(cmd,"@vehicleFee",pricing.CarTypeSurcharge);Money(cmd,"@fuelFee",pricing.FuelTypeSurcharge);
                        cmd.Parameters.Add("@staff",SqlDbType.NVarChar,50).Value=booking.StaffUsername;cmd.Parameters.Add("@start",SqlDbType.DateTime2).Value=start;
                        cmd.Parameters.Add("@due",SqlDbType.DateTime2).Value=start.AddDays(booking.NumberOfDays);cmd.Parameters.Add("@request",SqlDbType.NVarChar,64).Value=booking.RequestId;
                        booking.BookingId=Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    if(pricing.UnlimitedMileageSelected) AddExtra(con,tx,booking.BookingId,"Unlimited Mileage",pricing.UnlimitedMileageRate,booking.NumberOfDays);
                    if(pricing.BreakdownCoverSelected) AddExtra(con,tx,booking.BookingId,"Breakdown Cover",pricing.BreakdownCoverRate,booking.NumberOfDays);
                    using(var cmd=new SqlCommand("UPDATE dbo.Cars SET IsRented=1 WHERE CarId=@id AND IsRented=0",con,tx))
                    {cmd.Parameters.Add("@id",SqlDbType.Int).Value=actual.CarId;if(cmd.ExecuteNonQuery()!=1)throw new InvalidOperationException("Vehicle allocation failed.");}
                    Audit(con,tx,booking.StaffUsername,"BookingCreated",booking.BookingId,actual.RegistrationNumber);
                    tx.Commit();booking.SelectedCar=actual;actual.MarkAsRented();booking.CarId=actual.CarId;booking.TotalCost=pricing.GrandTotal;booking.Pricing=pricing;
                    booking.StartsAt=start;booking.DueAt=start.AddDays(booking.NumberOfDays);booking.CreatedAt=start;booking.PricingSnapshotVersion=1;
                    return booking.BookingId;
                }
                catch {try{tx.Rollback();}catch(InvalidOperationException){}throw;}
            }
        }
        private static void AddExtra(SqlConnection con,SqlTransaction tx,int id,string name,decimal rate,int days)
        {
            using(var cmd=new SqlCommand("INSERT dbo.BookingOptionalExtras(BookingId,ExtraName,DailyPrice,NumberOfDays) VALUES(@id,@name,@rate,@days)",con,tx))
            {cmd.Parameters.Add("@id",SqlDbType.Int).Value=id;cmd.Parameters.Add("@name",SqlDbType.NVarChar,50).Value=name;
             Money(cmd,"@rate",rate);cmd.Parameters.Add("@days",SqlDbType.Int).Value=days;cmd.ExecuteNonQuery();}
        }
        public int GetActiveBookingId(int carId)
        {
            using(var con=Open())using(var cmd=new SqlCommand("SELECT BookingId FROM dbo.Bookings WHERE CarId=@id AND Status='Active'",con))
            {cmd.Parameters.Add("@id",SqlDbType.Int).Value=carId;object result=cmd.ExecuteScalar();return result==null?0:Convert.ToInt32(result);}
        }
        public void ReturnBooking(int bookingId,string actor)
        {
            using(var con=Open())using(var tx=con.BeginTransaction())
            {
                try
                {
                    RequireActor(con,tx,actor);int carId;
                    using(var cmd=new SqlCommand("SELECT CarId FROM dbo.Bookings WHERE BookingId=@id",con,tx))
                    {cmd.Parameters.Add("@id",SqlDbType.Int).Value=bookingId;object result=cmd.ExecuteScalar();if(result==null)throw new ArgumentException("Booking not found.");carId=Convert.ToInt32(result);}
                    using(var cmd=new SqlCommand("SELECT CarId FROM dbo.Cars WITH(UPDLOCK,HOLDLOCK) WHERE CarId=@car",con,tx))
                    {cmd.Parameters.Add("@car",SqlDbType.Int).Value=carId;cmd.ExecuteScalar();}
                    using(var cmd=new SqlCommand("UPDATE dbo.Bookings SET Status='Returned',ReturnedAt=SYSUTCDATETIME(),ReturnedBy=@actor WHERE BookingId=@id AND Status='Active'",con,tx))
                    {cmd.Parameters.Add("@id",SqlDbType.Int).Value=bookingId;cmd.Parameters.Add("@actor",SqlDbType.NVarChar,50).Value=actor;if(cmd.ExecuteNonQuery()!=1)throw new InvalidOperationException("This booking is already closed.");}
                    using(var cmd=new SqlCommand("UPDATE dbo.Cars SET IsRented=0 WHERE CarId=@car",con,tx))
                    {cmd.Parameters.Add("@car",SqlDbType.Int).Value=carId;cmd.ExecuteNonQuery();}
                    Audit(con,tx,actor,"VehicleReturned",bookingId,"Vehicle checked in");tx.Commit();
                }
                catch {try{tx.Rollback();}catch(InvalidOperationException){}throw;}
            }
        }
    }
    public class Booking_repos:BookingRepository
    { public Booking_repos():base(){} public Booking_repos(string connection):base(connection){} }
}

