using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{

    [Range(0, 100)] public int MusicVolume = 40;
    [Range(0, 100)] public int EffectVolume = 30;
    
    public Sound[] effectSounds;

    public List<AudioClip> musicClips = new List<AudioClip>();
    //public Dictionary<string, AudioClip> effectClips = new Dictionary<string, AudioClip>();
    
    AudioSource[] musicSource = new AudioSource[5]; // 사용할 배경음악
    AudioSource[] effectSource = new AudioSource[20]; // 사용할 효과음

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
    }
    
    public void PlayWaveMusic(uint id)
    {
        id--;
        Debug.Log($"[Sound] ID {id} " + musicClips[(int)id].name + " Play");
        
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

    public void PlayPunchEffect()
    {
        // 10 combo

        // 11 hit effect
        effectSource[11].clip = effectSounds[11].clip;
        effectSource[11].volume = EffectVolume / 100.0f;
        effectSource[11].Play();
    }
    
    public void PlayToastHitEffect()
    {
        // 14~16 combo
        
        // 17~19 hit effect
        int id = Random.Range(17, 20);
        effectSource[id].clip = effectSounds[id].clip;
        effectSource[id].volume = EffectVolume / 100.0f;
        effectSource[id].Play();
    }
    
    public void SetMusicVolume(float _vol)
    {
        MusicVolume = (int)_vol;
    }

    public void PauseMusic(uint id, bool IsPause)
    {
        if (IsPause) {
            id--;
            musicSource[id].Pause();
        }
        else
        {   
            id--;
            musicSource[id].Play();
        }
    }

    public void RestartMusic(uint id, bool IsPause) // 처음부터
    {
        // [임시] 잠깐 음악 전환으로 변경쇼..
        if (IsPause) musicSource[id].Stop();
        else
        {
            musicSource[id].clip = musicClips[(int)id];
            musicSource[id].volume = MusicVolume / 100.0f;
            musicSource[id].Play();
        }

    }

    public void playEffect(string name)
    {
        Sound s = Array.Find(effectSounds, s => s.name == name);

        if (s == null)
        {
            Debug.Log("[Effect NOT FOUND] " + name);
        }

        // 비어있는 AudioSource를 찾아서 효과음을 재생
        foreach (var source in effectSource)
        {
            if (!source.isPlaying)
            {
                source.clip = s.clip;
                source.Play();
                StartCoroutine(CheckEffectCompletion(source, s.clip.length));
                break;
            }
        }
    }

    // 효과음 재생 후 다시 비워주기
    IEnumerator CheckEffectCompletion(AudioSource source, float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        source.clip = null;
        source.Stop();
    }
}
