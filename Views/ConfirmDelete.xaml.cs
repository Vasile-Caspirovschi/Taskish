using System.Windows;

namespace Taskish.Views
{
    /// <summary>
    /// Interaction logic for ConfirmDelete.xaml
    /// </summary>
    public partial class ConfirmDelete : Window
    {
        public ConfirmDelete(string message)
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
