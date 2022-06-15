using System.Collections.ObjectModel;
using System.Linq;
using Taskish.Models;

namespace Taskish.ViewModel
{
    public class SearchViewModel : OnPropertyChanged
    {
        private ObservableCollection<Task> _tasks;
        public ObservableCollection<Task> Tasks
        {
            get { return _tasks; }
            set { _tasks = value; NotifyPropertyChanged(nameof(Tasks)); }
        }
        public SearchViewModel(string name)
        {
            var allTasks = Functionality.GetTasksFromDB();
            Tasks = new ObservableCollection<Task>(Functionality.GetTasks(allTasks).Concat(Functionality.GetPassedTasks(allTasks)).Where(task => task.Name.Contains(name)));
        }
    }
}
