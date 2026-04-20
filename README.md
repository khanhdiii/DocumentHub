# DocumentHub

## Mô tả

DocumentHub là một ứng dụng WPF được xây dựng bằng .NET 8.0 để quản lý tài liệu đến và đi, theo dõi tiến độ công việc, và quản lý người dùng với hệ thống xác thực bảo mật.

## Tính năng chính

- **Quản lý tài liệu đến (Incoming Documents)**: Ghi nhận thông tin chi tiết như số đến, ngày đến, loại tài liệu, mức độ bảo mật, người gửi, vị trí, tóm tắt, và liên kết với nhân viên xây dựng, cán bộ nhận, người ký, người nhận.
- **Quản lý tài liệu đi (Outgoing Documents)**: Xử lý tài liệu gửi đi.
- **Theo dõi tiến độ công việc (Work Progress)**: Theo dõi các nhiệm vụ với tiến độ phần trăm, ngày bắt đầu, deadline, người giao việc, người phụ trách, và các chỉ số hàng tháng/năm/quý.
- **Xác thực người dùng**: Hệ thống đăng nhập với câu hỏi bảo mật để khôi phục PIN.
- **Thông báo**: Hiển thị thông báo trong ứng dụng.
- **Xuất dữ liệu**: Hỗ trợ xuất báo cáo sang PDF (PdfSharp) và Excel (ClosedXML).
- **Biểu đồ và giao diện**: Sử dụng LiveCharts cho biểu đồ tiến độ, FontAwesome cho biểu tượng, và giao diện WPF hiện đại.

## Yêu cầu hệ thống

- .NET 8.0
- Hệ điều hành Windows (do sử dụng WPF)
- Cơ sở dữ liệu: SQLite (tự động tạo file app.db)

## Cài đặt

1. Sao chép repository này về máy tính của bạn.
2. Mở file `DocumentHub.sln` trong Visual Studio 2022 hoặc phiên bản mới hơn.
3. Khôi phục các gói NuGet: Vào Tools > NuGet Package Manager > Restore NuGet Packages.

## Chạy ứng dụng

1. Xây dựng dự án: Build > Build Solution.
2. Chạy ứng dụng: Debug > Start Debugging (hoặc nhấn F5).

Ứng dụng sẽ tự động áp dụng các migration để tạo và cập nhật cơ sở dữ liệu SQLite.

## Xuất bản ứng dụng

Để tạo phiên bản tự chứa (self-contained) cho Windows x64:

```
dotnet publish DocumentHub.csproj -c Release -r win-x64 --self-contained true
```

File xuất bản sẽ nằm trong thư mục `bin/Release/net8.0-windows/win-x64/publish/`.

## Cấu trúc dự án

