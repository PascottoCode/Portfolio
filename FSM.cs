using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace RPG.StateMachine
{
    [Serializable]
    public abstract class FSM : State
    {
        [OdinSerialize] private Enum _defaultState;
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
        [OdinSerialize] private Dictionary<Enum, State> _states = new Dictionary<Enum, State>();
        public GameObject Parent { get; private set; }

        public State AntePreviousState { get; private set; }
        public State PreviousState { get; private set; }
        public State CurrentState { get; private set; }
        
        //DEBUG INFO
        #if UNITY_EDITOR
        [ShowIf("@CurrentState != null"), FoldoutGroup("Debug", -1f), ReadOnly, ShowInInspector]
        public string CurrentStateName { get => CurrentState?.Name; }
        [ShowIf("@CurrentState is FSM"), FoldoutGroup("Debug", -1f), ReadOnly, ShowInInspector] private FSM CurrentFSM
        {
            get
            {
                if (CurrentState is FSM fsm)
                {
                    return fsm;
                }
                return null;
            }
        } 
        #endif


        public static void InitRootStateMachine(FSM rootFSM, string name, GameObject parent)
        {
            rootFSM.Name = name;
            rootFSM.Parent = parent;
            rootFSM.rootStateMachine = rootFSM;
            
            rootFSM.OnInit();
        }
        private void InitStates()
        {
            foreach (var (enumValue, state) in _states)
            {
                InitState(enumValue, state, this);
            }
        }

        public T PreviousStateEnum<T>() where T : Enum
        {
            return (T)(object)PreviousState?.Index;
        }
        public bool IsAntePreviousState<T>(T newStateEnum) where T : Enum
        {
            if(AntePreviousState == null) { return false; }
            return AntePreviousState.Index == (int)(object)newStateEnum;
        }
        public bool IsPreviousState<T>(T newStateEnum) where T : Enum
        {
            if(PreviousState == null) { return false; }
            return PreviousState.Index == (int)(object)newStateEnum;
        }
        public bool IsCurrentState<T>(T newStateEnum) where T : Enum
        {
            if(CurrentState == null) { return false; }
            return CurrentState.Index == (int)(object)newStateEnum;
        }

        public void TransitionToDefaultState()
        {
            TransitionToState(_defaultState);
        }
        public void TransitionToState<T>(T newStateEnum) where T : Enum
        {
            if(_states.IsNullOrEmpty() || !_states.ContainsKey(newStateEnum)) { return; }
            CurrentState?.Exit();

            AntePreviousState = PreviousState;
            PreviousState = CurrentState;
            CurrentState = _states[newStateEnum];
            CurrentState.Enter();
            OnStateTransitioned();
        }
        public void TransitionToRandomState<T>(params T[] excludedStateEnums) where T : Enum
        {
            var randomStateList = _states.Keys.ToList();

            for (var i = 0; i < excludedStateEnums.Length; i++)
            {
                randomStateList.Remove(excludedStateEnums[i]);
            }

            TransitionToState(randomStateList[MyMath.RandomIndex(randomStateList.Count)]);
        }
        public void TransitionToRandomState()
        {
            var randomStateList = _states.Keys.ToList();
            TransitionToState(randomStateList[MyMath.RandomIndex(randomStateList.Count)]);
        }
        
        protected sealed override void OnEnter()
        {
            TransitionToDefaultState();
            OnFSMEnter();
        }
        public sealed override void OnUpdate()
        {
            CurrentState?.OnUpdate();
        }
        public sealed override void OnFixedUpdate()
        {
            CurrentState?.OnFixedUpdate();
        }
        protected sealed override void OnExit()
        {
            CurrentState?.Exit();
            CurrentState = null;
            PreviousState = null;
            AntePreviousState = null;
            OnFSMExit();
        }
        protected sealed override void OnInit()
        {
            OnFSMInit();
            InitStates();
        }
        protected sealed override void OnDestroy()
        {
            CurrentState?.Exit();
            OnFSMDestroy();
        }
        
        protected virtual void OnFSMInit(){}
        protected virtual void OnFSMEnter(){}
        protected virtual void OnFSMExit(){}
        protected virtual void OnFSMDestroy(){}
        protected virtual void OnStateTransitioned(){}
    }
}
