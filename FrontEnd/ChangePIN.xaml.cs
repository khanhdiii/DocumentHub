using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace DocumentHub.FrontEnd
{
    /// <summary>
    /// Interaction logic for ChangePIN.xaml
    /// </summary>
    public partial class ChangePIN : Window
    {
        //Set PIN with first value
        private const string CurrentPin = "111111";
        private const int PinLength = 6;

        public ChangePIN()
        {
            InitializeComponent();
            pb_OldPIN.Focus();

            // Capture win in center
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        /// Handle PreviewTextInput: Rule input is number (0-9).
        private void PinTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Regex number (0-9)
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

            // Delete error old 
            tb_Message.Text = string.Empty;
        }

        /// Handle PreviewKeyDown:Keyboard Space will not work
       
        private void PinTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ///  Keyboard (Backspace, Delete, Arrow Keys) is allow in use.
            if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Tab)
            {
                return;
            }

            // Keyboard is not allow in use
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        /// Hanle Change PIN
        
        private void btn_ChangePIN_Click(object sender, RoutedEventArgs e)
        {
            string oldPin = pb_OldPIN.Password;
            string newPin = pb_NewPIN.Password;
            string confirmPin = pb_ConfirmNewPIN.Password;

            tb_Message.Text = string.Empty;
            tb_Message.Foreground = Brushes.Red; 

            // 1. Check length
            if (oldPin.Length != PinLength || newPin.Length != PinLength || confirmPin.Length != PinLength)
            {
                tb_Message.Text = $"Tất cả mã PIN phải có {PinLength} chữ số.";
                return;
            }

            // 2. Check Old PIN
            if (oldPin != CurrentPin)
            {
                tb_Message.Text = "Mã PIN cũ không chính xác.";
                return;
            }

            // 3. Check new PIN and old PIN is not same
            if (newPin == oldPin)
            {
                tb_Message.Text = "Mã PIN mới không được trùng với mã PIN cũ.";
                return;
            }

            // 4. Check new PIN and confirm PIN is same
            if (newPin != confirmPin)
            {
                tb_Message.Text = "Mã PIN mới và Xác nhận PIN mới không khớp.";
                return;
            }

            //  Show message Change PIN successful
            tb_Message.Text = "Đổi mã PIN thành công!";
            tb_Message.Foreground = Brushes.DarkGreen;

            // Set PIN is null when onClick
            pb_OldPIN.Clear();
            pb_NewPIN.Clear();
            pb_ConfirmNewPIN.Clear();

        }

        /// Handle Cancel

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}