using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class MonoRoot : MonoBehaviour {

    public static MonoRoot Instance;
    private void OnLevelWasLoaded(int level)
    {
        AppMgr.GetInstance().OnLevelLoaded();
    }

    private void Awake()
    {
        //PlayerPrefs.SetString("logphone", "");
#if UNITY_EDITOR
        PlayerPrefs.DeleteAll();
#endif
        GameObject.DontDestroyOnLoad(gameObject);
        Instance = this;
        gameObject.AddComponent<EventSystem.AsyncEventQueue>();

        SceneManager.LoadScene(1);
        AppMgr.GetInstance().OnStart();
    }  
}
