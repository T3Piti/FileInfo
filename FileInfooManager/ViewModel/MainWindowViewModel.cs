using FileInfooManager.FileDialogs;
using FileInfooManager.RelayCommands;
using FilesInfo;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FileInfooManager.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string currentDir;
        public string CurrentDir
        {
            get => currentDir;
            set
            {
                currentDir = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<FilesModel> FilesList { get; set; }
        #region Commands
        private RelayCommand saveToHTMLCommand;
        /// <summary>
        /// Saves html file
        /// </summary>
        public RelayCommand SaveToHTMLCommand => saveToHTMLCommand ?? (saveToHTMLCommand = new RelayCommand(SaveToHTML));
        private void SaveToHTML(object obj)
        {
            var html = Files.GenerateReportHTMLString(FilesList);
            CustomFileDialog ofd = new CustomFileDialog();
            ofd.SaveFileDialog(html);
        }
        #endregion
        public MainWindowViewModel()
        {
            CurrentDir = Environment.CurrentDirectory;
            var filesPath = Files.GetFiles(currentDir);
            FilesList = new ObservableCollection<FilesModel>(Files.GetInfoByPathList(filesPath));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
