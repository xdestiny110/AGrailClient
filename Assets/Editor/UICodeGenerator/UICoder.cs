using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Editor.UIGen
{
    public sealed class UICoder
    {
        private static UICoder instance;
        private static object locker = new object();
        public static UICoder Instance
        {
            get
            {
                if(instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                            instance = new UICoder();
                    }
                }
                return instance;
            }
        }

        private UICoder() { }


        const string UICodePath = "Scripts/UI/Auto";
        static List<string> ControlNameFixPre = new List<string>
        {
            "Txt",//Text
            "Lab",//Label
            "Btn", //Button
            "Inpt", //InputField
            "Img",//Image
            "Tog",//Toggle
        };

        FileWriter writer = null;
        string winAssetPath;
        string codeAssetPath;
        string winName;
        GameObject winInst;
        Dictionary<string, string> ControlNameToPath = new Dictionary<string, string>();

        public void Generate(string winPath)
        {
            this.winAssetPath = winPath;
            winName = winPath.Substring(winPath.LastIndexOf("/")+1);
            winName = winName.Substring(0, winName.LastIndexOf("."));
            codeAssetPath = string.Format("{0}/{1}.cs", UICodePath, winName);
            winInst = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(winAssetPath);
            winInst = GameObject.Instantiate<GameObject>(winInst);
            GetFixNameControls();
            if (writer != null)
                writer.Dispose();
            writer = new FileWriter(EditorTool.UnityPathToSystemPath(codeAssetPath));
            WriteFile();
            writer.Dispose();
            writer = null;
            GameObject.DestroyImmediate(winInst);
            ControlNameToPath.Clear();
        }

        void GetFixNameControls()
        {
            ControlNameToPath.Clear();
            LoopTrans(winInst.transform);
        }

        void LoopTrans(Transform trans)
        {
            DealTrans(trans);
            for(int i=0; i<trans.childCount; ++i)
            {
                LoopTrans(trans.GetChild(i));
            }
        }

        void DealTrans(Transform trans)
        {
            string name = trans.gameObject.name;
            foreach(string pre in ControlNameFixPre)
            {
                if(name.StartsWith(pre))
                {
                    AddTrans(trans);
                    return;
                }
            }
        }

        void AddTrans(Transform trans)
        {
            Stack<string> stack = new Stack<string>();
            Transform loopTrans = trans;
            while(loopTrans.parent != null)
            {
                stack.Push(loopTrans.gameObject.name);
                loopTrans = loopTrans.parent;
            }
            string s = "";
            while(stack.Count>0)
            {
                s = s.Length == 0 ? stack.Pop() : string.Format("{0}/{1}", s, stack.Pop());
            }
            ControlNameToPath.Add(trans.name, s);
        }

        void WriteFile()
        {
            writer.Append("using UnityEngine;");
            writer.Append("using System.Collections;");
            writer.Append("using System;");
            writer.Append("using UnityEngine.UI;");
            writer.Append("using System.Collections.Generic;");
            writer.Append("namespace UI");
            writer.Append("{");

            writer.Append(string.Format("    public partial class {0} : Window, IUIEventListener", winName));
            writer.Append("    {");
            writer.Append(string.Format("        const string name = \"{0}\";",winName));
            writer.Append("        public override string winName{ get { return name; }}");
            writer.Append(string.Format("        public {0}(int identity) : base(identity) ", winName));
            writer.Append("        {}");
            WriteTrans();
            writer.Append("    }");
            writer.Append("}");
        }

        void WriteTrans()
        {
            foreach(KeyValuePair<string,string> kvp in ControlNameToPath)
            {
                writer.Append(string.Format("        const string {0}Path = \"{1}\";",kvp.Key, kvp.Value));
            }
        }
    }
}