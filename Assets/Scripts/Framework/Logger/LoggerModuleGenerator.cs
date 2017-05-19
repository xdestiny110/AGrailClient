using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.Log
{
    public class LoggerModuleGenerator : ScriptableObject
    {
#if UNITY_EDITOR
        public const string ConfigPath = "Assets/Scripts/Framework/Logger/Resources/Config.asset";
#else
        public const string ConfigPath = "Config";
#endif
        public const string LoggerModulePath = "Assets/Scripts/Framework/Logger/LoggerModule.cs";

        private static LoggerModuleGenerator instance;
        public static LoggerModuleGenerator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<LoggerModuleGenerator>(ConfigPath);
                    if (instance == null)
                    {
#if UNITY_EDITOR
                        instance = CreateInstance<LoggerModuleGenerator>();
                        AssetDatabase.CreateAsset(instance, ConfigPath);
                        AssetDatabase.Refresh();
#else
                        Debug.LogError("Can not open logger module generator");
#endif
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


