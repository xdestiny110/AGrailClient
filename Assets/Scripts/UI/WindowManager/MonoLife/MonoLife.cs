using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace UI
{
    public class MonoLife : MonoBehaviour
    {
        List<System.Action> startActions = new List<System.Action>();
        List<System.Action> updateActions = new List<System.Action>();
        List<System.Action> enableActions = new List<System.Action>();
        List<System.Action> disableActions = new List<System.Action>();

        // Use this for initialization
        void Start()
        {
            foreach(System.Action action in startActions)
            {
                action();
            }
        }

        // Update is called once per frame
        void Update()
        {
            foreach (System.Action action in updateActions)
            {
                action();
            }
        }

        private void OnEnable()
        {
            foreach (System.Action action in enableActions)
            {
                action();
            }
        }

        private void OnDisable()
        {
            foreach (System.Action action in disableActions)
            {
                action();
            }
        }

        public void AddEventFromStart(System.Action listener)
        {
            if(!startActions.Contains(listener))
            {
                startActions.Add(listener);
            }
        }

        public void AddEventFromUpdate(System.Action listener)
        {
            if (!updateActions.Contains(listener))
            {
                updateActions.Add(listener);
            }
        }

        public void AddEventFromEnable(System.Action listener)
        {
            if (!enableActions.Contains(listener))
            {
                enableActions.Add(listener);
            }
        }

        public void AddEventFromUpDisable(System.Action listener)
        {
            if (!disableActions.Contains(listener))
            {
                disableActions.Add(listener);
            }
        }
    }
}