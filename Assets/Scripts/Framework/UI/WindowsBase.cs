using UnityEngine;
using System.Collections;
using Framework.EventSystem;
using System;

namespace Framework.UI
{
    public abstract class WindowsBase : MonoBehaviour, IEventListener
    {
        public abstract void OnShow();
        public abstract void OnHide();
        public abstract void OnPause();
        public abstract void OnResume();
        public abstract void OnDestroy();

        public virtual void OnEventTrigger(EventSystem.EventType eventType, params object[] parameters)
        {
            
        }
    }
}


