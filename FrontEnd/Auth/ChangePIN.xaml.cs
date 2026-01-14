using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using DocumentHub.Helpers;


namespace DocumentHub.FrontEnd.Auth
{
    public partial class ChangePIN : Window
    {
        public ChangePIN()
        {
            InitializeComponent();
        }

        // Is Text Numeric
        private static bool IsTextNumeric(string text)
        {
            return Regex.IsMatch(text, "^[0-9]$");
        }

        private void PinTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private void PinTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Check spacing
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void btn_ChangePIN_Click(object sender, RoutedEventArgs e)
        {
            string oldPin = pb_OldPIN.Password;
            string newPin = pb_NewPIN.Password;
            string confirmPin = pb_ConfirmNewPIN.Password;

            // Reset message
            tb_Message.Text = "";
            tb_Message.Foreground = new SolidColorBrush(Colors.Red);

            // Check null
            if (string.IsNullOrEmpty(oldPin) || string.IsNullOrEmpty(newPin) || string.IsNullOrEmpty(confirmPin))
            {
                tb_Message.Text = "⚠️ Vui lòng nhập đầy đủ thông tin.";
                return;
            }

            // Check Length
            if (newPin.Length != 6)
            {
                tb_Message.Text = "⚠️ Mã PIN phải đúng 6 chữ số.";
                return;
            }

            //Check same
            if (newPin != confirmPin)
            {
                tb_Message.Text = "PIN mới không khớp!";
                return;
            }

            using (var db = new DocumentHub.Data.AppDbContext())
            {
                var credential = db.UserCredentials.FirstOrDefault(u => u.PIN == oldPin);
                if (credential != null)
                {
                    credential.PIN = newPin;
                    db.SaveChanges();

                    DocumentHub.Components.NotificationManager.Show(
                        NotificationContainer,
                        " Đổi PIN thành công!",
                        true
                    );
                }
                else
                {
                    DocumentHub.Components.NotificationManager.Show(
                        NotificationContainer,
                        "Mã PIN cũ không đúng!",
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
