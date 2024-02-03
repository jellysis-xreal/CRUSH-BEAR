using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundManager : MonoBehaviour
{
    [Range(0, 100)] public int EffectVolume = 50;
    [Range(0, 100)] public int MusicVolume = 50;
    public List<AudioClip> musicClips = new List<AudioClip>();
    public Dictionary<string,AudioClip> effectClips = new Dictionary<string, AudioClip>();

    AudioSource[] musicSource = new AudioSource[2]; // 사용할 배경음악
    AudioSource[] effectSource = new AudioSource[4]; // 사용할 효과음

    public void Init()
    {
        for (int i = 0; i < musicSource.Length; i++)
        {
            musicSource[i] = gameObject.AddComponent<AudioSource>();
        }
        
        for (int i = 0; i < effectSource.Length; i++)
        {
            effectSource[i] = gameObject.AddComponent<AudioSource>();
        }
        
        // 효과음 추가 예시
        //effectSources.Add("Boss_Battle", Resources.Load<AudioClip>("Sounds/Battle_Boss"));
        //effectSources.Add("Battle", Resources.Load<AudioClip>("Sounds/Battle"));
    }
    
    // 효과음 재생 예시
    // public void PlayButtonSound()
    // {
    //     audioSource[0].clip = sounds["Button"];
    //     audioSource[0].Play();
    // }

    // [ContextMenu("PlayFunction/PlaySong1BGM")]
    // public void PlaySong1BGM()
    // {
    //     audioSource[0].clip = sounds["Play_Song_1"];
    //     audioSource[0].Play();
    // }

    // [ContextMenu("PlayFunction/PlaySong2BGM")]
    // public void PlaySong2BGM()
    // {
    //     audioSource[1].clip = sounds["Play_Song_1"];
    //     audioSource[1].Play();
    // }

    public void PlayWaveMusic(uint id)
    {
        musicSource[id].clip = musicClips[(int)id];
        musicSource[id].volume = MusicVolume / 100.0f;
        musicSource[id].Play();
    }
    
    public void StopWaveMusic(uint id)
    {
        //musicSource[id].clip = musicClips[(int)id];
        //musicSource[id].volume = MusicVolume / 100.0f;
        musicSource[id].Pause();
    }

    public void SetMusicVolume(float _vol)
    {
        MusicVolume = (int)_vol;
    }

    // public void Update()
    // {
    //     foreach (var audio in audioSource)
    //     {
    //         audio.volume = this.volume;
    //     }
    // }

    public void PauseMusic(uint id, bool IsPause)
    {
        if (IsPause) musicSource[id].Pause();
        else {
            musicSource[id].clip = musicClips[(int)id];
            musicSource[id].volume = MusicVolume / 100.0f;
            musicSource[id].Play();
        }
    }
}
