using System;

namespace Framework.Log
{
    [Serializable]
    public class LoggerModule
    {
        public string moduleName;
        public bool flag = true;
    }

    public enum LoggerModuleType
    {
        //Framework
        Net,
        UI,
        FSM,
        MessageSystem,

        //Custom
    }
}
