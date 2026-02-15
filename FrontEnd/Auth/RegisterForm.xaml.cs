using System.Linq;
using System.Windows;
using DocumentHub.Data;
using DocumentHub.Model;
using DocumentHub.Components;

namespace DocumentHub.FrontEnd.Auth
{
    public partial class RegisterForm : Window
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void btn_RegisterPIN_Click(object sender, RoutedEventArgs e)
        {
            string pin = tb_Pin.Password.Trim();
            string confirmPin = tb_ConfirmPin.Password.Trim();

        if (string.IsNullOrEmpty(pin) || string.IsNullOrEmpty(confirmPin))
        {
            tb_Message.Text = "⚠️ Vui lòng nhập đầy đủ PIN!";
            return;
        }

        if (pin.Length != 6 || confirmPin.Length != 6)
        {
            tb_Message.Text = "⚠️ PIN phải gồm đúng 6 số!";
            return;
        }

        if (pin != confirmPin)
        {
            tb_Message.Text = "⚠️ PIN xác nhận không khớp!";
            return;
        }


            using (var db = new AppDbContext())
            {
                if (db.UserCredentials.Any(u => u.PIN == pin))
                {
                    NotificationManager.Show(NotificationContainer, "⚠️ PIN này đã tồn tại, vui lòng chọn PIN khác!", false);
                    return;
                }

                var newUser = new UserCredential { PIN = pin };
                db.UserCredentials.Add(newUser);
                db.SaveChanges();
            }

            NotificationManager.Show(NotificationContainer, "✅ Đăng ký PIN thành công!", true);
            var loginForm = new LoginForm("Đăng ký PIN thành công!", true);
            loginForm.Show();
            this.Close();

        }

       private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            var loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }
    }
}
