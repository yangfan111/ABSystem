using System;
using System.IO;

namespace IO
{
    public static class FileUtil
    {
        public static void CreateDirFromFile(string filePath)
        {
            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath)) //判断路径是否存在
            {
                Directory.CreateDirectory(dirPath);
            }
        }
        
        /// <summary>
        /// Create from FileStream
        /// </summary>
       
        public static void CreateFile(string filePath, byte[] bytes, int length)
        {
            //先看看Dir是否存在再说
            CreateDirFromFile(filePath);
           
            FileStream sw;
            FileInfo file = new FileInfo(filePath);
            if (!file.Exists)
            {
                sw = file.Create();
            }
            else
            {
                return;
            }

            sw.Write(bytes, 0, length);
            sw.Close();
            sw.Dispose();
        }
        /// <summary>
        /// Create from FileStream
        /// </summary>
        public static void CreateFileByCallback(string filePath,System.Action<FileStream> callback)
        {
            CreateDirFromFile(filePath);
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                callback(fs);
            }
        }
    }

}