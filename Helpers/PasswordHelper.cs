using System.Windows.Controls;

namespace DocumentHub.Helpers
{
    public static class PasswordHelper
    {
        public static string GetPassword(PasswordBox passwordBox)
        {
            return passwordBox.Password;
        }
    }
}
