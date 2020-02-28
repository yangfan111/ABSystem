using System.IO;

namespace IO
{
    
    public static class DirectoryUtil 
    {
        
        public static string AppRootPath
        {
            get { return System.IO.Path.GetDirectoryName(UnityEngine.Application.dataPath); }
        }

        public static string[] GetDirFiltedFiles(string dirPath,string filters)
        {
            var fileNames = System.IO.Directory.GetFiles(dirPath, filters, System.IO.SearchOption.AllDirectories);
            return fileNames;
        }
        
        public static void CreateDirFromFile(string filePath)
        {
            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath)) //判断路径是否存在
            {
                Directory.CreateDirectory(dirPath);
            }
        }
        /// <summary>
        /// Directory.CreateDirectory(destDirName);
        /// file.CopyTo(temppath, true)
        /// </summary>
        /// <returns></returns>
        public static bool DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            var dir = new System.IO.DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                UnityEngine.Debug.LogError("WwiseUnity: Source directory doesn't exist");
                return false;
            }

            var dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it. 
            if (!System.IO.Directory.Exists(destDirName))
                System.IO.Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = System.IO.Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (var subdir in dirs)
                {
                    var temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }

            return true;
        }

       
    }
}