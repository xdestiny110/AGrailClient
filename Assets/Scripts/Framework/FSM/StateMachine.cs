using System;

namespace Framework.FSM
{
    public class StateMachine<T>
    {
        private StateBase<T> current;
        private StateBase<T> parent;

        public string MachineName { get; private set; }        

        public StateMachine(string name)
        {
            MachineName = name;
        }

        public void ChangeState<U>(T msg, params object[] paras) where U : StateBase<T>
        {            
            parent = current;
            current = Activator.CreateInstance(typeof(U), this) as StateBase<T>;
            if (parent != null)
                parent.Exit(msg, paras);
            current.Enter(msg);
        }
    }
}
