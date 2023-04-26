using DG.Tweening;
using UnityEngine.UI;
using Assets.Scripts.StateMachine;
public class LoadingScene : MonoSingleton<LoadingScene>
{
    public Slider slider;
    void Start()
    {
        slider.DOValue(1, 1).onComplete = () => { GameStateManager.Instance.SetGameStateMachineFlag(GameTriggerID.GameLoadingFinish); };
    }
}
