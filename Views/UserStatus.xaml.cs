using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
