using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEditor.VersionControl.Asset;

namespace Assets.Scripts.StateMachine
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Trigger</typeparam>
    /// <typeparam name="TS">State</typeparam>
    /// <typeparam name="TID">TriggerID</typeparam>
    /// <typeparam name="TSID">StateID</typeparam>
    internal abstract class StateMachine<T, TS> : IStateMachine, IReset
        where T : Trigger
        where TS : State<T>
    {
        protected Enum defaultstate_id;
        private List<TS> states;
        public TS Curstate { get; set; }
        private int flag;
        public StateMachine()
        {
            states = new List<TS>();
            Initialize();
            SetDefaultState();
        }
        protected abstract void Initialize();
        public void ChangeState(Enum state_id)
        {
            Curstate?.ExitState(this);
            Curstate = states.Find((s)=> { return s.StateID.Equals(state_id); });
            Curstate.EnterState(this);
        }
        protected S CreateState<S>() where S:TS,new()
        {
            var tempState = new S();
            states.Add(tempState);
            return tempState;
        }
        private void SetDefaultState()
        {
            ChangeState(defaultstate_id);
        }
        public void Tick()
        {
            Curstate.Reason(this);
            Curstate.ActionState(this);
        }

        public void OnReset()
        {
           
        }

        public void SetFlag(Enum e)
        {
            flag |= (int)(GameTriggerID)e;
        }
        /// <summary>
        /// Get flag and Clear flag
        /// </summary>
        /// <returns></returns>
        public int GetAndClearFlag()
        {
            int tempFlag = flag;
            flag = 0;
            return tempFlag;
        }
    }
}
