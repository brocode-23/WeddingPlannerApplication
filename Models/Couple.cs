﻿namespace WeddingPlannerApplication.Models
{
    public class Couple
    {
        public Guid Id { get; set; }
        public DateTime WeddingDate { get; set; }
        public decimal Budget { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<CoupleMember> Members { get; set; }
        public ICollection<WeddingChecklist> Checklists { get; set; }
        public ICollection<Guest> Guests { get; set; }
        public ICollection<WeddingBudget> Budgets { get; set; }
        public ICollection<WeddingTimeline> Timelines { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<WeddingPlanner> Planners { get; set; }
    }
}
