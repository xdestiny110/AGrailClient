using UnityEngine;

namespace Framework.FSM
{
    public abstract class StateBase<T>
    {
        private StateMachine<T> stateMachine; 

        public StateBase(StateMachine<T> machine)
        {
            stateMachine = machine;
        }

        public abstract string StateName
        {
            get;
        }

        public virtual void Enter(T msg, params object[] paras)
        {
            Debug.Log("Enter " + StateName);
        }

        public virtual void Exit(T msg, params object[] paras)
        {
            Debug.Log("Exit " + StateName);
        }

        public abstract void Process(T msg, params object[] paras);

    }
}
