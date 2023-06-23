using System;

namespace Assets.Scripts.StateMachine
{
    public enum GameTriggerID
    {
        GameLoadingFinish = 1<<0,
        GameEntryButton = 1<<1,
        GameRoomButton = 1<<2,
        GameSelectTrap = 1<<3,
        GamePlay = 1<<4,
        GameResult = 1<<5,
        GameStopNet = 1<<6,
        GameStartCountDown = 1<<7,
        GameReturnToRoom = 1<<8,
        GameEntryGamePlay = 1<<9,
        GameSetTrap = 1<<10,
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
