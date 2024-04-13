using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    double lastbeat;
    double songStartTime;
    double songPosition;
    double secondsPerBeat;
    float bpm;
    bool isStarted;

    [SerializeField] private AudioSource testSound;
    public event Action onBeat;

    public void Init(float bpm) // 노래 시작할 때 해당 함수 호출
    {

        this.bpm = bpm;
        songStartTime = AudioSettings.dspTime;
        lastbeat = 0;
        secondsPerBeat = 60 / bpm;
        StartCoroutine(CheckBeat());
    }

    IEnumerator CheckBeat()
    {

        while (true)
        {
            songPosition = AudioSettings.dspTime - songStartTime;
            if (songPosition > lastbeat + secondsPerBeat)
            {
                onBeat?.Invoke(); // 비트마다 실행되는 함수
                lastbeat += secondsPerBeat;
                PlaySound();
                Debug.LogError("비트!");
            }
            yield return null;
        }
    }
    void PlaySound()
    {
        testSound.Play();
    }
}
