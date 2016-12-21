using UnityEngine;
using UnityEngine.SceneManagement;

public class MonoRoot : MonoBehaviour {

    public static MonoRoot Instance;
    private void OnLevelWasLoaded(int level)
    {
        AppMgr.Instance.OnLevelLoaded();
    }

    private void Awake()
    {
#if UNITY_EDITOR
        PlayerPrefs.DeleteAll();
#endif
        DontDestroyOnLoad(gameObject);
        Instance = this;
        gameObject.AddComponent<Network.AsyncEventQueue>();

        SceneManager.LoadScene(1);
        AppMgr.Instance.OnStart();
    }  
}
