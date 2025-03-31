namespace CarRentalPortal.Models.DTO_s.Admin
{
    public class ChangePasswordDTO
    {
        public Guid UserId { get; set; }
        public string NewPassword { get; set; }
    }
}
