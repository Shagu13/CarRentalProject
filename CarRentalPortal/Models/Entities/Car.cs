using CarRentalPortal.Enums;


namespace CarRentalPortal.Models.Entities
{
    public class Car : BaseClass
    {
        public Guid Id { get; set; } 
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public int YearOfManufacture { get; set; }
        public Decimal DailyRentalPrice { get; set; }
        public int CarCapacity { get; set; }
        public int UserPhoneNumber { get; set; }
        public string City { get; set; }
        public TransmissionType TransmissionType { get; set; }
        public EngineType EngineType { get; set; }
        public int? FuelTankCapacity { get; set; } 
        public int? RangeInKm { get; set; } 
        public List<string> Images { get; set; } = new List<string>();

        public int RentalCount { get; set; } = 0;
        public Guid UserId { get; set; }

        public User User { get; set; }
    }
}
