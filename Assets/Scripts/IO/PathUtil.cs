using System;
using System.IO;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;

namespace IO
{
    public static class PathUtil
    {
        //XXXXXX/Windows/
        #region //for assetbundle
     public static string ProcessAssetBundleBasePath(string path)
        {
            string processedPath = path;

            if (string.IsNullOrEmpty(processedPath))
                processedPath = ".";

            if (!processedPath.StartsWith("http", StringComparison.Ordinal) && !processedPath.StartsWith("ftp", StringComparison.Ordinal))
                processedPath = Path.GetFullPath(processedPath).Replace("\\", "/");

            if (!processedPath.EndsWith("/", StringComparison.Ordinal))
                processedPath += "/";

            return processedPath + PlatformUtil.GetPlatformName() + "/";
        }
        
        public static string GetNameWithoutVariant(string bundleName)
        {
            string baseName = bundleName;

            var dotIndex = bundleName.IndexOf('.');
            if (dotIndex != -1)
                baseName = bundleName.Substring(0, dotIndex);

            return baseName;
        }

        //Application.platform
        //EditorUserBuildSettings.activeBuildTarget
     
        #endregion

        #region //for wise
        /// <summary>
        /// string path_unityEditor = System.IO.Path.Combine(Application.dataPath, "Assets/CoreRes/Sound/WiseBank");
        /// FixSlashes(ref path_unityEditor);
        /// </summary>
        /// <param name="path"></param>
        /// <param name="separatorChar">要替换的Trim</param>
        /// <param name="badChar">要被替换的Trim</param>
        /// <param name="addTrailingSlash">是否在尾部加上下划线</param>
    public static void FixSlashes(ref string path, char separatorChar, char badChar, bool addTrailingSlash)
    {
        if (string.IsNullOrEmpty(path))
            return;
//rim("abcd".ToCharArray())就是删除字符串头部及尾部出现的a或b或c或d字符，删除的过程直到碰到一个既不是a也不是b也不是c也不是d的字符才结束。
        path = path.Trim().Replace(badChar, separatorChar).TrimStart('\\');

        // Append a trailing slash to play nicely with Wwise
        if (addTrailingSlash && !path.EndsWith(separatorChar.ToString()))
            path += separatorChar;
    }
     public static readonly char wrongSeparatorChar = System.IO.Path.DirectorySeparatorChar == '/' ? '\\' : '/';
    public static void FixSlashes(ref string path,bool shortFix = false)
    {
#if UNITY_WSA
		var separatorChar = '\\';
#else
        var separatorChar = System.IO.Path.DirectorySeparatorChar;
#endif // UNITY_WSA
        var badChar = separatorChar == '\\' ? '/' : '\\';
        if (shortFix)
        {
            path = path.Replace(badChar, separatorChar);
            return;
        }
        FixSlashes(ref path, separatorChar, badChar, true);
    }
    /// <summary>
    /// 1.new System.Uri 标准化路径
    /// 2.str.Replace    替换错误字符
    /// </summary>
    public static string GetFullNormalizedPath(string str)
    {
        str = System.IO.Path.GetFullPath(new System.Uri(str).LocalPath);
        return str.Replace(wrongSeparatorChar, System.IO.Path.DirectorySeparatorChar);
    }

    /// <summary>
    /// new System.Uri(tmpString):用于Path的标准化
    /// </summary>
    public static string GetFullPath(string BasePath, string RelativePath)
    {
        string tmpString;
        if (string.IsNullOrEmpty(BasePath))
            return "";

        FixSlashes(ref BasePath);
        if (string.IsNullOrEmpty(RelativePath))
            return BasePath;
        if (System.IO.Path.GetPathRoot(RelativePath) != "")
            FixSlashes(ref RelativePath);

        tmpString = System.IO.Path.Combine(BasePath, RelativePath);
        tmpString = System.IO.Path.GetFullPath(new System.Uri(tmpString).LocalPath);
        return tmpString;
    }
    /// <summary>
    /// fromUri.MakeRelativeUri(toUri);
    /// </summary>
    public static string MakeRelativePath(string fromPath, string toPath)
    {
        // MONO BUG: https://github.com/mono/mono/pull/471
        // In the editor, Application.dataPath returns <Project Folder>/Assets. There is a bug in
        // mono for method Uri.GetRelativeUri where if the path ends in a folder, it will
        // ignore the last part of the path. Thus, we need to add fake depth to get the "real"
        // relative path.
        fromPath += "/fake_depth";
        try
        {
            if (string.IsNullOrEmpty(fromPath))
                return toPath;

            if (string.IsNullOrEmpty(toPath))
                return "";

            var fromUri = new System.Uri(fromPath);
            var toUri   = new System.Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme)
                return toPath;

            var relativeUri  = fromUri.MakeRelativeUri(toUri);
            var relativePath = System.Uri.UnescapeDataString(relativeUri.ToString());

            return relativePath;
        }
        catch
        {
            return toPath;
        }
    }

        #endregion
    }
}