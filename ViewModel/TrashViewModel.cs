using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Taskish.Models;

namespace Taskish.ViewModel
{
    public class TrashViewModel : OnPropertyChanged
    {
        private ObservableCollection<Deleted> _removedTasks;
        public ObservableCollection<Deleted> RemovedTasks
        {
            get { return _removedTasks; }
            set { _removedTasks = value; NotifyPropertyChanged(nameof(RemovedTasks)); }
        }

        public TrashViewModel()
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
            RemovedTasks = Functionality.GetRemovedTasks(Functionality.GetTasksFromDB());
        }

        private bool _isLoading = true;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; NotifyPropertyChanged(nameof(IsLoading)); }
        }
    }
}
