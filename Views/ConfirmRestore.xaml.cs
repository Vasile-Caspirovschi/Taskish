using System.Windows;

namespace Taskish.Views
{
    /// <summary>
    /// Interaction logic for ConfirmRestore.xaml
    /// </summary>
    public partial class ConfirmRestore : Window
    {
        public ConfirmRestore(string message)
        {
            InitializeComponent();
            messageBlock.Text = message;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
