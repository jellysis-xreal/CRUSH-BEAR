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
    public float comboValueFever = 0f;
    public float comboMultiflier = 0f;
    public Coroutine comboCoroutine = null;
    public Slider comboSlider;
    public TextMeshPro comboMultiflierTMP;
    public TMP_Text comboMultiflierTMPd;
    public TMP_Text comboValueTMP;
    
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
        comboValueFever += 0.05f;
        comboValue += 1;
        
        if(comboValueFever > 1f) comboValueFever = 1f;
    }
    public void ActionFailed()
    {
        comboValueFever -= 0.05f;
        comboValue = 0;
        if(comboValueFever < 0f) comboValueFever = 0f;
    }
    public void ActionMissed()
    {
        comboValueFever -= 0.05f;
        comboValue = 0;
        if(comboValueFever < 0f) comboValueFever = 0f;
    }

    public void SetComboMultiflier(float value)
    {
        /* comboMultiflier = 1f;
        if (value > 0.8f) comboMultiflier = 8f;
        else if (value > 0.6f) comboMultiflier = 4f;
        else if (value > 0.4f) comboMultiflier = 2f;
        else comboMultiflier = 1f;

        comboMultiflierTMPd.text = $"x {comboMultiflier}"; */
        //comboValueTMP.text = $"Combo {comboValue}";
        comboValueTMP.text = comboValue.ToString();
    }

    public void InitComboUI()
    {
        // comboValueFever = 0f;
        // comboSlider.value = comboValueFever;
        comboMultiflier = 1f;
        comboMultiflierTMPd.text = $"x {comboMultiflier}";
    }
    IEnumerator ComboRoutine()
    {
        float waitSecond = 0.1f;
        while (true)
        {
            // comboValueFever -= Time.fixedDeltaTime * 0.5f;
            // if(comboValueFever < 0f) comboValueFever = 0f;
            // comboSlider.value = comboValueFever;
            SetComboMultiflier(comboValueFever);
            // Debug.Log($"update combo {comboValueFever} deltaTime {Time.deltaTime}");

            yield return new WaitForSeconds(waitSecond);
        }
    }

    private void Update()
    {
        // Debug.Log($"Routine : {comboCoroutine != null}");
    }
}
