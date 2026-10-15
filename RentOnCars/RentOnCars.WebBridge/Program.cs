using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using RentOnCars;
using RentOnCars.Models;
using RentOnCars.Repos;
using RentOnCars.Services;

class Program
{
    static readonly JavaScriptSerializer Json=new JavaScriptSerializer { MaxJsonLength=65536 };
    static BookingRepository Repository;
    static string Text(Dictionary<string,object> p,string key,string fallback="") => p.ContainsKey(key)?Convert.ToString(p[key],CultureInfo.InvariantCulture):fallback;
    static int Integer(Dictionary<string,object> p,string key,int fallback=0)
    {
        if(!p.ContainsKey(key))return fallback;
        if(p[key] is string || p[key] is bool || p[key]==null)throw new ArgumentException(key+" must be a whole number.");
        decimal n=Convert.ToDecimal(p[key],CultureInfo.InvariantCulture);
        if(n!=Math.Truncate(n)||n>int.MaxValue||n<int.MinValue)throw new ArgumentException(key+" must be a whole number.");
        return (int)n;
    }
    static bool Flag(Dictionary<string,object> p,string key) => p.ContainsKey(key)&&p[key] is bool&&((bool)p[key]);
    static string Date(DateTime? date)=>date.HasValue?date.Value.ToUniversalTime().ToString("o",CultureInfo.InvariantCulture):null;
    static object CarDto(Car c)=>new { id=c.CarId,registration=c.RegistrationNumber,category=c.CarType.ToString(),fuel=c.FuelType.ToString(),dailyRate=c.DailyRate,rented=c.IsRented };
    static object PriceDto(PricingBreakdown p)=>new { days=p.RentalDays,dailyRate=p.DailyBaseRate,baseTotal=p.BaseRentTotal,vehicleFee=p.CarTypeSurcharge,
        fuelFee=p.FuelTypeSurcharge,mileageRate=p.UnlimitedMileageRate,coverRate=p.BreakdownCoverRate,mileageTotal=p.UnlimitedMileageCost,
        coverTotal=p.BreakdownCoverCost,total=p.GrandTotal };
    static object BookingDto(Booking b)=>new { id=b.BookingId,reference="WAC-BK-"+b.BookingId.ToString("D5"),firstName=b.CustomerFirstName,surname=b.CustomerSurname,
        address=b.CustomerAddress,age=b.CustomerAge,licence=b.HasValidLicense,days=b.NumberOfDays,car=CarDto(b.SelectedCar),pricing=PriceDto(b.Pricing),
        total=b.TotalCost,mileage=b.Extras.UnlimitedMileage,cover=b.Extras.BreakdownCover,status=b.Status,createdAt=Date(b.CreatedAt),startsAt=Date(b.StartsAt),
        dueAt=Date(b.DueAt),returnedAt=Date(b.ReturnedAt),staff=b.StaffUsername,returnedBy=b.ReturnedBy,anonymized=b.IsAnonymized,legacySnapshot=b.PricingSnapshotVersion==0 };
    static object Handle(string action,Dictionary<string,object> p,string actor)
    {
        if(action=="health")return new { database=Repository.TestConnection(out string error),message=error,schemaVersion=2 };
        if(action=="login") {var identity=StaffAuthentication.Authenticate(DatabaseInitializer.ConnectionString,Text(p,"username"),Text(p,"password"));return identity==null?null:new { username=identity.Username,role=identity.Role };}
        if(action=="catalog")return new { baseDaily=PricingCalculator.BaseRentalDailyRate,mileageDaily=PricingCalculator.UnlimitedMileageDailyRate,coverDaily=PricingCalculator.BreakdownCoverDailyRate,
            categories=Enum.GetValues(typeof(CarType)).Cast<CarType>().Select(c=>new {value=c.ToString(),surcharge=c.GetSurcharge()}),
            fuels=Enum.GetValues(typeof(FuelType)).Cast<FuelType>().Select(f=>new {value=f.ToString(),surcharge=f.GetSurcharge()}) };
        if(action=="public-fleet") {
            var page=Repository.GetFleet(Text(p,"search"),"available",Integer(p,"page",1),Integer(p,"pageSize",50));
            return new {items=page.Items.Select(CarDto),total=page.Total};
        }
        if(action=="estimate")
        {
            int days=Integer(p,"days"); if(days<1||days>28)throw new ArgumentException("Choose 1 to 28 whole days.");
            var category=VehicleEnumExtensions.ParseCarType(Text(p,"category"));var fuel=VehicleEnumExtensions.ParseFuelType(Text(p,"fuel"));
            int selectedId=Integer(p,"carId");
            Car car=selectedId>0?Repository.GetCar(selectedId):Repository.GetAvailableCar(category,fuel);
            if(car==null||car.IsRented||car.CarType!=category||car.FuelType!=fuel)return new { available=false,car=(object)null,pricing=(object)null };
            var pricing=PricingCalculator.CalculateEstimate(days,category,fuel,Flag(p,"mileage"),Flag(p,"cover"),car.DailyRate);
            return new { available=true,car=CarDto(car),pricing=PriceDto(pricing) };
        }
        if(action=="public-lookup")
        {
            string refStr=Text(p,"reference").Trim().ToUpperInvariant();
            string surname=Text(p,"surname").Trim();
            if(string.IsNullOrEmpty(refStr)||string.IsNullOrEmpty(surname)) throw new ArgumentException("Please enter your booking reference and surname.");
            int bookingId=0;
            if(refStr.StartsWith("WAC-BK-") && int.TryParse(refStr.Substring(7), out int parsedId)) bookingId=parsedId;
            else if(int.TryParse(refStr, out int plainId)) bookingId=plainId;
            if(bookingId<=0) throw new ArgumentException("Booking not found. Please check your reference number.");
            var b=Repository.GetBooking(bookingId);
            if(b==null || !string.Equals(b.CustomerSurname.Trim(), surname, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Booking not found or surname does not match our records.");
            return BookingDto(b);
        }
        if(action=="quote" && actor=="self-service")
        {
            CarType category=VehicleEnumExtensions.ParseCarType(Text(p,"category"));FuelType fuel=VehicleEnumExtensions.ParseFuelType(Text(p,"fuel"));
            int selectedId=Integer(p,"carId");
            Car car=selectedId>0?Repository.GetCar(selectedId):Repository.GetAvailableCar(category,fuel);
            var validation=BookingValidator.Validate(Text(p,"firstName"),Text(p,"surname"),Text(p,"address"),Integer(p,"age"),Flag(p,"licence"),Integer(p,"days"),category,fuel,car);
            if(!validation.IsValid)throw new ArgumentException(validation.BuildSummary());
            var pricing=PricingCalculator.CalculateEstimate(Integer(p,"days"),category,fuel,Flag(p,"mileage"),Flag(p,"cover"),car.DailyRate);
            return new { firstName=Text(p,"firstName").Trim(),surname=Text(p,"surname").Trim(),address=Text(p,"address").Trim(),age=Integer(p,"age"),licence=true,
                days=Integer(p,"days"),mileage=Flag(p,"mileage"),cover=Flag(p,"cover"),car=CarDto(car),pricing=PriceDto(pricing),requestId=Guid.NewGuid().ToString("N") };
        }
        if(action=="save" && actor=="self-service")
        {
            var vehicle=(Dictionary<string,object>)p["car"];var price=(Dictionary<string,object>)p["pricing"];
            var booking=new Booking { CustomerFirstName=Text(p,"firstName"),CustomerSurname=Text(p,"surname"),CustomerAddress=Text(p,"address"),CustomerAge=Integer(p,"age"),
                HasValidLicense=Flag(p,"licence"),NumberOfDays=Integer(p,"days"),StaffUsername=actor,RequestId=Text(p,"requestId"),
                ReviewedTotal=Convert.ToDecimal(price["total"]),Extras=new OptionalExtras(Flag(p,"mileage"),Flag(p,"cover")),
                SelectedCar=new Car(Integer(vehicle,"id"),Text(vehicle,"registration"),VehicleEnumExtensions.ParseCarType(Text(vehicle,"category")),
                    VehicleEnumExtensions.ParseFuelType(Text(vehicle,"fuel")),false,Convert.ToDecimal(vehicle["dailyRate"])) };
            int id=Repository.SaveBooking(booking);return BookingDto(Repository.GetBooking(id));
        }
        if(string.IsNullOrWhiteSpace(actor)||!Repository.IsStaffActive(actor))throw new UnauthorizedAccessException("Sign in to continue.");
        if(action=="stats")return Repository.GetDashboardStats();
        if(action=="fleet") {var page=Repository.GetFleet(Text(p,"search"),Text(p,"status","all"),Integer(p,"page",1),Integer(p,"pageSize",25));return new {items=page.Items.Select(CarDto),total=page.Total};}
        if(action=="history") {var page=Repository.GetDetails(Text(p,"search"),Integer(p,"page",1),Integer(p,"pageSize",25),0,Text(p,"sort","newest"));return new {items=page.Items.Select(BookingDto),total=page.Total};}
        if(action=="booking") {var b=Repository.GetBooking(Integer(p,"id"));if(b==null)throw new ArgumentException("Booking not found.");return BookingDto(b);}
        if(action=="return") {Repository.ReturnBooking(Integer(p,"id"),actor);return new {returned=true};}
        if(action=="estimate")
        {
            int days=Integer(p,"days"); if(days<1||days>28)throw new ArgumentException("Choose 1 to 28 whole days.");
            var category=VehicleEnumExtensions.ParseCarType(Text(p,"category"));var fuel=VehicleEnumExtensions.ParseFuelType(Text(p,"fuel"));
            int selectedId=Integer(p,"carId");
            Car car=selectedId>0?Repository.GetCar(selectedId):Repository.GetAvailableCar(category,fuel);
            if(car==null||car.IsRented||car.CarType!=category||car.FuelType!=fuel)return new { available=false,car=(object)null,pricing=(object)null };
            var pricing=PricingCalculator.CalculateEstimate(days,category,fuel,Flag(p,"mileage"),Flag(p,"cover"),car.DailyRate);
            return new { available=true,car=CarDto(car),pricing=PriceDto(pricing) };
        }
        if(action=="quote")
        {
            CarType category=VehicleEnumExtensions.ParseCarType(Text(p,"category"));FuelType fuel=VehicleEnumExtensions.ParseFuelType(Text(p,"fuel"));
            int selectedId=Integer(p,"carId");
            Car car=selectedId>0?Repository.GetCar(selectedId):Repository.GetAvailableCar(category,fuel);
            var validation=BookingValidator.Validate(Text(p,"firstName"),Text(p,"surname"),Text(p,"address"),Integer(p,"age"),Flag(p,"licence"),Integer(p,"days"),category,fuel,car);
            if(!validation.IsValid)throw new ArgumentException(validation.BuildSummary());
            var pricing=PricingCalculator.CalculateEstimate(Integer(p,"days"),category,fuel,Flag(p,"mileage"),Flag(p,"cover"),car.DailyRate);
            return new { firstName=Text(p,"firstName").Trim(),surname=Text(p,"surname").Trim(),address=Text(p,"address").Trim(),age=Integer(p,"age"),licence=true,
                days=Integer(p,"days"),mileage=Flag(p,"mileage"),cover=Flag(p,"cover"),car=CarDto(car),pricing=PriceDto(pricing),requestId=Guid.NewGuid().ToString("N") };
        }
        if(action=="save")
        {
            var vehicle=(Dictionary<string,object>)p["car"];var price=(Dictionary<string,object>)p["pricing"];
            var booking=new Booking { CustomerFirstName=Text(p,"firstName"),CustomerSurname=Text(p,"surname"),CustomerAddress=Text(p,"address"),CustomerAge=Integer(p,"age"),
                HasValidLicense=Flag(p,"licence"),NumberOfDays=Integer(p,"days"),StaffUsername=actor,RequestId=Text(p,"requestId"),
                ReviewedTotal=Convert.ToDecimal(price["total"]),Extras=new OptionalExtras(Flag(p,"mileage"),Flag(p,"cover")),
                SelectedCar=new Car(Integer(vehicle,"id"),Text(vehicle,"registration"),VehicleEnumExtensions.ParseCarType(Text(vehicle,"category")),
                    VehicleEnumExtensions.ParseFuelType(Text(vehicle,"fuel")),false,Convert.ToDecimal(vehicle["dailyRate"])) };
            int id=Repository.SaveBooking(booking);return BookingDto(Repository.GetBooking(id));
        }
        throw new ArgumentException("Unknown operation.");
    }
    static int Main(string[] args)
    {
        Console.InputEncoding=System.Text.Encoding.UTF8;Console.OutputEncoding=new System.Text.UTF8Encoding(false);
        if(args.Length>0 && args[0]=="--backup")
        { try {Console.WriteLine(DatabaseMaintenance.Backup(DatabaseInitializer.ConnectionString,args[1]));return 0;}catch(Exception ex){Console.Error.WriteLine(ex.Message);return 1;} }
        if(args.Length>0 && args[0]=="--restore")
        { try {DatabaseMaintenance.RestoreToNewCatalog(DatabaseInitializer.ConnectionString,args[1],args[2]);Console.WriteLine("Restored to new catalog: "+args[2]);return 0;}catch(Exception ex){Console.Error.WriteLine(ex.Message);return 1;} }
        if(args.Length>0 && args[0]=="--privacy")
        { try {int count=DatabaseMaintenance.AnonymizeClosedRecords(DatabaseInitializer.ConnectionString,int.Parse(args[1]),args.Length>2&&args[2]=="--apply");Console.WriteLine(count+" eligible record(s)");return 0;}catch(Exception ex){Console.Error.WriteLine(ex.Message);return 1;} }
        var init=DatabaseInitializer.Initialize();
        Console.WriteLine(Json.Serialize(new {ready=init.IsSuccessful,message=init.Message}));Console.Out.Flush();
        if(!init.IsSuccessful)return 1;
        if(args.Length>0 && args[0]=="--setup")return 0;
        Repository=new BookingRepository();string line;
        while((line=Console.ReadLine())!=null)
        {
            object id=null;
            try
            {
                var request=Json.Deserialize<Dictionary<string,object>>(line);id=request["id"];
                var payload=request.ContainsKey("payload")?(Dictionary<string,object>)request["payload"]:new Dictionary<string,object>();
                object result=Handle(Text(request,"action"),payload,Text(request,"actor"));
                Console.WriteLine(Json.Serialize(new {id,ok=true,result}));
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                int status=ex is UnauthorizedAccessException?401:ex is ArgumentException||ex is FormatException?400:ex is InvalidOperationException?409:503;
                string message=status==503?"The database is unavailable. Your changes were not confirmed; retry using the same request.":ex.Message;
                Console.WriteLine(Json.Serialize(new {id,ok=false,status,message}));
            }
            Console.Out.Flush();
        }
        return 0;
    }
}
