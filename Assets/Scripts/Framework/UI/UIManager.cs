using UnityEngine;
using System.Collections.Generic;

namespace Framework
{
    namespace UI
    {
        public class UIManager
        {
            private static UIManager instance;
            private static object locker = new object();
            public static UIManager Instance
            {
                get
                {
                    if(instance == null)
                    {
                        lock (locker)
                        {
                            if(instance == null)
                            {
                                instance = new UIManager();
                            }
                        }
                    }
                    return instance;
                }
            }            

            private Stack<Window> stack = new Stack<Window>();

            private UIManager() { }

            public void Push(Window window, WindowMsg toParentMsg)
            {
                Window win = null;
                if (stack.TryPeek(ref win))
                    win.OnMsg(toParentMsg);
                window.OnMsg(WindowMsg.Show);
                stack.Push(win);
            }

            public void Pop()
            {
                Window win = null;
                if (stack.TryPop(ref win))
                    win.OnMsg(WindowMsg.Destroy);
                else
                    throw new System.ArgumentOutOfRangeException("UI Stack is empty!");
            }
        }
    }
}


