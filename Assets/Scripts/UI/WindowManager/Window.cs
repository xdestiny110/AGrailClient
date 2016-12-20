using UnityEngine;
using System.Collections;
using Common;
namespace UI
{
    public abstract class Window : IPoolUser
    {
        public GameObject winHandle; //是否使能表示窗口是否打开
        protected virtual string winName { get; private set; } //判断窗口是否是compise的，通过名字由用户自行判断
        public int identity;

        public Window(int identity)
        {
            this.identity = identity;
        }

        public void Show(ParentMsg pMsg)
        {            
            WinStack.GetStack(identity).Push(this, pMsg); //暂时先放在这，最终可能导致窗口出来慢的时候，窗口切换有空隙
            DoShow();
        }    

        public virtual void Hide()
        {
            WinStack.GetStack(identity).Pop(this);
            DoHide();
        }
     
        public virtual void Destroy()
        {
            DoDestroy();
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
        public virtual void OnMsg(ParentMsg msg, params object[] parameters)
        {
            switch(msg)
            {
                case ParentMsg.Null:
                    break;
                case ParentMsg.Show:
                    DoShow();
                    break;             
                case ParentMsg.Hide:
                    DoHide();
                    break;
                case ParentMsg.Destroy:
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
            //DoDestroy();
        }

        protected void DoDestroy()
        {
            UnRegistModuleEvent();
            willDestroy();
            ABPool.Instance.GiveBack(winHandle);
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