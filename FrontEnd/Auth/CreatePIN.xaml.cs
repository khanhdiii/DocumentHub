using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DocumentHub.FrontEnd.Auth
{
    public partial class CreatePIN : Window
    {
        private readonly int _userId;
        private readonly string _secondaryPassword;
        public CreatePIN(int userId, string secondaryPassword)
        {
            InitializeComponent();
            _userId = userId;
            _secondaryPassword = secondaryPassword;
        }

        // Handle check regex Number 
        private void PinTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "[0-9]");
        }

        // Check not char
        private void PinTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        // Handle Create PIN
        private void btn_CreatePIN_Click(object sender, RoutedEventArgs e)
        {
            string newPin = pb_NewPIN.Password;
            string confirmPin = pb_ConfirmNewPIN.Password;

            // Reset message
            tb_Message.Text = "";
            tb_Message.Foreground = new SolidColorBrush(Colors.Red);

            // Check null
            if (string.IsNullOrEmpty(newPin) || string.IsNullOrEmpty(confirmPin))
            {
                tb_Message.Text = "Vui lòng nhập đầy đủ mã PIN.";
                return;
            }

            // Check length
            if (newPin.Length != 6)
            {
                tb_Message.Text = "Mã PIN phải đúng 6 chữ số.";
                return;
            }

            // Check same pin
            if (newPin != confirmPin)
            {
                tb_Message.Text = "Mã PIN xác nhận không khớp.";
                return;
            }

            //If done -> save db
            using (var db = new DocumentHub.Data.AppDbContext())
            {
                var credential = db.UserCredentials
                    .FirstOrDefault(u => u.SecondaryPassword == _secondaryPassword);

                if (credential != null)
                {
                    credential.PIN = newPin;
                    db.SaveChanges();


                    var loginForm = new LoginForm(" Mã PIN đã được tạo và lưu thành công!", true);
                    loginForm.Show();
                    this.Close();

                }
                else
                {
                    DocumentHub.Components.NotificationManager.Show(
                        NotificationContainer,
                        " Không tìm thấy người dùng hoặc mật khẩu bảo mật không khớp!",
                        false
                    );
                }
            }

        }
        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            var loginForm = new LoginForm();
            loginForm.Show();

            this.Close();
        }
    }
}
