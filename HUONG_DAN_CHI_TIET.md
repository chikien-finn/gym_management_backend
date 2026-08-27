# HƯỚNG DẪN CODE CHI TIẾT TỪNG THƯ MỤC

Dưới đây là cẩm nang hướng dẫn cho từng thư mục trong dự án, bao gồm giải thích chức năng, nhiệm vụ của từng người và code mẫu C# cơ bản để các bạn copy/paste.

---

## 1. Thư mục `Models/`
**Làm gì:** Chứa các "Thực thể". Database của bạn có bảng gì, thì trong này có Class đó tương ứng. Nó là khung xương của ứng dụng.

**Phân công:**
- **Bạn A (Core/Auth):** Tạo `User.cs`, `Role.cs`.
- **Bạn B (PT/Class):** Tạo `PersonalTrainer.cs`, `GymClass.cs`, `Booking.cs`.
- **Bạn C (Membership/Payment):** Tạo `MembershipPackage.cs`, `Invoice.cs`.

**Code mẫu (User.cs):**
```csharp
namespace WebApplication1.Models
{
    public class User
    {
        public int Id { get; set; } // Luôn phải có Id (Khóa chính)
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Admin", "Member", "PT"
    }
}
```

---

## 2. Thư mục `DTOs/`
**Làm gì:** DTO (Data Transfer Object) dùng để Gửi và Nhận dữ liệu từ Frontend/Postman. Không được dùng trực tiếp Model vì Model thường chứa các trường nhạy cảm (như Id, Password) hoặc quá cồng kềnh.

**Phân công:**
- **Bạn A:** `LoginRequestDTO.cs`, `UserProfileResponseDTO.cs`
- **Bạn B:** `CreateClassRequestDTO.cs`, `BookingDTO.cs`
- **Bạn C:** `BuyMembershipRequestDTO.cs`, `InvoiceResponseDTO.cs`

**Code mẫu (LoginRequestDTO.cs):**
```csharp
namespace WebApplication1.DTOs
{
    public class LoginRequestDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
```

---

## 3. Thư mục `Data/`
**Làm gì:** Chứa cầu nối với Database bằng Entity Framework. File quan trọng nhất là `AppDbContext.cs`. **Cả 3 người cùng làm việc chung trên file này.**

**Phân công:** Khi viết xong Model, hãy vào đây khai báo DbSet.

**Code mẫu (AppDbContext.cs):**
```csharp
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Bạn A viết
        public DbSet<User> Users { get; set; } 
        
        // Bạn B viết
        // public DbSet<GymClass> GymClasses { get; set; }

        // Bạn C viết
        // public DbSet<MembershipPackage> MembershipPackages { get; set; }
    }
}
```

---

## 4. Thư mục `Repositories/`
**Làm gì:** Nơi DUY NHẤT được phép gọi `AppDbContext` để tương tác với Database (Lấy data, Thêm, Sửa, Xóa). Tuyệt đối không gọi DbContext ở Controller.

**Phân công:**
- **Bạn A:** `UserRepository.cs`
- **Bạn B:** `PTRepository.cs`, `ClassRepository.cs` 
- **Bạn C:** `MembershipRepository.cs`, `InvoiceRepository.cs`

**Code mẫu (UserRepository.cs):**
```csharp
using WebApplication1.Data;
using WebApplication1.Models;
using System.Linq;

namespace WebApplication1.Repositories
{
    public class UserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) 
        { 
            _context = context; 
        }

        public User GetById(int id) 
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }
    }
}
```

---

## 5. Thư mục `Services/`
**Làm gì:** Trái tim dự án, nơi chứa toàn bộ LOGIC và NGHIỆP VỤ.
Luồng: `Controller` gọi `Service` -> `Service` tính toán/kiểm tra lỗi -> Gọi `Repository` để lấy data.

**Phân công:**
- **Bạn A:** `AuthService.cs`, `UserService.cs`
- **Bạn B:** `ClassService.cs` (kiểm tra lớp học có trống không)
- **Bạn C:** `MembershipService.cs` (kiểm tra gói tập, tính tiền)

**Code mẫu (UserService.cs):**
```csharp
using WebApplication1.Repositories;
using WebApplication1.Models;
using System;

namespace WebApplication1.Services
{
    public class UserService
    {
        private readonly UserRepository _repo;

        public UserService(UserRepository repo) 
        { 
            _repo = repo; 
        }

        public User GetUserInfo(int id)
        {
            if (id <= 0) throw new Exception("ID không hợp lệ!");
            
            var user = _repo.GetById(id);
            if (user == null) throw new Exception("Không tìm thấy user!");

            return user;
        }
    }
}
```

---

## 6. Thư mục `Controllers/`
**Làm gì:** API Endpoints (URL) để Frontend gọi vào.
Luật: Controller không tính toán logic. Nó chỉ nhận DTO -> Gọi Service -> Trả về Kết quả.

**Phân công:**
- **Bạn A:** `AuthController.cs`, `UserController.cs`
- **Bạn B:** `PTController.cs`, `ClassController.cs`
- **Bạn C:** `MembershipController.cs`, `PaymentController.cs`

**Code mẫu (UserController.cs):**
```csharp
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;

        public UserController(UserService service) 
        { 
            _service = service; 
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try 
            {
                var user = _service.GetUserInfo(id);
                return Ok(user);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
```
