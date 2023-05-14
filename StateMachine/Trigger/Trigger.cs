using System;

namespace Assets.Scripts.StateMachine
{
    public enum GameTriggerID
    {
        GameLoadingFinish = 1>>0,
        GameEntryButton = 1>>1,

    };
    /// <summary>
    /// Trigger
    /// </summary>
    /// <typeparam name="T">TriggerID</typeparam>
    internal abstract class Trigger : ITrigger
    {
        public Trigger()
        {
            Init();
        }
        public Enum TriggerID { get; set; }
        public abstract bool HandleTrigger(IStateMachine sm);
        protected virtual void Init() { }
    }
}