- **Model/**: Các lớp mô hình dữ liệu (IncomingDocument, WorkProgress, UserCredential, v.v.).
- **ViewModel/**: Logic nghiệp vụ và binding dữ liệu.
- **FrontEnd/Views/**: Các file XAML cho giao diện người dùng.
- **Data/**: AppDbContext cho Entity Framework Core.
- **Helpers/**: Các lớp trợ giúp như converters (BoolToVisibilityConverter), commands (RelayCommand), và services (AuthService).
- **Components/**: Các control tùy chỉnh như NotificationControl, WeeklyWorkView.
- **Migrations/**: Các file migration cho cơ sở dữ liệu.
- **Assets/**: Hình ảnh, biểu tượng, và file JSON cho câu hỏi bảo mật.

## Khắc phục sự cố

- Nếu gặp lỗi database, xóa file `app.db` và chạy lại ứng dụng để tạo mới.
- Đảm bảo .NET 8.0 SDK đã được cài đặt.
- Kiểm tra quyền truy cập file cho thư mục Assets.

## Đóng góp

Nếu bạn muốn đóng góp, vui lòng tạo issue hoặc pull request trên repository này.

## Giấy phép

Dự án này sử dụng giấy phép MIT. Xem file LICENSE để biết thêm chi tiết (nếu có).# DocumentHub

## Mô tả

DocumentHub là một ứng dụng WPF được xây dựng bằng .NET 8.0 để quản lý tài liệu đến và đi, theo dõi tiến độ công việc, và quản lý người dùng với hệ thống xác thực bảo mật.

## Tính năng chính

- **Quản lý tài liệu đến (Incoming Documents)**: Ghi nhận thông tin chi tiết như số đến, ngày đến, loại tài liệu, mức độ bảo mật, người gửi, vị trí, tóm tắt, và liên kết với nhân viên xây dựng, cán bộ nhận, người ký, người nhận.
- **Quản lý tài liệu đi (Outgoing Documents)**: Xử lý tài liệu gửi đi.
- **Theo dõi tiến độ công việc (Work Progress)**: Theo dõi các nhiệm vụ với tiến độ phần trăm, ngày bắt đầu, deadline, người giao việc, người phụ trách, và các chỉ số hàng tháng/năm/quý.
- **Xác thực người dùng**: Hệ thống đăng nhập với câu hỏi bảo mật để khôi phục PIN.
- **Thông báo**: Hiển thị thông báo trong ứng dụng.
- **Xuất dữ liệu**: Hỗ trợ xuất báo cáo sang PDF (PdfSharp) và Excel (ClosedXML).
- **Biểu đồ và giao diện**: Sử dụng LiveCharts cho biểu đồ tiến độ, FontAwesome cho biểu tượng, và giao diện WPF hiện đại.

## Yêu cầu hệ thống

- .NET 8.0
- Hệ điều hành Windows (do sử dụng WPF)
- Cơ sở dữ liệu: SQLite (tự động tạo file app.db)

## Cài đặt

1. Sao chép repository này về máy tính của bạn.
2. Mở file `DocumentHub.sln` trong Visual Studio 2022 hoặc phiên bản mới hơn.
3. Khôi phục các gói NuGet: Vào Tools > NuGet Package Manager > Restore NuGet Packages.

## Chạy ứng dụng

1. Xây dựng dự án: Build > Build Solution.
2. Chạy ứng dụng: Debug > Start Debugging (hoặc nhấn F5).

Ứng dụng sẽ tự động áp dụng các migration để tạo và cập nhật cơ sở dữ liệu SQLite.

## Xuất bản ứng dụng

Để tạo phiên bản tự chứa (self-contained) cho Windows x64:

```
dotnet publish DocumentHub.csproj -c Release -r win-x64 --self-contained true
```

File xuất bản sẽ nằm trong thư mục `bin/Release/net8.0-windows/win-x64/publish/`.

## Cấu trúc dự án

- **Model/**: Các lớp mô hình dữ liệu (IncomingDocument, WorkProgress, UserCredential, v.v.).
- **ViewModel/**: Logic nghiệp vụ và binding dữ liệu.
- **FrontEnd/Views/**: Các file XAML cho giao diện người dùng.
- **Data/**: AppDbContext cho Entity Framework Core.
- **Helpers/**: Các lớp trợ giúp như converters (BoolToVisibilityConverter), commands (RelayCommand), và services (AuthService).
- **Components/**: Các control tùy chỉnh như NotificationControl, WeeklyWorkView.
- **Migrations/**: Các file migration cho cơ sở dữ liệu.
- **Assets/**: Hình ảnh, biểu tượng, và file JSON cho câu hỏi bảo mật.

## Khắc phục sự cố

- Nếu gặp lỗi database, xóa file `app.db` và chạy lại ứng dụng để tạo mới.
- Đảm bảo .NET 8.0 SDK đã được cài đặt.
- Kiểm tra quyền truy cập file cho thư mục Assets.

## Đóng góp

Nếu bạn muốn đóng góp, vui lòng tạo issue hoặc pull request trên repository này.

## Giấy phép

Dự án này sử dụng giấy phép MIT. Xem file LICENSE để biết thêm chi tiết (nếu có).

Coding by Huỳnh Duy Khánh D5S-VB2A-DDTHS3-T05 

Đại đội 3 VB22 K02 2025-2026
