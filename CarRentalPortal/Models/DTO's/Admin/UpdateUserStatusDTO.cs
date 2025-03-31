using CarRentalPortal.Enums;
using System.Text.Json.Serialization;

namespace CarRentalPortal.Models.DTO_s.Admin
{
    public class UpdateUserStatusDTO
    {
        public Guid UserId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserStatus Status { get; set; }
    }
}
