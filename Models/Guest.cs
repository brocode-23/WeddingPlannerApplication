namespace WeddingPlannerApplication.Models
{
    public class Guest
    {
        public int Id { get; set; }
        public int CoupleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string RSVPStatus { get; set; }
        public string MealPreference { get; set; }
        public string Allergies { get; set; }
        public string SeatingArrangement { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
