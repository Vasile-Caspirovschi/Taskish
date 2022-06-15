using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Taskish.Commands;
using Taskish.Models;
using Taskish.Pages;

namespace Taskish.ViewModel
{
    public class ShowCategoryTasksViewModel : OnPropertyChanged
    {
        private ObservableCollection<Models.Task> _categoryTasks;
        public ObservableCollection<Models.Task> CategoryTasks
        {
            get { return _categoryTasks; }
            set { _categoryTasks = value; NotifyPropertyChanged(nameof(CategoryTasks)); }
        }

        public ShowCategoryTasksViewModel(Category selectedCategory)
        {
            ExportTasksToWord = new ExportTaskAsync("word");
            ExportTasksToPdf = new ExportTaskAsync("pdf");

            IsLoading = true;
            //call the long running method on a background thread...
            System.Threading.Tasks.Task.Run(() => LongRunningMethod(selectedCategory))
                .ContinueWith(task =>
                {
                    //and set the IsLoading property back to false back on the UI thread once the task has finished
                    IsLoading = false;
                }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void LongRunningMethod(Category selectedCategory)
        {
            Categories = Functionality.GetCategoriesFromDB();
            CategoryTasks = Functionality.GetTasksCategory(Functionality.GetTasks(Functionality.GetTasksFromDB()), selectedCategory);
        }
        public ICommand ExportTasksToWord { get; }
        public ICommand ExportTasksToPdf { get; }

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
        private ObservableCollection<Category> _categories;
        public ObservableCollection<Category> Categories
        {
            get { return _categories; }
            set { _categories = value; NotifyPropertyChanged(nameof(Categories)); }
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
                    if (page.Title == "CategoryTasks")
                        TaskCategory = ShowCategoryTasks.SelectedCategory;
                    if (string.IsNullOrWhiteSpace(TaskName))
                    {
                        Functionality.OpenMessageWindow("Please write a task.", "#c23c2f", Window.GetWindow(page));
                    }
                    else
                    {
                        Functionality.AddTaskToBD(TaskName, TaskDueDate, TaskCategory, CategoryTasks, page.Title);
                        TaskName = string.Empty;
                        TaskDueDate = null;
                        TaskCategory = null;
                    }
                });
            }
        }
    }
}
