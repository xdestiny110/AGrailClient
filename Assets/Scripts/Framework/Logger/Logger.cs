using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Framework.Log
{
    public static class Logger
    {
        private static Dictionary<LoggerModuleType, bool> moduleTable = new Dictionary<LoggerModuleType, bool>();
        private static ConcurrentQueue<KeyValuePair<LogLevel, string>> logBuffer = new ConcurrentQueue<KeyValuePair<LogLevel, string>>();
        private static FileWriter fw = null;
        private static Thread th;

        public static void Init()
        {
            string logDirPath = Application.persistentDataPath + "/logs/";
            if (!Directory.Exists(logDirPath))
                Directory.CreateDirectory(logDirPath);

            string logFilePath = Path.Combine(logDirPath, DateTime.Now.ToString("MM_DD_hh:mm:ss") + "_log.txt");
            if (LoggerModuleGenerator.Instance.SaveLogFile)            
                fw = new FileWriter(logFilePath);            
            if (LoggerModuleGenerator.Instance.UseThread)
            {
                th = new Thread(() => 
                {
                    while (true)
                    {
                        while(logBuffer.Count > 0)
                        {
                            var kp = logBuffer.Dequeue();
                            if (fw != null)
                                fw.Append(kp.Value);
                            consoleOutput(kp.Key, kp.Value);
                        }
                    }
                });
                
            }
            UnityEngine.Debug.LogFormat("Log path = {0}, save log = {1}, userThread = {2}", 
                logFilePath, LoggerModuleGenerator.Instance.SaveLogFile, LoggerModuleGenerator.Instance.UseThread);

            foreach (var v in Enum.GetNames(typeof(LoggerModuleType)))
            {
                var idx = LoggerModuleGenerator.Instance.frameworkLoggerModules.FindIndex(s => { return s.moduleName == v; });
                if (idx >= 0)
                {
                    moduleTable.Add((LoggerModuleType)Enum.Parse(typeof(LoggerModuleType), v),
                        LoggerModuleGenerator.Instance.frameworkLoggerModules[idx].flag);
                }
            }         
        }

        public static void Debug(LoggerModuleType module, string str)
        {
            Log(LogLevel.Debug, module, str);
        }

        public static void Info(LoggerModuleType module, string str)
        {
            Log(LogLevel.Info, module, str);
        }

        public static void Warning(LoggerModuleType module, string str)
        {
            Log(LogLevel.Warning, module, str);
        }
        
        public static void Error(LoggerModuleType module, string str)
        {
            Log(LogLevel.Error, module, str);
        }
        
        public static void Exception(LoggerModuleType module, string str)
        {
            Log(LogLevel.Exception, module, str);
        }

        public static void Log(LogLevel level, LoggerModuleType module, string str)
        {
            if(moduleTable.ContainsKey(module) && moduleTable[module])
            {
                string content = string.Format("[{0}][{1}][{2}]: {3}",
                    DateTime.Now.ToString("hh:mm:ss"), module, level, str);
                if (LoggerModuleGenerator.Instance.UseThread)
                    logBuffer.Enqueue(new KeyValuePair<LogLevel, string>(level, content));
                else
                {
                    if (fw != null)
                        fw.Append(content);
                    consoleOutput(level, content);
                }
            }
        }

        private static void consoleOutput(LogLevel level, string str)
        {
            switch (level)
            {
                case LogLevel.Error:
                case LogLevel.Exception:
                    UnityEngine.Debug.LogError(str);
                    break;
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(str);
                    break;
                case LogLevel.Debug:
                case LogLevel.Info:
                    UnityEngine.Debug.Log(str);
                    break;
            }
        }

        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error,
            Exception
        }
    }
}

