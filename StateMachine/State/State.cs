﻿using System;
using System.Collections.Generic;

namespace Assets.Scripts.StateMachine
{
    public enum GameStateID
    {
        LoadingGame = 1 << 0,
        EnterGame = 1 << 1,
        Room = 1 << 2,
        SelectTrap = 1 << 3,
        EnterGamePlayRoom = 1 << 4,
        StartCountDown = 1 << 5,
        Play = 1 << 6,
        Result = 1 << 7,
        StopNet = 1 << 8,
        SetTrap = 1 << 9,
    };
    /// <summary>
    /// State
    /// </summary>
    /// <typeparam name="T">Trigger</typeparam>
    /// <typeparam name="TID">TiggerID</typeparam>
    /// <typeparam name="TSID">StateID</typeparam>
    internal abstract class State<T> : IState 
        where T:Trigger
    {
        public Enum StateID { get; set; }
        private Dictionary<Enum, Enum> map_trigger2state;
        private List<T> list_trigger;
        public State()
        {
            map_trigger2state = new Dictionary<Enum, Enum>();
            list_trigger = new List<T>();
            Init();
        }

        public State<T> AddMap(T tri, Enum state_id)
        {
            list_trigger.Add(tri);
            map_trigger2state.Add(tri.TriggerID, state_id);
            return this;
        }

        public virtual void ActionState(IStateMachine sm) { }
        public virtual void EnterState(IStateMachine sm) { }
        public virtual void ExitState(IStateMachine sm) { }

        protected virtual void Init() { }

        Enum tempStateID;
        public void Reason(IStateMachine sm)
        {
            for (int i = 0; i < list_trigger.Count; i++)
            {
                bool flag = list_trigger[i].HandleTrigger(sm);
                if (flag)
                {
                    tempStateID = map_trigger2state[list_trigger[i].TriggerID];
                    sm.ChangeState(tempStateID);
                    return;
                }
            }
        }
    }
}
