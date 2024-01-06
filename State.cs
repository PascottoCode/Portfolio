using System;

namespace RPG.StateMachine
{
    public abstract class State
    {
        protected FSM parentStateMachine;
        protected FSM rootStateMachine;

        public string Name { get; protected set; }
        public int Index { get; private set; } = -1;
        public T EnumValue<T>() where T : Enum => (T)(object)Index;
        protected bool IsActive { get; private set; }
        
        ~State()
        {
            OnDestroy();
        }

        protected static void InitState(Enum enumValue, State state, FSM newParentStateMachine)
        {
            state.Name = enumValue.ToString();
            state.Index = (int)(object) enumValue;
            state.parentStateMachine = newParentStateMachine;
            state.rootStateMachine = newParentStateMachine.rootStateMachine;

            state.OnInit();
        }

        public void Enter() { IsActive = true; OnEnter(); }
        public void Exit() { IsActive = false; OnExit(); }
        protected virtual void OnInit(){}
        protected virtual void OnDestroy(){}
        protected virtual void OnEnter(){}
        protected virtual void OnExit(){}
        public virtual void OnUpdate(){}
        public virtual void OnFixedUpdate(){}
    }
}