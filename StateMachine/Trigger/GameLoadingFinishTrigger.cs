using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.StateMachine
{
    internal class GameLoadingFinishTrigger : Trigger
    {
        protected override void Init()
        {
            base.Init();
            TriggerID = GameTriggerID.GameLoadingFinish;
        }
        public override bool HandleTrigger(IStateMachine sm)
        {
            return (sm.GetAndClearFlag() & (int)GameTriggerID.GameLoadingFinish)!=0;
        }
    }
}
