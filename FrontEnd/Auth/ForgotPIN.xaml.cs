using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using DocumentHub.Components;

using Newtonsoft.Json;

namespace DocumentHub.FrontEnd.Auth
{
    public partial class ForgotPIN : Window
    {
        private readonly int _userId;
        public ForgotPIN(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadQuestions();
        }

        // Verify VietNamese
        private bool ContainsVietnameseCharacters(string text)
        {
            return Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(text)) != text;
        }

        // Submit
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            // Reset notification
            tb_MessageQuestion1.Text = "";
            tb_MessageQuestion2.Text = "";
            tb_MessagePassword.Text = "";
            tb_MessageConfirmPassword.Text = "";

            string answer1 = tbAnswer1.Text.Trim();
            string answer2 = tbAnswer2.Text.Trim();
            string newPassword = pbNewPassword.Visibility == Visibility.Visible ? pbNewPassword.Password.Trim() : tbNewPassword.Text.Trim();

            string confirmPassword = pbConfirmPassword.Visibility == Visibility.Visible ? pbConfirmPassword.Password.Trim(): tb_MessageConfirmPassword.Text.Trim();


            bool hasError = false;

            if (cbQuestion1.SelectedIndex == -1)
            {
                tb_MessageQuestion1.Text = "Vui lòng chọn câu hỏi 1!";
                hasError = true;
            }

            if (cbQuestion2.SelectedIndex == -1)
            {
                tb_MessageQuestion2.Text = "Vui lòng chọn câu hỏi 2!";
                hasError = true;
            }

            if (string.IsNullOrEmpty(answer1))
            {
                tb_MessageQuestion1.Text = "Vui lòng nhập câu trả lời câu hỏi 1!";
                hasError = true;
            }

            if (string.IsNullOrEmpty(answer2))
            {
                tb_MessageQuestion2.Text = "Vui lòng nhập câu trả lời câu hỏi 2!";
                hasError = true;
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                tb_MessagePassword.Text = "Vui lòng nhập mật khẩu mới!";
                hasError = true;
            }

            if (string.IsNullOrEmpty(confirmPassword))
            {
                tb_MessageConfirmPassword.Text = "Vui lòng xác nhận mật khẩu!";
                hasError = true;
            }

            if (hasError) return;

            if (newPassword != confirmPassword)
            {
                tb_MessagePassword.Text = "Mật khẩu và xác nhận không khớp!";
                tb_MessageConfirmPassword.Text = "Mật khẩu và xác nhận không khớp!";
                pbNewPassword.Clear();
                pbConfirmPassword.Clear();
                return;
            }

            if (ContainsVietnameseCharacters(newPassword))
            {
                tb_MessagePassword.Text = "Mật khẩu không được chứa ký tự tiếng Việt!";
                pbNewPassword.Clear();
                pbConfirmPassword.Clear();
                return;
            }

            // Successful
            using (var db = new DocumentHub.Data.AppDbContext())
            {
                var credential = db.UserCredentials.FirstOrDefault(u => u.Id == _userId);

                if (credential != null)
                {
                    bool matchQ1 = credential.SecurityQuestion1 == cbQuestion1.SelectedItem?.ToString()
                                   && credential.SecurityAnswer1 == tbAnswer1.Text.Trim();

                    bool matchQ2 = credential.SecurityQuestion2 == cbQuestion2.SelectedItem?.ToString()
                                   && credential.SecurityAnswer2 == tbAnswer2.Text.Trim();

                    if (matchQ1 && matchQ2)
                    {
                        var createPinWindow = new CreatePIN(_userId);
                        createPinWindow.Show();
                        this.Close();

                        tb_MessagePassword.Foreground = Brushes.Green;
                        tb_MessagePassword.Text = "Xác minh thành công!";
                        pbNewPassword.Clear();
                        pbConfirmPassword.Clear();
                    }
                    else
                    {
                        notificationControl.Show("❌ Câu hỏi hoặc câu trả lời không đúng!", false);
                    }
                }
            }

            tb_MessagePassword.Foreground = Brushes.Green;
            tb_MessagePassword.Text = "Xác minh thành công!";
            pbNewPassword.Clear();
            pbConfirmPassword.Clear();
        }

        // Load Question JSON
        public class SecurityQuestions
        {
            public List<string> textQuestions { get; set; }
            public List<string> numberQuestions { get; set; }
        }

        private void LoadQuestions()
        {
            string path = "/DocumentHub/Assets/Questions/SecurityQuestions.json";

            if (!File.Exists(path))
            {
                MessageBox.Show("Không tìm thấy file câu hỏi!", "Lỗi");
                return;
            }

            string json = File.ReadAllText(path);
            SecurityQuestions questions = JsonConvert.DeserializeObject<SecurityQuestions>(json);

            cbQuestion1.ItemsSource = questions.textQuestions;
            cbQuestion2.ItemsSource = questions.numberQuestions;
        }

        // Toggle New Password
        private void ToggleNewPassword_Click(object sender, RoutedEventArgs e)
        {
            if (pbNewPassword.Visibility == Visibility.Visible)
            {
                tbNewPassword.Text = pbNewPassword.Password;
                pbNewPassword.Visibility = Visibility.Collapsed;
                tbNewPassword.Visibility = Visibility.Visible;
            }
            else
            {
                pbNewPassword.Password = tbNewPassword.Text;
                tbNewPassword.Visibility = Visibility.Collapsed;
                pbNewPassword.Visibility = Visibility.Visible;
            }
        }

        // Toggle Confirm Password
        private void ToggleConfirmPassword_Click(object sender, RoutedEventArgs e)
        {
            if (pbConfirmPassword.Visibility == Visibility.Visible)
            {
                tbConfirmPassword.Text = pbConfirmPassword.Password;
                pbConfirmPassword.Visibility = Visibility.Collapsed;
                tbConfirmPassword.Visibility = Visibility.Visible;
            }
            else
            {
                pbConfirmPassword.Password = tbConfirmPassword.Text;
                tbConfirmPassword.Visibility = Visibility.Collapsed;
                pbConfirmPassword.Visibility = Visibility.Visible;
            }
        }


        // Icon i - Infomation
        private void InfoPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "• Không dùng tiếng Việt\n" +
                "• Không dấu\n" +
                "• Ít nhất 6 ký tự\n" +
                "• Nên có chữ & số",
                "Quy định mật khẩu",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        // Icon i - Infomation
        private void InfoConfirmPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "• Không dùng tiếng Việt\n" +
                "• Không dấu\n" +
                "• Ít nhất 6 ký tự\n" +
                "• Nên có chữ & số",
                "Quy định mật khẩu",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

    }
}
