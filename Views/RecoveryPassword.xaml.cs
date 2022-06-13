using Taskish.Models;
using Taskish.ViewModel;
using System.Windows;
using System.Windows.Media.Effects;

namespace Taskish.Views
{
    /// <summary>
    /// Interaction logic for RecoveryPassword.xaml
    /// </summary>
    public partial class RecoveryPassword : Window
    {
        public RecoveryPassword()
        {
            InitializeComponent();
        }
        private int remainingAttempts = 3;

        private void goBack_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginRegister();
            loginWindow.Show();
            Close();
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private async void recovery_Click(object sender, RoutedEventArgs e)
        {
            if (username.Text.Trim().Length != 0 && question.SelectedValue != null
                && password.Password.Trim().Length != 0 && repeatPassword.Password.Trim().Length != 0 && answer.Text.Trim().Length != 0)
            {
                LoadingSpinner loading = new LoadingSpinner()
                {
                    Width = Width,
                    Height = Height,
                    Owner = this,
                    ShowInTaskbar = false,
                };

                if (remainingAttempts != 0)
                {
                    Effect = new BlurEffect();
                    loading.Show();
                }

                var user = await Functionality.GetUserAsync(username.Text.Trim());
                if (user != null)
                {

                    if (Functionality.IsPasswordValid(password.Password, out string message))
                    {
                        if (password.Password == repeatPassword.Password)
                        {
                            if (question.SelectedValue.ToString() == user.Question)
                            {
                                if (remainingAttempts > 0)
                                {
                                    if (answer.Text == user.QuestionAnswer)
                                    {
                                        if (await Functionality.ChangeUserPasswordAsync(user, password.Password.Trim()))
                                        {
                                            loading.Close();
                                            Success();
                                        }
                                    }
                                    else
                                    {
                                        loading.Close();
                                        Functionality.OpenMessageWindow($"The answer to the security question is incorrect!\nRemaining attempts {--remainingAttempts}.", "#c23c2f", this);
                                    }
                                }
                                if (remainingAttempts == -1)
                                {
                                    if (token.Text.Trim() == user.ResetPasswordToken)
                                    {
                                        if (user.TokenSentTime.Value.AddMinutes(30) > System.DateTime.Now)
                                        {
                                            if (await Functionality.ChangeUserPasswordAsync(user, password.Password.Trim()))
                                            {
                                                loading.Close();
                                                Success();
                                            }
                                        }
                                        else
                                        {
                                            loading.Close();
                                            Functionality.OpenMessageWindow($"Password code was expired!\nA new password code was sent to your email!", "#c23c2f", this);
                                            SendResetTokenToEmail(user);
                                        }
                                    }
                                    else
                                    {
                                        loading.Close();
                                        Functionality.OpenMessageWindow($"The reset password code is incorrect!", "#c23c2f", this);
                                    }
                                }
                                if (remainingAttempts == 0)
                                {
                                    securityQuestionSection.Visibility = Visibility.Collapsed;
                                    tokenResetSection.Visibility = Visibility.Visible;
                                    SendResetTokenToEmail(user);
                                    Functionality.OpenMessageWindow($"Remaining attempts {remainingAttempts}.\nAn email password reset code has been sent to your email!", "#c23c2f", this);
                                    remainingAttempts = -1;
                                }
                            }
                            else
                            {
                                loading.Close();
                                Functionality.OpenMessageWindow("User security question does not match!", "#c23c2f", this);
                            }
                        }
                        else
                        {
                            loading.Close();
                            Functionality.OpenMessageWindow("The passwords entered do not match each other!", "#c23c2f", this);
                        }
                    }
                    else
                    {
                        loading.Close();
                        Functionality.OpenMessageWindow(message, "#c23c2f", this);
                    }
                }
                else
                {
                    loading.Close();
                    Functionality.OpenMessageWindow("User with such username or email does not exist!", "#c23c2f", this);
                }
            }
        }

        private void Success()
        {
            Functionality.OpenMessageWindow("The password was successfully changed!", "#17622c", this);
            LoginRegister window = new LoginRegister();
            window.Show();
            Close();
        }
        private async void SendResetTokenToEmail(User user)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                string token = Functionality.GenerateResetTokenAsync(user).Result;
                string body = SendEmail.PopulateBody(user.UserName,token);
                SendEmail.SendMessageToEmail(Functionality.Decrypt(user.Email), "Reset password", body);
            });
        }

        private void showRepeatPassword_Checked(object sender, RoutedEventArgs e) => Functionality.ShowPassword(repeatPassword, repeatPasswordUnmask);
        private void showRepeatPassword_Unchecked(object sender, RoutedEventArgs e) => Functionality.HidePassword(repeatPassword, repeatPasswordUnmask);

        private void showPassword_Checked(object sender, RoutedEventArgs e) => Functionality.ShowPassword(password, passwordUnmask);

        private void showPassword_Unchecked(object sender, RoutedEventArgs e) => Functionality.HidePassword(password, passwordUnmask);

        private void Border_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}

