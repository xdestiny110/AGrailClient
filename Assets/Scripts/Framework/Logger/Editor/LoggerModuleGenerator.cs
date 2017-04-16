using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Log
{
    public class LoggerModuleGenerator : ScriptableObject
    {
        public const string ConfigPath = "Assets/Scripts/Framework/Logger/Editor/Config.asset";
        public const string LoggerModulePath = "Assets/Scripts/Framework/Logger/LoggerModule.cs";

        private static LoggerModuleGenerator instance;
        public static LoggerModuleGenerator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = AssetDatabase.LoadAssetAtPath<LoggerModuleGenerator>(ConfigPath);
                    if (instance == null)
                    {
                        instance = CreateInstance<LoggerModuleGenerator>();
                        AssetDatabase.CreateAsset(instance, ConfigPath);
                        AssetDatabase.Refresh();
                    }
                }
                return instance;
            }
        }

        [HideInInspector]        
        public List<LoggerModule> loggerModules = new List<LoggerModule>()
        {
            new LoggerModule() { moduleName = "Net", flag = true },
        };

    }
}


