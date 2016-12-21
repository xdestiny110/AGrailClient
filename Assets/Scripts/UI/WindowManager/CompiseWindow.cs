using System.Collections;
using System.Collections.Generic;
namespace UI
{
    public abstract class CompiseWindow:Window
    {
        //在stack里只加入自己，children不会加入stack，因此Hide() 函数调用base.Hide()，children只通知msg就可以了
        List<CompiseWindow> children = new List<CompiseWindow>();
        CompiseWindow currentShowChild = null;
        CompiseWindow parent = null;

        public CompiseWindow(int ideitity) : base(ideitity) { }

        //当窗口做为子窗口显示时，由父窗口如此调用
        public void ShowAsChild(CompiseWindow parent)
        {
            this.parent = parent;
            DoShow();
            parent.OnMsg(WinMsg.ShowChild, this);
        }

        public override void OnMsg(WinMsg msg, params object[] parameters)
        {
            switch (msg)
            {
                case WinMsg.Null:
                    break;
                case WinMsg.Show:
                    {
                        DoShow();
                        if (currentShowChild != null)
                        {
                            currentShowChild.OnMsg(msg);
                        }
                        break;
                    }
                case WinMsg.Hide:
                    {
                        DoHide();
                        foreach (CompiseWindow child in children)
                        {
                            child.OnMsg(WinMsg.Hide);
                        }
                        break;
                    }
                case WinMsg.Destroy:
                    {
                        DoDestroy();
                        foreach(CompiseWindow child in children)
                        {
                            child.OnMsg(WinMsg.Destroy);
                        }
                        children.Clear();
                        currentShowChild = null;
                        break;
                    }    
                case WinMsg.ShowChild:
                    {
                        CompiseWindow child = parameters[0] as CompiseWindow;
                        if (!children.Contains(child))
                        {
                            children.Add(child);
                        }
                        currentShowChild = child;
                        break;
                    }
            }
        }     
    }
}