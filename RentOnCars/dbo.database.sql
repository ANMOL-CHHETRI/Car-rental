/* Canonical non-destructive schema and versioned migrations. Select the target database first. */
SET XACT_ABORT ON;
GO
IF OBJECT_ID(N'dbo.Cars',N'U') IS NULL CREATE TABLE dbo.Cars(
 CarId INT IDENTITY PRIMARY KEY, RegistrationNumber NVARCHAR(20) NOT NULL UNIQUE,
 IsRented BIT NOT NULL DEFAULT 0, DailyRate DECIMAL(10,2) NOT NULL CHECK(DailyRate>0),
 CarType NVARCHAR(30) NOT NULL CHECK(CarType IN('CityCar','FamilyCar','SportsCar','SUV')),
 FuelType NVARCHAR(30) NOT NULL CHECK(FuelType IN('Petrol','Diesel','Hybrid','FullElectric')));
GO
IF OBJECT_ID(N'dbo.Bookings',N'U') IS NULL CREATE TABLE dbo.Bookings(
 BookingId INT IDENTITY PRIMARY KEY, CarId INT NOT NULL REFERENCES dbo.Cars(CarId),
 CustomerFirstName NVARCHAR(50) NOT NULL,CustomerSurname NVARCHAR(50) NOT NULL,CustomerAddress NVARCHAR(150) NOT NULL,
 CustomerAge INT NOT NULL CHECK(CustomerAge BETWEEN 18 AND 120),HasValidLicense BIT NOT NULL,
 NumberOfDays INT NOT NULL CHECK(NumberOfDays BETWEEN 1 AND 28),CarType NVARCHAR(30) NOT NULL,FuelType NVARCHAR(30) NOT NULL,
 TotalCost DECIMAL(10,2) NOT NULL CHECK(TotalCost>0),
 CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Bookings_CreatedAt DEFAULT SYSUTCDATETIME());
GO
IF OBJECT_ID(N'dbo.BookingOptionalExtras',N'U') IS NULL CREATE TABLE dbo.BookingOptionalExtras(
 BookingOptionalExtraId INT IDENTITY PRIMARY KEY,BookingId INT NOT NULL REFERENCES dbo.Bookings(BookingId) ON DELETE CASCADE,
 ExtraName NVARCHAR(50) NOT NULL,DailyPrice DECIMAL(10,2) NOT NULL CHECK(DailyPrice>=0),
 NumberOfDays INT NOT NULL CHECK(NumberOfDays BETWEEN 1 AND 28));
GO
IF OBJECT_ID(N'dbo.SchemaVersions',N'U') IS NULL CREATE TABLE dbo.SchemaVersions(
 Version INT PRIMARY KEY,AppliedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME());
GO
IF COL_LENGTH('dbo.Bookings','DailyBaseRate') IS NULL ALTER TABLE dbo.Bookings ADD DailyBaseRate DECIMAL(10,2) NULL;
IF COL_LENGTH('dbo.Bookings','VehicleSurcharge') IS NULL ALTER TABLE dbo.Bookings ADD VehicleSurcharge DECIMAL(10,2) NULL;
IF COL_LENGTH('dbo.Bookings','FuelSurcharge') IS NULL ALTER TABLE dbo.Bookings ADD FuelSurcharge DECIMAL(10,2) NULL;
IF COL_LENGTH('dbo.Bookings','PricingSnapshotVersion') IS NULL ALTER TABLE dbo.Bookings ADD PricingSnapshotVersion INT NOT NULL DEFAULT 0;
IF COL_LENGTH('dbo.Bookings','StaffUsername') IS NULL ALTER TABLE dbo.Bookings ADD StaffUsername NVARCHAR(50) NOT NULL DEFAULT 'legacy';
IF COL_LENGTH('dbo.Bookings','StartsAt') IS NULL ALTER TABLE dbo.Bookings ADD StartsAt DATETIME2 NULL;
IF COL_LENGTH('dbo.Bookings','DueAt') IS NULL ALTER TABLE dbo.Bookings ADD DueAt DATETIME2 NULL;
IF COL_LENGTH('dbo.Bookings','ReturnedAt') IS NULL ALTER TABLE dbo.Bookings ADD ReturnedAt DATETIME2 NULL;
IF COL_LENGTH('dbo.Bookings','ReturnedBy') IS NULL ALTER TABLE dbo.Bookings ADD ReturnedBy NVARCHAR(50) NULL;
IF COL_LENGTH('dbo.Bookings','RequestId') IS NULL ALTER TABLE dbo.Bookings ADD RequestId NVARCHAR(64) NULL;
IF COL_LENGTH('dbo.Bookings','IsAnonymized') IS NULL ALTER TABLE dbo.Bookings ADD IsAnonymized BIT NOT NULL DEFAULT 0;
IF COL_LENGTH('dbo.Bookings','Status') IS NULL ALTER TABLE dbo.Bookings ADD Status NVARCHAR(20) NOT NULL DEFAULT 'Closed';
GO
/* Old records retain their stored totals and are explicitly identified as legacy snapshots. */
IF NOT EXISTS(SELECT 1 FROM dbo.SchemaVersions WHERE Version=1) BEGIN
 UPDATE b SET DailyBaseRate=c.DailyRate,
 VehicleSurcharge=CASE b.CarType WHEN 'SUV' THEN 65 WHEN 'FamilyCar' THEN 50 WHEN 'SportsCar' THEN 75 ELSE 0 END,
 FuelSurcharge=CASE b.FuelType WHEN 'Hybrid' THEN 30 WHEN 'FullElectric' THEN 50 ELSE 0 END,
 StartsAt=b.CreatedAt,DueAt=DATEADD(day,b.NumberOfDays,b.CreatedAt)
 FROM dbo.Bookings b JOIN dbo.Cars c ON b.CarId=c.CarId;
 UPDATE b SET Status='Active' FROM dbo.Bookings b JOIN dbo.Cars c ON b.CarId=c.CarId
 WHERE c.IsRented=1 AND b.BookingId=(SELECT MAX(x.BookingId) FROM dbo.Bookings x WHERE x.CarId=b.CarId);
 INSERT dbo.SchemaVersions(Version) VALUES(1);
