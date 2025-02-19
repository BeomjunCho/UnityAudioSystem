using UnityEngine;
using FMODUnity;

public class AudioManager : Singleton<AudioManager>
{
  
    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }
}
