using System;

namespace Framework.FSM
{
    public partial class StateMachine<T>
    {
        public StateBase<T> Current { get; private set; }
        public StateBase<T> Parent { get; private set; }

        public string MachineName { get; private set; }        

        public StateMachine(string name)
        {
            MachineName = name;
        }

        public void ChangeState<U>(T msg, bool isRecord, params object[] paras) where U : StateBase<T>
        {
            ChangeState(typeof(U), isRecord, msg, paras);
        }

        public void ChangeState(Type t, bool isRecord, T msg, params object[] paras)
        {
            if(isRecord)
                Parent = Current;
            Current = Activator.CreateInstance(t, this) as StateBase<T>;
            if (Parent != null)
                Parent.Exit(msg, paras);
            Current.Enter(msg);
        }

        public void HandleMessage(T msg, params object[] paras)
        {
            Current.Process(msg, paras);
        }
    }
}
