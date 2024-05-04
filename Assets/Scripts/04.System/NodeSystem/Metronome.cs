using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    double lastbeat;
    double songStartTime;
    double songPosition;
    public double secondsPerBeat;
    bool isBeated;
    bool isPlaying;
    public int currentBeat;
    public int shootStandard; // 노트를 몇 BPM 전에 발사하는 지, 그 기준.

    [SerializeField] private AudioSource testSound;
    private event Action<int> onBeat;

    public void Init(float bpm, uint musicGUID) // 웨이브 시작 시 해당 함수 호출!
    {
        GameManager.Sound.PlayWaveMusic(musicGUID); //음악 start
        songStartTime = AudioSettings.dspTime;
        lastbeat = 0;
        secondsPerBeat = 60 / bpm;
        isPlaying = true;
        currentBeat = 0;
        shootStandard = (int) bpm / 13;
        StartCoroutine(CheckBeat());
    }

    IEnumerator CheckBeat()
    {
        while (isPlaying)
        {
            isBeated = false;
            songPosition = AudioSettings.dspTime - songStartTime;
            if (songPosition > lastbeat + secondsPerBeat)
            {
                currentBeat++;
                onBeat?.Invoke(currentBeat); // 비트마다 호출되는 이벤트
                lastbeat += secondsPerBeat;
                isBeated = true;
                PlaySound();
                Debug.LogWarning($"{currentBeat}비트!");
            }
            yield return null;
        }

        onBeat = null; // 이벤트 초기화!
    }
    void PlaySound()
    {
        testSound.Play();
    }

    public void BindEvent(Action<int> someAction)
    {
        onBeat -= someAction;
        onBeat += someAction;
    }

    public bool IsBeated() => isBeated;
    public bool SetGameEnd() => isPlaying = false;
}
