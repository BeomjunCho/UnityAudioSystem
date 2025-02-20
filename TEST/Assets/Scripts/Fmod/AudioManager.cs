using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

public class AudioManager : Singleton<AudioManager>
{
    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;
    private EventInstance ambienceEventInstance;
    private EventInstance musicEventInstance;

    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume = 1;
    [Range(0, 1)]
    public float musicVolume = 1;
    [Range(0, 1)]
    public float ambienceVolume = 1;
    [Range(0, 1)]
    public float sfxVolume = 1;

    private Bus masterBus;
    private Bus musicBus;
    private Bus ambienceBus;
    private Bus sfxBus;


    private void Awake()
    {
        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }
    private void Start()
    {
        AudioManager.Instance.InitializeMusic(FmodEvents.Instance.testMusic);
        AudioManager.Instance.InitializeAmbience(FmodEvents.Instance.windArea);
    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        ambienceBus.setVolume(ambienceVolume);
        sfxBus.setVolume(sfxVolume);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        if (emitter == null)
        {
            emitter = emitterGameObject.AddComponent<StudioEventEmitter>();
        }
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    public void InitializeAmbience(EventReference ambEventRef)
    {
        if (ambienceEventInstance.isValid())
        {
            ambienceEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            ambienceEventInstance.release();
        }

        ambienceEventInstance = CreateEventInstance(ambEventRef);
        ambienceEventInstance.start();
    }

    public void InitializeMusic(EventReference ambEventRef)
    {
        if (musicEventInstance.isValid())
        {
            musicEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicEventInstance.release();
        }

        musicEventInstance = CreateEventInstance(ambEventRef);
        musicEventInstance.start();
    }

    public void SetMusicArea(MusicArea area)
    {
        musicEventInstance.setParameterByName("Area", (float) area);
    }

    public void SetAmbienceParameter(string parameterName, float value)
    {
        ambienceEventInstance.setParameterByName(parameterName, value);
    }

    public float GetAmbienceParameter(string parameterName)
    {
        float parameterValue = 0f;
        FMOD.RESULT result = ambienceEventInstance.getParameterByName(parameterName, out parameterValue);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Failed to get parameter: " + parameterName + " (result: " + result + ")");
        }
        return parameterValue;
    }

    private void CleanUp()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        eventInstances.Clear(); // Prevent memory leaks

        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            if (emitter != null)
                emitter.Stop();
        }
        eventEmitters.Clear(); // Prevent memory leaks

        if (ambienceEventInstance.isValid())
        {
            ambienceEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            ambienceEventInstance.release();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}
