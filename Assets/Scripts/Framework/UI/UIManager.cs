using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using XLua;

namespace Framework.UI
{
    [LuaCallCSharp]
    public class UIManager
    {
        private List<UIBase> winStack = new List<UIBase>();

        public void PushWindow(WindowType type, WinMsg msg, int sortLayer = -1, Vector3 initPos = default(Vector3), params object[] parameters){
            PushWindow(type.ToString(), msg, sortLayer, initPos, parameters);
        }

        public void PushWindow(string uiName, WinMsg msg, int sortLayer = -1, Vector3 initPos = default(Vector3), params object[] parameters)
        {
            var go = UIFactory.Instance.CreateUI(uiName);
            go.name = uiName;
            go.transform.position = initPos;
            var win = go.GetComponent<UIBase>();
            if (win == null)
                win = go.AddComponent<UIBase>();
            win.Parameters = parameters;
            if (winStack.Count > 0)
            {
                dealWinMsg(winStack.Last(), msg);
                if (sortLayer == -1)
                {
                    if (win.Canvas.sortingOrder == 0)
                        win.Canvas.sortingOrder = winStack.Last().Canvas.sortingOrder + 1;
                }
                else
                    win.Canvas.sortingOrder = sortLayer;
            }
            winStack.Add(win);
            winStack.Sort((w1, w2) =>
            {
                if (w1.Canvas.sortingOrder < w2.Canvas.sortingOrder)
                    return -1;
                else if (w1.Canvas.sortingOrder >= w2.Canvas.sortingOrder)
                    return 1;
                return 0;
            });
        }

        public IEnumerator PushWindowAsyn(string uiName, WinMsg msg, Vector3 initPos = default(Vector3), params object[] parameters)
        {
            //先加载等待画面
            yield return UIFactory.Instance.CreateUIAnyn(uiName);
            //之后再完善
        }

        public UIBase PushWindowFromResource(WindowType type, WinMsg msg, int sortLayer = -1, Vector3 initPos = default(Vector3), params object[] parameters)
        {
            return PushWindowFromResource(type.ToString(), msg, sortLayer, initPos, parameters);
        }

        public UIBase PushWindowFromResource(string uiName, WinMsg msg, int sortLayer = -1, Vector3 initPos = default(Vector3), params object[] parameters)
        {
            var go = UIFactory.Instance.CreateUI(uiName, true);
            go.name = uiName;
            go.transform.position = initPos;
            var win = go.GetComponent<UIBase>();
            win.Parameters = parameters;
            if (winStack.Count > 0)
            {
                dealWinMsg(winStack.Last(), msg);
                if (sortLayer == -1)
                {
                    if (win.Canvas.sortingOrder == 0)
                        win.Canvas.sortingOrder = winStack.Last().Canvas.sortingOrder + 1;
                }
                else
                    win.Canvas.sortingOrder = sortLayer;
            }
            winStack.Add(win);
            winStack.Sort((w1, w2) =>
            {
                if (w1.Canvas.sortingOrder < w2.Canvas.sortingOrder)
                    return -1;
                else if (w1.Canvas.sortingOrder >= w2.Canvas.sortingOrder)
                    return 1;
                return 0;
            });
            return win;
        }

        public void PopWindow(WinMsg msg)
        {
            if (winStack.Count > 0)
            {
                GameObject.Destroy(winStack.Last().gameObject);
                winStack.RemoveAt(winStack.Count - 1);
                if (winStack.Count > 0)
                    dealWinMsg(winStack.Last(), msg);
            }
            else
                throw new System.Exception("Winstack is empty!");
        }

        public void PopAllWindow()
        {
            while (winStack.Count > 0)
                PopWindow(WinMsg.None);
        }

        public void ClearAllWindow()
        {
            winStack.Clear();            
        }

        public string PeekWindowType()
        {
            if (winStack.Count == 0)
                return WindowType.None.ToString();
            return winStack.Last().Type;
        }

        public UIBase PeekWindow()
        {
            if (winStack.Count == 0)
                return null;
            return winStack.Last();
        }

        private void dealWinMsg(UIBase topWin, WinMsg msg)
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


