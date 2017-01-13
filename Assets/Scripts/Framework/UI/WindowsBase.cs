using UnityEngine;
using Framework.Message;

namespace Framework.UI
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class WindowsBase : MonoBehaviour, IMessageListener
    {
        public virtual void OnShow()
        {
            MessageSystem.Notify(MessageType.OnUIShow, this);
        }

        public virtual void OnHide()
        {
            MessageSystem.Notify(MessageType.OnUIHide, this);
        }

        public virtual void OnPause()
        {
            MessageSystem.Notify(MessageType.OnUIPause, this);
        }

        public virtual void OnResume()
        {
            MessageSystem.Notify(MessageType.OnUIResume, this);
        }

        public virtual void Awake()
        {
            MessageSystem.Notify(MessageType.OnUICreate, this);
        }

        public virtual void OnDestroy()
        {
            MessageSystem.Notify(MessageType.OnUIDestroy, this);
        }

        public virtual void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            
        }
    }
}


