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
