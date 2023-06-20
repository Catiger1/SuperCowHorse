using DG.Tweening;
using Mirror;
using System;
using UnityEngine;
using UnityEngine.UI;

public enum TimerFormatType
{
    Time,
    Number
}

public class NetCountdownTimer : NetworkBehaviour
{
    public Text timerText;
    [SyncVar(hook = nameof(TimerValueChange))]
    public int SynTime = 30;
    private float countdownDuration = 30.0f;
    public int maxCountdownTime = 30;
    private Tweener countdownTween;
    private Action endFunc;
    private Action startFunc;
    public TimerFormatType timerType;
    private void OnEnable()
    {
        Reset();
    }

    public NetCountdownTimer InitTimer(Action startfunc,Action endfunc)
    {
        startFunc = startfunc;
        endFunc = endfunc;
        return this;
    }
    //[Command(requiresAuthority = false)]
    public void TimerValueChange(int _,int newValue)
    {
        timerText.text = FormatTime(newValue,timerType);
    }

    public void Reset()
    {
        countdownDuration = (int)maxCountdownTime;
        SynTime = (int)maxCountdownTime;
    }

    public void StartTimer()
    {
        startFunc?.Invoke();
        if(countdownTween==null)
        countdownTween = DOTween.To(() => countdownDuration, x => countdownDuration = x, 0.0f, countdownDuration)
            .OnUpdate(() => {
                if((int)countdownDuration< SynTime)
                {
                    SynTime = (int)countdownDuration;
                }
            }).SetEase(Ease.Linear)
            .OnComplete(() => {
                endFunc?.Invoke();
                countdownTween = null;
                SetActive(false);
                Debug.Log("µ¹¼ÆÊ±½áÊø£¡");
            });
    }

    public void StopTimerAndCallEndFunc()
    {
        countdownTween.Kill();
        countdownTween = null;
        endFunc?.Invoke();
    }

    private string FormatTime(float time,TimerFormatType timerType)
    {
        string retFormat;
        if (timerType == TimerFormatType.Time)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            retFormat = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            retFormat = ((int)time).ToString();
        }
        return retFormat;
    }

    public void SetActive(bool flag)
    {
        if (flag)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = Vector3.zero;
    }
}
