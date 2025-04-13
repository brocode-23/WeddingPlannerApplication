namespace WeddingPlannerApplication.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public Booking Booking { get; set; }
    }
}
