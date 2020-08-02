using System.IO;
using System.Web;

namespace RealEstate.Services.Helpers
{
    public static class FileHelpers
    {
        public static string ChangeFileExtension(string fileName, string newFileExtention)
        {
            var directoryName = Path.GetDirectoryName(fileName) ?? "";
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var resultFilePath = Path.Combine(directoryName, fileNameWithoutExtension + newFileExtention);
            return File.Exists(Path.Combine(HttpRuntime.AppDomainAppPath.TrimEnd('\\'), resultFilePath.TrimStart('\\'))) ? resultFilePath : fileName;
        }

        public static string AppendToFileName(string fileName, string stringToAppend)
        {
            var directoryName = Path.GetDirectoryName(fileName) ?? "";
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var fileExtention = Path.GetExtension(fileName);
            var resultFilePath = Path.Combine(directoryName, fileNameWithoutExtension + stringToAppend + fileExtention);
            return resultFilePath;
        }
    }
}