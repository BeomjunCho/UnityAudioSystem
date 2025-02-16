using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFX2DManager : Singleton<SFX2DManager>
{
    [Header("2D Audio Settings")]
    public int maxPoolSize = 5;
    public AudioMixerGroup sfx2dMixerGroup; // Optional.

    private List<AudioSource> audioSourcePool = new List<AudioSource>();
    private Dictionary<string, AudioSource> active2dSounds = new Dictionary<string, AudioSource>();

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < maxPoolSize; i++)
        {
            CreateAudioSource("2D_AudioSource_" + i);
        }
    }

    private AudioSource CreateAudioSource(string name)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = transform;
        AudioSource source = go.AddComponent<AudioSource>();
        source.spatialBlend = 0f; // Fully 2D.
        source.playOnAwake = false;
        source.loop = false;
        if (sfx2dMixerGroup != null)
        source.outputAudioMixerGroup = sfx2dMixerGroup;
        go.SetActive(false);
        audioSourcePool.Add(source);
        return source;
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource source in audioSourcePool)
        {
            if (!source.isPlaying)
            {
                ResetAudioSource(source);
                return source;
            }
        }
        if (audioSourcePool.Count < maxPoolSize * 2)
        {
            AudioSource newSource = CreateAudioSource("2D_AudioSource_Extra_" + audioSourcePool.Count);
            ResetAudioSource(newSource);
            return newSource;
        }
        Debug.LogWarning("No available 2D AudioSource in the pool!");
        return null;
    }

    private void ResetAudioSource(AudioSource source)
    {
        source.pitch = 1f;
        source.spatialBlend = 0f; // Always 2D.
    }

    public void Play2dSfx(string sfxName, AudioClip clip, float volume)
    {
        AudioSource source = GetAvailableAudioSource();
        if (source == null) return;
        source.clip = clip;
        source.volume = volume;
        source.loop = false;
        source.gameObject.SetActive(true);
        source.Play();
        string key = sfxName + "_" + Time.time;
        active2dSounds[key] = source;
        StartCoroutine(DeactivateAfterDuration(key, clip.length));
    }

    private IEnumerator DeactivateAfterDuration(string key, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (active2dSounds.ContainsKey(key))
        {
            AudioSource source = active2dSounds[key];
            source.Stop();
            source.gameObject.SetActive(false);
            active2dSounds.Remove(key);
        }
    }

    /// <summary>
    /// Stops a specific 2D sound effect.
    /// </summary>
    public void Stop2dSound(string sfxName)
    {
        List<string> keysToRemove = new List<string>();

        foreach (var kvp in active2dSounds)
        {
            if (kvp.Key.StartsWith(sfxName))
            {
                AudioSource source = kvp.Value;
                source.Stop();
                source.gameObject.SetActive(false);
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (string key in keysToRemove)
        {
            active2dSounds.Remove(key);
        }
    }

    /// <summary>
    /// Stops all active 2D sound effects.
    /// </summary>
    public void StopAll2dSounds()
    {
        foreach (var kvp in active2dSounds)
        {
            AudioSource source = kvp.Value;
            source.Stop();
            source.gameObject.SetActive(false);
        }
        active2dSounds.Clear();
    }

    public void Adjust2DAudioMixerVolume(float volume)
    {
        if (sfx2dMixerGroup != null && sfx2dMixerGroup.audioMixer != null)
        {
            sfx2dMixerGroup.audioMixer.SetFloat("Volume", volume);
        }
        else
        {
            Debug.LogWarning("SFX3DManager: AudioMixer or Mixer Group is not assigned!");
        }
    }
}
