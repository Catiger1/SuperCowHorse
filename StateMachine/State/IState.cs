using System;

namespace Assets.Scripts.StateMachine
{
    internal interface IState
    {
        void Reason(IStateMachine sm);
        void EnterState(IStateMachine sm);
        void ExitState(IStateMachine sm);
        void ActionState(IStateMachine sm);
    }
}
