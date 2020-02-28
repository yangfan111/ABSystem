using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace IO
{
    public static partial class FileUtil
    {
        public static string GetMd5HashString(string input)
        {
            MD5           md5      = MD5.Create();
            byte[]        data     = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public static int GetMd5Hash(string input)
        {
            MD5    md5  = MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return data[0]
                            | data[1] << 8
                            | data[2] << 16
                            | data[3] << 24;
        }

        public static string GetMd5HashStringFromFile(string filename)
        {
            if (!File.Exists(filename)) return string.Empty;
            MD5    md5  = MD5.Create();
            byte[] data = md5.ComputeHash(File.ReadAllBytes(filename));

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}