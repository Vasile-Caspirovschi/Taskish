using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Taskish.Commands;
using Taskish.Models;

namespace Taskish.ViewModel
{
    public class ImportantViewModel : OnPropertyChanged
    {
        private ObservableCollection<Models.Task> _allTasks;

        private ObservableCollection<Category> _categories = Functionality.GetCategoriesFromDB();
        public ObservableCollection<Category> Categories
        {
            get { return _categories; }
            set { _categories = value; NotifyPropertyChanged(nameof(Categories)); }
        }

        private ObservableCollection<Models.Task> _importantTasks;
        public ObservableCollection<Models.Task> ImportantTasks
        {
            get { return _importantTasks; }
            set { _importantTasks = value; NotifyPropertyChanged(nameof(ImportantTasks)); }
        }
        public ICommand ExportTasksToWord { get; }
        public ICommand ExportTasksToPdf { get; }
        public ImportantViewModel()
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
            ImportantTasks = Functionality.GetImportantTasks(Functionality.GetTasks(_allTasks), Functionality.GetPassedTasks(_allTasks)); ;
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
        public RelayCommand ClearCategory
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    TaskCategory = null;
                });
            }
        }
        public RelayCommand ClearDueDate
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    TaskDueDate = null;
                });
            }
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
                        Functionality.AddTaskToBD(TaskName, TaskDueDate, TaskCategory, ImportantTasks, page.Title);
                        TaskName = string.Empty;
                        TaskDueDate = null;
                        TaskCategory = null;
                    }
                });
            }
        }
    }
}
