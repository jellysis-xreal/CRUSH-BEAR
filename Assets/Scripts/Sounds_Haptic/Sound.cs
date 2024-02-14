using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0, 100)] public int volume = 50;

    //public Sound(string n, AudioClip c)
    //{
    //    name = n;
    //    clip = c;
    //}
}
