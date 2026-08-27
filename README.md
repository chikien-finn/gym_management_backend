# DỰ ÁN QUẢN LÝ PHÒNG GYM (GYM MANAGEMENT BACKEND)

**Ngôn ngữ/Framework:** C# / ASP.NET Core Web API  
**Kiến trúc:** Mô hình 3 lớp (3-Tier Architecture)

---

## 1. MÔ TẢ DỰ ÁN
Dự án cung cấp hệ thống API backend phục vụ cho nghiệp vụ quản lý phòng Gym, bao gồm quản lý hội viên, quản lý huấn luyện viên (PT), xếp lịch tập, đăng ký khóa học, mua gói tập và thanh toán hóa đơn.

## 2. PHÂN CÔNG THÀNH VIÊN (3 NGƯỜI)

Dự án được chia việc theo **Tính năng (Feature-based)**. Mỗi thành viên sẽ tự code toàn bộ luồng từ Database lên API cho mảng của mình để không bị phụ thuộc/chờ đợi nhau.

* **🧑‍💻 Thành viên A (Core & Auth):** 
  * Quản lý Tài khoản (Login, Register).
  * Quản lý thông tin Hội viên.
* **🧑‍💻 Thành viên B (Lịch trình & PT):** 
  * Quản lý Huấn luyện viên (PT).
  * Quản lý Khóa học/Lớp học.
  * Quản lý Lịch tập (Booking).
* **🧑‍💻 Thành viên C (Tài chính):** 
  * Quản lý các Gói tập (Membership Packages).
  * Xử lý Thanh toán, Hóa đơn (Invoices).

---

## 3. CẤU TRÚC THƯ MỤC & NHIỆM VỤ

Dự án tuân thủ nghiêm ngặt quy tắc chia thư mục sau. **(Lưu ý: Trong mỗi thư mục đều đã có file `_HUONG_DAN.txt` chi tiết kèm code mẫu cho mọi người)**

| Thư mục | Chức năng (Làm gì?) |
| :--- | :--- |
| **`Models/`** | Chứa các Class tương ứng với các Bảng trong Database (Ví dụ: `User`, `GymClass`). |
| **`DTOs/`** | Chứa các Class "Trung gian" chỉ dùng để Gửi/Nhận data qua mạng (Ví dụ: `LoginRequest`). |
| **`Data/`** | Chứa `AppDbContext.cs` là Cầu nối duy nhất với Database. (Cả 3 người làm chung file này). |
| **`Repositories/`** | Nơi duy nhất chứa code lấy dữ liệu, thêm, sửa, xóa với Database thông qua `AppDbContext`. |
| **`Services/`** | **Trái tim dự án.** Nơi viết toàn bộ Logic nghiệp vụ, tính toán tiền, kiểm tra lỗi... |
| **`Controllers/`** | Lễ tân. Chứa các API `/api/xxx`. Chỉ nhận DTO -> Gọi Service xử lý -> Trả về kết quả. |

---

## 4. QUY TRÌNH CODE CHUẨN (LUỒNG NƯỚC CHẢY)

Khi bắt tay vào code bất kỳ chức năng nào, **BẮT BUỘC** phải code theo thứ tự từ dưới lên như sau, tuyệt đối không nhảy cóc:

1. 🗄️ **Models:** Viết class định nghĩa bảng trước.
2. 🔌 **Data:** Vào `AppDbContext` đăng ký `DbSet`.
3. 💾 **Repositories:** Viết hàm lấy/thêm/sửa/xóa Data.
4. 🧠 **Services:** Viết logic kiểm tra, nghiệp vụ.
5. 🌐 **Controllers:** Mở API cho Frontend gọi (gọi Service ở bước 4 ra dùng).

---

## 5. NHỮNG GÌ ĐÃ LÀM ĐƯỢC ĐẾN HIỆN TẠI
- [x] Khởi tạo thành công project ASP.NET Core Web API cơ bản.
- [x] Thiết lập chuẩn cấu trúc thư mục 3 lớp (Controller - Service - Repo).
- [x] Tạo file tài liệu `_HUONG_DAN.txt` hướng dẫn trực tiếp trong source code cho cả 3 thành viên.
- [x] Hoàn thành phân chia đầu việc chi tiết.

**(Dự án đã sẵn sàng để code bước đầu tiên: Tạo Models!)**
