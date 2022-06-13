using Microsoft.Win32;
using Taskish.ViewModel;
using Spire.Doc;
using Spire.Doc.Documents;
//using Word = Microsoft.Office.Interop.Word;
using System;
//using Microsoft.Office.Interop.Word;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;

namespace Taskish.Models
{
    public class ExportTasks
    {
        public static ObservableCollection<Task> GetTasksForExport(string pageTitle, Category category, DateOnly? dateOnly)
        {
            ObservableCollection<Task> tasks;
            switch (pageTitle)
            {
                case "Inbox":
                    tasks = Functionality.GetTasks(Functionality.GetTasksFromDB());
                    break;
                case "Today":
                    tasks = Functionality.GetTasksForToday(Functionality.GetTasks(Functionality.GetTasksFromDB()));
                    break;
                case "Planned":
                    tasks = new ObservableCollection<Task>(Functionality.GetTasks(Functionality.GetTasksFromDB()).Where(task => task.DueDate == dateOnly));
                    
                    break;
                case "Important":
                    tasks = Functionality.GetImportantTasks(Functionality.GetTasks(Functionality.GetTasksFromDB()), Functionality.GetPassedTasks(Functionality.GetTasksFromDB()));
                    break;
                case "CategoryTasks":
                    tasks = Functionality.GetTasksCategory(Functionality.GetTasks(Functionality.GetTasksFromDB()), category);
                    break;
                default:
                    tasks = Functionality.GetTasks(Functionality.GetTasksFromDB());
                    break;
            }
            return tasks;
        }

        public static bool Export(string pageTitle, Category category, string format, DateOnly? dueDate)
        {
            var title = pageTitle == "CategoryTasks" ? category.CategoryName : pageTitle;

            string templatePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Templates\ExportTasksTemplate.dotx";
            if (File.Exists(templatePath))
            {
                bool resultOperation = false;
                try
                {
                    Document document = new Document();
                    document.LoadFromFile(templatePath, FileFormat.Dot);
                    Regex regex = new Regex(@"«pageTitle»");
                    document.Replace(regex, title);

                    var table = document.Sections[0].Tables[0] as Table;

                    var rowIndex = 1;
                    foreach (var task in GetTasksForExport(pageTitle, category, dueDate))
                    {
                        table.AddRow(6);
                        table.Rows[rowIndex].Cells[0].AddParagraph().AppendText(task.Name);
                        table.Rows[rowIndex].Cells[1].AddParagraph().AppendText(string.IsNullOrEmpty(task.Description) ? "No description" : task.Description);
                        table.Rows[rowIndex].Cells[2].AddParagraph().AppendText(string.IsNullOrEmpty(task.DueDate.ToString()) ? "No date" : task.DueDate.Value.ToString("ddd, dd MMMM"));
                        table.Rows[rowIndex].Cells[3].AddParagraph().AppendText(task.Category != null ? task.Category.CategoryName : "Inbox");
                        table.Rows[rowIndex].Cells[4].AddParagraph().AppendText(WritePriority(task.Priority));
                        rowIndex++;
                    }

                    ParagraphStyle style = new ParagraphStyle(document);
                    style.Name = "MyHeader";
                    style.CharacterFormat.TextColor = Color.White;
                    style.CharacterFormat.FontName = "Arial Black";
                    style.CharacterFormat.FontSize = 9;
                    style.CharacterFormat.Bold = true;
                    document.Styles.Add(style);

                    foreach (TableCell cell in table.FirstRow.Cells)
                    {
                        foreach (Paragraph par in cell.Paragraphs)
                        {
                            par.ApplyStyle("MyHeader");
                        }
                    }

                    resultOperation = SaveDocument(document, title, format);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                return resultOperation;
            }
            else
            {
                MessageBox.Show("Template was not found!");
                return false;
            }
        }

        private static string WritePriority(byte priority)
        {
            switch (priority)
            {
                case 0: return "None";
                case 1: return "Low";
                case 2: return "Medium";
                case 3: return "High";
                default: return "None";
            }
        }

        private static bool SaveDocument(Document document, string title, string format)
        {
            SaveFileDialog saveFile;
            if (format == "word")
            {
                saveFile = new SaveFileDialog()
                {
                    Filter = "Word Documents|*.docx",
                    RestoreDirectory = true,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    Title = "Save document",
                    FileName = @$"{title}.docx"
                };
            }
            else
            {
                saveFile = new SaveFileDialog()
                {
                    Filter = "Pdf Files|*.pdf",
                    RestoreDirectory = true,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    Title = "Save document",
                    FileName = @$"{title}.pdf"
                };
            }

            string fileName = saveFile.FileName;
            if (saveFile.ShowDialog() == true)
            {
                if (fileName != "")
                {
                    if (format == "word")
                        document.SaveToFile(saveFile.FileName, FileFormat.Docx);
                    else
                        document.SaveToFile(saveFile.FileName, FileFormat.PDF);
                    document.Close();
                    return true;
                }
            }
            return false;
        }
    }
}
