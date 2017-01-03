using UnityEngine;
using System.Collections.Generic;
using System;

namespace Framework
{
    namespace UI
    {
        public abstract class Window
        {
            private static HashSet<int> identityPool = new HashSet<int>();
            private static int generateIdentity()
            {
                System.Random rd = new System.Random();
                for (;;)
                {
                    var v = rd.Next();
                    if (!identityPool.Contains(v))
                        identityPool.Add(v);
                    return v;
                }
            }

            public GameObject GOHandle { get; set; }
            public int Identity { get; set; }
            public Window()
            {
                Identity = generateIdentity();
            }

            protected abstract void registEvent();
            protected abstract void unregistEvent();
            protected abstract void onActualShow();
            protected abstract void onDestroy();

            public virtual void OnMsg(WindowMsg msg)
            {
                switch (msg)
                {
                    case WindowMsg.Show:
                        show();
                        break;
                    case WindowMsg.Hide:
                        hide();
                        break;
                    case WindowMsg.Uninteractable:
                        uninteractable();
                        break;
                    case WindowMsg.Destroy:
                        destroy();
                        break;
                }
            }

            protected virtual void show()
            {
                if(GOHandle == null)                
                    GOHandle = GameObjectPool.Instance.InstanceGO(this.GetWindowType());
                                                
                GOHandle.SetActive(true);
                GOHandle.GetComponent<CanvasGroup>().interactable = true;
                registEvent();                
                onActualShow();
            }

            protected virtual void hide()
            {
                unregistEvent();
                GOHandle.SetActive(false);                
            }

            protected virtual void destroy()
            {
                unregistEvent();
                GOHandle = null;
                identityPool.Remove(Identity);
            }

            protected virtual void uninteractable()
            {
                GOHandle.GetComponent<CanvasGroup>().interactable = false;
            }

        }
    }
}


