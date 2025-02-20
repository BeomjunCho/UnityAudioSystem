using UnityEngine;
using FMODUnity;

public class FmodEvents : Singleton<FmodEvents>
{
    [field: Header("2D Audio")]
    [field: SerializeField] public EventReference test2dSound {  get; private set; }


    [field: Header("3D Audio")]
    [field: SerializeField] public EventReference enemy { get; private set; }


    [field: Header("Player")]
    [field: SerializeField] public EventReference footSteps { get; private set; }


    [field: Header("Ambience")]
    [field: SerializeField] public EventReference windArea { get; private set; }


    [field: Header("Music")]
    [field: SerializeField] public EventReference testMusic { get; private set; }



}
