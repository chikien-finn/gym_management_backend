# HƯỚNG DẪN RÚT GỌN — LÀM TỪNG PHẦN, TỪ DỄ ĐẾN KHÓ

**Nguyên tắc thay đổi so với bản gốc:**
Bản gốc chia theo **layer** (làm hết Model → hết Config → hết Repo → hết Service → hết Controller). Cách này đúng lý thuyết nhưng dễ loạn khi làm 1 mình vì bạn phải nhớ 9 bảng cùng lúc.

Bản này chia theo **vertical slice** — mỗi lần chỉ làm **1 luồng nghiệp vụ trọn vẹn** (đủ cả Model → Config → Repo → Service → Controller của riêng luồng đó), test chạy được ngay, rồi mới sang luồng tiếp theo. Áp dụng đúng thứ tự 5 bước NHƯNG chỉ trong phạm vi 1 tính năng nhỏ tại 1 thời điểm.

Lý do: làm xong 1 slice là có thể bấm Swagger test ngay, thấy nó chạy → có động lực, dễ debug vì lỗi chỉ nằm trong 3-4 file vừa đụng, không phải soi cả project.

---

## 1. VỀ VIỆC CHIA NHÓM — SỬA LẠI QUAN ĐIỂM

**KHÔNG chia theo màn hình (người A làm Login, người B làm Register).**

Lý do: Login và Register dùng CHUNG `User.cs`, CHUNG `UserRepository`, CHUNG `AuthService.cs`, CHUNG `AuthController.cs`. Nếu 2 người cùng đụng 1 file này song song → conflict nát code, đúng y hệt vấn đề "làm chung 1 lúc" mà Kiên đang muốn tránh.

**Quy tắc chia việc nhóm đúng: chia theo domain dữ liệu (module), giữ nguyên như bản gốc:**

| Người | Module | Sở hữu file nào |
|---|---|---|
| A | Core & Auth | `User`, `Member`, toàn bộ Auth |
| B | Schedule & PT | `Trainer`, `GymClass`, `Booking`, `Enrollment` |
| C | Finance | `Package`, `MemberPackage`, `Invoice` |

Mỗi người **không bao giờ** sửa file thuộc module người khác. Muốn thêm field vào bảng chung (VD: thêm `Address` vào `Member`) → phải báo trước, không tự ý sửa.

Trong nội bộ module của mình, mỗi người tự áp dụng cách chia độ khó ở mục 2 bên dưới — không làm hết Model của cả module rồi mới quay lại Repository.

---

## 2. LỘ TRÌNH ĐỘ KHÓ TĂNG DẦN (áp dụng cho A, B, C đều theo pattern này)

### Cấp độ 1 — Auth (A làm, B và C ngồi xem/học theo vì sẽ copy pattern)
Đây là nền móng, làm đúng 1 lần thì B và C chỉ việc rập khuôn lại cho module của mình.

**Slice 1.1 — Login (làm trước, dễ hơn Register vì không cần validate nhiều)**

Thứ tự file, đọc từ dưới lên là thứ tự code (cái trên cần cái dưới tồn tại trước):

```
AuthController.cs   → nhận LoginRequestDto, gọi AuthService, trả AuthResponseDto
        ↑
AuthService.cs       → Login(): tìm User → so password (BCrypt.Verify) → sinh JWT
        ↑ cần cả 2 nhánh dưới
JwtHelper.cs                          UserRepository.cs
(GenerateToken(User user))            (GetByUsernameAsync(string username))
                                              ↑
                                       UserConfig.cs
                                       (ràng buộc Username là unique)
                                              ↑
                                       User.cs
                                       (Id, Username, PasswordHash, Role)
```

