namespace WebApplication1.Models
{
    public class MemberShipPlan
    {
        public int Id { get; set; }
        // đây là nơi chứa các gói tập: 1 tháng, 3 tháng, 6 tháng, 12 tháng ( sinh viên, người lớn )
        public String PlanName { get; set; }
        //giá của gói tập ( sv: 180k, người lớn: 200k )
        public String Price { get; set; }
        //số tháng của gói tập ( 1, 3, 6, 12 )
        public int DurationMonths { get; set; }
        //đối tượng khách hàng mà gói tập hướng tới ( sinh viên, người lớn )
        public String TargetCustumer { get; set; }


    }
}
