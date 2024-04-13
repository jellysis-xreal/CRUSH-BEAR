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
    float bpm;
    bool isBeated;
    public int currentBeat;

    [SerializeField] private AudioSource testSound;
    public event Action<int> onBeat;

    public void Init(float bpm, uint musicGUID) // 웨이브 시작 시 해당 함수 호출!
    {
        GameManager.Sound.PlayWaveMusic(musicGUID); //음악 start
        this.bpm = bpm;
        songStartTime = AudioSettings.dspTime;
        lastbeat = 0;
        secondsPerBeat = 60 / bpm;
        StartCoroutine(CheckBeat());
        currentBeat = 0;
        onBeat = null; // 이벤트 초기화!
    }

    IEnumerator CheckBeat()
    {
        while (true)
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
                Debug.LogError($"{currentBeat}비트!");
            }
            yield return null;
        }
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
}
