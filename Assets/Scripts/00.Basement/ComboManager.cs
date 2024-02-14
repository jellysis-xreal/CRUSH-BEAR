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
    public float comboValue = 0f;
    public float comboMultiflier = 0f;
    public Coroutine comboCoroutine = null;
    public Slider comboSlider;
    public TextMeshPro comboMultiflierTMP;
    public TMP_Text comboMultiflierTMPd;
    
    public void GameStart()
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
        comboValue += 0.05f;
        if(comboValue > 1f) comboValue = 1f;
    }
    public void ActionFailed()
    {
        comboValue -= 0.05f;
        if(comboValue < 0f) comboValue = 0f;
    }
    public void ActionMissed()
    {
        comboValue -= 0.05f;
        if(comboValue < 0f) comboValue = 0f;
    }

    public void SetComboMultiflier(float value)
    {
        comboMultiflier = 1f;
        if (value > 0.8f) comboMultiflier = 8f;
        else if (value > 0.6f) comboMultiflier = 4f;
        else if (value > 0.4f) comboMultiflier = 2f;
        else comboMultiflier = 1f;

        comboMultiflierTMPd.text = $"x {comboMultiflier}";
    }

    public void InitComboUI()
    {
        comboValue = 0f;
        comboSlider.value = comboValue;
        comboMultiflier = 1f;
        comboMultiflierTMPd.text = $"x {comboMultiflier}";
    }
    IEnumerator ComboRoutine()
    {
        float waitSecond = 0.1f;
        while (true)
        {
            comboValue -= Time.fixedDeltaTime * 0.5f;
            if(comboValue < 0f) comboValue = 0f;
            comboSlider.value = comboValue;
            SetComboMultiflier(comboValue);
            Debug.Log($"update combo {comboValue} deltaTime {Time.deltaTime}");

            yield return new WaitForSeconds(waitSecond);
        }
    }

    private void Update()
    {
        // Debug.Log($"Routine : {comboCoroutine != null}");
    }
}
