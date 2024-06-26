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
    public int shootStandard; // ��Ʈ�� �� BPM ���� �߻��ϴ� ��, �� ����.

    private event Action<int> onBeat;

    public void Init(float bpm, uint musicGUID) // ���̺� ���� �� �ش� �Լ� ȣ��!
    {
        GameManager.Sound.PlayWaveMusic(musicGUID); //���� start
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
            if (songPosition > lastbeat + secondsPerBeat -0.05)
            {
                currentBeat++;
                onBeat?.Invoke(currentBeat); // ��Ʈ���� ȣ��Ǵ� �̺�Ʈ
                lastbeat += secondsPerBeat;
                isBeated = true;
            }
            yield return null;
        }

        onBeat = null; // �̺�Ʈ �ʱ�ȭ!
    }
    public void BindEvent(Action<int> someAction)
    {
        onBeat -= someAction;
        onBeat += someAction;
    }

    public bool IsBeated() => isBeated;
    public bool SetGameEnd() => isPlaying = false;
}
