namespace WebApplication1.Models
{

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Lưu ý: Thực tế sau này phải hash mật khẩu, tạm thời cứ để string
        public string Email { get; set; }
        public string Phone { get; set; }

        // Chứa 1 trong 3 giá trị: "Manager", "PT", "Member"
        public UserRole Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
