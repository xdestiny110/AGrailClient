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

        //以后考虑资源利用，调用hide-->giveback
        public static void ClearAll()
        {
            foreach(WinStack stack in all.Values)
            {
                stack.Clear();
            }
            all.Clear();
        }

        public static bool DoBack(GameObject winHandle)
        {
            bool ret = false;
            foreach(WinStack stack in all.Values)
            {
                ret |= stack._DoBack(winHandle);
            }
            return ret;
        }

        public static void CheckGroupClick(int clickIdentity)
        {
            foreach(WinStack stack in all.Values)
            {
                stack.CheckGroupedClicked(clickIdentity);
            }
        }


        /*
         * Winstack Class
         */
        Stack<Window> stack = new Stack<Window>();
        public int identity;
        public Vector3 position;
        public Quaternion quaternion;
        public Vector3 scale;
        public string startWinName;
        public List<int> group;

        public WinStack(int identity, Vector3 position, Quaternion quaternion, Vector3 scale,string startWinName, List<int> group)
        {
            this.identity = identity;
            this.position = position;
            this.scale = scale;
            this.quaternion = quaternion;
            this.startWinName = startWinName;
            this.group = group;
        }

        //新窗口打开时调用
        public void Push(Window win, ParentMsg msg)
        {
            Window topWin = Peek();
            if (topWin != null)
            {
                topWin.OnMsg(msg);               
            }
            stack.Push(win);           
        }

      
        public void Pop(Window wantedPopWin)
        {
            if (Peek() == wantedPopWin)
            {
                stack.Pop();
                Window topWin = Peek();
                if (topWin != null)
                {
                    topWin.OnMsg(ParentMsg.Show);
                }
            }
        }

        public int Count()
        {
            return stack.Count;
        }

        protected Window Peek()
        {
            return (stack.Count == 0)? null:  stack.Peek();
        }

        protected void Clear()
        {
            while(stack.Count>0)
            {
                Window win = stack.Pop();
                win.OnMsg(ParentMsg.Destroy);
            }
        }

        protected bool _DoBack(GameObject winHandle)
        {
            if (stack.Count <= 1)
                return false;
            Window top = Peek();
            if(top.winHandle == winHandle)
            {
                stack.Pop();
                top.OnMsg(ParentMsg.Hide);
                stack.Peek().OnMsg(ParentMsg.Show);
                return true;
            }
            return false;
        }

        protected void CheckGroupedClicked(int clickedIdentity)
        {
            if(group.Count>0 && group.Contains(clickedIdentity) && clickedIdentity != identity)
            {
                while(stack.Count>1)
                {
                    Window win = stack.Pop();
                    win.OnMsg(ParentMsg.Hide);
                }
            }
        }

    }
}