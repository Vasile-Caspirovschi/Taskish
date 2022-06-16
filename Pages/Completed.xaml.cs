using Taskish.Models;
using Taskish.ViewModel;
using Taskish.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Taskish.Pages
{
    /// <summary>
    /// Interaction logic for Completed.xaml
    /// </summary>
    public partial class Completed : Page
    {
        public Completed()
        {
            InitializeComponent();
        }
        private async void completePage_Loaded(object sender, RoutedEventArgs e)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.DataContext = new CompletedViewModel();
                });
            });
        }
        private void GetInfo_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Effect = new BlurEffect();
            MessageWindow.Message = "Expired tasks are automatically moved to the Trash. See the expiration date displayed in red.";
            MessageWindow.Color = (SolidColorBrush)new BrushConverter().ConvertFrom("#c23c2f");
            MessageWindow messageWindow = new MessageWindow()
            {
                Owner = Window.GetWindow(this),
                ShowInTaskbar = false,
                Topmost = true,
            };
            messageWindow.ShowDialog();
            Window.GetWindow(this).Effect = null;
        }
        private async void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            Models.Completed task = cb.DataContext as Models.Completed;
            if (cb.IsChecked == true)
                Functionality.RestoreCompleteTask(task);
            else
                Functionality.RestoreCompleteTask(task);
            await System.Threading.Tasks.Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.DataContext = new CompletedViewModel();
                });
            });
        }
        private async void MoveToTrash_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Models.Completed task = button.DataContext as Models.Completed;

            Window.GetWindow(this).Effect = new BlurEffect();
            ConfirmDelete confirm = new ConfirmDelete("Are you sure? This task will be moved to the trash.")
            {
                Topmost = true,
                Owner = Window.GetWindow(this),
                ShowInTaskbar = false,
            };
            confirm.ShowDialog();

            if (confirm.DialogResult == true)
            {
                Functionality.MoveToTrashCompleteTask(task);
                await System.Threading.Tasks.Task.Run(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.DataContext = new CompletedViewModel();
                    });
                });
            }
            Window.GetWindow(this).Effect = null;
        }
    }
}
