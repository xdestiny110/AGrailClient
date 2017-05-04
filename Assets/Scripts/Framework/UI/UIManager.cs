using UnityEngine;
using System.Collections.Generic;

namespace Framework.UI
{
    public class UIManager
    {
        private Stack<WindowsBase> winStack = new Stack<WindowsBase>();
        private Dictionary<WindowType, GameObject> goPool = new Dictionary<WindowType, GameObject>();        

        public WindowsBase PushWindow(WindowType type, WinMsg msg, Vector3 initPos = default(Vector3), params object[] parameters)
        {
            var go = WindowFactory.Instance.CreateWindows(type);
            go.name = type.ToString();            
            go.transform.position = initPos;
            var win = go.GetComponent<WindowsBase>();
            win.Parameters = parameters;
            WindowsBase topWin;
            if(winStack.TryPeek(out topWin))
            {
                win.Canvas.sortingOrder = topWin.Canvas.sortingOrder + 1;
                dealWinMsg(topWin, msg);
            }                
            winStack.Push(win);
            return win;
        }

        public void PopWindow(WinMsg msg)
        {
            WindowsBase topWin;
            if (winStack.TryPop(out topWin))
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
                    topWin.CanvasGroup.interactable = true;
                    topWin.OnResume();
                    break;
                case WinMsg.Hide:
                    topWin.CanvasGroup.interactable = false;
                    topWin.OnPause();
                    topWin.OnHide();
                    break;
                case WinMsg.Pause:
                    topWin.CanvasGroup.interactable = false;
                    topWin.OnPause();
                    break;
                case WinMsg.Resume:
                    topWin.CanvasGroup.interactable = true;
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


