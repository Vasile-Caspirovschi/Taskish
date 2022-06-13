using Taskish.Models;
using Taskish.Pages;
using Taskish.ViewModel;
using Taskish.Views;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;

namespace Taskish.Commands
{
    public class ExportTaskAsync : AsyncCommand
    {
        private readonly string _formatExport;
        //private readonly Category _category;

        public ExportTaskAsync(string formatExport)
        {
            _formatExport = formatExport;
        }

        protected override async System.Threading.Tasks.Task ExecuteAync(object? parameter)
        {
            Page page = parameter as Page;
            LoadingSpinner loading = new LoadingSpinner()
            {
                Width = Window.GetWindow(page).Width,
                Height = Window.GetWindow(page).Height,
                Owner = Window.GetWindow(page),
                ShowInTaskbar = false,
            };
            Window.GetWindow(page).Effect = new BlurEffect();
            loading.Show();
            if (await DoExportTasks(page.Title, _formatExport))
            {
                loading.Close();
                Functionality.OpenMessageWindow("Congratulations your tasks was succesfully exported!", "#17622c", Window.GetWindow(page));
            }
            else
            {
                loading.Close();
                Functionality.OpenMessageWindow("Sorry, something went wrong with the export!", "#b33126", Window.GetWindow(page));
            }
        }

        private async Task<bool> DoExportTasks(string pageTitle, string format)
        {
            bool resultOperation = false;
            Category category = null;
            if (pageTitle == "CategoryTasks")
                category = ShowCategoryTasks.SelectedCategory;
            await System.Threading.Tasks.Task.Run(() =>
            {
                resultOperation = ExportTasks.Export(pageTitle, category, format, InboxViewModel.DateForExport);
            });
            InboxViewModel.DateForExport = null;
            return resultOperation;
        }
    }
}
