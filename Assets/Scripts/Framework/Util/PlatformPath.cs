using UnityEngine;

namespace Framework
{
    public static class PlatformPath
    {
        public enum Location
        {
            NULL = 0,
            Persist,
            Stream
        }

        public static string StreamFileSysPath(string pathUnderStream, bool isNative)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer
                || Application.platform == RuntimePlatform.WindowsPlayer
                || Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.OSXEditor)
                return string.Format("{0}/{1}", Application.streamingAssetsPath, pathUnderStream);
            else
                return string.Format("{0}!assets/{1}", Application.dataPath, pathUnderStream);//android  中 streamAssetsPath自动有前缀jar:///..
        }

        public static string PersistFileSysPath(string pathUnterPersist)
        {
            return string.Format("{0}/{1}", Application.persistentDataPath, pathUnterPersist);
        }

        public static string StreamPath2URL(string pathUnderStream)
        {
			if (Application.platform == RuntimePlatform.WindowsPlayer
			             || Application.platform == RuntimePlatform.WindowsEditor) 
			{
				return pathUnderStream.StartsWith (Application.streamingAssetsPath) ? 
                    string.Format ("file:///{0}", pathUnderStream) : 
                    string.Format ("file:///{0}/{1}", Application.streamingAssetsPath, pathUnderStream);
			} 
			else if (Application.platform == RuntimePlatform.IPhonePlayer ||
			        Application.platform == RuntimePlatform.OSXEditor) {
				return pathUnderStream.StartsWith (Application.streamingAssetsPath) ? 
					string.Format ("file://{0}", pathUnderStream) : 
					string.Format ("file://{0}/{1}", Application.streamingAssetsPath, pathUnderStream);
			}
            else
            {
                return pathUnderStream.StartsWith(Application.streamingAssetsPath) ? 
                    pathUnderStream : 
                    string.Format("{0}/{1}", Application.streamingAssetsPath, pathUnderStream); //android  中 streamAssetsPath自动有前缀jar:///..
            }
        }

        public static string PersistPath2URL(string pathUnderPersit)
        {
            if (Application.platform == RuntimePlatform.WindowsPlayer
                || Application.platform == RuntimePlatform.WindowsEditor)
            {
                return pathUnderPersit.StartsWith(Application.persistentDataPath) ? 
                    string.Format("file:///{0}", pathUnderPersit) : 
                    string.Format("file:///{0}/{1}", Application.persistentDataPath, pathUnderPersit);
            }
            else
            {
                return pathUnderPersit.StartsWith(Application.persistentDataPath) ? 
                    string.Format("file://{0}", pathUnderPersit) : 
                    string.Format("file://{0}/{1}", Application.persistentDataPath, pathUnderPersit); //android  中 persistentDataPath :  /data/....
            }
        }

        public static string WriteablePersisPath()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
            }

            return Application.persistentDataPath;
        }
    }
}