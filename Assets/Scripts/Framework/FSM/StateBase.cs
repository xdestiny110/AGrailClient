using UnityEngine;

namespace Framework.FSM
{
    public abstract class StateBase<T>
    {
        protected StateMachine<T> stateMachine;

        public StateBase(StateMachine<T> machine)
        {
            stateMachine = machine;
        }

        public abstract string StateName
        {
            get;
        }

        public abstract uint StateNumber
        {
            get;
        }

        public virtual void Enter(T msg, params object[] paras)
        {
            Debug.LogFormat("Enter {0}:{1}", StateName, StateNumber);
        }

        public virtual void Exit(T msg, params object[] paras)
        {
            Debug.LogFormat("Exit {0}:{1}", StateName, StateNumber);
        }

        public abstract void Process(T msg, params object[] paras);

    }
}
