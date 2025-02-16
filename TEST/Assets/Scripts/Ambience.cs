using UnityEngine;

public class Ambience : MonoBehaviour
{
    public AudioClip testAmb;
    private void Start()
    {
        SFX3DManager.Instance.Play3dAmbience("testAmbience", testAmb, transform, 1.0f, 10f, 20f);
    }


}
