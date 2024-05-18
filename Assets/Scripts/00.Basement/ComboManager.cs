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
    public int comboValue = 0;
    public float comboValdueFever = 0f;
    public int comboMultiflier = 0;
    public Coroutine comboCoroutine = null;
    public Slider comboSlider;
    public TextMeshPro comboMultiflierTMP;
    public TMP_Text comboMultiflierTMPd;
    public TMP_Text comboValueTMP;
    
    public void Init()
    {
        comboCoroutine = StartCoroutine(ComboRoutine());
        InitComboUI();
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
            InitComboUI();
        }
        else Debug.Log("Coroutine is null");
    }
    public void ActionSucceed()
    {
        comboValue += 1;
    }
    public void ActionFailed()
    {
        comboValue = 0;
    }
    public void ActionMissed()
    {
        comboValue = 0;
    }

    public void SetComboMultiflier(float value)
    {
        /* comboMultiflier = 1f;
        if (value > 0.8f) comboMultiflier = 8f;
        else if (value > 0.6f) comboMultiflier = 4f;
        else if (value > 0.4f) comboMultiflier = 2f;
        else comboMultiflier = 1f;

        comboMultiflierTMPd.text = $"x {comboMultiflier}"; */
        comboValueTMP.text = $"Combo {comboValue}";
    }

    public void InitComboUI()
    {
        // comboValueFever = 0f;
        comboSlider.value = 0;
        comboMultiflier = 1;
        comboMultiflierTMPd.text = $"x {comboMultiflier}";
    }
    IEnumerator ComboRoutine()
    {
        float waitSecond = 0.5f;
        while (true)
        {
            comboMultiflier = comboValue / 10;
            if (comboMultiflier > 3) comboMultiflier = 3;
            if (comboMultiflier == 0)
            {
                comboMultiflierTMPd.text = $"x1"; 
                comboSlider.value = 1;
            }
            else
            {
                comboMultiflierTMPd.text = $"x{comboMultiflier + 1}";
                comboSlider.value = comboMultiflier + 1;
            }
            yield return new WaitForSeconds(waitSecond);
        }
    }

    private void Update()
    {
        // Debug.Log($"Routine : {comboCoroutine != null}");
    }
}
