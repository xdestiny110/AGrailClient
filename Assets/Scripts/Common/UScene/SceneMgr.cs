using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace UScene
{
    public class SceneMgr
    {
        private static object locker = new object();
        private static SceneMgr instance;
        private SceneMgr() { }
        public static SceneMgr Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                            instance = new SceneMgr();
                    }
                }
                return instance;
            }
        }

        string wantedLoadSceneName;

        public void LoadWithLoading(string sceneName)
        {
            if (IsLoading())
                return;
            wantedLoadSceneName = sceneName;           
            SceneManager.LoadSceneAsync(SceneDefine.LoadingName);
        }
        
        public void LoadDirect(string sceneName)
        {
            if (IsLoading())
                return;
            wantedLoadSceneName = sceneName;           
            SceneManager.LoadSceneAsync(wantedLoadSceneName);
        }

        //called from SceneMangerProxy
        public void OnLevelLoadDone(SceneDefine.SceneType t)
        {    
            if(t== SceneDefine.SceneType.Loading)
            {
                SceneManager.LoadSceneAsync(wantedLoadSceneName);
            }
            else
            {
                wantedLoadSceneName = "";
            }            
        }

        bool IsLoading()
        {
            return wantedLoadSceneName.Length > 0;
        }
    }
}