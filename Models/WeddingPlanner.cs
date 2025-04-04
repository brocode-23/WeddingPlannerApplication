namespace WeddingPlannerApplication.Models
{
    public class WeddingPlanner
    {
        public int Id { get; set; }
        public string PlannerUserId { get; set; }
        public int CoupleId { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Couple> AssignedCouples { get; set; }
    }
}
