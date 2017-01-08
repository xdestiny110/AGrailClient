using UnityEngine;
using Framework.Message;

namespace Framework.UI
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class WindowsBase : MonoBehaviour, IMessageListener
    {
        public abstract void OnShow();
        public abstract void OnHide();
        public abstract void OnPause();
        public abstract void OnResume();
        public abstract void Awake();
        public abstract void OnDestroy();

        public virtual void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            
        }
    }
}


