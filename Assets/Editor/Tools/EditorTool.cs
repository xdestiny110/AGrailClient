using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace Framework
{
    public static class EditorTool 
    {
        //unityFolder的上级目录为Assets
        public static List<string> AssetPathOfUnityFolder(string unityFolder, params string[] append)
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
       
    }
}