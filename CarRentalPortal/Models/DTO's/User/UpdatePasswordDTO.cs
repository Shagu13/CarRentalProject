namespace CarRentalPortal.Models.DTO_s.User
{
    public class UpdatePasswordDTO
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; } 
    }
}
