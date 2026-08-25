namespace WebApplication1.Models
{
    // tạo class này ra mục đích ấn định các role để người dùng khi nhập tìm kiếm sẽ không bị sai sót , ví dụ: "manager" thay vì "Manager"
    public enum UserRole
    {
        Manager,
        PT,
        Member
    }
}
