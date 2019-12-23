using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AutoClearLog : MonoBehaviour
{
    private List<string> ContentLines = new List<string>();
    private float CurrentLogPos;
    public void AutoClearText(string content,int maxLineCount)
    {
        Debug.LogWarning(content);

        string[] contentLines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        string newString = content;
        if (contentLines.Length > maxLineCount)
        {
            newString = "";
            for (int i = 0; i < contentLines.Length; i++)
            {
                //这里真是坑啊，还要手动移动标签
                if (contentLines[i].StartsWith("</color>"))
                {
                    contentLines[i - 1] += "</color>";
                    contentLines[i] = contentLines[i].Remove(0, 8);
                }
            }
            Debug.LogWarning("second line:"+contentLines[1]);
            //UpdateContentLines(contentLines);
            int startLine = contentLines.Length - maxLineCount;
            
            for(int i = startLine; i < contentLines.Length; i++)
            {
                newString += contentLines[i]+ Environment.NewLine;
            }

            Debug.LogWarning("New String:" + newString);
            //content = newString;
        }
        //Debug.LogWarning(contentLines[1]);

        var text = GetComponent<Text>();
        text.text = newString;
        //var lines = text.cachedTextGenerator.lineCount;

        //if (lines > maxLineCount)
        //{
        //    var content = text.text;
        //    var chari = content.ToCharArray();

        //    var lineInfo = text.cachedTextGenerator.lines;
        //    int idx=0;
        //    int endCharIdx=0;
        //    foreach(var item in lineInfo)
        //    {
        //        if (idx > lines - maxLineCount)
        //        {
        //            endCharIdx = item.startCharIdx;
        //            break;
        //        }
        //        idx++;
        //    }
        //    Debug.LogWarning(chari[endCharIdx]);
        //    content= content.Substring(endCharIdx);
        //    text.text = content;
        //    LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        //}
    }

    //上下预留15行，一次加载10行，下0上1
    //public void ChangeText(float value)
    //{
    //    CurrentLogPos = value;
    //}

    private void UpdateContentLines(string[] fixedDatas)
    {
        foreach(var item in fixedDatas)
        {
            if (!ContentLines.Contains(item))
            {
                ContentLines.Add(item);
            }
        }
    }
}
