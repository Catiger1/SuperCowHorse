using Assets.Scripts.StateMachine;
using System;

public class GameStateManager : MonoSingleton<GameStateManager>
{
    private static Action ticks;
    private static GameStateMachine gameStateMachine;
    public override void Init()
    {
        base.Init();
        InitComponent();
    }
    private void InitComponent()
    {
        gameStateMachine = new GameStateMachine();
        //Add State Machine Tick Function
        AddUpdateFunc(gameStateMachine.Tick);
    }
    /// <summary>
    /// modify Game State
    /// </summary>
    public void SetGameStateMachineFlag(GameTriggerID gameTriggerID)
    {
        gameStateMachine.SetFlag(gameTriggerID);
    }
    public void AddUpdateFunc(Action call)
    {
        ticks += call;
    }
    public void RemoveUpdateFunc(Action call)
    {
        ticks -= call;
    }
    // Update is call function once per frame
    void Update()
    {
        ticks?.Invoke();
    }
}
