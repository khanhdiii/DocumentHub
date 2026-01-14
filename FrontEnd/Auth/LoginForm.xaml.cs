using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using DocumentHub.FrontEnd.Views;
using DocumentHub.ViewModel.Auth;

namespace DocumentHub.FrontEnd.Auth
{
    public partial class LoginForm : Window
    {
        //Action call Notification
        public event Action<string, bool> Notify;

        private readonly AuthViewModel _viewModel;
        private readonly char[] _pinDigits = new char[6];
        private readonly string _message;
        private readonly bool _isSuccess;

        public LoginForm()
        {
            InitializeComponent();
            _viewModel = new AuthViewModel();
            DataContext = _viewModel;
            tb_Pin1.Focus();
        }

        public LoginForm(string message, bool isSuccess) : this()
        {
            if (!string.IsNullOrEmpty(message))
            {
                DocumentHub.Components.NotificationManager.Show(NotificationContainer, message, isSuccess);
            }
        }

        private static bool IsTextNumeric(string text) => Regex.IsMatch(text, "^[0-9]$");

        private void PinTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private void MoveFocusToNext(TextBox currentTextBox)
        {
            if (currentTextBox == tb_Pin1)
                tb_Pin2.Focus();
            else if (currentTextBox == tb_Pin2)
                tb_Pin3.Focus();
            else if (currentTextBox == tb_Pin3)
                tb_Pin4.Focus();
            else if (currentTextBox == tb_Pin4)
                tb_Pin5.Focus();
            else if (currentTextBox == tb_Pin5)
                tb_Pin6.Focus();
        }

        private int GetPinIndex(TextBox tb)
        {
            if (tb == tb_Pin1)
                return 0;
            if (tb == tb_Pin2)
                return 1;
            if (tb == tb_Pin3)
                return 2;
            if (tb == tb_Pin4)
                return 3;
            if (tb == tb_Pin5)
                return 4;
            if (tb == tb_Pin6)
                return 5;
            return -1;
        }

        private void PinTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null || tb.Text.Length != 1)
                return;

            int index = GetPinIndex(tb);
            if (index == -1)
                return;

            _pinDigits[index] = tb.Text[0];

            tb.TextChanged -= PinTextBox_TextChanged;
            tb.Text = "●";
            tb.CaretIndex = 1;
            tb.TextChanged += PinTextBox_TextChanged;

            if (tb != tb_Pin6)
            {
                MoveFocusToNext(tb);
            }
            else
            {
                var delayTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
                delayTimer.Tick += (s, args) =>
                {
                    delayTimer.Stop();

                    string fullPin = new string(_pinDigits);

                    using (var db = new DocumentHub.Data.AppDbContext())
                    {
                        var credential = db.UserCredentials.FirstOrDefault(u => u.PIN == fullPin);

                        if (credential != null)
                        {
                            // If have infor -> open Main
                            if (!string.IsNullOrEmpty(credential.SecurityQuestion1) &&
                                !string.IsNullOrEmpty(credential.SecurityAnswer1) &&
                                !string.IsNullOrEmpty(credential.SecurityQuestion2) &&
                                !string.IsNullOrEmpty(credential.SecurityAnswer2) &&
                                !string.IsNullOrEmpty(credential.SecondaryPassword))
                            {
                                var mainWindow = new Main("Đăng nhập thành công!", true);
                                mainWindow.Show();
                                this.Close();
                            }
                            else
                            {
                                // If not enough → open CreateForgotPIN
                                var forgotPinWindow = new CreateForgotPIN(credential.Id);
                                forgotPinWindow.Show();
                                this.Close();
                            }
                        }
                        else
                        {
                            // Not find PIN → Notification Error
                            ClearPinAndFocus();
                            DocumentHub.Components.NotificationManager.Show(NotificationContainer, "Mã PIN không chính xác", false);
                        }
                    }
                };
                delayTimer.Start();
            }
        }

        private void OpenGuide_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window
            {
                Title = "Hướng dẫn sử dụng",
                Content = new DocumentHub.FrontEnd.Views.UserGuide(),
                Height = 600,
                Width = 900
            };
            window.ShowDialog();
        }

        private void ClearPinAndFocus()
        {
            tb_Pin1.Clear();
            tb_Pin2.Clear();
            tb_Pin3.Clear();
            tb_Pin4.Clear();
            tb_Pin5.Clear();
            tb_Pin6.Clear();
            tb_Pin1.Focus();
            Array.Clear(_pinDigits, 0, _pinDigits.Length);
        }

        private void MoveFocusToPrevious(TextBox currentTextBox)
        {
            if (currentTextBox == tb_Pin6)
                tb_Pin5.Focus();
            else if (currentTextBox == tb_Pin5)
                tb_Pin4.Focus();
            else if (currentTextBox == tb_Pin4)
                tb_Pin3.Focus();
            else if (currentTextBox == tb_Pin3)
                tb_Pin2.Focus();
            else if (currentTextBox == tb_Pin2)
                tb_Pin1.Focus();
        }

        private void PinTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null)
                return;

            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                e.Handled = true;
                if (tb.Text.Length > 0)
                {
                    tb.Clear();
                    MoveFocusToPrevious(tb);
                    return;
                }
                MoveFocusToPrevious(tb);
            }
        }

        private void ForgotPIN_Click(object sender, RoutedEventArgs e)
        {
            // Open ForgotPIN
            var forgotPinWindow = new ForgotPIN(0); 
            forgotPinWindow.Show();

            this.Close();
        }

        private void ChangePIN_Click(object sender, RoutedEventArgs e)
        {
            var changePinWindow = new ChangePIN();
            changePinWindow.Show();

            this.Close();
        }
    }
}
