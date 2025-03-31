using CarRentalPortal.Enums;
using System.Text.Json.Serialization;

namespace CarRentalPortal.Models.DTO_s.Car
{
    public class AddCarDTO
    {
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public int YearOfManufacture { get; set; }
        public Decimal DailyRentalPrice { get; set; }
        public int CarCapacity { get; set; }
        [JsonIgnore]
        public int UserPhoneNumber { get; set; }
        public string City { get; set; }
        public TransmissionType TransmissionType { get; set; }
        public EngineType EngineType { get; set; }
        public int? FuelTankCapacity { get; set; } 
        public int? RangeInKm { get; set; } 

        public List<IFormFile> Images { get; set; }

    }
}
