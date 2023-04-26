using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEditor.VersionControl.Asset;

namespace Assets.Scripts.StateMachine
{
    public enum GameStateID
    {
        LoadingGame = 1 >> 0,
        EnterGame = 1 >> 1,

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
                if (list_trigger[i].HandleTrigger(sm))
                {
                    tempStateID = map_trigger2state[list_trigger[i].TriggerID];
                    sm.ChangeState(tempStateID);
                    return;
                }
            }
        }
    }
}
