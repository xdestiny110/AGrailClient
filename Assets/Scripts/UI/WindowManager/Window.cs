using UnityEngine;
using System.Collections;
using Common;
namespace UI
{
    public abstract class Window : IPoolUser
    {
        public GameObject winHandle; //是否使能表示窗口是否打开
        public virtual string winName { get; private set; } //判断窗口是否是compise的，通过名字由用户自行判断
        public int identity;

        public Window(int identity)
        {
            this.identity = identity;
        }

        protected abstract void willDestroy();
        //注册界面按钮等相关回调
        protected abstract void RegistUiEvent();
        protected abstract void UnregistUiEvent();
        //注册监听module层事件
        protected abstract void RegistModuleEvent();
        protected abstract void UnRegistModuleEvent();

        //窗口实际打开时调用
        protected abstract void OnActualShow();

        //当有新窗口打开时，做为原先的当前窗口会收到此消息
        public virtual void OnMsg(WinMsg msg, params object[] parameters)
        {
            switch(msg)
            {
                case WinMsg.Null:
                    break;
                case WinMsg.Show:
                    DoShow();
                    break;             
                case WinMsg.Hide:
                    DoHide();
                    break;
                case WinMsg.Destroy:
                    DoDestroy();
                    break;
            }
        }

        public virtual void OnPoolInstanceDone(string userBundleName, object obj)
        {
            winHandle = obj as GameObject;
            DoShow();

            UnregistUiEvent();
            RegistUiEvent();      
        }

        protected void DoShow()
        {
            if (winHandle == null)
            {
                ABPool.Instance.InstanceAsync(ABName.UserAssetName2UserBundleName(string.Format("{0}.prefab", winName), ABPath.UIWin), this);
                return;
            }
            winHandle.SetActive(true);
            winHandle.transform.position = WinStack.GetStack(identity).position;
            winHandle.transform.rotation = WinStack.GetStack(identity).quaternion;

            winHandle.transform.localScale = WinStack.GetStack(identity).scale;

            SetCanvasLayer();
            OnActualShow();
            RegistModuleEvent();            
        }

        protected void DoHide()
        {
            UnRegistModuleEvent();
            if (winHandle != null)
                winHandle.SetActive(false);          
        }

        protected void DoDestroy()
        {
            UnregistUiEvent();
            UnRegistModuleEvent();
            willDestroy();
            ABPool.Instance.GiveBack(winHandle);
            winHandle = null;
        }

        void SetCanvasLayer()
        {
            Canvas canvas = winHandle.GetComponent<Canvas>();
            if(canvas == null)
            {
                canvas = winHandle.transform.GetChild(0).GetComponent<Canvas>();
            }
            canvas.sortingOrder  = WinStack.GetStack(identity).Count()-100;
        }

    }
}