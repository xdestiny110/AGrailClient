using System;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Framework.Log
{
    public class LogHandler : ILogHandler
    {
        private FileStream fs = null;
        private StreamWriter sw = null;
        private Thread th = null;
        private ConcurrentQueue<string> logBuffer = new ConcurrentQueue<string>();
        private ILogHandler defaultLoghandler = Debug.logger.logHandler;

        public LogHandler()
        {
            string logDirPath = Application.persistentDataPath + "/logs/";
            if (!Directory.Exists(logDirPath))
                Directory.CreateDirectory(logDirPath);

            string logFilePath = Path.Combine(logDirPath, DateTime.Now.ToString("MM_dd_hh_mm_ss") + ".log");

            fs = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            sw = new StreamWriter(fs);
            //th = new Thread(() => 
            //{
            //    using(fs = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            //    {
            //        sw = new StreamWriter(fs);
            //        while (true)
            //        {
            //            while (logBuffer.Count > 0)
            //            {
            //                var str = logBuffer.Dequeue();
            //                sw.WriteLine(str);
            //                sw.Flush();
            //            }
            //        }
            //    }
            //});
            //th.Start();            

            Application.logMessageReceivedThreaded += HandleLog;

            Debug.logger.logHandler = this;
            Debug.LogFormat("Log path = {0}", logFilePath);
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            defaultLoghandler.LogException(exception, context);
        }

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            format = string.Format("[{0}][{1}]", DateTime.Now.ToString("hh:mm:ss"), logType) + format;
            defaultLoghandler.LogFormat(logType, context, format, args);
        }

        public void Close()
        {
            Debug.Log("Close log file");
            sw.Close();
            fs.Close();
        }

        void HandleLog(string log, string stackTrace, LogType type)
        {
            //logBuffer.Enqueue(log);
            //logBuffer.Enqueue(stackTrace);
            sw.WriteLine(log);
            sw.WriteLine(stackTrace);
            sw.Flush();
        }

    }

}

