using Microsoft.Win32;
using Taskish.Commands;
using Taskish.Models;
using Taskish.Pages;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Taskish.ViewModel
{
    public class MainViewModel : OnPropertyChanged
    {
        private ObservableCollection<Category> categories = Functionality.GetCategoriesFromDB();

        private static ObservableCollection<string> _taskNames;
        public static ObservableCollection<string> TaskNames
        {
            get { return _taskNames; }
            set { _taskNames = value; }
        }
        public User LoggedUser { get; set; } = Functionality.GetCurrentLoggedUser();
        public MainViewModel()
        {
            TaskNames = Functionality.GetTaskNamesFromDB();
            _userImage = LoggedUser.Photo != null ? Functionality.ToBitmapImage(LoggedUser.Photo) : null;
        }
        public ObservableCollection<Category> Categories
        {
            get { return categories; }
            set { categories = value; NotifyPropertyChanged(nameof(Categories)); }
        }

        private BitmapImage _userImage;
        public BitmapImage UserImage
        {
            get { return _userImage; }
            set { _userImage = value; NotifyPropertyChanged(nameof(UserImage)); }
        }

        private string _categoryName;
        public string CategoryName
        {
            get { return _categoryName; }
            set { _categoryName = value; NotifyPropertyChanged(nameof(CategoryName)); }
        }

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get { return _selectedCategory; }
            set { _selectedCategory = value; NotifyPropertyChanged(nameof(SelectedCategory)); }
        }
        public RelayCommand LoadPhoto
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    OpenFileDialog op = new OpenFileDialog();
                    op.Title = "Select a picture";
                    op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                        "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                        "Portable Network Graphic (*.png)|*.png";
                    if (op.ShowDialog() == true)
                    {
                        UserImage = new BitmapImage(new Uri(op.FileName));
                        Functionality.SaveUserImage(UserImage);
                    }
                });
            }
        }
        public RelayCommand AddCategory
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var window = obj as Window;
                    if (string.IsNullOrWhiteSpace(CategoryName))
                    {
                        Functionality.OpenMessageWindow("Please write a category.", "#c23c2f", window);
                    }
                    else
                    {
                        if (Functionality.IsValidCategory(CategoryName))
                        {
                            Functionality.OpenMessageWindow("A such a category already exists.", "#c23c2f", window);
                        }
                        else
                        {
                            Functionality.AddCategoryToBD(CategoryName, Categories);
                        }
                        CategoryName = string.Empty;
                    }
                });
            }
        }
        public RelayCommand RemoveCategory
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var frame = obj as Frame;
                    if (frame.Content is ShowCategoryTasks)
                        if (ShowCategoryTasks.SelectedCategory == SelectedCategory)
                        {
                            frame.Navigate(new Uri("Pages/Imbox.xaml", UriKind.RelativeOrAbsolute));
                            (frame.Tag as RadioButton).IsChecked = true;
                        }
                    Functionality.DeleteCategory(SelectedCategory, Categories);
                });
            }
        }
    }
}
