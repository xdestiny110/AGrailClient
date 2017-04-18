using UnityEngine;
using Framework.Message;

namespace Framework.UI
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class WindowsBase : MonoBehaviour, IMessageListener<MessageType>
    {
        public abstract WindowType Type { get; }

        protected CanvasGroup canvasGroup = null;
        protected Canvas canvas = null;

        private object[] parameters;
        public virtual object[] Parameters
        {
            set
            {
                parameters = value;
            }
            get
            {
                return parameters;
            }
        }

        public virtual void OnShow()
        {
            MessageSystem<MessageType>.Notify(MessageType.OnUIShow, this);
        }

        public virtual void OnHide()
        {
            MessageSystem<MessageType>.Notify(MessageType.OnUIHide, this);
        }

        public virtual void OnPause()
        {
            MessageSystem<MessageType>.Notify(MessageType.OnUIPause, this);
        }

        public virtual void OnResume()
        {
            MessageSystem<MessageType>.Notify(MessageType.OnUIResume, this);
        }

        public virtual void Awake()
        {
            MessageSystem<MessageType>.Notify(MessageType.OnUICreate, this);
            canvasGroup = GetComponent<CanvasGroup>();
            canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
        }

        public virtual void OnDestroy()
        {
            MessageSystem<MessageType>.Notify(MessageType.OnUIDestroy, this);
        }

        public virtual void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            
        }
    }
}


