using CarRentalPortal.Enums;

namespace CarRentalPortal.Models.Entities
{
    public class User : BaseClass
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PhoneNumber { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpirationDate { get; set; }
        public List<Car> Cars { get; set; } = new List<Car>();
        public UserStatus Status { get; set; } = UserStatus.Active;

        public List<RentalRecord> Rentals { get; set; } = new();

    }
}


