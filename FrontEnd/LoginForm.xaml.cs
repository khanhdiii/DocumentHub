using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace DocumentHub.FrontEnd
{
    public partial class LoginForm : Window
    {
        public LoginForm()
        {
            InitializeComponent();
            tb_Pin1.Focus();
        }

        //Handle check number
        private static bool IsTextNumeric(string text)
        {
            return Regex.IsMatch(text, "^[0-9]$");
        }

        //Function input number
        private void PinTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        //Handle focus next
        private void MoveFocusToNext(TextBox currentTextBox)
        {
            if (currentTextBox == tb_Pin1) tb_Pin2.Focus();
            else if (currentTextBox == tb_Pin2) tb_Pin3.Focus();
            else if (currentTextBox == tb_Pin3) tb_Pin4.Focus();
            else if (currentTextBox == tb_Pin4) tb_Pin5.Focus();
            else if (currentTextBox == tb_Pin5) tb_Pin6.Focus();
        }

        //Function Change number to "*"
        // When input target to next input and change number to "*"
        private void PinTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            if (tb.Text.Length == 1)
            {
                // Change number to "*"
                tb.TextChanged -= PinTextBox_TextChanged;
                tb.Text = "*";
                tb.CaretIndex = 1;
                tb.TextChanged += PinTextBox_TextChanged;
                MoveFocusToNext(tb);
            }
        }

        //Handle focus previous
        private void MoveFocusToPrevious(TextBox currentTextBox)
        {
            if (currentTextBox == tb_Pin6) tb_Pin5.Focus();
            else if (currentTextBox == tb_Pin5) tb_Pin4.Focus();
            else if (currentTextBox == tb_Pin4) tb_Pin3.Focus();
            else if (currentTextBox == tb_Pin3) tb_Pin2.Focus();
            else if (currentTextBox == tb_Pin2) tb_Pin1.Focus();
        }

        //Function change target when press Backspace and Delete
        private void PinTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            // If input Backspace or Delete
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                e.Handled = true;
                // If have number => delete number and target preview input
                if (tb.Text.Length > 0)
                {
                    tb.Clear();
                    MoveFocusToPrevious(tb);
                    return;
                }
                MoveFocusToPrevious(tb);
                return ;
            }

        }
    }
}
