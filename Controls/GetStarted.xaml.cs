using System.Windows;
using System.Windows.Controls;

namespace Taskish.Controls
{
    /// <summary>
    /// Interaction logic for GetStarted.xaml
    /// </summary>
    public partial class GetStarted : UserControl
    {
        public GetStarted()
        {
            InitializeComponent();
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(GetStarted));
    }
}
