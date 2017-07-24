using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditor;

namespace Framework
{
    public static class EditorTool
    {
        //unityFolder的上级目录为Assets
        public static List<string> AssetPathOfUnityFolder(string unityFolder, bool isRecurision, params string[] append)
        {
            List<string> ret = new List<string>();
            string sysFolderPath = UnityPathToSystemPath(unityFolder);           
            string[] filePaths = Directory.GetFiles(sysFolderPath);
            foreach(string fp in filePaths)
            {
                if(FileEndWith(fp,append))
                {
                    ret.Add(SystemPathToUnityPath(fp));
                }
            }
            if (isRecurision)
            {
                var folderPaths = Directory.GetDirectories(sysFolderPath);
                foreach(var folder in folderPaths)
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
            Debug.Log(systemPath);
            systemPath = systemPath.Replace("\\", "/");
            return systemPath.Substring(systemPath.IndexOf("Assets/"));
        }

        public static void DeleteDirContent(string srcPath, bool isDeleteDir = false)
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    subdir.Delete(true); 
                }
                else
                    File.Delete(i.FullName);
            }
        }

        [MenuItem("Framework/Utils/Clean PlayerPref")]
        static void Clean()
        {
            Debug.Log("Clean PlayerPref");
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("Framework/Utils/Get world position")]
        static void GetWorldPos()
        {
            var go = Selection.activeGameObject;
            Debug.LogFormat("World postion = {0}", go.transform.position);
        }
    }
}