using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            throw new NotImplementedException();
        }
    }
}
