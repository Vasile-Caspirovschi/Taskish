using Taskish.Models;
using Taskish.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Taskish.Views
{
    /// <summary>
    /// Interaction logic for EditTask.xaml
    /// </summary>
    public partial class EditTask : Window
    {
        public byte Priority { get; set; }
        public bool Done { get; set; }
        public DateOnly? DueDate { get; set; }
        private Task Task { get; set; }
        public EditTask(Task task)
        {
            InitializeComponent();
            DataContext = task;
            Task = task;
            Priority = task.Priority;
            DueDate = task.DueDate;
            MainViewModel mainViewModel = new MainViewModel();
            setCategory.DataContext = mainViewModel;
            if (task.Category != null)
                setCategory.SelectedValue = task.Category.CategoryName;
            removeTask.DataContext = mainViewModel;
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

        private void dueDate_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = (DateTime)((Calendar)sender).SelectedDate;
            dueDateBlock.Text = $"Due {selectedDate.ToString("ddd, dd MMMM")}";
            DueDate = DateOnly.FromDateTime(selectedDate);
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            Task task = DataContext as Task;
            Functionality.EditTask(DataContext as Task, taskName.Text, taskDescription.Text, DueDate, setCategory.SelectedItem as Category, Done, Priority);
            DialogResult = true;
            Close();
        }
        private void SetHighPriority_Click(object sender, RoutedEventArgs e)
        {
            priority.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#de4c4a");
            Priority = 3;
        }

        private void SetMediumPriority_Click(object sender, RoutedEventArgs e)
        {
            priority.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#f49c18");
            Priority = 2;
        }

        private void SetLowPriority_Click(object sender, RoutedEventArgs e)
        {
            priority.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#4073d6");
            Priority = 1;
        }

        private void SetNonePriority_Click(object sender, RoutedEventArgs e)
        {
            priority.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#687681");
            Priority = 0;
        }

        private void status_Checked(object sender, RoutedEventArgs e) => Done = true;

        private void status_Unchecked(object sender, RoutedEventArgs e) => Done= false;

        private void taskName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(taskName.Text))
            {
                Functionality.OpenMessageWindow("The title of the task must not be empty", "#c23c2f", this);
                taskName.Text = Task.Name;
            }
        }

        private void removeTask_Click(object sender, RoutedEventArgs e)
        {
            Effect = new BlurEffect();
            ConfirmDelete confirm = new ConfirmDelete("Are you sure? This task will be moved to the trash.")
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false,
            };
            confirm.ShowDialog();
            Effect = null;
            Task task = DataContext as Task;
            if (confirm.DialogResult == true)
            {
                Functionality.RemoveTask(task);
                DialogResult = true;
                Effect = null;
                Close();
            }
        }
    }
}
