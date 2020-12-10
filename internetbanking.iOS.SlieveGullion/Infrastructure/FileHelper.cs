using InternetBanking.Contracts;
using InternetBanking.iOS.SlieveGullion;
using System;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper))]
namespace InternetBanking.iOS.SlieveGullion
{
    public class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string fileName)
        {
            var documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var libraryFolder = Path.Combine(documentFolder, "..", "Library", "Databases");

            if (!Directory.Exists(libraryFolder))
            {
                Directory.CreateDirectory(libraryFolder);
            }

            return Path.Combine(libraryFolder, fileName);
        }
    }
}
