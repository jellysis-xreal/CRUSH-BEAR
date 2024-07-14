using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{

    [Range(0, 100)] public int MusicVolume = 40;
    [Range(0, 100)] public int EffectVolume = 30;
    
    public Sound[] effectSounds;
    public List<AudioClip> backgroundClips = new List<AudioClip>();
    public List<AudioClip> musicClips = new List<AudioClip>();
    //public Dictionary<string, AudioClip> effectClips = new Dictionary<string, AudioClip>();
    
    AudioSource[] musicSource = new AudioSource[6]; // 사용할 배경음악
    AudioSource[] effectSource = new AudioSource[20]; // 사용할 효과음

    private List<AudioSource> pausedSources = new List<AudioSource>();

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
        //Debug.Log($"[Sound] ID {id} " + musicClips[(int)id].name + " Play");
        
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

    public void PlayEffect_UI_ButtonSelect()
    {
        playEffect("sfx_btn_select");
    }

    public void PlayEffect_UI_ButtonConfirm()
    {
        playEffect("sfx_btn_confirm");
    }
    
    public void PlayEffect_UI_ButtonClose()
    {
        playEffect("sfx_btn_confirm");
    }
    
    public void PlayEffect_UI_PopUp()
    {
        playEffect("sfx_btn_confirm");
    }
    
    
    
    public void PlayEffect_Countdown(int time, bool IsStart)
    {
        if (time == 1 && IsStart)
            playEffect("sfx_com_countdown_start");
        else
            playEffect("sfx_com_countdown");
    }
    
    public void PlayEffect_Punch()
    {
        // TODO combo

        // hit effect
        playEffect("sfx_cookie_hit");
    }
    
    public void PlayEffect_ToastHit()
    {
        // TODO 14~16 combo
        
        // 17~19 hit effect
        int id = Random.Range(1, 4);
        playEffect("sfx_toast_hit"+id);
    }
    
    public void PlayMusic_Lobby(bool play)
    {
        AudioClip audioClip = backgroundClips[0];
        
        // 비어있는 AudioSource를 찾거나, 쓰고 있는 AudioSource를 찾아서 정지
        foreach (var source in effectSource)
        {
            if (play)
            {
                if (!source.isPlaying)
                {
                    source.clip = audioClip;
                    source.volume = MusicVolume / 100.0f;
                    source.Play();
                    StartCoroutine(CheckEffectCompletion(source, audioClip.length));
                    break;
                }
            }
            else if (source.clip == audioClip)
            {
                StopCoroutine(CheckEffectCompletion(source, audioClip.length));
                source.clip = null;
                source.Stop();
            }
        }
    }

    public void PlayEffectMusic_GameWin()
    {
        playEffect("sfx_com_game_win");

        AudioClip audioClip = backgroundClips[1];
        FindBlankAudioSource(audioClip);
    }

    public void PlayEffectMusic_GameOver()
    {
        //Effect Sound
        playEffect("sfx_com_game_over");

        //Music Sound
        AudioClip audioClip = backgroundClips[2];
        FindBlankAudioSource(audioClip);
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
            //Debug.Log("[Effect NOT FOUND] " + name);
        }

        // 비어있는 AudioSource를 찾아서 효과음을 재생
        foreach (var source in effectSource)
        {
            if (!source.isPlaying)
            {
                source.clip = s.clip;
                source.volume = EffectVolume / 100.0f;
                source.Play();
                StartCoroutine(CheckEffectCompletion(source, s.clip.length));
                break;
            }
        }
    }

    private void FindBlankAudioSource(AudioClip audioClip)
    {
        // 비어있는 AudioSource를 찾아서 효과음을 재생
        foreach (var source in effectSource)
        {
            if (!source.isPlaying)
            {
                source.clip = audioClip;
                source.volume = MusicVolume / 100.0f;
                source.Play();
                StartCoroutine(CheckEffectCompletion(source, audioClip.length));
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

    public void SetMusic(StageData stage)
    {
        musicClips = stage.musicClips;
    }
    
    private List<AudioSource> GetActiveSources(AudioSource[] sources)
    {
        List<AudioSource> activeSources = new List<AudioSource>();
        foreach (var source in sources)
        {
            if (source.isPlaying)
            {
                activeSources.Add(source);
            }
        }
        return activeSources;
    }
    
    public void PauseAllSound()
    {
        pausedSources.Clear();

        foreach (var source in musicSource)
        {
            if (source.isPlaying)
            {
                source.Pause();
                pausedSources.Add(source);
            }
        }

        foreach (var source in effectSource)
        {
            if (source.isPlaying)
            {
                source.Pause();
                pausedSources.Add(source);
            }
        }
    }

    public void ResumeAllSound()
    {
        foreach (var source in pausedSources)
        {
            source.Play();
        }
    }
}
