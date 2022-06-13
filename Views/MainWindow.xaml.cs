using Taskish.Models;
using Taskish.Pages;
using Taskish.ViewModel;
using Taskish.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Taskish
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            stat.DataContext = new StatisticsViewModel();
        }
        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            contentContainer.Navigate(new Uri("Pages/Today.xaml", UriKind.RelativeOrAbsolute));
            HowAreYou();
        }
        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnToday_Click(object sender, RoutedEventArgs e)
        {
            categories.SelectedItem = null;
            contentContainer.Navigate(new Uri("Pages/Today.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnPlanned_Click(object sender, RoutedEventArgs e)
        {
            categories.SelectedItem = null;
            contentContainer.Navigate(new Uri("Pages/Planned.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnTasks_Click(object sender, RoutedEventArgs e)
        {
            categories.SelectedItem = null;
            contentContainer.Navigate(new Uri("Pages/Imbox.xaml", UriKind.RelativeOrAbsolute));
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            ListViewItem myListViewItem = (ListViewItem)(categories.ItemContainerGenerator.ContainerFromItem(categories.SelectedItem));
            ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(myListViewItem);
            DataTemplate dataTemplate = contentPresenter.ContentTemplate;
            TextBox titleCategoryTextBox = (TextBox)dataTemplate.FindName("title", contentPresenter);

            titleCategoryTextBox.IsEnabled = true;
            titleCategoryTextBox.Focus();
            titleCategoryTextBox.CaretIndex = titleCategoryTextBox.Text.Length;
        }
        private void LostFocus_Event(object sender, RoutedEventArgs e)
        {
            TextBox textBox = ((TextBox)sender);
            textBox.IsEnabled = false;
            Functionality.EditCategory(textBox.Text, categories.SelectedItem as Category);
            UpdateFrameContent();
        }
        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private void RenameCategory(object sender, KeyEventArgs e)
        {
            TextBox newName = ((TextBox)sender);
            if (e.Key == Key.Return)
            {
                Functionality.EditCategory(newName.Text, categories.SelectedItem as Category);
                newName.IsEnabled = false;
                UpdateFrameContent();
            }
        }

        private void btnCompleted_Click(object sender, RoutedEventArgs e)
        {
            categories.SelectedItem = null;
            contentContainer.Navigate(new Uri("Pages/Completed.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnTrash_Click(object sender, RoutedEventArgs e)
        {
            categories.SelectedItem = null;
            contentContainer.Navigate(new Uri("Pages/Trash.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnImportant_Click(object sender, RoutedEventArgs e)
        {
            categories.SelectedItem = null;
            contentContainer.Navigate(new Uri("Pages/Important.xaml", UriKind.RelativeOrAbsolute));
        }

        private void categoryName_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateFrameContent();
        }

        void UpdateFrameContent()
        {
            var currentPage = (Page)contentContainer.Content;
            if (currentPage != null)
            {
                if (currentPage.Title == "CategoryTasks")
                    contentContainer.Content = new ShowCategoryTasks(categories.SelectedItem as Category);
                switch (currentPage.Title)
                {
                    case "Inbox": currentPage.DataContext = new InboxViewModel();
                        break;
                    case "Today":
                        currentPage.DataContext = new TodayViewModel();
                        break;
                    case "Planned":
                        currentPage.DataContext = new InboxViewModel();
                        break;
                    case "Important":
                        currentPage.DataContext = new ImportantViewModel();
                        break;
                    case "CategoryTasks":
                        currentPage.DataContext = new ShowCategoryTasksViewModel(categories.SelectedItem as Category);
                        break;
                }
            }
        }

        private void categories_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                foreach (var menuItem in FindVisualChilds<RadioButton>(this))
                    menuItem.IsChecked = false;
                contentContainer.Content = new ShowCategoryTasks(item as Category);
            }
        }

        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            LoginRegister loginRegister = new LoginRegister();
            loginRegister.Show();
            Close();
        }

        private void fellsToday_Click(object sender, RoutedEventArgs e)
        {
            HowAreYou();
        }

        private void HowAreYou()
        {
            Effect = new BlurEffect();
            UserStatus userStatusWindow = new UserStatus()
            {
                Owner = this,
                ShowInTaskbar = false,
            };
            userStatusWindow.ShowDialog();
            userStatus.Data = UserStatus.Path.Data;
            Effect = null;
        }

        public static IEnumerable<T> FindVisualChilds<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChilds<T>(ithChild)) yield return childOfChild;
            }
        }

        private void statistics_Checked(object sender, RoutedEventArgs e)
        {
            stat.DataContext = new StatisticsViewModel();
            stat.IsPopupOpen = true;
        }

        private void statistics_Unchecked(object sender, RoutedEventArgs e)
        {
            stat.DataContext = new StatisticsViewModel();
            stat.IsPopupOpen = true;
        }

        private void searchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                contentContainer.Content = new SearchResults(searchBox.Text);
                searchBox.Text = "";
            }
        }

        private void searchBox_GotFocus(object sender, RoutedEventArgs e) =>
            searchBox.ItemsSource = MainViewModel.TaskNames;

        private void searchBox_LostFocus(object sender, RoutedEventArgs e) => searchBox.Text = string.Empty;

    }
}
