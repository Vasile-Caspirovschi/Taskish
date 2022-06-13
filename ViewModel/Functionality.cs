using Microsoft.EntityFrameworkCore;
using Taskish.Models;
using Taskish.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace Taskish.ViewModel
{
    public class Functionality
    {
        public static void OpenMessageWindow(string message, string color, Window owner)
        {
            owner.Effect = new BlurEffect();
            MessageWindow.Message = message;
            MessageWindow.Color = (SolidColorBrush)new BrushConverter().ConvertFrom(color);
            MessageWindow window = new MessageWindow()
            {
                ShowInTaskbar = false,
                Topmost = true,
            };
            window.ShowDialog();
            owner.Effect = null;
        }
        public static ConfirmDelete ConfirmDelete(Page page, string message, out ConfirmDelete confirm)
        {
            Window.GetWindow(page).Effect = new BlurEffect();
            confirm = new ConfirmDelete(message)
            {
                Owner = Window.GetWindow(page),
                Topmost = true,
                ShowInTaskbar = false,
            };
            return confirm;
        }
        public static bool IsPasswordValid(string password, out string ErrorMessage)
        {
            var input = password;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                ErrorMessage = "Password should not be empty!";
                return false;
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{8,15}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(input))
            {
                ErrorMessage = "Password should contain At least one lower case letter!";
                return false;
            }
            else if (!hasUpperChar.IsMatch(input))
            {
                ErrorMessage = "Password should contain At least one upper case letter!";
                return false;
            }
            else if (!hasMiniMaxChars.IsMatch(input))
            {
                ErrorMessage = "Password should not be less than or greater than 12 characters!";
                return false;
            }
            else if (!hasNumber.IsMatch(input))
            {
                ErrorMessage = "Password should contain At least one numeric value!";
                return false;
            }

            else if (!hasSymbols.IsMatch(input))
            {
                ErrorMessage = "Password should contain At least one special case characters!";
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool IsUsernameValid(string username, out string ErrorMessage)
        {
            ErrorMessage = string.Empty;

            var firstCharacterMustBeALetter = new Regex(@"[0-9]+");// start with a letter,
            var hasMiniMaxLength = new Regex(@".{8,49}");// length between 8 to 50.


            if (string.IsNullOrWhiteSpace(username))
            {
                ErrorMessage = "Username should not be empty!";
                return false;
            }

            if (firstCharacterMustBeALetter.IsMatch(username[0].ToString()))
            {
                ErrorMessage = "The first character of the username must be a letter!";
                return false;
            }

            else if (!hasMiniMaxLength.IsMatch(username))
            {
                ErrorMessage = "Username length must be between 8 and 50 characters!";
                return false;
            }
            else
            {
                return true;
            }
        }

        internal static void SetPriorityForTask(System.Threading.Tasks.Task? task, byte none)
        {
            throw new NotImplementedException();
        }

        public static bool IsEmailValid(string email, out string ErrorMessage)
        {
            ErrorMessage = string.Empty;
            var emailPattern = new Regex(@"^[a-z0-9][-a-z0-9._]+@([-a-z0-9]+\.)+[a-z]{2,5}$");

            if (string.IsNullOrWhiteSpace(email))
            {
                ErrorMessage = "Email should not be empty!";
                return false;
            }

            if (!emailPattern.IsMatch(email))
            {
                ErrorMessage = "Invalid email!\nPlease note that the application accepts only Google emails at the moment!";
                return false;
            }
            else
            {
                return true;
            }
        }
        public static void ShowPassword(PasswordBox passwordHidden, TextBox passwordShow)
        {
            passwordShow.Visibility = Visibility.Visible;
            passwordHidden.Visibility = Visibility.Hidden;
            passwordShow.Text = passwordHidden.Password;
        }
        public static void HidePassword(PasswordBox passwordHidden, TextBox passwordShow)
        {
            passwordShow.Visibility = Visibility.Hidden;
            passwordHidden.Visibility = Visibility.Visible;
        }
        public static string EncryptPassword(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static async System.Threading.Tasks.Task<User> GetUserAsync(string username)
        {
            User user = null;
            await System.Threading.Tasks.Task.Run(() =>
            {
                using (ApplicationContext context = new ApplicationContext())
                {
                    user = IsEmailValid(username, out string error) ? context.Users.FirstOrDefault(user => user.Email == EncryptPassword(username))
                    : context.Users.FirstOrDefault(user => user.UserName == username);
                }
            });
            return user;
        }
        public static async System.Threading.Tasks.Task<bool> ChangeUserPasswordAsync(User user, string password)
        {
            var wasChanged = false;
            await System.Threading.Tasks.Task.Run(() =>
            {
                using (ApplicationContext context = new ApplicationContext())
                {
                    user.Password = Functionality.EncryptPassword(password);
                    context.Entry(user).State = EntityState.Modified;
                    context.SaveChanges();
                    wasChanged = true;
                }
            });
            return wasChanged;
        }
        public static async System.Threading.Tasks.Task<string> GenerateResetTokenAsync(User user)
        {
            var token = string.Empty;
            await System.Threading.Tasks.Task.Run(() =>
            {
                StringBuilder stringBuilder = new StringBuilder();
                Random random = new Random();
                for (int i = 0; i < 8; i++)
                    stringBuilder.Append(random.Next(0, 9));
                token = stringBuilder.ToString();

                using (ApplicationContext context = new ApplicationContext())
                {
                    user.ResetPasswordToken = token;
                    user.TokenSentTime = DateTime.Now;
                    context.Entry(user).State = EntityState.Modified;
                    context.SaveChanges();
                }
            });
            return token;
        }
        public static async System.Threading.Tasks.Task<bool> RegisterUserAsync(string username, string password, string email, string question, string answer)
        {
            bool isUserValid = false;
            await System.Threading.Tasks.Task.Run(() =>
            {
                using (ApplicationContext context = new ApplicationContext())
                {
                    if (!context.Users.Any(user => user.UserName == username))
                    {
                        User user = new User()
                        {
                            UserName = username,
                            Email = Functionality.EncryptPassword(email),
                            Password = Functionality.EncryptPassword(password),
                            Question = question,
                            QuestionAnswer = answer,
                        };
                        context.Users.Add(user);
                        context.SaveChanges();
                        LoginRegister.Username = username;
                        isUserValid = true;
                    }
                }
            });
            return isUserValid;
        }

        public static int CalculateOffset(DayOfWeek current, DayOfWeek desired)
        {
            // f( c, d ) = [7 - (c - d)] mod 7
            // f( c, d ) = [7 - c + d] mod 7
            // c is current day of week and 0 <= c < 7
            // d is desired day of the week and 0 <= d < 7
            int c = (int)current;
            int d = (int)desired;
            int offset = (7 - c + d) % 7;
            return offset == 0 ? 7 : offset;
        }

        public static ObservableCollection<Task> GetTasksFromDB()
        {
            ObservableCollection<Task> tasks;

            using (ApplicationContext context = new ApplicationContext())
            {
                var getTasksWithCategory = new List<Task>(
                    (from task in context.Tasks
                     join category in context.Categories on task.CategoryId equals category.CategoryId
                     where task.UserId == GetCurrentLoggedUserId()
                     select new Task
                     {
                         TaskID = task.TaskID,
                         Name = task.Name,
                         Description = task.Description,
                         DueDate = task.DueDate,
                         CreatedOn = task.CreatedOn,
                         IsCompleted = task.IsCompleted,
                         IsDeleted = task.IsDeleted,
                         Priority = task.Priority,
                         Category = category,
                         CategoryId = category.CategoryId,
                         UserId = task.UserId,
                     }));
                var getTasksWithoutCategory = new List<Task>(context.Tasks.Where(task => task.UserId == GetCurrentLoggedUserId() && task.CategoryId == null));
                var getTasks = getTasksWithoutCategory.Concat(getTasksWithCategory);
                tasks = new ObservableCollection<Task>(getTasks);
            }
            return tasks;
        }
        public static ObservableCollection<string> GetTaskNamesFromDB()
        {
            var allTasks = GetTasksFromDB();
            var tasks = new ObservableCollection<Task>(GetTasks(allTasks).Concat(GetPassedTasks(allTasks)));
            ObservableCollection<string> names;
            names = new ObservableCollection<string>(tasks.Select(task => task.Name));
            return names;
        }
        public static ObservableCollection<Task> GetTasks(ObservableCollection<Task> allTasks)
        {
            ObservableCollection<Task> tasks;
            tasks = new ObservableCollection<Task>(allTasks.Where(task => task.IsDeleted == false && task.IsCompleted == false && task.IsPassed == "Hasn't passed"));
            return tasks;
        }
        public static ObservableCollection<Task> GetPassedTasks(ObservableCollection<Task> allTasks)
        {
            ObservableCollection<Task> passedTasks;
            passedTasks = new ObservableCollection<Task>(allTasks.Where(task => task.IsDeleted == false && task.IsCompleted == false && task.IsPassed == "Has passed"));
            return passedTasks;
        }
        public static ObservableCollection<Deleted> GetRemovedTasks(ObservableCollection<Task> allTasks)
        {
            ObservableCollection<Deleted> removedTasks;
            using (ApplicationContext context = new ApplicationContext())
            {
                var getRemovedTasks = new List<Deleted>(
                    (from task in allTasks
                     join deletedTask in context.Deleted on task.TaskID equals deletedTask.TaskId
                     where task.UserId == GetCurrentLoggedUserId()
                     select new Deleted
                     {
                         DeletedTaskId = deletedTask.DeletedTaskId,
                         DeletedAt = deletedTask.DeletedAt,
                         Expire = deletedTask.Expire,
                         TaskId = task.TaskID,
                         Task = task
                     }));
                removedTasks = new ObservableCollection<Deleted>(getRemovedTasks);
            }
            CheckIfTaskHasExpired(removedTasks);
            return removedTasks;
        }
        public static ObservableCollection<Completed> GetCompletedTasks(ObservableCollection<Task> allTasks)
        {
            ObservableCollection<Completed> completedTasks;
            using (ApplicationContext context = new ApplicationContext())
            {
                var getCompletedTasks = new List<Completed>(
                    (from task in allTasks join completedTask in context.Completed on task.TaskID equals completedTask.TaskId
                     where task.UserId == GetCurrentLoggedUserId() && task.IsDeleted == false && task.IsCompleted == true
                     select new Completed
                     {
                         CompletedTaskId = completedTask.CompletedTaskId,
                         CompletedAt = completedTask.CompletedAt,
                         Expire = completedTask.Expire,
                         TaskId = task.TaskID,
                         Task = task
                     }));
                completedTasks = new ObservableCollection<Completed>(getCompletedTasks);
            }
            CheckIfTaskHasExpired(completedTasks);
            return completedTasks;
        }
        public static ObservableCollection<Task> GetTasksForToday(ObservableCollection<Task> tasks)
        {
            ObservableCollection<Task> tasksToday;
            tasksToday = new ObservableCollection<Task>(tasks.Where(task => task.DueDate.HasValue)
                .Where(task => task.DueDate.Value == DateOnly.FromDateTime(DateTime.Today)));
            return tasksToday;
        }
        public static ObservableCollection<Task> GetImportantTasks(ObservableCollection<Task> tasks, ObservableCollection<Task> passedTasks)
        {
            ObservableCollection<Task> importantTasks;
            importantTasks = new ObservableCollection<Task>(tasks.Where(task => task.Priority == 3).Concat(passedTasks.Where(passedTask => passedTask.Priority == 3)));
            return importantTasks;
        }
        public static ObservableCollection<Task> GetCompletedTodayTasks(ObservableCollection<Task> allTasks)
        {
            ObservableCollection<Task> tasksToday;
            tasksToday = new ObservableCollection<Task>(allTasks.Where(task => task.DueDate.HasValue)
                .Where(task => task.DueDate.Value == DateOnly.FromDateTime(DateTime.Today) && task.Status == "Completed" && task.IsDeleted == false));
            return tasksToday;
        }
        public static ObservableCollection<Task> GetTasksCategory(ObservableCollection<Task> tasks, Category selectedCategory)
        {
            ObservableCollection<Task> tasksCategory;
            tasksCategory = new ObservableCollection<Task>(
                    (from task in tasks
                     join category in GetCategoriesFromDB() on task.CategoryId equals category.CategoryId
                     where task.CategoryId == selectedCategory.CategoryId
                     select new Task
                     {
                         TaskID = task.TaskID,
                         Name = task.Name,
                         Description = task.Description,
                         DueDate = task.DueDate,
                         IsCompleted = task.IsCompleted,
                         IsDeleted = task.IsDeleted,
                         Priority = task.Priority,
                         Category = category,
                         CategoryId = category.CategoryId,
                         UserId = task.UserId,
                     }));
            return tasksCategory;
        }

        public static ObservableCollection<Completed> CheckIfTaskHasExpired(ObservableCollection<Completed> completedTasks)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                foreach (var completedTask in completedTasks)
                {
                    if (completedTask.Expire < DateTime.Now)
                    {
                        Task taskToRemove = context.Tasks.Find(completedTask.TaskId);
                        RemoveTask(taskToRemove);
                        completedTasks.Remove(completedTask);
                    }
                }
            }
            return completedTasks;
        }
        public static ObservableCollection<Deleted> CheckIfTaskHasExpired(ObservableCollection<Deleted> removedTasks)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                foreach (var removedTask in removedTasks)
                {
                    if (removedTask.Expire < DateTime.Now)
                    {
                        DeleteTask(removedTask);
                        removedTasks.Remove(removedTask);
                    }
                }
            }
            return removedTasks;
        }

        public static int GetCurrentLoggedUserId()
        {
            int currentUserId = 0;
            using (ApplicationContext context = new ApplicationContext())
            {
                currentUserId = context.Users.FirstOrDefault(user => user.UserName == LoginRegister.Username).UserId;
            }
            return currentUserId;
        }
        public static User GetCurrentLoggedUser()
        {
            User user;
            using (ApplicationContext context = new ApplicationContext())
            {
                user = context.Users.FirstOrDefault(user => user.UserId == GetCurrentLoggedUserId());
            }
            return user;
        }
        public static void SaveUserImage(BitmapImage image)
        {
            var user = GetCurrentLoggedUser();
            user.Photo = BitmapSourceToByteArray(image);
            using (ApplicationContext context = new ApplicationContext())
            {
                context.Attach(user);
                context.Update(user);
                context.SaveChanges();
            }
        }
        public static BitmapImage ToBitmapImage(byte[] array)
        {
            using (var ms = new MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
        private static byte[] BitmapSourceToByteArray(BitmapSource image)
        {
            using (var stream = new MemoryStream())
            {
                var encoder = new JpegBitmapEncoder(); // or some other encoder
                encoder.QualityLevel = 100;
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                return stream.ToArray();
            }
        }

        public static void AddTaskToBD(string taskName, DateTime? taskDueDate, Category category, ObservableCollection<Task> tasks, string addTo = "default")
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                Task newTask = new Task()
                {
                    Name = taskName,
                    DueDate = taskDueDate.HasValue ? DateOnly.FromDateTime(taskDueDate.Value)
                            : (addTo == "Today" ? DateOnly.FromDateTime(DateTime.Today) : null),
                    UserId = GetCurrentLoggedUserId(),
                    Description = string.Empty,
                    Category = category != null ? category : null,
                    CategoryId = category != null ? category.CategoryId : null,
                    Priority = (byte)(addTo == "Important" ? 3 : 0),
                };

                if (category != null)
                    context.Categories.Attach(category);

                context.Tasks.Add(newTask);
                context.SaveChanges();
                if (newTask.DueDate == DateOnly.FromDateTime(DateTime.Today) && addTo == "Today")
                    tasks.Add(newTask); 
                if (addTo == "Important" || addTo == "Inbox" || addTo == "Planned")
                    tasks.Add(newTask);
                if (addTo == "CategoryTasks")
                    tasks.Add(newTask);
                MainViewModel.TaskNames.Add(newTask.Name);
            }
        }
        public static void RemoveTask(Task task)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                Task taskToRemove = context.Tasks.Find(task.TaskID);
                if (taskToRemove != null)
                {
                    taskToRemove.IsDeleted = true;
                    Deleted deletedTask = new Deleted()
                    {
                        DeletedAt = DateTime.Now,
                        Expire = DateTime.Now.AddDays(30),
                        Task = taskToRemove,
                        TaskId = taskToRemove.TaskID
                    };
                    context.Deleted.Add(deletedTask);
                    context.Tasks.Attach(taskToRemove);
                    context.Tasks.Update(taskToRemove);
                    context.SaveChanges();
                    if (MainViewModel.TaskNames.Contains(task.Name))
                        MainViewModel.TaskNames = GetTaskNamesFromDB();
                }
            }
        }
        public static void EditTask(Task task, string newName, string description, DateOnly? dueDate, Category category, bool done, byte priority)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                Task taskToEdit = context.Tasks.Find(task.TaskID);
                if (taskToEdit != null)
                {
                    taskToEdit.Name = newName;
                    taskToEdit.Description = description;
                    taskToEdit.DueDate = dueDate;
                    taskToEdit.IsCompleted = done;
                    taskToEdit.Priority = priority;
                    taskToEdit.Category = category != null ? category : null;
                    taskToEdit.CategoryId = category != null ? category.CategoryId : null;

                    if (category != null)
                        context.Categories.Attach(category);

                    //SetPriorityForTask(taskToEdit, priority);
                    context.Tasks.Attach(taskToEdit);
                    context.Tasks.Update(taskToEdit);
                    if (done)
                        CompleteTask(taskToEdit);
                    context.SaveChanges();
                }
            }
        }
        public static void DeleteTask(Deleted taskToDelete)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                Task task = context.Tasks.Find(taskToDelete.TaskId);
                context.Tasks.Remove(task);
                context.SaveChanges();
            }
        }
        public static void DeleteAllTasks()
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                foreach (var task in context.Deleted)
                    DeleteTask(task);
            }
        }
        public static void RestoreTask(Deleted taskToRestore)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                Task restoreTask = context.Tasks.Find(taskToRestore.TaskId);
                restoreTask.IsDeleted = false;
                if (restoreTask.Category != null)
                    context.Categories.Attach(restoreTask.Category);
                context.Tasks.Update(restoreTask);
                context.SaveChanges();
                if (restoreTask.IsCompleted == true)
                    CompleteTask(restoreTask);
                else MainViewModel.TaskNames.Add(restoreTask.Name);
            }
            using (ApplicationContext newContext = new ApplicationContext())
            {
                newContext.Deleted.Remove(taskToRestore);
                newContext.SaveChanges();
            }
        }
        public static void RestoreAllTasks()
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                foreach (var task in context.Deleted)
                    RestoreTask(task);
            }
        }
        public static void CompleteTask(Task task)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                Task taskToComplete = context.Tasks.Find(task.TaskID);
                if (taskToComplete != null)
                {
                    taskToComplete.IsCompleted = true;
                    Completed completedTask = new Completed()
                    {
                        CompletedAt = DateTime.Now,
                        Expire = DateTime.Now.AddDays(7),
                        Task = taskToComplete,
                        TaskId = taskToComplete.TaskID
                    };
                    context.Completed.Add(completedTask);
                    context.Tasks.Attach(taskToComplete);
                    context.Tasks.Update(taskToComplete);
                    context.SaveChanges();
                    if (MainViewModel.TaskNames.Contains(task.Name))
                        MainViewModel.TaskNames = GetTaskNamesFromDB();
                }
            }
        }
        public static void IncompleteTask(Task task)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                Task taskToComplete = context.Tasks.Find(task.TaskID);
                Completed completedTask = context.Completed.FirstOrDefault(t => t.TaskId == task.TaskID);
                if (taskToComplete != null)
                {
                    taskToComplete.IsCompleted = false;
                    context.Tasks.Attach(taskToComplete);
                    context.Tasks.Update(taskToComplete);
                }
                context.Completed.Remove(completedTask);
                context.SaveChanges();
                MainViewModel.TaskNames.Add(taskToComplete.Name);
            }
        }
        public static void RestoreCompleteTask(Completed task)
        {
            IncompleteTask(task.Task);
        }
        public static void MoveToTrashCompleteTask(Completed task)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                Completed completedTask = context.Completed.Find(task.CompletedTaskId);
                if (completedTask != null)
                {
                    context.Completed.Remove(completedTask);
                    context.SaveChanges();
                    Task taskToRemove = context.Tasks.Find(completedTask.TaskId);
                    RemoveTask(taskToRemove);
                }
            }
        }
        public static void MoveToTrashAllCompleteTask()
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                foreach (var task in context.Completed)
                    MoveToTrashCompleteTask(task);
            }
        }
        public static void SetPriorityForTask(Task task, byte priority)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                Task taskForSet = context.Tasks.Find(task.TaskID);
                taskForSet.Priority = priority;
                context.Tasks.Update(taskForSet);
                context.SaveChanges();
            }
        }
        public static void MoveToTrashPassedTasks(ObservableCollection<Task> passedTasks)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                foreach(var task in passedTasks)
                {
                    Task passedTask = context.Tasks.Find(task.TaskID);
                    if (passedTask != null)
                        RemoveTask(passedTask);
                }
            }
        }

        public static ObservableCollection<Category> GetCategoriesFromDB()
        {
            ObservableCollection<Category> categories;

            using (ApplicationContext context = new ApplicationContext())
            {
                categories = new ObservableCollection<Category>(context.Categories.Where(category => category.UserId == GetCurrentLoggedUserId()));
            }
            return categories;
        }
        public static void AddCategoryToBD(string categoryName, ObservableCollection<Category> categories)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                var newCategory = new Category()
                {
                    CategoryName = categoryName,
                    UserId = GetCurrentLoggedUserId(),
                };
                context.Categories.Add(newCategory);
                context.SaveChanges();
                categories.Add(newCategory);
            }
        }
        public static bool IsValidCategory(string categoryName)
        {
            bool checkIsExist;
            using (ApplicationContext context = new ApplicationContext())
            {
                checkIsExist = context.Categories.Where(c => c.UserId == GetCurrentLoggedUserId()).Any(c => c.CategoryName.ToLower().Trim() == categoryName.ToLower().Trim());
            }
            return checkIsExist;
        }
        public static void DeleteCategory(Category category, ObservableCollection<Category> categories)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                bool checkIsExist = context.Categories.Any(c => c.CategoryId == category.CategoryId);
                if (checkIsExist)
                {
                    context.Categories.Remove(category);
                    context.SaveChanges();
                    categories.Remove(category);
                }
            }
        }
        public static void EditCategory(string newName, Category category)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                bool checkIsExist = context.Categories.Any(c => c.CategoryId == category.CategoryId);
                if (checkIsExist)
                {
                    category.CategoryName = newName;
                    context.Categories.Attach(category);
                    context.Categories.Update(category);
                    context.SaveChanges();
                }
            }
        }
    }
}
