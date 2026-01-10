using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

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

            if (newPin != confirmPin)
            {
                tb_Message.Text = "❌ PIN mới không khớp!";
                return;
            }

            var auth = new AuthService();
            if (auth.ChangePin(oldPin, newPin))
            {
                tb_Message.Text = "✅ Đổi PIN thành công!";
            }
            else
            {
                tb_Message.Text = "❌ Đổi PIN thất bại!";
            }
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
