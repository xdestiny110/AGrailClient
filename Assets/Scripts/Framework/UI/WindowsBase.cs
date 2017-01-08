using UnityEngine;
using System.Collections;
using Framework.EventSystem;
using System;

namespace Framework.UI
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class WindowsBase : MonoBehaviour, IEventListener
    {
        public abstract void OnShow();
        public abstract void OnHide();
        public abstract void OnPause();
        public abstract void OnResume();
        public abstract void Awake();
        public abstract void OnDestroy();

        public virtual void OnEventTrigger(EventSystem.EventType eventType, params object[] parameters)
        {
            
        }
    }
}


