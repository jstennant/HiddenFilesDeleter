using System;
using System.IO;

namespace HiddenFilesDeleter
{
    class Program
    {
        private static bool dryRun = true;
        private static StreamWriter resultsFile = null;

        public static void DeleteFile(FileInfo file)
        {
            resultsFile.WriteLine(file.FullName);
            if(!dryRun)
            {
                file.Delete();
            }
        }

        public static void RecursiveDeleteHiddenFiles(DirectoryInfo baseDir)
        {
            foreach (FileInfo file in baseDir.EnumerateFiles())
            {
                if(!file.Attributes.HasFlag(FileAttributes.System) && !file.Attributes.HasFlag(FileAttributes.Directory) && (file.Name.StartsWith(".") || file.Attributes.HasFlag(FileAttributes.Hidden)))
                {
                    DeleteFile(file);
                }
            }

            foreach (DirectoryInfo dir in baseDir.EnumerateDirectories())
            {
                if(!dir.Name.StartsWith("$"))
                {
                    RecursiveDeleteHiddenFiles(dir);
                }
            }
        }

        static void Main(string[] args)
        {
            dryRun = args.Length > 0 && String.Compare(args[0], "dry", true) == 0;

            string currentTime = DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss");
            resultsFile = new StreamWriter("HiddenFilesDeleterResults_" + currentTime + ".txt");
            resultsFile.WriteLine("Dry Run? " + dryRun);

            RecursiveDeleteHiddenFiles(new DirectoryInfo("."));
            resultsFile.Close();
        }
    }
}
