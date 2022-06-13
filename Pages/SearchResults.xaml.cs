using Taskish.Models;
using Taskish.ViewModel;
using Taskish.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Taskish.Pages
{
    /// <summary>
    /// Interaction logic for SearchResults.xaml
    /// </summary>
    public partial class SearchResults : Page
    {
        private string TaskName;
        public SearchResults(string taskName)
        {
            InitializeComponent();
            TaskName = taskName;
            DataContext = new SearchViewModel(TaskName);
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
                    this.DataContext = new SearchViewModel(TaskName);
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
                    this.DataContext = new SearchViewModel(TaskName);
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
                    this.DataContext = new SearchViewModel(TaskName);
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
                    this.DataContext = new SearchViewModel(TaskName);
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
                    this.DataContext = new SearchViewModel(TaskName);
                });
            });
        }

        private async void tasksList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var senderList = (ListView)sender;
            var clickedItem = senderList.SelectedItem as Models.Task;
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
                {
                    await System.Threading.Tasks.Task.Run(() => { this.Dispatcher.Invoke(() => { this.DataContext = new SearchViewModel(TaskName); }); });
                }
                Window.GetWindow(this).Effect = null;
            }
        }
    }
}
