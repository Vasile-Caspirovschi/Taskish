using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Taskish.Commands;
using Taskish.Models;
using Taskish.Views;

namespace Taskish.ViewModel
{
    public class CompletedViewModel : OnPropertyChanged
    {
        private ObservableCollection<Models.Completed> _completedTasks;
        public ObservableCollection<Models.Completed> CompletedTasks
        {
            get { return _completedTasks; }
            set { _completedTasks = value; NotifyPropertyChanged(nameof(CompletedTasks)); }
        }

        public CompletedViewModel()
        {
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
            CompletedTasks = Functionality.GetCompletedTasks(Functionality.GetTasksFromDB());
        }

        private bool _isLoading = true;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; NotifyPropertyChanged(nameof(IsLoading)); }
        }

        public RelayCommand MoveToTrashTasks
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var page = obj as Page;
                    MoveToTrashCompletedTasks(page);
                });
            }
        }

        private void MoveToTrashCompletedTasks(Page page)
        {
            string message = "Are you sure? All tasks below will be moved to the trash.";
            ConfirmDelete confirm;
            Functionality.ConfirmDelete(page, message, out confirm);
            confirm.ShowDialog();
            if (confirm.DialogResult == true)
            {
                Functionality.MoveToTrashAllCompleteTask();
                page.DataContext = new CompletedViewModel();
            }
            Window.GetWindow(page).Effect = null;
        }
    }
}