END;
GO
/* New timestamps are UTC; existing timestamps are retained as legacy values. */
DECLARE @createdDefault sysname;
SELECT @createdDefault=d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id=d.object_id
WHERE c.object_id=OBJECT_ID('dbo.Bookings') AND c.name='CreatedAt';
IF @createdDefault IS NOT NULL BEGIN
 DECLARE @dropDefault nvarchar(max)=N'ALTER TABLE dbo.Bookings DROP CONSTRAINT '+QUOTENAME(@createdDefault);
 EXEC(@dropDefault);
END;
ALTER TABLE dbo.Bookings ADD CONSTRAINT DF_Bookings_CreatedAt DEFAULT SYSUTCDATETIME() FOR CreatedAt;
GO
/* NOCHECK preserves legacy records but enforces these rules for all new writes. */
IF OBJECT_ID('dbo.CK_Bookings_Required','C') IS NULL ALTER TABLE dbo.Bookings WITH NOCHECK ADD CONSTRAINT CK_Bookings_Required CHECK(
 LEN(LTRIM(RTRIM(CustomerFirstName)))>0 AND LEN(LTRIM(RTRIM(CustomerSurname)))>0 AND LEN(LTRIM(RTRIM(CustomerAddress)))>0
 AND HasValidLicense=1 AND CustomerAge BETWEEN 18 AND 120 AND TotalCost>0
 AND CarType IN('CityCar','FamilyCar','SportsCar','SUV') AND FuelType IN('Petrol','Diesel','Hybrid','FullElectric'));
IF OBJECT_ID('dbo.CK_Bookings_Status','C') IS NULL ALTER TABLE dbo.Bookings WITH NOCHECK ADD CONSTRAINT CK_Bookings_Status CHECK(Status IN('Active','Returned','Closed','Cancelled'));
IF OBJECT_ID('dbo.CK_Bookings_Snapshot','C') IS NULL ALTER TABLE dbo.Bookings WITH NOCHECK ADD CONSTRAINT CK_Bookings_Snapshot CHECK(
 PricingSnapshotVersion=0 OR(DailyBaseRate>0 AND VehicleSurcharge>=0 AND FuelSurcharge>=0 AND StartsAt IS NOT NULL AND DueAt IS NOT NULL AND DueAt>StartsAt AND LEN(StaffUsername)>0));
