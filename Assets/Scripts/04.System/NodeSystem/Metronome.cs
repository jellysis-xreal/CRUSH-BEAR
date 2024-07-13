using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    private uint musicGUID;
    double lastbeat;
    double songStartTime;
    double songPosition;
    public double secondsPerBeat;
    bool isBeated;
    bool isPlaying;
    public int currentBeat;
    public int shootStandard; // 노트를 몇 BPM 전에 발사하는 지, 그 기준.

    private event Action<int> onBeat;

    public void Init(float bpm, uint musicGUID) // 웨이브 시작 시 해당 함수 호출!
    {
        this.musicGUID = musicGUID;
        lastbeat = 0;
        secondsPerBeat = 60 / bpm;
        currentBeat = 0;
        shootStandard = (int) bpm / 13;
    }

    public void StartMusic()
    {
        GameManager.Sound.PlayWaveMusic(musicGUID); //음악 start
        songStartTime = AudioSettings.dspTime;
        isPlaying = true;
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
            }
            yield return null;
        }

        onBeat = null; // 이벤트 초기화!
    }
    public void BindEvent(Action<int> someAction)
    {
        onBeat -= someAction;
        onBeat += someAction;
    }
    public void UnBindEvent(Action<int> someAction) => onBeat -= someAction;
    public bool IsBeated() => isBeated;
    public bool SetGameEnd() => isPlaying = false;
}
