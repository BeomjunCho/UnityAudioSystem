using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : Singleton<MusicManager>
{
    [Header("Music Settings")]
    public AudioSource musicSource; // Audio source to play music
    public AudioMixerGroup musicMixerGroup; // Mixer group for music

    [System.Serializable]
    public struct MusicEntry
    {
        public MusicTrack trackType;
        public AudioClip clip;
    }

    [Header("Music Tracks")]
    public List<MusicEntry> musicTracks; // Store multiple music tracks
    private Dictionary<MusicTrack, AudioClip> musicDictionary;

    private void Awake()
    {
        // Initialize AudioSource if not assigned
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.parent = transform;
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.spatialBlend = 0f; // 2D sound.
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.bypassReverbZones = true; // Ignore reverb effects
            if (musicMixerGroup != null)
            musicSource.outputAudioMixerGroup = musicMixerGroup;
        }

        // Convert List to Dictionary for faster lookup
        musicDictionary = new Dictionary<MusicTrack, AudioClip>();
        foreach (var entry in musicTracks)
        {
            if (!musicDictionary.ContainsKey(entry.trackType))
            {
                musicDictionary.Add(entry.trackType, entry.clip);
            }
        }
    }

    /// <summary>
    /// Plays a specific music track using the MusicTrack enum.
    /// </summary>
    public void PlayMusicByEnum(MusicTrack track, float volume = 0.5f, bool loop = true)
    {
        if (musicDictionary.ContainsKey(track))
        {
            PlayMusic(musicDictionary[track], volume, loop);
        }
        else
        {
            Debug.LogWarning("Music track not found: " + track);
        }
    }

    /// <summary>
    /// Plays a music clip.
    /// </summary>
    public void PlayMusic(AudioClip clip, float volume, bool loop = true)
    {
        if (musicSource == null) return;
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.loop = loop;
        musicSource.spatialBlend = 0f; // Always 2D
        musicSource.bypassReverbZones = true; // Prevents reverb effects
        musicSource.Play();
    }

    /// <summary>
    /// Stops the currently playing music.
    /// </summary>
    public void StopMusic()
    {
        if (musicSource == null) return;
        musicSource.Stop();
    }

    /// <summary>
    /// Adjusts the music volume.
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        if (musicSource == null) return;
        musicSource.volume = volume;
    }

    public void AdjustMusicMixerVolume(float volume)
    {
        musicSource.outputAudioMixerGroup.audioMixer.SetFloat("Volume", volume);
    }
}
