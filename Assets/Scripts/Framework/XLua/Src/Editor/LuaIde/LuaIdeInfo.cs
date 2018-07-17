using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;


  
    public class LuaIdeVarInfo{
        public FieldInfo fieldInfo;
        public PropertyInfo propertyInfo;
        public EventInfo eventInfo;
        public string value="";
        
        public bool isget= false;
        public bool isset=false;
    }
    public class LuaIdeInfo
    {
        public static LuaIdeInfo luaInfo;
        public static List<LuaIdeInfo> luaInfos = new List<LuaIdeInfo>();
        public string tableName;
        public string baseName;
        public bool IsEnum;
        public bool isStaticClass;
        public bool isNewFun = false;
        public List<MethodInfo> methods;
        public List<ConstructorInfo> ctorList;
        public bool isDefConstructorInfo = false;
        public List<LuaIdeVarInfo> varInfos = new List<LuaIdeVarInfo>();


        public List<string> events = new List<string>();
       
        public void addVar(LuaIdeVarInfo varInfo)
        {
            varInfos.Add(varInfo);
        }
       

        public string toStr() {
         
          if (tableName.IndexOf("+") > -1)
          {
              tableName = tableName.Replace("+", ".");
            }
            StringBuilder sb = new StringBuilder();
          if (IsEnum)
            {
                sb.AppendLine("--" + tableName + "  Enum");
            }
            else {
                if (this.baseName != null) {
                    sb.AppendLine("--@SuperType [luaIde#" + this.baseName + "]");
                }
                
                
            }
          sb.AppendLine(tableName + " = {}");
            if (this.isDefConstructorInfo)
            {
                string commenStr = "--[[\n";
                commenStr += "\t@return [luaIde#" + tableName + "]\n";
                commenStr += "]]";
                sb.AppendLine("function " + tableName + ":() end");
            }
            else
            {
                if (this.ctorList != null)
                {

                    foreach (ConstructorInfo info in this.ctorList)
                    {
                        string argStr = "";

                        string commenStr = "--[[\n";

                        ParameterInfo[] parameterInfos = info.GetParameters();
                        string parstr = "";
                        for (int i = 0; i < parameterInfos.Length; i++)
                        {
                            ParameterInfo param = parameterInfos[i];
                            string paramStr = "";

                            if (param.Name == "end")
                            {
                                paramStr = param.Name + "_";
                            }
                            else
                            {
                                paramStr = param.Name;
                            }
                            parstr += paramStr;
                            bool isadd = false;
                            paramStr = "\t@" + paramStr + " " + getTypeStr(param.ParameterType, out isadd);
                            paramStr += "\n";
                            commenStr += paramStr;
                            if (i < parameterInfos.Length - 1)
                            {
                                parstr += ",";

                            }
                        }
                        commenStr += "\t@return [luaIde#" + tableName + "]\n";
                        commenStr += "]]";
                        sb.AppendLine(commenStr);
                        sb.AppendLine("function " + tableName + "(" + parstr + ") end");

                    }
                }
            }
            //字段
            List<string> propertyInfoFunName = new List<string>();
            foreach (LuaIdeVarInfo info in varInfos)
            {
                string varName = "";
                string commenStr = "--[[\n";
                Type type_ = null;
                if (info.fieldInfo != null)
                {
                    object[] obj = info.fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (IsEnum && info.fieldInfo.Name == "value__") {
                        continue;
                    }
                   type_ = info.fieldInfo.FieldType;
                    varName = info.fieldInfo.Name;
                }
                else if (info.propertyInfo != null) {
                    type_ = info.propertyInfo.PropertyType;
                    varName = info.propertyInfo.Name;
                    propertyInfoFunName.Add("get_" + varName);
                    propertyInfoFunName.Add("set_" + varName);
                }
                else if (info.eventInfo != null) {
                    type_ = info.eventInfo.EventHandlerType;
                    varName = info.eventInfo.Name;
                }
                if (varName == "end") continue;
                bool isadd = false;
                string type__ = getTypeStr(type_, out isadd);
                string typestr = "";
                if (isadd && !type_.IsEnum)
                {

                    typestr = "\t@RefType [luaIde#" + type__ + "]";
                }
                else {
                    if (!IsEnum)
                    {
                        typestr = "\t" + type__;
                    }
                }
                


               commenStr += typestr + "\n";
                if (info.isget) {
                    commenStr += "\t Get ";
                }
                if (info.isset) {
                    commenStr += "\t Set ";
                }
                string str = tableName + "." + varName;
                string doc = LuaIdeClassDoc.getDoc(str);
                

                if (info.value != "")
                {
                    varName += " = " + info.value;
                }
                else {
                    varName += " = nil";
                }
                sb.AppendLine(commenStr);
               
                if (doc != "")
                {
                    sb.AppendLine( doc);
                }
                sb.AppendLine("--]]");
                sb.AppendLine(tableName + "."+varName);
                
            }
            
          

            //方法
            foreach (MethodInfo m in methods) {

                if (m.Name == "end") {
                    continue;
                }
                if (m.Name.IndexOf(".") > -1)
                {
                    continue;
                }

                if (m.Name.IndexOf("set_") == 0 || m.Name.IndexOf("get_") == 0)
                {
                    continue;
                }
                string commenStr = "--[[\n";

                LuaIdeClassDocInfo docInfo = LuaIdeClassDoc.getLuaIdeClassDocInfo(tableName + "." + m.Name);
                if (docInfo != null && docInfo.doc != "")
                {
                    commenStr += docInfo.doc + "\n";
                }
                ParameterInfo[] parameterInfos = m.GetParameters();
                string parstr = "";
                for (int i = 0; i < parameterInfos.Length;i++ )
                {
                    ParameterInfo param = parameterInfos[i];
                    string paramStr = "";

                    if (param.Name == "end")
                    {
                        paramStr = param.Name + "_";
                    }
                    else
                    {
                        paramStr = param.Name;
                    }
                    parstr += paramStr;
                    bool isadd = false;
                    paramStr = "\t@" + paramStr + " " + getTypeStr(param.ParameterType,out isadd) ;
                    if (docInfo != null)
                    {
                        if (docInfo.paraminfo.ContainsKey(param.Name))
                        {
                            paramStr +=docInfo.paraminfo[param.Name]+ "\n";
                        }
                        else {
                            paramStr += "\n";
                        }
                        
                    }
                    else
                    { 
                       paramStr += "\n";
                    }
                   
                   
                    commenStr += paramStr;
                    if (i < parameterInfos.Length - 1)
                    {
                        parstr += ",";
                        
                    }
                }

                if (m.ReturnType != null && m.ReturnType.FullName != "System.Void")
                {
                    bool isadd = false;
                    if (m.ReturnType == typeof(UnityEngine.Transform)) {
                        int dd = 1;
                    }
                    string type__ = this.getTypeStr(m.ReturnType, out isadd);
                    if (isadd && !m.ReturnType.IsEnum)
                    {
                        commenStr += "\t@return [luaIde#" + type__ + "]\n";
                    }
                    else {
                        commenStr += "\treturn "+ type__ + "\n";
                    }
                    
                }
                commenStr += "--]]";
                if (parameterInfos.Length > 0 || (docInfo != null && docInfo.doc != "") || m.ReturnType == null && m.ReturnType.FullName == "System.Void")
                {
                    sb.AppendLine(commenStr);
                }
                sb.AppendLine("function " + tableName + ":" + m.Name + "(" + parstr + ") end");
            }
            return sb.ToString();
        
        }

        public string getTypeStr(Type type,out bool isadd) {



            isadd = false;
            string type_ = type.FullName;
        if(type_ == null){
            return type.Name;
        }
            int index = type_.IndexOf('+');

            if (type_.IndexOf('+') > -1)
            {

                type_ = type_.Replace('+', '.');
            }
            type_ = "CS." + type_;
            if (type_.IndexOf("UnityEngine.Transform") > -1)
            {
                int dd = 1;
            }
            foreach (LuaIdeInfo linfo in luaInfos)
            {
                if (linfo.tableName != null) {
                    
                    if (linfo.tableName == type_)
                    {
                        isadd = true;
                    }
                }
                
                
            }
                
            
            if (type_.IndexOf('[') > -1)
            {
                type_ = type_.Replace("[", "{");
            }
            if (type_.IndexOf(']') > -1)
            {
                type_ = type_.Replace("]", "}");
            }

            return type_;
        }
    }

