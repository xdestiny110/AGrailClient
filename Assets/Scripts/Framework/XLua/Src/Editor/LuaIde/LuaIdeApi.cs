
using CSObjectWrapEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using UnityEngine;
using XLua;

namespace Assets.XLua.Src.Editor.LuaIde
{
    public class LuaIdeApi : ScriptableObject
    {
        public TextAsset Template;
        static List<Type> LuaCallCSharp;

        static List<Type> CSharpCallLua;

        static List<Type> GCOptimizeList;

        static Dictionary<Type, List<string>> AdditionalProperties;

        static List<Type> ReflectionUse;

        static List<List<string>> BlackList;

    

        static Dictionary<Type, OptimizeFlag> OptimizeCfg;
        static IEnumerable<Type> type_has_extension_methods = null;

        
       
        [MenuItem("LuaIde/LuaIde Api", false, 1)]
        public static void GenLinkXml()
        {
            var d = ScriptableObject.CreateInstance<LuaIdeApi>();
            Generator.CustomGen(Application.dataPath + "/Xlua/Src/Editor/LuaIde/LuaIdeApi.tpl.txt", GetTasks);
        }
        public static void addLuaIdeInfo(Type type){
           
             LuaIdeInfo.luaInfo = new LuaIdeInfo();
                LuaIdeInfo.luaInfo.tableName = type.FullName;
                if (LuaIdeInfo.luaInfo.tableName.IndexOf('+') > -1)
                {

                    LuaIdeInfo.luaInfo.tableName = LuaIdeInfo.luaInfo.tableName.Replace('+', '.');
                }
                if (type.BaseType != null) {
                    LuaIdeInfo.luaInfo.baseName = type.BaseType.FullName;
                    if (LuaIdeInfo.luaInfo.baseName.IndexOf('+') > -1)
                    {

                        LuaIdeInfo.luaInfo.baseName = LuaIdeInfo.luaInfo.baseName.Replace('+', '.');
                    }
                    LuaIdeInfo.luaInfo.baseName = "CS." + LuaIdeInfo.luaInfo.baseName;
                }
                LuaIdeInfo.luaInfo.tableName = "CS." + LuaIdeInfo.luaInfo.tableName;
                List<MethodInfo> methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly)
                    .Where(method => !method.IsDefined(typeof(ExtensionAttribute), false) || method.DeclaringType != type)
                    .Where(method => !isMethodInBlackList(method) &&(!method.IsGenericMethod)).ToList();
                LuaIdeInfo.luaInfo.methods = methods;

                List<PropertyInfo> ps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly)
                .Where(prop =>  prop.Name != "Item" && !isObsolete(prop) && !isMemberInBlackList(prop)).ToList();
                foreach (PropertyInfo p in ps) {
                    LuaIdeInfo.luaInfo.addVar(new LuaIdeVarInfo()
                    {
                        propertyInfo = p,
                        isget = p.CanRead,
                        isset = p.CanWrite
                    });
                }
                List<FieldInfo> fs = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly)
                 .Where(field => !isObsolete(field) && !isMemberInBlackList(field)).ToList();
                foreach (FieldInfo f in fs)
                {
                    string value = "";
                    if (type.IsEnum) {
                        if (f.Name != "value__")
                        {
                            value = Convert.ToInt32(f.GetValue(null)).ToString();
                        }
                        else {
                            continue;
                        }
                        
                    }
                    LuaIdeInfo.luaInfo.addVar(new LuaIdeVarInfo()
                    {
                        fieldInfo = f,
                        isget =true,
                        isset = true,
                        value = value
                    });
                }

                LuaIdeInfo.luaInfo.ctorList = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase).ToList();
                LuaIdeInfo.luaInfos.Add(LuaIdeInfo.luaInfo);




        }
        public static IEnumerable<CustomGenTask> GetTasks(LuaEnv lua_env, UserConfig user_cfg)
        {
            LuaIdeClassDoc.checkDoc();
            LuaIdeInfo.luaInfos = new List<LuaIdeInfo>();
            LuaCallCSharp = user_cfg.LuaCallCSharp.ToList();// Generator.LuaCallCSharp;
            CSharpCallLua = user_cfg.CSharpCallLua.ToList();// Generator.CSharpCallLua;
            BlackList = Generator.BlackList;
            foreach (Type type in LuaCallCSharp) {

                addLuaIdeInfo(type);
            }
            //foreach (Type type in CSharpCallLua)
            //{

            //    addLuaIdeInfo(type);




            //}
            
            StringBuilder luasb = new StringBuilder();
            foreach (LuaIdeInfo luainfo in LuaIdeInfo.luaInfos)
            {
               
                if (luainfo.tableName != null)
                {
                    if (luainfo.tableName != null)
                    {
                        if (luainfo.tableName.IndexOf("System.Collections.Generic.List") > -1)
                            continue;
                        if (luainfo.tableName.IndexOf("System.Collections.Generic.Dictionary") > -1)
                            continue;
                         
                        luasb.AppendLine(luainfo.toStr());
                    }
                  
                }
                   
            }
            DirectoryInfo dirinfo = new DirectoryInfo(Application.dataPath);
            string dir = dirinfo.FullName + "/luaIde/";
            if (!Directory.Exists(dir))
            { 
                Directory.CreateDirectory(dir);
            }
            string filename = dir + "xluaApi.lua";
            using (StreamWriter textWriter = new StreamWriter(filename, false, Encoding.UTF8))
            {
                textWriter.Write(luasb.ToString());
                textWriter.Flush();
                textWriter.Close();
            }
            Debug.Log("Api 创建成功," + filename);
            LuaTable data = lua_env.NewTable();
            List<string> dd = new List<string>();
            
            data.Set("infos",dd);
            yield return new CustomGenTask
            {
                Data = data,
                Output = new StreamWriter(dir + "/readMe.txt",
        false, Encoding.UTF8)
            };
        }

        static bool isObsolete(MemberInfo mb)
        {
            if (mb == null) return false;
            return mb.IsDefined(typeof(System.ObsoleteAttribute), false);
        }

        static bool isMemberInBlackList(MemberInfo mb)
        {
            if (mb.IsDefined(typeof(BlackListAttribute), false)) return true;

            foreach (var exclude in BlackList)
            {
                if (mb.DeclaringType.FullName == exclude[0] && mb.Name == exclude[1])
                {
                    return true;
                }
            }

            return false;
        }

        static bool isMethodInBlackList(MethodBase mb)
        {
            if (mb.IsDefined(typeof(BlackListAttribute), false)) return true;

            foreach (var exclude in BlackList)
            {
                if (mb.DeclaringType.FullName == exclude[0] && mb.Name == exclude[1])
                {
                    var parameters = mb.GetParameters();
                    if (parameters.Length != exclude.Count - 2)
                    {
                        continue;
                    }
                    bool paramsMatch = true;

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (parameters[i].ParameterType.FullName != exclude[i + 2])
                        {
                            paramsMatch = false;
                            break;
                        }
                    }
                    if (paramsMatch) return true;
                }
            }
            return false;
        }
    }
}
