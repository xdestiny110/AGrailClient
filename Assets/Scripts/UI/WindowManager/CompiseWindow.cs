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
            parent.OnMsg(ParentMsg.ShowChild, this);
        }

        public override void Hide()
        {
            base.Hide();
            foreach(CompiseWindow child in children)
            {
                child.OnMsg(ParentMsg.Hide);
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            foreach (CompiseWindow child in children)
            {
                child.OnMsg(ParentMsg.Destroy);
            }    
        }

        public override void OnMsg(ParentMsg msg, params object[] parameters)
        {
            switch (msg)
            {
                case ParentMsg.Null:
                    break;
                case ParentMsg.Show:
                    {
                        DoShow();
                        if (currentShowChild != null)
                        {
                            currentShowChild.OnMsg(msg);
                        }
                        break;
                    }
                case ParentMsg.Hide:
                    {
                        DoHide();
                        foreach (CompiseWindow child in children)
                        {
                            child.OnMsg(ParentMsg.Hide);
                        }
                        break;
                    }             
                case ParentMsg.ShowChild:
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