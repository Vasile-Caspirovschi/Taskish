using System.Windows;
using System.Windows.Shapes;

namespace Taskish.Views
{
    /// <summary>
    /// Interaction logic for UserStatus.xaml
    /// </summary>
    public partial class UserStatus : Window
    {
        public static Path Path { get; set; }
        public UserStatus()
        {
            InitializeComponent();
        }

        private void happy_Click(object sender, RoutedEventArgs e)
        {
            Path = userStatus1;
            Close();
        }

        private void confused_Click(object sender, RoutedEventArgs e)
        {
            Path = userStatus2;
            Close();
        }

        private void sad_Click(object sender, RoutedEventArgs e)
        {
            Path = userStatus3;
            Close();
        }
    }
}
