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

    public void Init(float bpm) // �뷡 ������ �� �ش� �Լ� ȣ��
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
                onBeat?.Invoke(); // ��Ʈ���� ����Ǵ� �Լ�
                lastbeat += secondsPerBeat;
                PlaySound();
                Debug.LogError("��Ʈ!");
            }
            yield return null;
        }
    }
    void PlaySound()
    {
        testSound.Play();
    }
}
