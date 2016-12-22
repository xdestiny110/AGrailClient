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
            private Stack<Window> idle = new Stack<Window>();

            private UIManager() { }

            


        }
    }
}


