using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DocumentHub.FrontEnd
{
    public partial class CreatePIN : Window
    {
        public CreatePIN()
        {
            InitializeComponent();
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

            if (string.IsNullOrEmpty(newPin) || string.IsNullOrEmpty(confirmPin))
            {
                tb_Message.Text = "Vui lòng nhập đầy đủ mã PIN.";
                return;
            }

            if (newPin.Length < 4 || newPin.Length > 6)
            {
                tb_Message.Text = "Mã PIN phải từ 4 đến 6 chữ số.";
                return;
            }

            if (newPin != confirmPin)
            {
                tb_Message.Text = "Mã PIN xác nhận không khớp.";
                return;
            }

            // If ok
            tb_Message.Text = "Mã PIN đã được tạo thành công!";
            tb_Message.Foreground = new SolidColorBrush(Colors.Green);

        }

        // Handle cancel
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
