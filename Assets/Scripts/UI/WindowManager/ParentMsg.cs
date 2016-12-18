using UnityEngine;
using System.Collections;
namespace UI
{
    /*
     * 窗口管理方式：
     * 如果有导航栏形式的窗口即A有子窗口BCD，那么整体用CompiseWindow表示A，然后把A压入stack
     */
    public enum ParentMsg 
    {
        Null = 0,
        Show,
        Hide,
       // View,     //View使用Canvas.layer实现，由stack保证
        Destroy,    
        ShowChild
    }
}