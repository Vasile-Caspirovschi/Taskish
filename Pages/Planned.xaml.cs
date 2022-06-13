using Taskish.Models;
using Taskish.ViewModel;
using Taskish.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;

namespace Taskish.Pages
{
    /// <summary>
    /// Interaction logic for Planned.xaml
    /// </summary>
    public partial class Planned : Page
    {
        public Planned()
        {
            InitializeComponent();
        }
        private async void plannedPage_Loaded(object sender, RoutedEventArgs e)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.DataContext = new InboxViewModel();
                });
            });
        }

        private void setToday_Click(object sender, RoutedEventArgs e)
        {
            dueDate.SelectedDate = DateTime.Today;
        }
        private void setTomorrow_Click(object sender, RoutedEventArgs e)
        {

            dueDate.SelectedDate = DateTime.Today.AddDays(1);
        }
        private void setNextWeek_Click(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Now.Date;
            int offset = Functionality.CalculateOffset(now.DayOfWeek, DayOfWeek.Monday);
            DateTime nextWeek = now.AddDays(offset);
            dueDate.SelectedDate = nextWeek;
        }

        private void setDefaultCategory(object sender, RoutedEventArgs e)
        {
            setCategory.SelectedValue = "Inbox";
        }
        private async void changeStatus_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            Task task = cb.DataContext as Task;
            if (cb.IsChecked == true)
                Functionality.CompleteTask(task);
            else
                Functionality.IncompleteTask(task);
            await System.Threading.Tasks.Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.DataContext = new InboxViewModel();
                });
            });
        }

        private async void SetHighPriority_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Task task = button.DataContext as Task;

            Functionality.SetPriorityForTask(task, (byte)Task.TaskPriorities.High);
            await System.Threading.Tasks.Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.DataContext = new InboxViewModel();
                });
            });
        }

        private async void SetMediumPriority_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Task task = button.DataContext as Task;

            Functionality.SetPriorityForTask(task, (byte)Task.TaskPriorities.Medium);
            await System.Threading.Tasks.Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.DataContext = new InboxViewModel();
                });
            });
        }

        private async void SetLowPriority_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Task task = button.DataContext as Task;

            Functionality.SetPriorityForTask(task, (byte)Task.TaskPriorities.Low);
            await System.Threading.Tasks.Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.DataContext = new InboxViewModel();
                });
            });
        }

        private async void SetNonePriority_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Task task = button.DataContext as Task;

            Functionality.SetPriorityForTask(task, (byte)Task.TaskPriorities.None);
            await System.Threading.Tasks.Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.DataContext = new InboxViewModel();
                });
            });
        }

        private async void tasksList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var senderList = (ListView)sender;
            var clickedItem = senderList.SelectedItem as Task;
            if (clickedItem != null)
            {
                Window.GetWindow(this).Effect = new BlurEffect();
                EditTask editTaskWindow = new EditTask(clickedItem)
                {
                    ShowInTaskbar = false,
                    Topmost = true,
                    Owner = Window.GetWindow(this),
                };
                editTaskWindow.ShowDialog();
                if (editTaskWindow.DialogResult == true)
                    await System.Threading.Tasks.Task.Run(() => { this.Dispatcher.Invoke(() => { this.DataContext = new InboxViewModel(); }); });
                Window.GetWindow(this).Effect = null;
            }
        }
    }
}
