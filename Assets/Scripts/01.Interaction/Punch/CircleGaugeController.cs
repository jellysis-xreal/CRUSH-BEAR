using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CircleGaugeController : MonoBehaviour
{
    public RectTransform gaugeRectTransform; // RectTransform of the gauge image
    public TextMeshProUGUI sliderText;

    private float fixedWidth = 6.0f; // 고정된 Width 값
    public float maxHeight = 30.0f;  // 최대 Height 값
    private float minHeight = 0.0f;  // 최소 Height 값
    private float smoothSpeed = 10.0f;   // 크기 변경 속도
    public float targetValue;            // 목표 크기 값
    private bool isTargetSet = false;    // 목표 값이 설정되었는지 여부

    void Start()
    {
        InitSettings();
    }

    public void InitSettings()
    {
        if (gaugeRectTransform != null)
        {
            gaugeRectTransform.sizeDelta = new Vector2(fixedWidth, minHeight); // 시작 크기 설정
        }
        if (sliderText != null)
            sliderText.text = 0.ToString();
    }

    void Update()
    {
        float currentHeight = gaugeRectTransform.sizeDelta.y; // 현재 Height 값을 사용

        if (isTargetSet)
        {
            // 목표 값에 거의 즉시 도달
            currentHeight = targetValue;
            isTargetSet = false;
        }
        else
        {
            // 항상 일정하게 크기 감소
            currentHeight = Mathf.MoveTowards(currentHeight, minHeight, smoothSpeed * Time.deltaTime);
        }

        // 크기 값 업데이트 (Width는 고정, Height는 조정)
        gaugeRectTransform.sizeDelta = new Vector2(fixedWidth, currentHeight);
        UpdateSliderText(currentHeight);
    }

    private void UpdateSliderText(float value)
    {
        if (sliderText != null)
        {
            sliderText.text = value.ToString("F1");
        }
    }

    public void SetGaugeHeight(float height)
    {
        targetValue = Mathf.Clamp(height, minHeight, maxHeight);  // 크기 범위 내로 제한
        isTargetSet = true; // 목표 크기 값 설정
        UpdateSliderText(height); // 텍스트에 height 값을 업데이트
    }

    public void Scaling(Vector3 newScale)
    {
        transform.localScale = newScale;
    }
}
