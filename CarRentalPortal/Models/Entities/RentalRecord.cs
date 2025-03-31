namespace CarRentalPortal.Models.Entities
{
    public class RentalRecord
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid CarId { get; set; }
        public Car Car { get; set; }

        public DateTime RentalStartDate { get; set; }
        public DateTime RentalEndDate { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
