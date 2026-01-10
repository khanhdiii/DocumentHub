using System.ComponentModel;
using System.Windows.Input;

using DocumentHub.Helpers;

namespace DocumentHub.ViewModel.Auth
{
    public class AuthViewModel : INotifyPropertyChanged
    {
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
                Message = "⚠️ Vui lòng nhập mã PIN!";
                return;
            }

            if (auth.TryLogin(PIN, out string resultMessage))
            {
                // Login Success 
                Message = resultMessage; 
            }
            else
            {
                // Login Fail
                Message = resultMessage; 
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
