
using System;

namespace Assets.Scripts.StateMachine
{
    internal interface IStateMachine
    {
        void Tick();
        void ChangeState(Enum state_id);
        bool GetAndClearFlag(Enum e);
        void SetFlag(Enum e);
    }
}
