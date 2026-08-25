namespace WebApplication1.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        // 1. liên kết với user
        public int UserId { get; set; }
        public User User { get; set; }
        // 2. liên kết với membershipplan
        public int MemberShipPlanId { get; set; }
        public MemberShipPlan MemberShipPlan { get; set; }
        // 3. thời gian
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        // 4. trạng thái
        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
