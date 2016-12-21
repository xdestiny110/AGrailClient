using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
namespace Module
{
    public class MonoRoot : MonoBehaviour
    {

        public static MonoRoot Instance;

        List<Action> levelLoadedActions = new List<Action>();
        List<Action> updateActions = new List<Action>();

        public void AddLevelAction(Action levelChangeAction)
        {
            if (!levelLoadedActions.Contains(levelChangeAction))
            {
                levelLoadedActions.Add(levelChangeAction);
            }
        }

        public void RemoveLevelAction(Action levelChangeAction)
        {
            if (levelLoadedActions.Contains(levelChangeAction))
            {
                levelLoadedActions.Remove(levelChangeAction);
            }
        }

        public void AddUpdateAction(Action updateAction)
        {
            if (!updateActions.Contains(updateAction))
            {
                updateActions.Add(updateAction);
            }
        }

        public void RemoveUpdateAction(Action updateAction)
        {
            if (updateActions.Contains(updateAction))
            {
                updateActions.Remove(updateAction);
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;            
        }

        void Update()
        {
            updateActions.ForEach(ac => ac());
        }

        public void OnLevelWasLoaded(int level)
        {
            levelLoadedActions.ForEach(ac => ac());
        }
    }
}