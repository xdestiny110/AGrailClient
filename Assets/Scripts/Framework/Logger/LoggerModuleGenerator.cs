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

        [SerializeField]
        private bool useThread = false;
        public bool UseThread { get { return useThread; } }
        [SerializeField]
        private bool saveLogFile = false;
        public bool SaveLogFile { get { return saveLogFile; } }

        [HideInInspector]
        public List<LoggerModule> frameworkLoggerModules = new List<LoggerModule>()
        {
            new LoggerModule() { moduleName = "Net", flag = true },
            new LoggerModule() { moduleName = "UI", flag = true },
            new LoggerModule() { moduleName = "FSM", flag = true },
            new LoggerModule() { moduleName = "MessageSystem", flag = true },
        };
        [HideInInspector]
        public List<LoggerModule> loggerModules = new List<LoggerModule>();

    }
}


