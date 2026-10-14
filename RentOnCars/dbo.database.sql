/*
    WeAreCars - Database Setup Script
    SQL Server / LocalDB
    Matches the WeAreCars Booking System class structure and business requirements:
    Cars, Bookings, and BookingOptionalExtras tables.
    Preserves existing records safely and is idempotent (safe to run repeatedly).
*/

-- Step 1: Create Database if it does not already exist
IF DB_ID(N'RentOnCarsDB') IS NULL
BEGIN
    CREATE DATABASE RentOnCarsDB;
END
GO

USE RentOnCarsDB;
GO

-- Step 2: Create Cars table safely
IF OBJECT_ID(N'dbo.Cars', N'U') IS NULL
BEGIN
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
END
GO

-- Step 3: Create Bookings table safely
IF OBJECT_ID(N'dbo.Bookings', N'U') IS NULL
BEGIN
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
END
GO

-- Step 4: Create BookingOptionalExtras table safely
IF OBJECT_ID(N'dbo.BookingOptionalExtras', N'U') IS NULL
BEGIN
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
END
GO

-- Step 5: Seed Starter Fleet Non-Destructively (No duplicates, preserves existing records)
IF NOT EXISTS (SELECT 1 FROM dbo.Cars WHERE RegistrationNumber = N'WAC-001')
    INSERT INTO dbo.Cars (RegistrationNumber, IsRented, DailyRate, CarType, FuelType) VALUES (N'WAC-001', 0, 25.00, N'CityCar', N'Petrol');

IF NOT EXISTS (SELECT 1 FROM dbo.Cars WHERE RegistrationNumber = N'WAC-002')
    INSERT INTO dbo.Cars (RegistrationNumber, IsRented, DailyRate, CarType, FuelType) VALUES (N'WAC-002', 0, 25.00, N'CityCar', N'Diesel');

IF NOT EXISTS (SELECT 1 FROM dbo.Cars WHERE RegistrationNumber = N'WAC-003')
    INSERT INTO dbo.Cars (RegistrationNumber, IsRented, DailyRate, CarType, FuelType) VALUES (N'WAC-003', 0, 25.00, N'FamilyCar', N'Petrol');

IF NOT EXISTS (SELECT 1 FROM dbo.Cars WHERE RegistrationNumber = N'WAC-004')
    INSERT INTO dbo.Cars (RegistrationNumber, IsRented, DailyRate, CarType, FuelType) VALUES (N'WAC-004', 0, 25.00, N'FamilyCar', N'Diesel');

IF NOT EXISTS (SELECT 1 FROM dbo.Cars WHERE RegistrationNumber = N'WAC-005')
    INSERT INTO dbo.Cars (RegistrationNumber, IsRented, DailyRate, CarType, FuelType) VALUES (N'WAC-005', 0, 25.00, N'SportsCar', N'Petrol');

IF NOT EXISTS (SELECT 1 FROM dbo.Cars WHERE RegistrationNumber = N'WAC-006')
    INSERT INTO dbo.Cars (RegistrationNumber, IsRented, DailyRate, CarType, FuelType) VALUES (N'WAC-006', 0, 25.00, N'SportsCar', N'Hybrid');

IF NOT EXISTS (SELECT 1 FROM dbo.Cars WHERE RegistrationNumber = N'WAC-007')
    INSERT INTO dbo.Cars (RegistrationNumber, IsRented, DailyRate, CarType, FuelType) VALUES (N'WAC-007', 0, 25.00, N'SUV', N'Diesel');

IF NOT EXISTS (SELECT 1 FROM dbo.Cars WHERE RegistrationNumber = N'WAC-008')
    INSERT INTO dbo.Cars (RegistrationNumber, IsRented, DailyRate, CarType, FuelType) VALUES (N'WAC-008', 0, 25.00, N'SUV', N'Hybrid');

IF NOT EXISTS (SELECT 1 FROM dbo.Cars WHERE RegistrationNumber = N'WAC-009')
    INSERT INTO dbo.Cars (RegistrationNumber, IsRented, DailyRate, CarType, FuelType) VALUES (N'WAC-009', 0, 25.00, N'SUV', N'FullElectric');

IF NOT EXISTS (SELECT 1 FROM dbo.Cars WHERE RegistrationNumber = N'WAC-010')
    INSERT INTO dbo.Cars (RegistrationNumber, IsRented, DailyRate, CarType, FuelType) VALUES (N'WAC-010', 0, 25.00, N'CityCar', N'FullElectric');
GO

-- Verification Queries
SELECT * FROM dbo.Cars ORDER BY CarId;
SELECT * FROM dbo.Bookings ORDER BY BookingId DESC;
SELECT * FROM dbo.BookingOptionalExtras ORDER BY BookingOptionalExtraId DESC;
GO
