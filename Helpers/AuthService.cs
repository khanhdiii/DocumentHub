using System;
using System.Linq;

using DocumentHub.Data;
using DocumentHub.Model;

namespace DocumentHub.Helpers
{
    public class AuthService
    {
        public bool TryLogin(string pin, out string message)
        {
            try
            {
                using var db = new AppDbContext();
                var cred = db.UserCredentials.FirstOrDefault();
                if (cred == null)
                {
                    message = " Không tìm thấy thông tin người dùng!";
                    return false;
                }

                if (cred.PIN == pin)
                {
                    message = " Đăng nhập thành công!";
                    return true;
                }
                else
                {
                    message = " Sai mã PIN!";
                    return false;
                }
            }
            catch (Exception ex)
            {
                message = $"⚠️ Lỗi kết nối DB: {ex.Message}";
                return false;
            }
        }

        public bool ChangePin(string oldPin, string newPin)
        {
            using var db = new AppDbContext();
            var cred = db.UserCredentials.FirstOrDefault();
            if (cred == null)
                return false;

            if (cred.PIN != oldPin)
                return false;

            cred.PIN = newPin;
            cred.LastUpdated = DateTime.Now;
            db.SaveChanges();
            return true;
        }


        public bool VerifyRecovery(string answer1, string answer2, string secondaryPassword, out string message)
        {
            using var db = new AppDbContext();
            var cred = db.UserCredentials.FirstOrDefault();
            if (cred == null)
            {
                message = "❌ Không tìm thấy thông tin người dùng!";
                return false;
            }

            bool ok = cred.SecurityAnswer1 == answer1 &&
                      cred.SecurityAnswer2 == answer2 &&
                      cred.SecondaryPassword == secondaryPassword;

            message = ok ? "✅ Xác minh thành công!" : "❌ Xác minh thất bại!";
            return ok;
        }

        public bool ResetPin(string newPin, out string message)
        {
            using var db = new AppDbContext();
            var cred = db.UserCredentials.FirstOrDefault();
            if (cred == null)
            {
                message = "❌ Không tìm thấy thông tin người dùng!";
                return false;
            }

            cred.PIN = newPin;
            cred.LastUpdated = DateTime.Now;
            db.SaveChanges();
            message = "✅ Reset PIN thành công!";
            return true;
        }
    }
}
