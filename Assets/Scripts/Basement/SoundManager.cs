using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public Dictionary<string,AudioClip> sounds = new Dictionary<string, AudioClip>();
    AudioSource[] audioSource = new AudioSource[2];

    public void Init()
    {
        for (int i = 0; i < audioSource.Length; i++)
        {
            audioSource[i] = gameObject.AddComponent<AudioSource>();
        }
        DontDestroyOnLoad(this);
        
        sounds.Add("Play_Song_1", Resources.Load<AudioClip>("Sounds/ImSorryImCute"));
        sounds.Add("Play_Song_2", Resources.Load<AudioClip>("Sounds/BANANA_SHAKE_SPED_UP"));
    }

    [ContextMenu("PlayFunction/PlaySong1BGM")]
    public void PlaySong1BGM()
    {
        audioSource[0].clip = sounds["Play_Song_1"];
        audioSource[0].Play();
    }
    
    [ContextMenu("PlayFunction/PlaySong2BGM")]
    public void PlaySong2BGM()
    {
        audioSource[1].clip = sounds["Play_Song_1"];
        audioSource[1].Play();
    }
}
