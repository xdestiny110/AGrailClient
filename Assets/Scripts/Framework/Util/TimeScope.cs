using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Framework
{
    public class TimeScope
    {
        static Dictionary<Guid, TimeScope> stopWatches = new Dictionary<Guid, TimeScope>();

        [Conditional("TIMINGON")]
        public static void Start(Guid key, string displayName)
        {
            var ts = new TimeScope() { displayName = displayName, UID = key };
            ts.sw.Start();
            stopWatches.Add(key, ts);
        }

        [Conditional("TIMINGON")]
        public static void Stop(Guid guid)
        {
            stopWatches[guid].sw.Stop();
            UnityEngine.Debug.LogFormat("{0} cost time = {1}ms", stopWatches[guid].displayName, stopWatches[guid].sw.ElapsedMilliseconds);
            stopWatches.Remove(guid);
        }

        Stopwatch sw = new Stopwatch();
        string displayName = null;
        Guid UID { get; set; }
    }
}
