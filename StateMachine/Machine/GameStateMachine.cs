
namespace Assets.Scripts.StateMachine
{
    /// <summary>
    /// 游戏流程控制状态机
    /// </summary>
    internal class GameStateMachine : StateMachine<Trigger, State<Trigger>>
    {
        protected override void Initialize()
        {
            //默认loading
            defaultstate_id = GameStateID.LoadingGame;
            GameLoadingState loadingState = CreateState<GameLoadingState>();
            GameEntryState entryState = CreateState<GameEntryState>();

            loadingState.AddMap(new GameLoadingFinishTrigger(), GameStateID.EnterGame);
        }
    }
}
