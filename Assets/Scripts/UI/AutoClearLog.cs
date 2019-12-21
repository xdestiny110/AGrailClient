using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class AutoClearLog : MonoBehaviour
{
    public void AutoClearText(int maxLineCount)
    {
        var text = GetComponent<Text>();
        var lines = text.cachedTextGenerator.lineCount;

        if (lines > maxLineCount)
        {
            
            using (StringReader sr = new StringReader(text.text))
            {
                string ntext="";
                string line;
                for(int i = 0; i < maxLineCount; i++)
                {
                    line = sr.ReadLine();
                    ntext += line;
                }
                text.text = ntext;
            }          
        }
    }
}
