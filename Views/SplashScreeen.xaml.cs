using System.Windows;
using System.Windows.Input;

namespace Taskish.Views
{
    /// <summary>
    /// Interaction logic for SplashScreeen.xaml
    /// </summary>
    public partial class SplashScreeen : Window
    {
        public SplashScreeen()
        {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
