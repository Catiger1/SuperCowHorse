using UnityEngine;

namespace Assets.Scripts.StateMachine
{
    internal class GameStopNetTrigger : Trigger
    {
        protected override void Init()
        {
            base.Init();
            TriggerID = GameTriggerID.GameStopNet;
        }
        public override bool HandleTrigger(IStateMachine sm)
        {
            return sm.GetAndClearFlag(GameTriggerID.GameStopNet);
        }
    }
}
