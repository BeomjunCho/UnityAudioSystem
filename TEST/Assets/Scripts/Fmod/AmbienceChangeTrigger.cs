using FMODUnity;
using UnityEngine;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class AmbienceChangeTrigger : MonoBehaviour
{
    [Header("Parameter Change")]
    [SerializeField] private string parameterName;
    [SerializeField] private float targetParameterValue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.SetAmbienceParameter(parameterName, targetParameterValue);
            // Use seek modulation in FMOD for smooth transition(right click the parameter)
        }
    }
}
