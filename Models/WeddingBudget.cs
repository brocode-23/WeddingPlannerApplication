namespace WeddingPlannerApplication.Models
{
    public class WeddingBudget
    {
        public int Id { get; set; }
        public int CoupleId { get; set; }
        public string Category { get; set; }
        public decimal AllocatedAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
