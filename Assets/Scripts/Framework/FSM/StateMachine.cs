using System;
using System.Collections.Generic;

namespace Framework.FSM
{
    public abstract class StateMachine<T>
    {
        public StateBase<T> Current { get; private set; }
        public Stack<Type> History = new Stack<Type>();

        public string MachineName { get; private set; }

        public StateMachine(string name)
        {
            MachineName = name;
        }

        public virtual void ChangeState<U>(T msg, bool isRecord, params object[] paras) where U : StateBase<T>
        {
            ChangeState(typeof(U), isRecord, msg, paras);
        }

        public virtual void ChangeState(Type t, bool isRecord, T msg, params object[] paras)
        {
            if (isRecord && Current != null)
                History.Push(Current.GetType());
            if(Current != null)
                Current.Exit(msg, paras);
            Current = Activator.CreateInstance(t, this) as StateBase<T>;
            Current.Enter(msg);
            if (cleanHistroy(t))
                History.Clear();
        }

        public virtual void HandleMessage(T msg, params object[] paras)
        {
            Current.Process(msg, paras);
        }

        public virtual void BackState(T msg, params object[] paras)
        {
            if(History.Count > 0)
            {
                var t = History.Pop();
                ChangeState(t, false, msg, paras);
            }
        }

        protected abstract bool cleanHistroy(Type t);

    }
}
