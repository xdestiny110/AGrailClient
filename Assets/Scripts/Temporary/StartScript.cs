using UnityEngine;
using System.Collections;

public class StartScript : MonoBehaviour {

	// Use this for initialization
	void Awake () {        
        if(GameObject.FindObjectOfType<MonoRoot>() == null)
        {
            GameObject prefab = Resources.Load("MonoRoot") as GameObject;
            var monoRoot = GameObject.Instantiate(prefab);
            monoRoot.name = "MonoRoot";
            GameObject.DontDestroyOnLoad(monoRoot);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
