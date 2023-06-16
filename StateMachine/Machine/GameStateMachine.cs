
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
            defaultstate_id = GameStateID.EnterGame;
            GameLoadingState loadingState = CreateState<GameLoadingState>();
            GameEntryState entryState = CreateState<GameEntryState>();
            RoomState roomState = CreateState<RoomState>();
            SelectTrapState selectTrapState = CreateState<SelectTrapState>();
            PlayState playState = CreateState<PlayState>();
            ResultState resultState = CreateState<ResultState>();
            GameStartCountDownState gameStartCountDownState = CreateState<GameStartCountDownState>();
            StopNetState stopNet = CreateState<StopNetState>();

            GameLoadingFinishTrigger gameLoadingTrigger = new GameLoadingFinishTrigger();
            GameRoomButtonTrigger gameRoomButtonTrigger = new GameRoomButtonTrigger();
            GameEntryGamePlayRoomTrigger gamePlayRoomTrigger = new GameEntryGamePlayRoomTrigger();
            GameStopNetTrigger gameStopNetTrigger = new GameStopNetTrigger();
            GameSelectTrapTrigger gameSelectTrapTrigger = new GameSelectTrapTrigger();
            GameCountDownTrigger gameCountDownTrigger = new GameCountDownTrigger();
            GamePlayTrigger gamePlayTrigger = new GamePlayTrigger();
            GameResultTrigger gameResultTrigger = new GameResultTrigger();
            GameReturnToRoomTrigger gameReturnToRoomTrigger = new GameReturnToRoomTrigger();

            //进入开始界面
            loadingState.AddMap(gameLoadingTrigger, GameStateID.EnterGame);
            stopNet.AddMap(gameLoadingTrigger, GameStateID.EnterGame);
            //进入游戏大厅
            entryState.AddMap(gameRoomButtonTrigger, GameStateID.Room);
            //大厅全都准备好了进入游戏
            roomState.AddMap(gamePlayRoomTrigger, GameStateID.EnterGamePlayRoom);
            //停止网络
            roomState.AddMap(gameStopNetTrigger, GameStateID.StopNet);
            resultState.AddMap(gameStopNetTrigger, GameStateID.StopNet);
            playState.AddMap(gameStopNetTrigger, GameStateID.StopNet);
            //选择陷阱
            playState.AddMap(gameSelectTrapTrigger, GameStateID.SelectTrap);
            //选择陷阱完成，进入倒计时
            selectTrapState.AddMap(gameCountDownTrigger, GameStateID.StartCountDown);
            //倒计时结束开始游戏
            gameStartCountDownState.AddMap(gamePlayTrigger, GameStateID.Play);
            //游戏结算
            playState.AddMap(gameResultTrigger, GameStateID.Result);
            //返回房间
            resultState.AddMap(gameReturnToRoomTrigger, GameStateID.Room);
            playState.AddMap(gameReturnToRoomTrigger, GameStateID.Room);
        }
    }
}
