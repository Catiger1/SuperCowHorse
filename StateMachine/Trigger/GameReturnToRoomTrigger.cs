using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.StateMachine
{
    internal class GameReturnToRoomTrigger : Trigger
    {
        protected override void Init()
        {
            base.Init();
            TriggerID = GameTriggerID.GameReturnToRoom;
        }
        public override bool HandleTrigger(IStateMachine sm)
        {
            return (sm.GetAndClearFlag() & (int)GameTriggerID.GameReturnToRoom) != 0;
        }
    }
}
