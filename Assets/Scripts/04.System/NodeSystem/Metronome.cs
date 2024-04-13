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

    public void Init(float bpm, uint musicGUID) // ���̺� ���� �� �ش� �Լ� ȣ��!
    {
        GameManager.Sound.PlayWaveMusic(musicGUID); //���� start
        this.bpm = bpm;
        songStartTime = AudioSettings.dspTime;
        lastbeat = 0;
        secondsPerBeat = 60 / bpm;
        StartCoroutine(CheckBeat());
        currentBeat = 0;
        onBeat = null; // �̺�Ʈ �ʱ�ȭ!
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
                onBeat?.Invoke(currentBeat); // ��Ʈ���� ȣ��Ǵ� �̺�Ʈ
                lastbeat += secondsPerBeat;
                isBeated = true;
                PlaySound();
                Debug.LogError($"{currentBeat}��Ʈ!");
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
