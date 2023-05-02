using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using UnityEngine;

[Serializable]
public class Sound
{
    public AudioClip audioClip;
    public AudioMixerGroup mixerGroup;
    public bool playOnAwake;
    public bool loop;
    [Range(0f, 1f)]
    public float volunm;

}

public class SoundsAsset : ScriptableObject
{
    private static SoundsAsset instance;
    public static SoundsAsset Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<SoundsAsset>("SoundsAsset");
                if (instance == null)
                {
                    Debug.LogError("Please Config Asset->Create Sounds ScriptObject");
                }
            }
            return instance;
        }
    }

    public List<Sound> Sounds;
}