Vì sao đi theo thứ tự này:
- `User.cs` phải có trước vì mọi thứ khác đều tham chiếu tới type `User`.
- `UserConfig.cs` phải xong TRƯỚC khi chạy migration — nếu quên ràng buộc unique cho `Username`, hệ thống cho phép 2 tài khoản trùng tên đăng nhập, bug này rất khó phát hiện bằng mắt thường vì code vẫn chạy bình thường, chỉ sai logic.
- `UserRepository` CHỈ được viết 1 hàm duy nhất cho slice này: `GetByUsernameAsync`. Không viết thêm hàm khác chưa cần dùng — nguyên tắc YAGNI (You Aren't Gonna Need It), tránh code thừa gây rối.
- `AuthService.Login()` là nơi DUY NHẤT được gọi `BCrypt.Verify()`. Không bao giờ so sánh password ở Controller hay Repository — đây là security awareness: logic xác thực phải tập trung 1 chỗ, dễ audit, dễ sửa khi đổi thuật toán hash.
- `AuthController` chỉ có 4 dòng thực chất: nhận DTO → check `ModelState.IsValid` → gọi Service → trả `Ok()`/`Unauthorized()`. Không viết `if (user.PasswordHash == ...)` ở đây — nếu thấy mình đang viết if/else nghiệp vụ trong Controller, tức là đang làm sai kiến trúc.

File hạ tầng bắt buộc phải xong TRƯỚC KHI test login (dễ bị quên, sinh viên hay bỏ sót):
- `appsettings.json`: phải có `Jwt:SecretKey`, `Jwt:Issuer`, `Jwt:Audience`.
- `Program.cs`: phải gọi `builder.Services.AddAuthentication(...).AddJwtBearer(...)` và `app.UseAuthentication()` TRƯỚC `app.UseAuthorization()`. Sai thứ tự 2 dòng này là lỗi runtime âm thầm — không crash, nhưng `[Authorize]` sẽ không hoạt động.

**Test xong slice 1.1 mới được sang slice tiếp theo. Tiêu chí "xong":** Swagger gọi `POST /api/auth/login` với username/password đúng trả về JWT token, sai trả về 401.

**Slice 1.2 — Register**

Chỉ thêm vào các file ĐÃ CÓ ở slice 1.1, không tạo luồng song song:
- `AuthController.cs`: thêm action `Register`
- `AuthService.cs`: thêm method `RegisterAsync()` — check username đã tồn tại chưa (gọi lại `UserRepository.GetByUsernameAsync`, tái sử dụng chứ không viết hàm mới), hash password bằng `BCrypt.HashPassword()`, tạo `User` + `Member` cùng lúc (vì 1 User luôn đi kèm 1 Member profile).
- `UserRepository.cs`: thêm `AddAsync()`
- Model mới cần: `Member.cs`, `MemberConfig.cs`, `IMemberRepository`/`MemberRepository` (chỉ cần `AddAsync`, chưa cần Get/Update ở bước này)

**Tiêu chí xong:** Đăng ký user mới → login lại được bằng chính tài khoản đó.

### Cấp độ 2 — Dashboard / trang chủ sau khi login

Đây là lúc B và C nhập cuộc, vì dashboard cần dữ liệu member (gói đang có, lớp đã đăng ký...) — nhưng ở giai đoạn đầu, dashboard chỉ cần đọc dữ liệu, CHƯA cần các luồng ghi phức tạp (booking, mua gói).

Gợi ý slice tiếp theo cho B: `GET /api/classes` (danh sách lớp, đọc thuần, chưa cần enroll) — dễ vì không có logic nghiệp vụ, chỉ là CRUD Read.
Gợi ý slice tiếp theo cho C: `GET /api/packages` (danh sách gói tập, tương tự, đọc thuần).

### Cấp độ 3 — Luồng có logic nghiệp vụ (khó hơn, làm sau)

- B: Enrollment (phải check Capacity) → Booking (phải check trùng lịch Trainer)
- C: Purchase package → Payment confirm (phải update 2 bảng cùng lúc, cân nhắc transaction)

---

## 3. TÓM TẮT QUY TẮC LÀM VIỆC (khắc cốt ghi tâm)

1. Mỗi lần chỉ hoàn thành 1 slice trọn vẹn (đủ Model→Controller), test được bằng Swagger rồi mới sang slice khác.
2. Không tạo file "phòng khi sau này cần" — cần đến đâu tạo đến đó.
3. Repository không bao giờ chứa if/else nghiệp vụ. Thấy Repository có if/else phức tạp → sai chỗ, chuyển lên Service.
4. Controller không bao giờ so sánh, tính toán nghiệp vụ. Thấy Controller dài hơn 15-20 dòng cho 1 action → có gì đó đang sai.
5. Người khác không đụng file module mình, mình không đụng file module người khác. Cần đổi field chung → hỏi trước trong group chat, đừng tự sửa `AppDbContext.cs` gốc hay Model chung.