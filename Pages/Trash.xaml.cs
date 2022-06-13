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
    /// Interaction logic for Trash.xaml
    /// </summary>
    public partial class Trash : Page
    {
        public Trash()
        {
            InitializeComponent();
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.DataContext = new TrashViewModel();
                });
            });
        }
        private async void deleteTask_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Deleted taskToDelete = button.DataContext as Deleted;

            Window.GetWindow(this).Effect = new BlurEffect();
            ConfirmDelete confirm = new ConfirmDelete("Are you sure? This task will be permanently deleted.")
            {
                Owner = Window.GetWindow(this),
                ShowInTaskbar = false,
            };
            confirm.ShowDialog();
            if (confirm.DialogResult == true)
            {
                Functionality.DeleteTask(taskToDelete);
                await System.Threading.Tasks.Task.Run(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.DataContext = new TrashViewModel();
                    });
                });
            }
            Window.GetWindow(this).Effect = null;
        }

        private async void restoreTask_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Deleted taskToRestore = button.DataContext as Deleted;

            Functionality.RestoreTask(taskToRestore);
            await System.Threading.Tasks.Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.DataContext = new TrashViewModel();
                });
            });
        }

        private async void deleteAllTask_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Effect = new BlurEffect();
            ConfirmDelete confirm = new ConfirmDelete("Are you sure? All tasks below will be permanently deleted.")
            {
                Owner = Window.GetWindow(this),
                ShowInTaskbar = false,
            };
            confirm.ShowDialog();
            if (confirm.DialogResult == true)
            {
                Functionality.DeleteAllTasks();
                await System.Threading.Tasks.Task.Run(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.DataContext = new TrashViewModel();
                    });
                });
            }
            Window.GetWindow(this).Effect = null;
        }

        private async void restoreALLTask_Click(object sender, RoutedEventArgs e)
        {
            this.Effect = new BlurEffect();
            ConfirmRestore confirm = new ConfirmRestore("Are you sure? All tasks below will be restored.")
            {
                //Owner = App.Current.MainWindow,
                ShowInTaskbar = false,
            };
            confirm.ShowDialog();
            if (confirm.DialogResult == true)
            {
                Functionality.RestoreAllTasks();
                await System.Threading.Tasks.Task.Run(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.DataContext = new TrashViewModel();
                    });
                });
            }
            this.Effect = null;
        }

        private void getInfo_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Effect = new BlurEffect();
            MessageWindow.Message = "The trash can is automatically cleaned of expired tasks. See the expiration date displayed in red.";
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
    }
}
