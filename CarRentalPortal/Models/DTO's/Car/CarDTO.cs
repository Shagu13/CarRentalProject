using CarRentalPortal.Enums;
using CarRentalPortal.Helpers;
using System.ComponentModel.DataAnnotations;

namespace CarRentalPortal.Models.DTO_s.Car
{
    public class CarDTO
    {
        public Guid Id { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public int YearOfManufacture { get; set; }
        public decimal DailyRentalPrice { get; set; }
        public int CarCapacity { get; set; }
        public int UserPhoneNumber { get; set; }
        public string City { get; set; }
        public TransmissionType TransmissionType { get; set; }
        public EngineType EngineType { get; set; }
        public int? FuelTankCapacity { get; set; } 
        public int? RangeInKm { get; set; } 
        public List<string> Images { get; set; }
    }
}
