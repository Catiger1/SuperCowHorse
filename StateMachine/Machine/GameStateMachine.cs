
using Assets.Scripts.StateMachine.State;

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
            RoomState roomState = CreateState<RoomState>();
            SelectTrapState selectTrapState = CreateState<SelectTrapState>();
            PlayState playState = CreateState<PlayState>();
            ResultState resultState = CreateState<ResultState>();
            GameStartCountDownState gameStartCountDownState = CreateState<GameStartCountDownState>();

            loadingState.AddMap(new GameLoadingFinishTrigger(), GameStateID.EnterGame);
            //进入游戏大厅
            entryState.AddMap(new GameRoomButtonTrigger(), GameStateID.Room);
            //大厅全都准备好了进入游戏
            roomState.AddMap(new GameEntryGamePlayRoomTrigger(), GameStateID.EnterGamePlayRoom);
            //停止网络
            roomState.AddMap(new GameStopNetTrigger(), GameStateID.EnterGame);
            resultState.AddMap(new GameStopNetTrigger(), GameStateID.EnterGame);
            playState.AddMap(new GameStopNetTrigger(), GameStateID.EnterGame);
            //选择陷阱
            playState.AddMap(new GameSelectTrapTrigger(), GameStateID.SelectTrap);
            //选择陷阱完成，进入倒计时
            selectTrapState.AddMap(new GameCountDownTrigger(), GameStateID.SelectTrap);
            //倒计时结束开始游戏
            gameStartCountDownState.AddMap(new GamePlayTrigger(), GameStateID.Play);
            //游戏结算
            playState.AddMap(new GameResultTrigger(), GameStateID.Result);
            //返回房间
            resultState.AddMap(new GameReturnToRoomTrigger(), GameStateID.Room);
            playState.AddMap(new GameReturnToRoomTrigger(), GameStateID.Room);
        }
    }
}