IF OBJECT_ID('dbo.CK_Extras_Name','C') IS NULL ALTER TABLE dbo.BookingOptionalExtras WITH NOCHECK ADD CONSTRAINT CK_Extras_Name CHECK(ExtraName IN('Unlimited Mileage','Breakdown Cover'));
GO
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='IX_Cars_Availability' AND object_id=OBJECT_ID('dbo.Cars'))
 CREATE INDEX IX_Cars_Availability ON dbo.Cars(CarType,FuelType,IsRented,CarId) INCLUDE(RegistrationNumber,DailyRate);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='UX_Bookings_ActiveCar' AND object_id=OBJECT_ID('dbo.Bookings'))
 CREATE UNIQUE INDEX UX_Bookings_ActiveCar ON dbo.Bookings(CarId) WHERE Status='Active';
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='UX_Bookings_Request' AND object_id=OBJECT_ID('dbo.Bookings'))
 CREATE UNIQUE INDEX UX_Bookings_Request ON dbo.Bookings(RequestId) WHERE RequestId IS NOT NULL;
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='IX_Bookings_History' AND object_id=OBJECT_ID('dbo.Bookings'))
 CREATE INDEX IX_Bookings_History ON dbo.Bookings(BookingId DESC) INCLUDE(CarId,Status,CreatedAt);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='UX_Extras_BookingName' AND object_id=OBJECT_ID('dbo.BookingOptionalExtras')) BEGIN
 IF EXISTS(SELECT BookingId,ExtraName FROM dbo.BookingOptionalExtras GROUP BY BookingId,ExtraName HAVING COUNT(*)>1)
  THROW 50001,'Duplicate legacy extras require review before migration. No records were removed.',1;
 CREATE UNIQUE INDEX UX_Extras_BookingName ON dbo.BookingOptionalExtras(BookingId,ExtraName);
END;
GO
CREATE OR ALTER TRIGGER dbo.TR_Extras_Duration ON dbo.BookingOptionalExtras AFTER INSERT,UPDATE AS BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM inserted i JOIN dbo.Bookings b ON b.BookingId=i.BookingId WHERE i.NumberOfDays<>b.NumberOfDays)
  THROW 50002,'Extra duration must match the booking.',1;
END;
GO
IF OBJECT_ID(N'dbo.AuditEvents',N'U') IS NULL CREATE TABLE dbo.AuditEvents(
 AuditId BIGINT IDENTITY PRIMARY KEY,OccurredAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
 Actor NVARCHAR(50) NOT NULL,Action NVARCHAR(30) NOT NULL,BookingId INT NULL,Detail NVARCHAR(300) NOT NULL);
IF OBJECT_ID(N'dbo.StaffAccounts',N'U') IS NULL CREATE TABLE dbo.StaffAccounts(
 Username NVARCHAR(50) PRIMARY KEY,PasswordHash VARBINARY(32) NOT NULL,Salt VARBINARY(32) NOT NULL,Iterations INT NOT NULL,
 Role NVARCHAR(10) NOT NULL CHECK(Role IN('staff','admin')),FailedAttempts INT NOT NULL DEFAULT 0,
 LockedUntil DATETIME2 NULL,IsActive BIT NOT NULL DEFAULT 1,IsDemo BIT NOT NULL DEFAULT 0);
GO
IF COL_LENGTH('dbo.StaffAccounts','IsDemo') IS NULL ALTER TABLE dbo.StaffAccounts ADD IsDemo BIT NOT NULL DEFAULT 0;
GO
IF NOT EXISTS(SELECT 1 FROM dbo.StaffAccounts WHERE Username='self-service')
 INSERT dbo.StaffAccounts(Username,PasswordHash,Salt,Iterations,Role,FailedAttempts,LockedUntil,IsActive,IsDemo)
 VALUES('self-service',0x00,0x00,600000,'staff',0,NULL,1,0);
GO
MERGE dbo.Cars WITH(HOLDLOCK) AS target
USING(VALUES('WAC-001','CityCar','Petrol'),('WAC-002','CityCar','Diesel'),
 ('WAC-003','FamilyCar','Petrol'),('WAC-004','FamilyCar','Diesel'),('WAC-005','SportsCar','Petrol'),
 ('WAC-006','SportsCar','Hybrid'),('WAC-007','SUV','Diesel'),('WAC-008','SUV','Hybrid'),
 ('WAC-009','SUV','FullElectric'),('WAC-010','CityCar','FullElectric')) AS source(Reg,Category,Fuel)
ON target.RegistrationNumber=source.Reg
WHEN NOT MATCHED THEN INSERT(RegistrationNumber,IsRented,DailyRate,CarType,FuelType) VALUES(source.Reg,0,25,source.Category,source.Fuel);
GO
IF NOT EXISTS(SELECT 1 FROM dbo.SchemaVersions WHERE Version=2) INSERT dbo.SchemaVersions(Version) VALUES(2);

