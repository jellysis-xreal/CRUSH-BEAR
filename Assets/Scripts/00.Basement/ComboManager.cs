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
    public int comboMultiflier = 0;
    public Coroutine comboCoroutine = null;
    public Slider comboSliderPunch;
    public Slider comboSliderHitting;
    public Transform[] comboMultiflierTransform;
    private ComboText comboMultiflierPunch, comboMultiflierHitting;

    
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
    public void InitComboUI()
    {
        // comboValueFever = 0f;
        // comboSlider.value = comboValueFever;
        comboMultiflierPunch = comboMultiflierTransform[0].GetComponent<ComboText>();
        comboMultiflierHitting = comboMultiflierTransform[1].GetComponent<ComboText>();
        comboMultiflier = 1;
        comboMultiflierPunch.ChangeTextToImage(comboMultiflier);
        comboMultiflierHitting.ChangeTextToImage(comboMultiflier);
    }
    IEnumerator ComboRoutine()
    {
        Debug.Log("Combo Routine " + comboMultiflier);

        float waitSecond = 0.5f;
        while (true)
        {
            comboMultiflier = comboValue / 10;
            if (comboMultiflier > 3) comboMultiflier = 3;
            if (comboMultiflier == 0)
            {
                comboMultiflierPunch.ChangeTextToImage(comboMultiflier + 1);
                comboMultiflierHitting.ChangeTextToImage(comboMultiflier + 1);
                comboSliderHitting.value = 1;
            }
            else
            {
                comboMultiflierPunch.ChangeTextToImage(comboMultiflier + 1);
                comboMultiflierHitting.ChangeTextToImage(comboMultiflier + 1);
                comboSliderPunch.value = comboMultiflier + 1;
                comboSliderHitting.value = comboMultiflier + 1;
            }
            yield return new WaitForSeconds(waitSecond);
        }
    }

    private void Update()
    {
        // Debug.Log($"Routine : {comboCoroutine != null}");
    }
}
