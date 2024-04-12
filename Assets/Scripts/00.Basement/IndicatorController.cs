using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EnumTypes;
using UnityEngine;

public class IndicatorController : MonoBehaviour
{
    [Header("----+ Time Setting +----")] 
    public float waveIndicatorDuration = 3.0f;
    public float strength = 0.3f;
    
    [Header("----+ Punch +----")] 
    public GameObject punchIndicator;
    
    [Space(10f)]
    
    [Header("----+ Hit +----")]
    public GameObject hitIndicator;
    private Vector3 _closeRotation = new Vector3(0, 360f, 0);
    private Vector3 _openRotation = new Vector3(0, 240f, 0);


    public void SetWaveIndicator(uint currenWaveNum, WaveType beforeWave, WaveType currentWave)
    {
        Debug.Log("[JMH] SetWaveIndicator");

        ShowStartWave(currentWave);

        if (currenWaveNum > 1)
            ShowFinishWave(beforeWave);
    }

    private void ShowStartWave(WaveType wave)
    {
        switch (wave)
        {
            case WaveType.Punching:
                punchIndicator.transform.DOShakePosition(waveIndicatorDuration, strength).SetUpdate(true);
                break;
            case WaveType.Hitting:
                hitIndicator.transform.DOShakePosition(waveIndicatorDuration, strength).SetUpdate(true);;
                StartCoroutine(RotateHitIndicator(_closeRotation, _openRotation));
                break;
        }
    }
    
    private void ShowFinishWave(WaveType wave)
    {
        switch (wave)
        {
            case WaveType.Punching:
                //punchIndicator.SetActive(true);
                break;
            case WaveType.Hitting:
                StartCoroutine(RotateHitIndicator(_openRotation, _closeRotation));
                break;
        }
    }
    
    private IEnumerator RotateHitIndicator(Vector3 startRotation, Vector3 endRotation)
    {
        float duration = waveIndicatorDuration; // 이 값은 원하는 대로 조정하실 수 있습니다.
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // 실제 시간을 사용합니다.
            float t = elapsed / duration;

            // Lerp 함수를 사용하여 startRotation에서 endRotation으로 점진적으로 회전시킵니다.
            hitIndicator.transform.localRotation = Quaternion.Euler(Vector3.Lerp(startRotation, endRotation, t));

            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime); // 실제 시간을 기준으로 대기합니다.
        }
    }
}
