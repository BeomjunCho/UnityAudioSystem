using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFX3DManager : Singleton<SFX3DManager>
{
    [Header("3D Audio Settings")]
    public int maxPoolSize = 10;
    public float defaultMinDistance = 1f;
    public float defaultMaxDistance = 20f;
    public float defaultDopplerLevel = 1f;
    public float defaultSpread = 0f;
    public AudioMixerGroup sfx3dMixerGroup;

    private List<AudioSource> audioSourcePool = new List<AudioSource>();
    private Dictionary<string, AudioSource> active3dSounds = new Dictionary<string, AudioSource>();
    private Camera cachedCamera;

    private void Awake()
    {
        if (Camera.main != null)
        {
            cachedCamera = Camera.main;
        }
        else
        {
            Debug.LogWarning("SFX3DManager: No Main Camera found! SpatialBlend adjustment may not work.");
        }
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < maxPoolSize; i++)
        {
            CreateAudioSource("3D_AudioSource_" + i);
        }
    }

    private AudioSource CreateAudioSource(string name)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = transform;
        AudioSource source = go.AddComponent<AudioSource>();
        source.spatialBlend = 1f; // Fully 3D.
        source.playOnAwake = false;
        source.loop = false;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.minDistance = defaultMinDistance;
        source.maxDistance = defaultMaxDistance;
        if (sfx3dMixerGroup != null)
        source.outputAudioMixerGroup = sfx3dMixerGroup;
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
        // Optionally expand the pool.
        if (audioSourcePool.Count < maxPoolSize * 2)
        {
            AudioSource newSource = CreateAudioSource("3D_AudioSource_Extra_" + audioSourcePool.Count);
            ResetAudioSource(newSource);
            return newSource;
        }
        Debug.LogWarning("No available 3D AudioSource in the pool!");
        return null;
    }

    private void ResetAudioSource(AudioSource source)
    {
        source.pitch = 1f;
        source.spatialBlend = 1f;
        source.dopplerLevel = defaultDopplerLevel;
        source.spread = defaultSpread;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.minDistance = defaultMinDistance;
        source.maxDistance = defaultMaxDistance;
    }

    /// <summary>
    /// Plays a 3D sound effect at the caller's position.
    /// </summary>
    public void Play3dSfx(string soundName, AudioClip clip, Transform spawnTransform, float volume, bool loop = false,
        float? minDistance = null, float? maxDistance = null)
    {
        AudioSource source = GetAvailableAudioSource();
        if (source == null) return;

        // Use designer-set values if not provided
        float finalMinDistance = minDistance ?? defaultMinDistance;
        float finalMaxDistance = maxDistance ?? defaultMaxDistance;

        // Parent the AudioSource so it follows the caller.
        source.transform.SetParent(spawnTransform, false);
        source.transform.localPosition = Vector3.zero;
        source.clip = clip;
        source.volume = volume;
        source.loop = loop;
        source.minDistance = finalMinDistance;
        source.maxDistance = finalMaxDistance;
        source.gameObject.SetActive(true);
        
        source.Play();

        if (loop)
        {
            active3dSounds[soundName] = source;
        }
        else
        {
            string key = soundName + "_" + Time.time;
            active3dSounds[key] = source;
            StartCoroutine(DeactivateAfterDuration(key, clip.length));
        }
    }

    /// <summary>
    /// Plays 3D ambience. Also starts a coroutine to adjust spatialBlend dynamically:
    /// forces fully 2D sound when within min distance and gradually transitions to 3D.
    /// </summary>
    public void Play3dAmbience(string soundName, AudioClip clip, Transform spawnTransform, float volume, float minDistance = 1f, float maxDistance = 50f)
    {
        Play3dSfx(soundName, clip, spawnTransform, volume, true, minDistance, maxDistance);

        if (active3dSounds.ContainsKey(soundName))
        {
            AudioSource ambienceSource = active3dSounds[soundName];
            StartCoroutine(AdjustSpatialBlendOverDistance(ambienceSource, minDistance, maxDistance));
            ambienceSource.dopplerLevel = 0f; // Disable Doppler for ambience.
        }
    }

    private IEnumerator AdjustSpatialBlendOverDistance(AudioSource source, float minDistance, float maxDistance)
    {
        while (source != null && source.isPlaying)
        {
            float distance = Vector3.Distance(source.transform.position, cachedCamera.transform.position);
            // Force 2D (spatialBlend = 0) when within minDistance.
            if (distance <= minDistance)
            {
                source.spatialBlend = 0f;
            }
            else
            {
                float blend = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));
                source.spatialBlend = blend;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator DeactivateAfterDuration(string key, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (active3dSounds.ContainsKey(key))
        {
            AudioSource source = active3dSounds[key];
            source.Stop();
            source.transform.SetParent(transform, false);
            source.gameObject.SetActive(false);
            active3dSounds.Remove(key);
        }
    }

    public void Stop3dSound(string soundName)
    {
        List<string> keysToRemove = new List<string>();
        foreach (var kvp in active3dSounds)
        {
            if (kvp.Key.StartsWith(soundName))
            {
                AudioSource source = kvp.Value;
                source.Stop();
                source.transform.SetParent(transform, false);
                source.gameObject.SetActive(false);
                keysToRemove.Add(kvp.Key);
            }
        }
        foreach (string key in keysToRemove)
        {
            active3dSounds.Remove(key);
        }
    }

    public void StopAll3dSounds()
    {
        foreach (var kvp in active3dSounds)
        {
            AudioSource source = kvp.Value;
            source.Stop();
            source.transform.SetParent(transform, false);
            source.gameObject.SetActive(false);
        }
        active3dSounds.Clear();
    }

    public void Adjust3DAudioMixerVolume(float volume)
    {
        if (sfx3dMixerGroup != null && sfx3dMixerGroup.audioMixer != null)
        {
            sfx3dMixerGroup.audioMixer.SetFloat("Volume", volume);
        }
        else
        {
            Debug.LogWarning("SFX3DManager: AudioMixer or Mixer Group is not assigned!");
        }
    }
}
