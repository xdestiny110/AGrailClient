using UnityEngine;
using System.Collections;
using System.Text;
namespace Common
{
    public static class ABName 
    {
        const string UserInterface = "UserInterface.";

        const string UserInterfaceScene = "UserInterface.Scene."; 
        const string UserInterfaceUIWin = "UserInterface.UIWin.";      
        const string UserInterfaceAudio = "UserInterface.Audio.";
        const string UserInterfaceAds = "UserInterface.Ads."; //广告海报
        static int UserInterfaceStrLen = UserInterface.Length;

        const string Unity3dStrAppend = ".unity3d";
        const string DotString = ".";
        static StringBuilder builder = new StringBuilder();

        public static bool IsUserSceneBundleName(string bundleName)
        {
            return bundleName.StartsWith(UserInterfaceScene);
        }

        public static bool IsUserBundleName(string bundleName)
        {
            return bundleName.StartsWith(UserInterface);
        }

        //将xxx.preafb xxx.anim变为可以从assetbundle中加载的资源名称。以xxx.prefab为例，assetbundle中存的名称只有xxx，而对于anim还要测试一下
        public static string UserAssetName2LoadableAssetName(string userAssetName)
        {
            return userAssetName.Substring(0, userAssetName.LastIndexOf(DotString));
        }

        public static string UserBundleName2UserAssetName(string userBundleName)
        {
            string s = userBundleName.Substring(userBundleName.IndexOf(DotString, UserInterfaceStrLen) + 1);          
            s = s.Substring(0, s.LastIndexOf(DotString));
            return s;
        }

        public static string UserBundleName2LoadableName(string userBundleName)
        {
            string s = userBundleName.Substring(0, userBundleName.LastIndexOf(DotString));
            s = s.Substring(0, s.LastIndexOf(DotString));
            return s.Substring(s.LastIndexOf(DotString) + 1);
        }

        public static ABPath Userbundlename2PathType(string userBundleName)
        {
            if (!userBundleName.StartsWith(UserInterface))
                return ABPath.NULL;
            string pre = userBundleName.Substring(0, userBundleName.IndexOf(DotString, UserInterfaceStrLen)+1);          
        
            if (pre.Equals(UserInterfaceUIWin))           
                return ABPath.UIWin;
            if (pre.Equals(UserInterfaceScene))          
                return ABPath.Scene;         
            if (pre.Equals(UserInterfaceAudio))          
                return ABPath.Audio;          
            if (pre.Equals(UserInterfaceAds))           
                return ABPath.Ads;
            
            return ABPath.NULL;
        }

        public static string UserAssetName2UserBundleName(string userAssetName, ABPath pathType)
        {
            builder.Length = 0;
            builder.Append(userAssetName);
            switch (pathType)
            {              
                case ABPath.UIWin:
                    builder.Insert(0, UserInterfaceUIWin);
                    builder.Append(Unity3dStrAppend);                 
                    break;             
                case ABPath.Ads:
                    builder.Insert(0, UserInterfaceAds);
                    builder.Append(Unity3dStrAppend);
                    break;             
                case ABPath.Scene:
                    builder.Insert(0, UserInterfaceScene);
                    builder.Append(Unity3dStrAppend);                   
                    break;
                case ABPath.Audio:
                    builder.Insert(0, UserInterfaceAudio);
                    builder.Append(Unity3dStrAppend);                    
                    break;             
            }
            return builder.ToString();
        }
    }
}