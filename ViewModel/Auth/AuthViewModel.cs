using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using DocumentHub.Helpers;

namespace DocumentHub.ViewModel.Auth
{
    public class AuthViewModel : INotifyPropertyChanged
    {
        //Action call Notification
        public event Action<string, bool> Notify;

        private string _pin;
        private string _message;

        public string PIN
        {
            get => _pin;
            set
            {
                if (_pin != value)
                {
                    _pin = value;
                    OnPropertyChanged(nameof(PIN));
                }
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged(nameof(Message));
                }
            }
        }

        public ICommand LoginCommand
        {
            get;
        }

        public AuthViewModel()
        {
            LoginCommand = new RelayCommand(_ => Login());
        }

        private void Login()
        {
            var auth = new AuthService();

            if (string.IsNullOrEmpty(PIN))
            {
                Notify?.Invoke("⚠️ Vui lòng nhập mã PIN!", false);
                return;
            }

            if (auth.TryLogin(PIN, out string resultMessage))
            {
                // Login Success 
                Message = resultMessage;

                // Open Main and noti job
                var mainWindow = new DocumentHub.FrontEnd.Main();
                mainWindow.Show();

                // Close LoginForm
                foreach (Window win in Application.Current.Windows)
                {
                    if (win is DocumentHub.FrontEnd.Auth.LoginForm)
                    {
                        win.Close();
                        break;
                    }
                }
            }
            else
            {
                // Login Fail
                Notify?.Invoke(resultMessage, false);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
