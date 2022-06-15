using System.Windows;
using System.Windows.Media;

namespace Taskish.Views
{
    /// <summary>
    /// Interaction logic for MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        public static string Message { get; set; }

        public static SolidColorBrush Color { get; set; }
        public MessageWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
