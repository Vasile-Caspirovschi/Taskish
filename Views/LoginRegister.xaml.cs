using Taskish.Models;
using Taskish.ViewModel;
using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Taskish.Views
{
    /// <summary>
    /// Interaction logic for LoginRegister.xaml
    /// </summary>
    public partial class LoginRegister : Window
    {
        private static string userName;
        public static string Username
        {
            get { return userName; }
            set { userName = value; }
        }

        public LoginRegister()
        {
            InitializeComponent();
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        bool IsHidden = false;
        private void moveToAnimation_Click(object sender, RoutedEventArgs e)
        {

            if (IsHidden)
            {
                IsHidden = false;
                welcomeBtn.Content = "Register";
                someText.Text = "If you are new, click on the button below to create your account!";
                WelcomeAnimation(0, -350);
                registerBtn.IsDefault = false;
                loginBtn.IsDefault = true;
            }
            else
            {
                IsHidden = true;
                welcomeBtn.Content = "Login";
                registerBtn.IsDefault = true;
                loginBtn.IsDefault = false;
                someText.Text = "If you already have an account click below to log in!";
                WelcomeAnimation(-350, 0);
            }
            ClearDataFields();
        }

        private bool WelcomeAnimation(int to, int from)
        {
            TranslateTransform moveWelcomeGrid = new TranslateTransform();
            welcome.RenderTransform = moveWelcomeGrid;
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = from;
            doubleAnimation.To = to;
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.4);
            moveWelcomeGrid.BeginAnimation(TranslateTransform.XProperty, doubleAnimation);
            return true;
        }

        private void recoveryPassword_Click(object sender, RoutedEventArgs e)
        {
            var recovery = new RecoveryPassword();
            recovery.Show();
            Close();
        }

        private async void login_Click(object sender, RoutedEventArgs e)
        {
            if (username.Text.Trim().Length != 0 && password.Password.Trim().Length != 0)
            {
                LoadingSpinner loading = new LoadingSpinner()
                {
                    Width = Width,
                    Height = Height,
                    Owner = this,
                    ShowInTaskbar = false,
                };
                Effect = new BlurEffect();
                loading.Show();
                
                try
                {
                    var user = await Functionality.GetUserAsync(username.Text);
                    if (user != null)
                    {
                        
                        if (user.Password == Functionality.EncryptPassword(password.Password.Trim()))
                        {
                            userName = user.UserName;
                            MainWindow window = new MainWindow();
                            loading.Close();
                            window.Show();
                            Close();
                        }
                        else
                        {
                            loading.Close();
                            var pass = Functionality.Decrypt(user.Password); 
                            Functionality.OpenMessageWindow("Password is incorrect!", "#c23c2f", this);
                        }
                    }
                    else
                    {
                        loading.Close();
                        Functionality.OpenMessageWindow("User with such username or email does not exist!", "#c23c2f", this);
                    }
                }
                catch (Exception ex)
                {
                    Functionality.OpenMessageWindow(ex.Message, "#c23c2f", this);
                }
            }
            else Functionality.OpenMessageWindow("To sign in, fill in all the boxes!", "#c23c2f", this);
        }

        private async void register_Click(object sender, RoutedEventArgs e)
        {
            if (usernameRegister.Text.Trim().Length != 0 && passwordRegister.Password.Trim().Length != 0
                && email.Text.Trim().Length != 0 && repeatPassword.Password.Length != 0
                && question.SelectedItem != null && answer.Text.Length != 0)
            {
                if (Functionality.IsUsernameValid(usernameRegister.Text.Trim(), out string message))
                {
                    if (Functionality.IsPasswordValid(passwordRegister.Password.Trim(), out string error))
                    {
                        if (Functionality.IsEmailValid(email.Text.Trim(), out string alert))
                        {
                            if (passwordRegister.Password == repeatPassword.Password)
                            {
                                if (question.SelectedItem != null && answer.Text.Trim().Length != 0)
                                {
                                    LoadingSpinner loading = new LoadingSpinner()
                                    {
                                        Width = Width,
                                        Height = Height,
                                        Owner = this,
                                        ShowInTaskbar = false,
                                    };
                                    Effect = new BlurEffect();
                                    loading.Show();
                                    if (await Functionality.RegisterUserAsync(usernameRegister.Text.Trim(), passwordRegister.Password.Trim(), email.Text.Trim(), question.SelectedValue.ToString(), answer.Text))
                                    {
                                        loading.Close();
                                        Functionality.OpenMessageWindow("Congratulations you have successfully registered!", "#17622c", this);
                                        MainWindow window = new MainWindow();
                                        window.Show();
                                        Close();
                                    }
                                    else
                                    {
                                        loading.Close();
                                        Functionality.OpenMessageWindow("A user with this name already exists!", "#c23c2f", this);
                                    }
                                }
                                else Functionality.OpenMessageWindow("Please choose and answer a security question!", "#c23c2f", this);
                            }
                            else Functionality.OpenMessageWindow("The passwords entered do not match each other!", "#c23c2f", this);
                        }
                        else Functionality.OpenMessageWindow(alert, "#c23c2f", this);
                    }
                    else Functionality.OpenMessageWindow(error, "#c23c2f", this);
                }
                else Functionality.OpenMessageWindow(message, "#c23c2f", this);
            }
            else Functionality.OpenMessageWindow("To register, fill all the boxes!", "#c23c2f", this);
        }
        private void showPassword_Checked(object sender, RoutedEventArgs e) => Functionality.ShowPassword(password, passwordUnmask);
        private void showPassword_Unchecked(object sender, RoutedEventArgs e) => Functionality.HidePassword(password, passwordUnmask);

        private void showPasswordRegister_Checked(object sender, RoutedEventArgs e) => Functionality.ShowPassword(passwordRegister, passwordRegisterUnmask);
        private void showPasswordRegister_Unchecked(object sender, RoutedEventArgs e) => Functionality.HidePassword(passwordRegister, passwordRegisterUnmask);

        private void showRepeatPassword_Checked(object sender, RoutedEventArgs e) => Functionality.ShowPassword(repeatPassword, repeatPasswordUnmask);
        private void showRepeatPassword_Unchecked(object sender, RoutedEventArgs e) => Functionality.HidePassword(repeatPassword, repeatPasswordUnmask);

        private void Border_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ClearDataFields()
        {
            username.Text = string.Empty;
            password.Password = string.Empty;
            repeatPassword.Password = string.Empty;
            usernameRegister.Text = string.Empty;
            question.SelectedItem = string.Empty;
            passwordRegister.Password = string.Empty;
            email.Text = string.Empty;
            answer.Text = string.Empty;
        }
    }
}
