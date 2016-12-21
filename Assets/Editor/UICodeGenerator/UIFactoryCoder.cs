using System.Collections;
using System.Collections.Generic;
namespace Editor.UIGen
{
    public class UIFactoryCoder
    {
        private static object locker = new object();
        private static UIFactoryCoder instance;
        private UIFactoryCoder() { }
        public static UIFactoryCoder Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                            instance = new UIFactoryCoder();
                    }
                }
                return instance;
            }
        }


        const string CodePath = "Scripts/UI/Auto";
        const string ClassName = "WinFactory";

        List<string> winAssetPathes = new List<string>();
        List<string> winNameDefineCode = new List<string>();
        List<string> winClassDefineCode = new List<string>();
        FileWriter writer;

        public void Generate(List<string> winAssetPathes)
        {
            this.winAssetPathes.Clear();
            this.winAssetPathes.AddRange(winAssetPathes);
            MarshalDynamicCode();
            if (writer != null)
                writer.Dispose();
            writer = new Editor.FileWriter(EditorTool.UnityPathToSystemPath(string.Format("{0}/{1}.cs", CodePath, ClassName)));
            WriteContent();
            this.winAssetPathes.Clear();
            writer.Dispose();
            writer = null;
        }

        void WriteContent()
        {
            writer.Append("using UnityEngine;");
            writer.Append("using System.Collections;");
            writer.Append("namespace UI");
            writer.Append("{");
            writer.Append(string.Format("    public class {0}",ClassName));
            writer.Append("    {");
            WriteWinNameDefines();
            writer.Append("        public static Window Create(string windowName, int identity)");
            writer.Append("        {");
            writer.Append("            switch (windowName)");
            writer.Append("            {");
            WriteWinClassDefines();
            writer.Append("                /*");
            writer.Append("                 * compisewindow 使用_p _c结尾来表示是父或是子");
            writer.Append("                 * case \"compise_p\":");
            writer.Append("                 * return new CompiseWindow(identity);");
            writer.Append("                 */");
            writer.Append("                default:");
            writer.Append("                    return null;");
            writer.Append("            }");
            writer.Append("        }");
            writer.Append("    }");
            writer.Append("}");
        }

        void WriteWinNameDefines()
        {
            foreach(string nameDefine in winNameDefineCode)
            {
                writer.Append(string.Format("        {0}", nameDefine));
            }
        }

        void WriteWinClassDefines()
        {
            for(int i=0; i<winClassDefineCode.Count; ++i)
            {
                writer.Append((i % 2 == 0) ? 
                    string.Format("                {0}", winClassDefineCode[i]) : 
                    string.Format("                    {0}", winClassDefineCode[i]));
            }
        }

        void MarshalDynamicCode()
        {
            winNameDefineCode.Clear();
            winClassDefineCode.Clear();
            foreach(string winAsset in winAssetPathes)
            {
                string wName = WindowName(winAsset);
                winNameDefineCode.Add(string.Format("public const string {0} = \"{1}\";", wName.ToUpper(), wName));
                winClassDefineCode.Add(string.Format("case {0}:", wName.ToUpper()));
                winClassDefineCode.Add(string.Format("return new {0}(identity);", wName));
            }
        }

        string WindowName(string winAsset)
        {
            string wName = winAsset.Substring(winAsset.LastIndexOf("/") + 1);
            return wName.Substring(0, wName.LastIndexOf("."));
        }
    }
}