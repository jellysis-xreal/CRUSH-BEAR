using System;
using System.Collections;
using System.Collections.Generic;
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
    public int comboMultiflier = 0;
    public Coroutine comboCoroutine = null;
    public Slider comboSliderPunch;
    public Slider comboSliderHitting;
    public TMP_Text comboMultiflierPunch;
    public TMP_Text comboMultiflierHitting;
    public TMP_Text comboValueTMPPunch;
    public TMP_Text comboValueTMPHitting;
    
    public void Init()
    {
        InitComboUI();
        comboCoroutine = StartCoroutine(ComboRoutine());
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
        comboValueTMPPunch.text = $"{comboValue}";
        comboValueTMPHitting.text = $"{comboValue}";
    }
    public void ActionFailed()
    {
        comboValue = 0;
        comboValueTMPPunch.text = $"{comboValue}";
        comboValueTMPHitting.text = $"{comboValue}";
    }
    public void ActionMissed()
    {
        comboValue = 0;
    }
    public void InitComboUI()
    {
        // comboValueFever = 0f;
        // comboSlider.value = comboValueFever;
        comboMultiflier = 1;
        comboMultiflierPunch.text = $"x {comboMultiflier}";
        comboMultiflierHitting.text = $"x {comboMultiflier}";
    }
    IEnumerator ComboRoutine()
    {
        Debug.Log("Combo Routine " + comboMultiflier);

        float waitSecond = 0.5f;
        while (true)
        {
            comboMultiflier = comboValue / 10;
            //comboPercent = comboValue % 10;
            if (comboMultiflier > 3)
            {
                comboMultiflier = 3;
                comboPercent = 9;
            }
            else
            {
                comboPercent = comboValue % 10;
            }

            if (comboMultiflier == 0)
            {
                comboMultiflierPunch.text = $"x1"; 
                comboMultiflierHitting.text = $"x1";
                //comboSliderPunch.value = 1;
                //comboSliderHitting.value = 1;
            }
            else
            {
                comboMultiflierPunch.text = $"x{comboMultiflier + 1}";
                comboMultiflierHitting.text = $"x{comboMultiflier + 1}";
                //comboSliderPunch.value = comboMultiflier + 1;
                //comboSliderHitting.value = comboMultiflier + 1;
            }
            comboSliderPunch.value = comboPercent;
            comboSliderHitting.value = comboPercent;

            yield return new WaitForSeconds(waitSecond);
        }
    }

    private void Update()
    {
        // Debug.Log($"Routine : {comboCoroutine != null}");
    }
}
