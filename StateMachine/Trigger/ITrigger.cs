
namespace Assets.Scripts.StateMachine
{
    interface ITrigger
    {
        bool HandleTrigger(IStateMachine sm);
    }
}
