using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Music Settings")]
    public AudioMixerGroup masterMixer;

    /*private void Start()
    {
        MusicManager.Instance.PlayMusicByEnum(MusicTrack.MainTheme);
    }*/
}
