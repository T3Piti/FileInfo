using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FilesInfo
{
    public static class Files
    {
        /// <summary>
        /// Asynchronously generates a list of files paths for all files in the selected directory and subdirectories
        /// </summary>
        /// <param name="path">path to selected directory</param>
        /// <returns>a list(List<string>) of files paths</returns>
        public static async Task<List<string>> GetFilesAsync(string path)
        {
            return await Task.Run(() => GetFiles(path));
        }
        /// <summary>
        /// Asynchronously collects and groups information about files
        /// </summary>
        /// <param name="files">A list of files paths</param>
        /// <returns>A list of FilesModel(FileExtension(string), Quantity(int), AvgFileSize(doble), Periodicity(string))</returns>
        public static async Task<List<FilesModel>> GetInfoByPathListAsync(List<string> filesPaths)
        {
            return await Task.Run(() => GetInfoByPathList(filesPaths));
        }
        /// <summary>
        /// Asynchronously generates an html string from the FilesModel list
        /// </summary>
        /// <param name="files">A list of FilesModel</param>
        /// <returns>HTML report string with tree structure</returns>
        public static async Task<string> GenerateReportHTMLStringAsync(IList<FilesModel> files)
        {
            return await Task.Run(() => GenerateReportHTMLString(files));
        }
        /// <summary>
        /// Generates a list of files paths for all files in the selected directory and subdirectories
        /// </summary>
        /// <param name="path">path to selected directory</param>
        /// <returns>a list(List<string>) of files paths</returns>
        public static List<string> GetFiles(string path)
        {
            List<string> files = new List<string>();
            string[] directories = Directory.GetDirectories(path);
            if(directories.Length > 0)
            {
                foreach(var directory in directories)
                {
                    files.AddRange(GetFiles(directory));
                }
            }
            files.AddRange(Directory.GetFiles(path));
            return files;
        }
        /// <summary>
        /// collects and groups information about files
        /// </summary>
        /// <param name="files">A list of files paths</param>
        /// <returns>A list of FilesModel(FileExtension(string), Quantity(int), AvgFileSize(doble), Periodicity(string))</returns>
        public static List<FilesModel> GetInfoByPathList(List<string> files)
        {
            List<FilesModel> filesReport = new List<FilesModel>();
            foreach(var file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                string extension = Path.GetExtension(file);
                var size = fileInfo.Length;
                if(filesReport.Any(f => f.FileExtension == extension))
                {
                    var curFile = filesReport.Where(f => f.FileExtension == extension).First();
                    curFile.Quantity++;
                    curFile.AvgFileSize += size;
                }
                else
                {
                    filesReport.Add(new FilesModel
                    {
                        Quantity = 1,
                        FileExtension = extension,
                        AvgFileSize = size
                    });
                }
            }
            foreach(var file in filesReport)
            {
                file.AvgFileSize = Math.Round(file.AvgFileSize / (double)file.Quantity, 2);
                file.Periodicity = $"{Math.Round(file.Quantity / (double)files.Count * 100, 2)}% ({file.Quantity}/{files.Count})";
            }
            return filesReport;
        }
        /// <summary>
        /// Generates html string from FilesModel list
        /// </summary>
        /// <param name="files">A list of FilesModel</param>
        /// <returns>HTML report string with tree structure</returns>
        public static string GenerateReportHTMLString(IList<FilesModel> files)
        {
            string html = "<!DOCTYPE html>" +
                "<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">" +
                "<head>" +
                "<meta charset = \"utf-8\" />" +
                "<style>" +
                "   ul, #myUL {" +
                "   list-style-type: none;" +
                "}" +
                "/* Remove margins and padding from the parent ul */" +
                "# myUL {" +
                "   margin: 0;" +
                "   padding: 0;" +
                "}" +
                "/* Style the caret/arrow */" +
                ".caret {" +
                "    cursor: pointer;" +
                "    user-select: none;" +
                " /* Prevent text selection */" +
                "}" +
                "/* Create the caret/arrow with a unicode, and style it */" +
                ".caret::before {" +
                "    content: \"\\25B6\";" +
                "    color: black;" +
                "    display: inline-block;" +
                "    margin-right: 6px;" +
                "}" +
                "/* Rotate the caret/arrow icon when clicked on (using JavaScript) */" +
                ".caret-down::before {" +
                "    transform: rotate(90deg);" +
                "}" +
                "/* Hide the nested list */" +
                ".nested" +
                "{" +
                "    display: none;" +
                "}" +
                "/* Show the nested list when the user clicks on the caret/arrow (with JavaScript) */" +
                ".active" +
                "{" +
                "    display: block;" +
                "}" +
                "</style>" +
                "</head >" +
                "<body>" +
                "<div>" +
                "<ul id=myUL>";
            foreach(var file in files)
            {
                html += "<li>" +
                    $"      <span class=\"caret\">{file.FileExtension}</span>" +
                    $"          <ul class=\"nested\">" +
                    $"              <li>Avarega file size = {file.AvgFileSize}</li>" +
                    $"              <li>Pereodicity = {file.Periodicity}</li>" +
                    "           </ul>" +
                    "    </li>";
            }
            return html += "</ul>" +
                        "</div>" +
                        "<script>" +
                        "   let toggler = document.getElementsByClassName(\"caret\");" +
                        "   let i;" +
                        "   for(i = 0; i < toggler.length; i++) {" +
                        "       toggler[i].addEventListener(\"click\", function() {" +
                        "       this.parentElement.querySelector(\".nested\").classList.toggle(\"active\");" +
                        "       this.classList.toggle(\"caret-down\");" +
                        "       });" +
                        "   }" +
                        "</script>";
        }
    }
}
