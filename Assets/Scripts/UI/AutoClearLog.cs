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
        string[] contentLines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        int lineCount = contentLines.Length;
        string newString = content;
        if (lineCount > maxLineCount)
        {
            newString = "";
            for (int i = 0; i < lineCount; i++)
            {
                //这里真是坑啊，还要手动移动标签
                if (contentLines[i].StartsWith("</color>"))
                {
                    contentLines[i - 1] += "</color>";
                    contentLines[i] = contentLines[i].Remove(0, 8);
                }
            }

            int startLine = lineCount - maxLineCount;
            
            for(int i = startLine; i < lineCount; i++)
            {
                newString += contentLines[i]+ Environment.NewLine;
            }
        }

        var text = GetComponent<Text>();
        text.text = newString;
    }

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
