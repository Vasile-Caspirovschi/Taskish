using Taskish.Commands;
using Taskish.Models;
using Taskish.Pages;
using Taskish.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Taskish.ViewModel
{
    public class InboxViewModel : OnPropertyChanged
    {
        private ObservableCollection<Models.Task> _allTasks;

        private ObservableCollection<Category> _categories;
        public ObservableCollection<Category> Categories
        {
            get { return _categories; }
            set { _categories = value; NotifyPropertyChanged(nameof(Categories)); }
        }

        private ObservableCollection<Models.Task> _tasks;
        public ObservableCollection<Models.Task> Tasks
        {
            get { return _tasks; }
            set { _tasks = value; NotifyPropertyChanged(nameof(Tasks)); }
        }

        private ObservableCollection<Models.Task> _passedTasks;
        public ObservableCollection<Models.Task> PassedTasks
        {
            get { return _passedTasks; }
            set { _passedTasks = value; NotifyPropertyChanged(nameof(PassedTasks)); }
        }
        public ICommand ExportTasksToWord { get; }
        public ICommand ExportTasksToPdf { get; }

        private List<DateOnly?> _dueDateTasksList;
        public List<DateOnly?> DueDateTasksList
        {
            get { return _dueDateTasksList; }
            set { _dueDateTasksList = value; NotifyPropertyChanged(nameof(DueDateTasksList)); }
        }
        public static DateOnly? DateForExport { get; set; }
        public InboxViewModel()
        {
            ExportTasksToWord = new ExportTaskAsync("word");
            ExportTasksToPdf = new ExportTaskAsync("pdf");

            IsLoading = true;
            //call the long running method on a background thread...
            System.Threading.Tasks.Task.Run(() => LongRunningMethod())
                .ContinueWith(task =>
                {
                    //and set the IsLoading property back to false back on the UI thread once the task has finished
                    IsLoading = false;
                }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void LongRunningMethod()
        {
            _allTasks = Functionality.GetTasksFromDB();
            Categories = Functionality.GetCategoriesFromDB();
            PassedTasks = Functionality.GetPassedTasks(_allTasks);
            Tasks = Functionality.GetTasks(_allTasks);
            DueDateTasksList = Tasks.Select(task => task.DueDate).Distinct().ToList();
        }

        private bool _isLoading = true;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; NotifyPropertyChanged(nameof(IsLoading)); }
        }

        private string _taskName;
        public string TaskName
        {
            get { return _taskName; }
            set { _taskName = value; NotifyPropertyChanged(nameof(TaskName)); }
        }

        private DateTime? _taskDueDate;
        public DateTime? TaskDueDate
        {
            get { return _taskDueDate; }
            set { _taskDueDate = value; NotifyPropertyChanged(nameof(TaskDueDate)); }
        }

        private Category? _taskCategory;
        public Category? TaskCategory
        {
            get { return _taskCategory; }
            set { _taskCategory = value; NotifyPropertyChanged(nameof(TaskCategory)); }
        }

        public RelayCommand AddTask
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Page page = obj as Page;
                    if (string.IsNullOrWhiteSpace(TaskName))
                    {
                        Functionality.OpenMessageWindow("Please write a task.", "#c23c2f", Window.GetWindow(page));
                    }
                    else
                    {
                        Functionality.AddTaskToBD(TaskName, TaskDueDate, TaskCategory, Tasks, page.Title);
                        TaskName = string.Empty;
                        TaskDueDate = null;
                        TaskCategory = null;
                    }
                });
            }
        }
        public RelayCommand MoveToTrashTasks
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var page = obj as Page;
                     MoveToTrashPassedTasks(page);
                });
            }
        }
        private async void MoveToTrashPassedTasks(Page page)
        {
            string message = "Are you sure? All passed tasks will be moved to the trash.";
            ConfirmDelete confirm;
            Functionality.ConfirmDelete(page, message, out confirm);
            confirm.ShowDialog();
            if (confirm.DialogResult == true)
            {
                Functionality.MoveToTrashPassedTasks(PassedTasks);
                await System.Threading.Tasks.Task.Run(() =>
                {
                    page.Dispatcher.Invoke(() =>
                    {
                        page.DataContext = new InboxViewModel();
                    });
                });
            }
            Window.GetWindow(page).Effect = null;
        }
    }
}
