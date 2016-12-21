using UnityEngine;
using System.Collections;
namespace Module
{
    public class Startup : MonoBehaviour
    {
        const string MonoRoot = "MonoRoot";
        private void Awake()
        {
            
            if (FindObjectOfType<MonoRoot>() == null)
            {
                //首次启动
                InitApplication();
            }
        }

        void InitApplication()
        {
            GameObject prefab = Resources.Load(MonoRoot) as GameObject;
            GameObject monoRoot = Instantiate(prefab);
            monoRoot.name = MonoRoot;
            DontDestroyOnLoad(monoRoot);

            //解析配置
            Common.Config.Parser.Start();

            //各个模块初始化
            var v1 = Network.NetworkManager.Instance;
           
            UScene.SceneProxy.Instance.Init();

            //启动时不会触发OnLevelWasLoaded调用，这里模拟触发一次
            MonoRoot mRootScript = monoRoot.GetComponent<MonoRoot>();
            mRootScript.OnLevelWasLoaded(0);

        }
    }
}