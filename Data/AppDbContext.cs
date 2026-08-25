using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data

{
    // ":" là kế thừa
    // DbContext là một lớp cơ sở trong Entity Framework Core, cung cấp các phương thức và thuộc tính để tương tác với cơ sở dữ liệu.
    // appdb kế thừa dbcontext, appdb sẽ có tất cả các phương thức và thuộc tính của dbcontext
    public class AppDbContext : DbContext
    {
        // dòng này buộc có vì nó là nhận các cấu hình từ program.cs, nó sẽ nhận các cấu hình từ program.cs và truyền vào base(options) để base(options) có thể sử dụng các cấu hình đó
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
        //khai báo bảng user 
        public DbSet<User> Users { get; set; }
        //khai báo bảng menbershipplan
        public DbSet<MemberShipPlan> MemberShipPlans { get; set; }
        // khai báo bảng subscription
        public DbSet<Subscription> Subcriptions { get; set; }
    }
}
