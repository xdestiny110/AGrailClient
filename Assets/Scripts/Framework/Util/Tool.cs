using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Framework
{
    public static class Tool 
    {
        //unityFolder的上级目录为Assets
        public static List<string> AssetPathOfUnityFolder(string unityFolder, bool isRecurision, params string[] append)
        {
            List<string> ret = new List<string>();
            string sysFolderPath = UnityPathToSystemPath(unityFolder);

            string[] filePaths = Directory.GetFiles(sysFolderPath);
            foreach (string fp in filePaths)
            {
                if (FileEndWith(fp, append))
                {
                    ret.Add(SystemPathToUnityPath(fp));
                }
            }
            if (isRecurision)
            {
                var folderPaths = Directory.GetDirectories(sysFolderPath);
                foreach (var folder in folderPaths)
                    ret.AddRange(AssetPathOfUnityFolder(SystemPathToUnityPath(folder), true, append));
            }
            return ret;
        }

        static bool FileEndWith(string fileName, params string[] append)
        {
            foreach(string app in append)
            {
                if (fileName.EndsWith(app))
                    return true;
            }
            return false;
        }

        public static string UnityPathToSystemPath(string unityPath)
        {
            unityPath = unityPath.Replace("\\", "/");
            if (unityPath.StartsWith("Assets"))
            {
                unityPath = unityPath.Substring(unityPath.IndexOf("/") + 1);
            }
            return string.Format("{0}/{1}", Application.dataPath, unityPath);
        }

        public static string SystemPathToUnityPath(string systemPath)
        {
            systemPath = systemPath.Replace("\\", "/");
            return systemPath.Substring(systemPath.IndexOf("Assets/"));
        }
       
        //获取transform从root开始自顶向下的路径以/分隔
        public static string TransPath(Transform trans)
        {
            Stack<string> stack = new Stack<string>();
            Transform loopTrans = trans;
            while (loopTrans.parent != null)
            {
                stack.Push(loopTrans.gameObject.name);
                loopTrans = loopTrans.parent;
            }
            string s = "";
            while (stack.Count > 0)
            {
                s = s.Length == 0 ? stack.Pop() : string.Format("{0}/{1}", s, stack.Pop());
            }
            return s;
        }

        public static void DeleteDirContent(string srcPath, Regex regex = null, bool isExcept = false)
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)
                {
                    DeleteDirContent(i.FullName);
                    Directory.Delete(i.FullName);
                }                    
                else if(regex == null ||
                    (!isExcept && regex.IsMatch(i.FullName)) ||
                    (isExcept && !regex.IsMatch(i.FullName)))
                    File.Delete(i.FullName);
            }
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, Regex regex = null, bool isExcept = false)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if(regex == null ||
                    (!isExcept && regex.IsMatch(file.FullName)) ||
                    (isExcept && !regex.IsMatch(file.FullName)))
                {
                    string temppath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(temppath, true);
                }
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }
        }
    }
}