using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class AudioGroupType
{
    public static string Master = "Master";
    public static string BGM = "BGM";
    public static string SFX = "SFX";
}

public class SoundName
{
    public static string BGM0 = "BGM0";
    public static string BGM1 = "BGM0";
    public static string Button = "Button";
}

public class AudioManager : MonoSingleton<AudioManager>
{
    public static AudioMixer audioMixer;
    private static Dictionary<string, AudioSource> audioDic = new Dictionary<string, AudioSource>();
    private static AudioSource curPlayingAudio;
    public override void Init()
    {
        base.Init();
        audioMixer = ResourcesManager.Load<AudioMixer>("Audio/AudioMixer");
        InitSoundsAsset();
    }
    
    public static void SetVolunm(string audioGroupType, float value)
    {
        audioMixer.SetFloat(audioGroupType, value);
    }
    public static float GetVolunm(string audioType)
    {
        audioMixer.GetFloat(audioType, out float returnValue);
        return returnValue;
    }
    private void InitSoundsAsset()
    {
        foreach (var sound in SoundsAsset.Instance.Sounds)
        {
            GameObject audioSourceGameObject = new GameObject(sound.audioClip.name);
            //audioSourceGameObject.transform.SetParent(transform);
            AudioSource source= audioSourceGameObject.AddComponent<AudioSource>();
            source.clip = sound.audioClip;
            source.playOnAwake = sound.playOnAwake;
            source.loop = sound.loop;
            source.outputAudioMixerGroup = sound.mixerGroup;
            source.volume = sound.volunm;
            DontDestroyOnLoad(audioSourceGameObject);
            audioDic.Add(source.clip.name, source);
        }
    }

    public static void Play(string name)
    {
        if (audioDic.ContainsKey(name))
        {
            if (audioDic[name].isPlaying)
                audioDic[name].Stop();
            audioDic[name].Play();
            curPlayingAudio = audioDic[name];
        }
        else
            Debug.LogError("Can't find audio with sound name" + name);
    }

    public static void Stop(string name)
    {
        if (audioDic.ContainsKey(name) && audioDic[name].isPlaying)
        {
            audioDic[name].Stop();
            if(name == curPlayingAudio.clip.name)
                curPlayingAudio = null;
        }
        else
            Debug.LogError("Can't find audio with sound name" + name);
    }

    public static void Stop()
    {
        if(curPlayingAudio!=null&&curPlayingAudio.isPlaying)
        {
            curPlayingAudio.Stop();
            curPlayingAudio = null;
        }
    }


}
