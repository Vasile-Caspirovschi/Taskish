using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Taskish.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace Taskish.ViewModel
{
    public class StatisticsViewModel
    {
        public SeriesCollection PrioritizationStatus { get; set; }
        public SeriesCollection CompletedLastSevenDays { get; set; }
        public SeriesCollection PlannedNextSevenDays { get; set; }
        public SeriesCollection CompletedLastFourWeeks { get; set; }
        public SeriesCollection PlannedNextFourWeeks { get; set; }

        public string[] PriorityLabels { get; set; } = { "None", "Low", "Medium", "High" };

        public string[] Last7DaysLabels { get; set; }
        private DateTime[] last7Days = Enumerable.Range(0, 7).Select(i => DateTime.Now.Date.AddDays(-i)).Reverse().ToArray();

        public string[] Next7DaysLabels { get; set; }
        private DateTime[] next7Days = Enumerable.Range(0, 7).Select(i => DateTime.Now.Date.AddDays(i)).Reverse().ToArray();


        public string[] Last4WeeksLabels { get; set; }
        private DateTime[] last4WeeksStartDays = new DateTime[4];
        private DateTime[] last4WeeksEndDays = new DateTime[4];

        public string[] Next4WeeksLabels { get; set; }
        private DateTime[] next4WeeksStartDays = new DateTime[4];
        private DateTime[] next4WeeksEndDays = new DateTime[4];

        ObservableCollection<Task> AllTasks = Functionality.GetTasksFromDB();
        ObservableCollection<Completed> CompletedTasks;
        ObservableCollection<Task> Tasks;

        public static User LoggedUser { get; set; } = Functionality.GetCurrentLoggedUser();
        public int AllTasksCount
        {
            get => Functionality.GetTasks(AllTasks).Count + Functionality.GetPassedTasks(AllTasks).Count;

        }
        public int ImportantTasksCount
        {
            get => Functionality.GetImportantTasks(Functionality.GetTasks(AllTasks), Functionality.GetPassedTasks(AllTasks)).Count;
        }
        public int CompletedTasksCount
        {
            get => AllTasks.Where(task => task.Status == "Completed").Count();
        }
        public int RemovedTasksCount
        {
            get => AllTasks.Where(task => task.IsDeleted == true).Count();
        }
        public int Lists
        {
            get => Functionality.GetCategoriesFromDB().Count;
        }

        public int TodayTasksCount
        {
            get => Functionality.GetTasksForToday(Functionality.GetTasks(AllTasks)).Count + Functionality.GetCompletedTodayTasks(AllTasks).Count;
        }
        public int TodayCompletedTasksCount
        {
            get => Functionality.GetCompletedTodayTasks(AllTasks).Count;
        }
        public string CurrentMonth { get; } = DateTime.Now.ToString("MMMM");
        public int CreatedTasksInCurrentMonth
        {
            get => AllTasks.Where(task => task.CreatedOn.Month == DateTime.Now.Month).Count();
        }
        public int CompletedTasksInCurrentMonth
        {
            get => CompletedTasks.Where(task => task.CompletedAt.Month == DateTime.Now.Month).Count();
        }
        private string? theBusiestDay;
        public string TheBusiestDay
        {
            get => theBusiestDay == null ?  "Indeterminate" : theBusiestDay;
            set => theBusiestDay = value;
        }
        private string? theFreestDay;
        public string TheFreestDay
        {
            get => theFreestDay == null ? "Indeterminate" : theFreestDay;
            set => theFreestDay = value;
        }

        public StatisticsViewModel()
        {
            CompletedTasks = Functionality.GetAllCompletedTasks(AllTasks);
            Tasks = Functionality.GetTasks(AllTasks);

            theBusiestDay = CompletedTasks.GroupBy(task => task.CompletedAt.Date).
                OrderByDescending(group => group.Count()).FirstOrDefault()?.Key.ToString("dddd, dd");

            theFreestDay = CompletedTasks.GroupBy(task => task.CompletedAt.Date).
                OrderBy(group => group.Count()).FirstOrDefault()?.Key.ToString("dddd, dd");

            PrioritizationStatus = PrioritizationTasksStatus();
            CompletedLastSevenDays = CompletedLast7Days();
            PlannedNextSevenDays = PlannedNext7Days();
            CompletedLastFourWeeks = CompletedLast4Weeks();
            PlannedNextFourWeeks = PlannedNext4Weeks();

            /* Sending email with monthly report can be optimized if the app will be in web environment.
                Because below it's sending if user are logged in app in the last day of current month.*/
            if (IsCurrentMonthEnd())
            {
                if (!LoggedUser.EmailWasSent)
                {
                    SendEmailWithMonthlyReport();
                    LoggedUser.EmailWasSent = true;
                }
            }
            else LoggedUser.EmailWasSent = false;
        }

        private DateTime GetFirstDayOfWeek(DateTime date)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = date.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
            if (diff < 0)
                diff += 7;
            return date.AddDays(-diff).Date;
        }
        private void GetLast4WeeksStartEnd()
        {
            last4WeeksStartDays[0] = GetFirstDayOfWeek(DateTime.Now);
            last4WeeksEndDays[0] = last4WeeksStartDays[0].AddDays(6);

            last4WeeksStartDays[1] = last4WeeksStartDays[0].AddDays(-7);
            last4WeeksEndDays[1] = last4WeeksStartDays[1].AddDays(6);

            last4WeeksStartDays[2] = last4WeeksStartDays[1].AddDays(-7);
            last4WeeksEndDays[2] = last4WeeksStartDays[2].AddDays(6);

            last4WeeksStartDays[3] = last4WeeksStartDays[2].AddDays(-7);
            last4WeeksEndDays[3] = last4WeeksStartDays[3].AddDays(6);
            Array.Reverse(last4WeeksStartDays);
            Array.Reverse(last4WeeksEndDays);
        }
        private void GetNext4WeeksStartEnd()
        {
            next4WeeksStartDays[0] = GetFirstDayOfWeek(DateTime.Now);
            next4WeeksEndDays[0] = next4WeeksStartDays[0].AddDays(6);

            next4WeeksStartDays[1] = DateTime.Now.AddDays(Functionality.CalculateOffset(DateTime.Now.DayOfWeek, DayOfWeek.Monday));
            next4WeeksEndDays[1] = next4WeeksStartDays[1].AddDays(6);

            next4WeeksStartDays[2] = next4WeeksEndDays[1].AddDays(1);
            next4WeeksEndDays[2] = next4WeeksStartDays[2].AddDays(6);

            next4WeeksStartDays[3] = next4WeeksEndDays[2].AddDays(1);
            next4WeeksEndDays[3] = next4WeeksStartDays[3].AddDays(6);
            Array.Reverse(next4WeeksStartDays);
            Array.Reverse(next4WeeksEndDays);
        }

        private SeriesCollection PrioritizationTasksStatus()
        {
            SeriesCollection prioritizationStatus = new SeriesCollection();
            for (double i = 0; i < PriorityLabels.Length; i++)
            {
                prioritizationStatus.Add(
                new ColumnSeries
                {
                    Title = PriorityLabels[(int)i],
                    Values = new ChartValues<ObservablePoint>
                        {
                            new ObservablePoint(i, AllTasks.Where(task => task.Priority == i).Count()),
                        },
                    Fill = PriorityColorConverter(i),
                    DataLabels = true,
                });
            }
            return prioritizationStatus;
        }
        private SeriesCollection CompletedLast7Days()
        {
            Last7DaysLabels = last7Days.Select(date => date.ToString("dddd")).ToArray();
            SeriesCollection completedLastSevenDays = new SeriesCollection();
            ChartValues<int> last7DaysValues = new ChartValues<int>();
            for (double i = 0; i < last7Days.Length; i++)
            {
                last7DaysValues.Add(
                        CompletedTasks.Where(task => task.CompletedAt.Date.Equals(last7Days[(int)i].Date)).Count()
                        );
            }
            completedLastSevenDays.Add(
                new RowSeries
                {
                    Title = "Tasks:",
                    Values = last7DaysValues,
                    Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#aeb1b4"),
                    DataLabels = true
                });

            return completedLastSevenDays;
        }
        private SeriesCollection PlannedNext7Days()
        {
            Next7DaysLabels = next7Days.Select(date => date.ToString("dddd")).ToArray();
            SeriesCollection plannedNextSevenDays = new SeriesCollection();
            ChartValues<int> next7DaysValues = new ChartValues<int>();
            for (int i = 0; i < last7Days.Length; i++)
            {
                next7DaysValues.Add(
                        Tasks.Where(task => task.DueDate.HasValue)
                        .Where(task => task.DueDate.Equals(DateOnly.FromDateTime(next7Days[i]))).Count()
                        );
            }
            plannedNextSevenDays.Add(
                new RowSeries
                {
                    Title = "Tasks:",
                    Values = next7DaysValues,
                    Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#aeb1b4"),
                    DataLabels = true
                });

            return plannedNextSevenDays;
        }
        private SeriesCollection CompletedLast4Weeks()
        {
            GetLast4WeeksStartEnd();

            var last4WeeksLabels = new string[4];
            for (int i = 0; i < last4WeeksStartDays.Length; i++)
            {
                last4WeeksLabels[i] = new string($"{last4WeeksStartDays[i]:dd} - {last4WeeksEndDays[i]:dd}");
            }
            Last4WeeksLabels = last4WeeksLabels;
            SeriesCollection completedLastFourWeeks = new SeriesCollection();
            ChartValues<int> last7DaysValues = new ChartValues<int>();
            for (int i = 0; i < last4WeeksStartDays.Length; i++)
            {
                last7DaysValues.Add(
                        CompletedTasks.Where(task => task.CompletedAt.Date >= last4WeeksStartDays[i].Date
                        && task.CompletedAt.Date <= last4WeeksEndDays[i].Date).Count());
            }
            completedLastFourWeeks.Add(
                new RowSeries
                {
                    Title = "Tasks:",
                    Values = last7DaysValues,
                    Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#aeb1b4"),
                    DataLabels = true
                });

            return completedLastFourWeeks;
        }
        private SeriesCollection PlannedNext4Weeks()
        {
            GetNext4WeeksStartEnd();

            var next4WeeksLabels = new string[4];
            for (int i = 0; i < next4WeeksStartDays.Length; i++)
            {
                next4WeeksLabels[i] = new string($"{next4WeeksStartDays[i]:dd} - {next4WeeksEndDays[i]:dd}");
            }
            Next4WeeksLabels = next4WeeksLabels;
            SeriesCollection plannedNextFourWeeks = new SeriesCollection();
            ChartValues<int> last7DaysValues = new ChartValues<int>();
            for (int i = 0; i < next4WeeksStartDays.Length; i++)
            {
                last7DaysValues.Add(
                        Tasks.Where(task => task.DueDate.HasValue)
                        .Where(task => task.DueDate >= DateOnly.FromDateTime(next4WeeksStartDays[i])
                        && task.DueDate <= DateOnly.FromDateTime(next4WeeksEndDays[i])).Count());
            }
            plannedNextFourWeeks.Add(
                new RowSeries
                {
                    Title = "Tasks:",
                    Values = last7DaysValues,
                    Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#aeb1b4"),
                    DataLabels = true
                });

            return plannedNextFourWeeks;
        }
        private SolidColorBrush PriorityColorConverter(double priority)
        {
            switch (priority)
            {
                case 3:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#de4c4a");
                case 2:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#f49c18");
                case 1:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#4073d6");
                case 0:
                    return (SolidColorBrush)new BrushConverter().ConvertFromString("#687681");
            }
            return (SolidColorBrush)new BrushConverter().ConvertFromString("#687681");
        }

        public async void SendEmailWithMonthlyReport()
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                string body = SendEmail.PopulateBody(LoggedUser.UserName, CurrentMonth, CreatedTasksInCurrentMonth.ToString(), CompletedTasksInCurrentMonth.ToString()
                 , AllTasksCount.ToString(), TheBusiestDay, TheFreestDay);
                SendEmail.SendMessageToEmail(Functionality.Decrypt(LoggedUser.Email), "Monthly statistics.", body);
            });
        }

        private bool IsCurrentMonthEnd()
        {
            DateTime date = DateTime.Now;
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            DateTime endOfMonth = new DateTime(date.Year, date.Month, daysInMonth);

            return date.Date == endOfMonth.Date;
        }
    }
}
