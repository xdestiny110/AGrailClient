using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AppMgr
{
    private static object locker = new object();
    private static AppMgr instance;
    public static AppMgr Instance
    {
        get
        {
            if (null == instance)
            {
                lock (locker)
                {
                    if (null == instance)
                        instance = new AppMgr();
                }
            }
            return instance;
        }
    }

    UI.WinStack loginStack;  
    public void OnStart()
    {       
        //解析配置
        Common.Config.Parser.Start();
        OnLevelLoaded();
       // checkLogIn();
    }

    public void OnLevelLoaded()
    {
        //这里应该是异步的，需要回调配置解析完成
        //InitWinStack();    

        //if(loginStack != null)//Square scene
        //{
        //    bool userlogged = Module.LogIn.GetInstance().LogInfoValid();
        //    ShowStackStartWin( userlogged? loginStack :null);
        //    UI.UIEvent.Notify(UI.UIEventType.UserLogStat, userlogged? 1 : 0);
        //}
        //else
        //{
        //    ShowStackStartWin(null);
        //}
    }

    //void ShowStackStartWin(UI.WinStack excludeStack)
    //{
    //    foreach (UI.WinStack stack in UI.WinStack.all.Values)
    //    {
    //        if (stack != excludeStack)
    //        {
    //            UI.Window win = UI.WinFactory.Create(stack.startWinName, stack.identity);
    //            win.Show(UI.ParentMsg.Null);
    //        }
    //    }
    //}

    ////记录下登陆界面所在的stack
    //void InitWinStack()
    //{
    //    loginStack = null;
    //    UI.WinStack.ClearAll();
    //    foreach (Common.Config.WinStackAttrib attrib in Common.Config.WinStackAttrib.all)
    //    {            
    //        if (attrib.SceneName == SceneManager.GetActiveScene().name)
    //        {
    //            UI.WinStack.AddStack(attrib.Identity, attrib.Position, attrib.Rotation, attrib.scale, attrib.StartWinName, attrib.group);
    //            if(attrib.StartWinName == "Login")
    //            {
    //                loginStack = UI.WinStack.GetStack(attrib.Identity);
    //            }
    //        }
    //    }      
    //}
}
