using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour
{
    // 플레이어의 인터랙션 결과에 따라 콤보 시스템을 업데이트하는 스크립트 
    public int comboValue = 0; // 현재 콤보
    public int comboPercent = 0;
    public float comboValueFever = 0f;
    public int comboMultiflier = 1;
    public Coroutine comboCoroutine = null;
    public Slider comboSliderPunch;
    public Slider comboSliderHitting;
    public Transform[] comboMultiflierTransform;
    private TextMeshProUGUI comboMultiflierPunch, comboMultiflierHitting;
    private uint maxCombo = 0;
    
    public void Init()
    {
        InitComboUI();
        ComboRoutine().Forget();
    }
    public void GamePause()
    {
        if (comboCoroutine != null)
        {
            StopCoroutine(comboCoroutine);
        }
        else Debug.Log("Coroutine is null");
    }
    public void GameStop()
    {
        if (comboCoroutine != null)
        {
            StopCoroutine(comboCoroutine);
            InitComboUI(); // 초기화
        }
        else Debug.Log("Coroutine is null");
    }
    public void ActionSucceed()
    {
        comboValue += 1;
    }
    public void ActionFailed()
    {
        if (maxCombo < comboValue)
        {
            maxCombo = (uint)comboValue;
        }
        comboValue = 0;
    }
    public void ActionMissed()
    {
        comboValue = 0;
    }
    public void InitComboUI()
    {
        comboValueFever = 0f;
        comboValue = 0;
        maxCombo = 0;
        comboPercent = 0;

        //comboSlider.value = comboValueFever;
        comboMultiflierPunch = comboMultiflierTransform[0].GetComponent<TextMeshProUGUI>();
        comboMultiflierHitting = comboMultiflierTransform[1].GetComponent<TextMeshProUGUI>();
        comboMultiflier = 1;
        comboMultiflierPunch.text = "x" + $"{comboMultiflier}";
        comboMultiflierHitting.text = "x" + $"{comboMultiflier}";
    }
    async UniTask ComboRoutine()
    {
        //Debug.Log("Combo Routine " + comboMultiflier);

        float waitSecond = 0.5f;
        while (true)
        {
            comboMultiflier = (comboValue / 10) + 1;

            if (comboMultiflier > 4)
            {
                comboMultiflier = 4;
                comboPercent = 9;
            }
            else
            {
                comboPercent = comboValue % 10;
            }

            comboMultiflierPunch.text = "x" + $"{comboMultiflier}";
            comboMultiflierHitting.text = "x" + $"{comboMultiflier}";

            comboSliderPunch.value = comboPercent;
            comboSliderHitting.value = comboPercent;

            await UniTask.WaitForSeconds(waitSecond);
        }
    }
    public uint GetMaxCombo()
    {
        return maxCombo;
    }

    private void Update()
    {
        // Debug.Log($"Routine : {comboCoroutine != null}");
    }
}
