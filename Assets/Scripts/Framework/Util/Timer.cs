using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class Timer
    {
        public bool MustExec { get; private set; }
        public bool IsRealTime { get; private set; }
        public long RegistTime { get; private set; }
        public long DelayTime { get; private set; }
        private Action ac;

        public Timer(long delayTime, Action ac, bool isRealTime = false, bool mustExec = false)
        {
            DelayTime = delayTime;
            IsRealTime = isRealTime;
            MustExec = mustExec;
            this.ac = ac;
            RegistTime = IsRealTime ? DateTime.Now.GetMiliSecFrom1970() : (long)(Time.realtimeSinceStartup * 1000);
        }

        public void Exec()
        {
            if(ac != null)
                ac();
        }

        public bool CanExec()
        {
            return ((IsRealTime && Time.realtimeSinceStartup * 1000 - RegistTime > DelayTime) ||
                (!IsRealTime && DateTime.Now.GetMiliSecFrom1970() - RegistTime > DelayTime));
        }
    }
}


