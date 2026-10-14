using System;
using System.Collections.Generic;

namespace RentOnCars.Models
{
    public enum CarType
    {
        CityCar,
        FamilyCar,
        SportsCar,
        SUV
    }

    public enum FuelType
    {
        Petrol,
        Diesel,
        Hybrid,
        FullElectric
    }

    public class Car
    {
        public int CarId { get; set; }
        public string RegistrationNumber { get; set; }
        public bool IsRented { get; set; }
        public decimal DailyRate { get; set; }
        public CarType CarType { get; set; }
        public FuelType FuelType { get; set; }

        public string GetCarDetails()
        {
            return RegistrationNumber + " - " + CarType + " - " + FuelType;
        }

        public void MarkAsRented()
        {
            IsRented = true;
        }

        public void MarkAsAvailable()
        {
            IsRented = false;
        }
    }

    public class OptionalExtras
    {
        public bool UnlimitedMileage { get; set; }
        public bool BreakdownCover { get; set; }

        public decimal CalculateExtrasCost(int numberOfDays)
        {
            decimal total = 0m;

            if (UnlimitedMileage)
                total += 10m * numberOfDays;

            if (BreakdownCover)
                total += 2m * numberOfDays;

            return total;
        }
    }

    public class Booking
    {
        public int BookingId { get; set; }
        public int CarId { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerSurname { get; set; }
        public string CustomerAddress { get; set; }
        public int CustomerAge { get; set; }
        public bool HasValidLicense { get; set; }
        public int NumberOfDays { get; set; }
        public decimal TotalCost { get; set; }
        public Car SelectedCar { get; set; }
        public OptionalExtras Extras { get; set; }

        public Booking()
        {
            Extras = new OptionalExtras();
        }

        public void CreateBooking()
        {
            if (!ValidateBooking())
                throw new InvalidOperationException("Booking information is not valid.");

            TotalCost = CalculateTotalCost();
        }

        public bool ValidateBooking()
        {
            return !string.IsNullOrWhiteSpace(CustomerFirstName)
                && !string.IsNullOrWhiteSpace(CustomerSurname)
                && !string.IsNullOrWhiteSpace(CustomerAddress)
                && CustomerAge >= 18
                && HasValidLicense
                && NumberOfDays >= 1
                && NumberOfDays <= 28
                && SelectedCar != null
                && !SelectedCar.IsRented;
        }

        public decimal CalculateTotalCost()
        {
            if (SelectedCar == null)
                throw new InvalidOperationException("A car must be selected.");

            decimal total = NumberOfDays * SelectedCar.DailyRate;
            total += GetCarTypeSurcharge(SelectedCar.CarType);
            total += GetFuelTypeSurcharge(SelectedCar.FuelType);
            total += Extras.CalculateExtrasCost(NumberOfDays);

            return total;
        }

        public void ConfirmBooking()
        {
            if (!ValidateBooking())
                throw new InvalidOperationException("Booking cannot be confirmed.");

            TotalCost = CalculateTotalCost();
            SelectedCar.MarkAsRented();
        }

        private decimal GetCarTypeSurcharge(CarType type)
        {
            switch (type)
            {
                case CarType.FamilyCar: return 50m;
                case CarType.SportsCar: return 75m;
                case CarType.SUV: return 65m;
                default: return 0m;
            }
        }

        private decimal GetFuelTypeSurcharge(FuelType type)
        {
            switch (type)
            {
                case FuelType.Hybrid: return 30m;
                case FuelType.FullElectric: return 50m;
                default: return 0m;
            }
        }
    }

    public class RentedCarsList
    {
        private readonly List<Car> rentedCars = new List<Car>();

        public List<Car> RentedCars
        {
            get { return rentedCars; }
        }

        public void DisplayRentedCars()
        {
            // The WinForms dashboard/report is responsible for displaying the list.
        }

        public void AddRentedCar(Car car)
        {
            if (car != null && !rentedCars.Contains(car))
                rentedCars.Add(car);
        }

        public void RemoveRentedCar(Car car)
        {
            if (car != null)
                rentedCars.Remove(car);
        }
    }
}
