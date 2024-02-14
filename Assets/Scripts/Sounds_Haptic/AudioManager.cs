using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] musicSounds, effectSounds;
    public Dictionary<string, AudioClip> effectClips = new Dictionary<string, AudioClip>();
    public AudioSource musicSource, effectSource;

    private float pausedTime;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }



    public void playMusic(string name) // �뷡 ���
    {
        Sound s = Array.Find(musicSounds, s => s.name == name);

        if (s == null)
        {
            Debug.Log("[Music NOT FOUND] " + name);
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void stopMusic() // �뷡 ������ ���߱� ex. ���� ���� (ó������ ����)
    {
        musicSource.Stop();
        musicSource.time = 0; // ���� �ʱ�ȭ
    }

    public void pauseMusic() // �뷡 �Ͻ� ���� (�Ŀ� ��� �� ����� �ð����� ����)
    {
        Debug.Log("==  pauseMusic");
        pausedTime = musicSource.time; // �Ͻ� ������ �ð��� ����
        Debug.Log("====  pausedTime " + pausedTime);
        musicSource.Pause();
    }

    public void resumeMusic() // ��� ���̴� �뷡 ��� (playMusic�� ���)
    {
        Debug.Log("==  resumeMusic");
        musicSource.time = pausedTime; // �Ͻ� ������ �ð����� �ٽ� ���
        musicSource.Play();
    }

    public void SetMusicVolume(float _vol)
    {
        musicSource.volume = _vol; // Mathf.Clamp01(_vol)
        Debug.Log("==  SetMusicVolume : " + _vol);

    }


    public void playEffect(string name)
    {
        Sound s = Array.Find(effectSounds, s => s.name == name);

        if (s == null)
        {
            Debug.Log("[Effect NOT FOUND] " + name);
        }
        else
        {
            effectSource.clip = s.clip;
            effectSource.Play();
        }
    }




}

/* //���� �ڵ�
 public class SoundManager : MonoBehaviour
{
    [Range(0, 100)] public int EffectVolume = 50;
    [Range(0, 100)] public int MusicVolume = 50;
    public List<AudioClip> musicClips = new List<AudioClip>();
    public Dictionary<string,AudioClip> effectClips = new Dictionary<string, AudioClip>();

    AudioSource[] musicSource = new AudioSource[2]; // ����� �������
    AudioSource[] effectSource = new AudioSource[4]; // ����� ȿ����

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
        
        // ȿ���� �߰� ����
        //effectSources.Add("Boss_Battle", Resources.Load<AudioClip>("Sounds/Battle_Boss"));
        //effectSources.Add("Battle", Resources.Load<AudioClip>("Sounds/Battle"));
    }
    
    // ȿ���� ��� ����
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

    public void RestartMusic(uint id, bool IsPause)
    {
        if (IsPause) musicSource[id].Stop();
        else
        {
            musicSource[id].clip = musicClips[(int)id];
            musicSource[id].volume = MusicVolume / 100.0f;
            musicSource[id].Play();
        }

    }
}*/
