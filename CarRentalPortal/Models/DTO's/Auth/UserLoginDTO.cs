namespace CarRentalPortal.Models.DTO_s.Auth
{
    public class UserLoginDTO
    {
        public int PhoneNumber { get; set; }
        public string Password { get; set; }

        public bool StaySignedIn { get; set; }
    }
}
