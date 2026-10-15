/*
    RentOnCars database setup
    SQL Server / LocalDB
    Matches the Booking System class structure in the report:
    Car, Booking, OptionalExtras and the rented-car list.
*/

IF DB_ID(N'RentOnCarsDB') IS NULL
BEGIN
    CREATE DATABASE RentOnCarsDB;
END
GO

USE RentOnCarsDB;
GO

IF OBJECT_ID(N'dbo.BookingOptionalExtras', N'U') IS NOT NULL
    DROP TABLE dbo.BookingOptionalExtras;
GO

IF OBJECT_ID(N'dbo.Bookings', N'U') IS NOT NULL
    DROP TABLE dbo.Bookings;
GO

IF OBJECT_ID(N'dbo.Cars', N'U') IS NOT NULL
    DROP TABLE dbo.Cars;
GO

CREATE TABLE dbo.Cars
(
    CarId INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_Cars PRIMARY KEY,
    RegistrationNumber NVARCHAR(20) NOT NULL
        CONSTRAINT UQ_Cars_Registration UNIQUE,
    IsRented BIT NOT NULL
        CONSTRAINT DF_Cars_IsRented DEFAULT (0),
    DailyRate DECIMAL(10,2) NOT NULL
        CONSTRAINT CK_Cars_DailyRate CHECK (DailyRate > 0),
    CarType NVARCHAR(30) NOT NULL
        CONSTRAINT CK_Cars_CarType CHECK
        (CarType IN (N'CityCar', N'FamilyCar', N'SportsCar', N'SUV')),
    FuelType NVARCHAR(30) NOT NULL
        CONSTRAINT CK_Cars_FuelType CHECK
        (FuelType IN (N'Petrol', N'Diesel', N'Hybrid', N'FullElectric'))
);
GO

CREATE TABLE dbo.Bookings
(
    BookingId INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_Bookings PRIMARY KEY,
    CarId INT NOT NULL,
    CustomerFirstName NVARCHAR(50) NOT NULL,
    CustomerSurname NVARCHAR(50) NOT NULL,
    CustomerAddress NVARCHAR(150) NOT NULL,
    CustomerAge INT NOT NULL
        CONSTRAINT CK_Bookings_Age CHECK (CustomerAge >= 18),
    HasValidLicense BIT NOT NULL,
    NumberOfDays INT NOT NULL
        CONSTRAINT CK_Bookings_Days CHECK (NumberOfDays BETWEEN 1 AND 28),
    CarType NVARCHAR(30) NOT NULL,
    FuelType NVARCHAR(30) NOT NULL,
    TotalCost DECIMAL(10,2) NOT NULL
        CONSTRAINT CK_Bookings_TotalCost CHECK (TotalCost >= 0),
    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Bookings_CreatedAt DEFAULT (SYSDATETIME()),

    CONSTRAINT FK_Bookings_Cars
        FOREIGN KEY (CarId) REFERENCES dbo.Cars(CarId)
);
GO

CREATE TABLE dbo.BookingOptionalExtras
(
    BookingOptionalExtraId INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_BookingOptionalExtras PRIMARY KEY,
    BookingId INT NOT NULL,
    ExtraName NVARCHAR(50) NOT NULL,
    DailyPrice DECIMAL(10,2) NOT NULL
        CONSTRAINT CK_BookingExtras_DailyPrice CHECK (DailyPrice >= 0),
    NumberOfDays INT NOT NULL
        CONSTRAINT CK_BookingExtras_Days CHECK (NumberOfDays BETWEEN 1 AND 28),

    CONSTRAINT FK_BookingExtras_Bookings
        FOREIGN KEY (BookingId) REFERENCES dbo.Bookings(BookingId)
        ON DELETE CASCADE
);
GO

/*
    Seed cars.
    Base rental price is £25 per day as stated in the use case.
    The type and fuel surcharges are calculated by the Booking class.
*/
INSERT INTO dbo.Cars (RegistrationNumber, IsRented, DailyRate, CarType, FuelType)
VALUES
(N'WAC-001', 0, 25.00, N'CityCar',      N'Petrol'),
(N'WAC-002', 0, 25.00, N'CityCar',      N'Diesel'),
(N'WAC-003', 0, 25.00, N'FamilyCar',    N'Petrol'),
(N'WAC-004', 0, 25.00, N'FamilyCar',    N'Diesel'),
(N'WAC-005', 0, 25.00, N'SportsCar',    N'Petrol'),
(N'WAC-006', 0, 25.00, N'SportsCar',    N'Hybrid'),
(N'WAC-007', 0, 25.00, N'SUV',          N'Diesel'),
(N'WAC-008', 0, 25.00, N'SUV',          N'Hybrid'),
(N'WAC-009', 0, 25.00, N'SUV',          N'FullElectric'),
(N'WAC-010', 0, 25.00, N'CityCar',      N'FullElectric');
GO

/*
    Useful verification queries.
*/
SELECT * FROM dbo.Cars ORDER BY CarId;
SELECT * FROM dbo.Bookings ORDER BY BookingId;
SELECT * FROM dbo.BookingOptionalExtras ORDER BY BookingOptionalExtraId;
GO
