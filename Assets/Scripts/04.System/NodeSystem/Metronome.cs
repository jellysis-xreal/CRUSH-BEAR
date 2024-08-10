using Cysharp.Threading.Tasks;
using System;
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
    public int shootStandard; // ��Ʈ�� �� BPM ���� �߻��ϴ� ��, �� ����.

    private event Action<int> onBeat;

    public void Init(float bpm, uint musicGUID) // ���̺� ���� �� �ش� �Լ� ȣ��!
    {
        this.musicGUID = musicGUID;
        lastbeat = 0;
        secondsPerBeat = 60 / bpm;
        currentBeat = 0;
        shootStandard = (int)bpm / 13;
    }

    public void StartMusic()
    {
        GameManager.Sound.PlayWaveMusic(musicGUID); //���� start
        songStartTime = AudioSettings.dspTime;
        isPlaying = true;
        CheckBeat().Forget();
    }

    async UniTask CheckBeat()
    {
        while (isPlaying)
        {
            isBeated = false;
            songPosition = AudioSettings.dspTime - songStartTime;
            if (songPosition > lastbeat + secondsPerBeat)
            {
                currentBeat++;
                onBeat?.Invoke(currentBeat); // ��Ʈ���� ȣ��Ǵ� �̺�Ʈ
                lastbeat += secondsPerBeat;
                isBeated = true;
            }

            //else
            //Debug.LogWarning("CheckBeat Error");
            await UniTask.Yield();
        }

        onBeat = null; // �̺�Ʈ �ʱ�ȭ!
    }
    public void BindEvent(Action<int> someAction)
    {
        onBeat -= someAction;
        onBeat += someAction;
    }
    public void UnBindEvent(Action<int> someAction) => onBeat -= someAction;
    public bool IsBeated() => isBeated;
    public bool SetGameEnd()
    {
        isPlaying = false;
        GameManager.Sound.StopWaveMusic(musicGUID);
        return isPlaying;
    }
}
