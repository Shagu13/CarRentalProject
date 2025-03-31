namespace CarRentalPortal.Models.DTO_s.Auth
{
    public class JwtOptionsDTO
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
    }
}
