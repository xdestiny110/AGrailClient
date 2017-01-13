using UnityEngine;
using System.Collections.Generic;

namespace Framework.UI
{
    public class UIManager
    {
        private Stack<WindowsBase> winStack = new Stack<WindowsBase>();
        private Dictionary<WindowType, GameObject> goPool = new Dictionary<WindowType, GameObject>();        

        public WindowsBase PushWindow(WindowType type, WinMsg msg, Vector3 initPos = default(Vector3))
        {
            var go = WindowFactory.Instance.CreateWindows(type);
            go.name = type.ToString();            
            go.transform.position = initPos;
            var win = go.GetComponent<WindowsBase>();
            WindowsBase topWin;
            if(winStack.TryPeek(out topWin))            
                dealWinMsg(topWin, msg);            
            winStack.Push(win);
            return win;
        }

        public void PopWindow(WinMsg msg)
        {
            WindowsBase topWin;
            if (winStack.TryPeek(out topWin))
            {
                GameObject.Destroy(topWin.gameObject);
                if (winStack.TryPeek(out topWin))                
                    dealWinMsg(topWin, msg);
            }
            else
                throw new System.Exception("Winstack is empty!");
        }

        private void dealWinMsg(WindowsBase topWin, WinMsg msg)
        {
            switch (msg)
            {
                case WinMsg.Show:
                    topWin.OnShow();
                    break;
                case WinMsg.Hide:
                    topWin.OnHide();
                    break;
                case WinMsg.Pause:
                    topWin.OnPause();
                    break;
                case WinMsg.Resume:
                    topWin.OnResume();
                    break;
                case WinMsg.Destroy:
                    topWin.OnDestroy();
                    break;
            }
        }
    }

    public enum WinMsg
    {
        None = 0,
        Show,
        Hide,
        Pause,
        Resume,
        Destroy
    }
}


