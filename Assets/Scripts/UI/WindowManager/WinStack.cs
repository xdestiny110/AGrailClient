using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace UI
{
    //在每个场景load之后就要对Winstack进行全部添加
    public class WinStack
    {
        /*
         * Static mgr
         * 关于wait功能的支持实现：
         * WinStack.Show(id, WinFactory.WAIT， WinMsg.NULL);
         * Wait窗口监听逻辑层事件StopWait
         * 逻辑层异步操作后（一般是协议）发StopWait(identity)事件,Wait窗口做WinStack.Hide(Identity)操作
         */

        public static Dictionary<int, WinStack> all = new Dictionary<int, WinStack>();
        public static WinStack GetStack(int identity)
        {
            return (all.ContainsKey(identity)) ? all[identity] : null;
        }

        public static void AddStack(int identity, Vector3 position, Quaternion quaternion, Vector3 scale, string startWinName, List<int> group)
        {
            all.Add(identity, new WinStack(identity, position, quaternion, scale, startWinName, group));
        }

        public static void Destroy()
        {
            foreach (WinStack stack in all.Values)
            {
                stack._Destroy();
            }
            all.Clear();
        }

        //按了back按钮之后若视点位于某界面上，调用此函数
        public static bool DoBack(GameObject winHandle)
        {
            bool ret = false;
            foreach (WinStack stack in all.Values)
            {
                ret |= stack._DoBack(winHandle);
            }
            return ret;
        }

        //不管当前窗口是否在wait都将会被取消
        public static void CheckGroupClick(int clickIdentity)
        {
            foreach (WinStack stack in all.Values)
            {
                stack.CheckGroupedClicked(clickIdentity);
            }
        }

        public static void Show(int identity, string windowName, WinMsg currentWindowMsg)
        {
            WinStack wStack = null;
            if (all.TryGetValue(identity, out wStack))
            {
                wStack._Show(windowName, currentWindowMsg);
            }
        }

        public static void Hide(int identity, string windowName)
        {
            WinStack wStack = null;
            if (all.TryGetValue(identity, out wStack))
            {
                wStack._Hide(windowName);
            }
        }

        /*
         * Winstack Class
         */
        Stack<Window> stack = new Stack<Window>();
        List<Window> idle = new List<Window>();
        public int identity;
        public Vector3 position;
        public Quaternion quaternion;
        public Vector3 scale;
        public string startWinName;
        public List<int> group;

        WinStack(int identity, Vector3 position, Quaternion quaternion, Vector3 scale, string startWinName, List<int> group)
        {
            this.identity = identity;
            this.position = position;
            this.scale = scale;
            this.quaternion = quaternion;
            this.startWinName = startWinName;
            this.group = group;
        }

        void _Show(string windowName, WinMsg currentWindowMsg)
        {
            Window currWin = Peek();
            if (currWin != null && currWin.winName.Equals(windowName))
            {
                Debug.LogWarning(string.Format("试图打开相同窗口 [{0}] 不予支持", windowName));
                return;
            }
            Window win = GetWindow(windowName);
            if (win != null)
            {
                Push(win, currentWindowMsg);
                win.OnMsg(WinMsg.Show);
            }
        }

        void _Hide(string windowName)
        {
            if (Peek().winName == windowName)
            {
                Window win = Pop();
                win.OnMsg(WinMsg.Hide);
                idle.Add(win);
            }
        }

        void _Destroy()
        {
            while (stack.Count > 0)
            {
                Window win = stack.Pop();
                win.OnMsg(WinMsg.Destroy);
            }
        }

        bool _DoBack(GameObject winHandle)
        {
            if (stack.Count <= 1)
                return false;
            Window top = Peek();
            if (top.winHandle == winHandle)
            {
                stack.Pop();
                top.OnMsg(WinMsg.Hide);
                stack.Peek().OnMsg(WinMsg.Show);
                return true;
            }
            return false;
        }

        //新窗口打开时调用
        void Push(Window win, WinMsg msg)
        {
            Window topWin = Peek();
            if (topWin != null)
            {
                topWin.OnMsg(msg);
            }
            stack.Push(win);
        }

        Window Pop()
        {
            Window ret = stack.Pop();
            Window topWin = Peek();
            if (topWin != null)
            {
                topWin.OnMsg(WinMsg.Show);
            }
            return ret;
        }

        public int Count()
        {
            return stack.Count;
        }

        Window Peek()
        {
            return (stack.Count == 0) ? null : stack.Peek();
        }

        void CheckGroupedClicked(int clickedIdentity)
        {
            if (group.Count > 0 && group.Contains(clickedIdentity) && clickedIdentity != identity)
            {
                while (stack.Count > 1)
                {
                    Window win = stack.Pop();
                    win.OnMsg(WinMsg.Hide);
                }
            }
        }

        Window GetWindow(string winName)
        {
            foreach (Window win in idle)
            {
                if (win.winName == winName)
                {
                    idle.Remove(win);
                    return win;
                }
            }
            return WinFactory.Create(winName, identity);
        }
    }
}