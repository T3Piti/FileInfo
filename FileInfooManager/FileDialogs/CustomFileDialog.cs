using Microsoft.Win32;
using System.Windows;

namespace FileInfooManager.FileDialogs
{
    public class CustomFileDialog
    {
        string FilePath { get; set; }
        public bool SaveFileDialog(string html)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files(*.html)|*.html";
            if(saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                System.IO.File.WriteAllText(FilePath, html);
                MessageBox.Show("Файл сохранен");
                return true;
            }
            return false;
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}
