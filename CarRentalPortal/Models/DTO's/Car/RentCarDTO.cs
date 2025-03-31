using System.ComponentModel.DataAnnotations;

namespace CarRentalPortal.Models.DTO_s.Car
{
    public class RentCarDTO
    {
        public Guid CarId { get; set; }

        [DataType(DataType.Date)]
        public DateOnly StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateOnly EndDate { get; set; }
    }
}
