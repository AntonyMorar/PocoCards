using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MusicData", order = 1)]
public class MusicData : ScriptableObject
{
    public AudioClip clip;
    public float pitch;
    public float volume;

}
