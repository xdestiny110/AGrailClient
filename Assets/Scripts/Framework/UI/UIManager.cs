using UnityEngine;
using System.Collections.Generic;

namespace Framework.UI
{
    public class UIManager : Singleton<UIManager>
    {
        private Stack<WindowsBase> winStack = new Stack<WindowsBase>();

        public WindowsBase PushWindow(WindowType type, WinMsg msg)
        {
            switch (msg)
            {
                case WinMsg.Show:
                    break;
                case WinMsg.Hide:
                    break;
                case WinMsg.Pause:
                    break;
                case WinMsg.Resume:
                    break;
                case WinMsg.Destroy:
                    break;                    
            }
            return null;
        }

        public void PopWindow(WinMsg msg)
        {
            
        }

        public void PeekWindow()
        {

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


