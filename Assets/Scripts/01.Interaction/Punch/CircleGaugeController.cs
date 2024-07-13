using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CircleGaugeController : MonoBehaviour
{
    public RectTransform circleRectTransform;
    //public Transform circleTransform; // Circle Sprite의 Transform
    public TextToImage sliderText;
    //public TextToImage sliderText;// TextToImage 컴포넌트에 대한 참조

    public float maxScaleAmount = 14.0f; // 최대 Width/Height
    private float minScaleAmount = 0.0f; // 최소 Width/Height
    //private float maxScaleAmount = 7.0f; // 최대 스케일
    //private float minScaleAmount = 1.0f; // 최소 스케일
    private float smoothSpeed = 10.0f;   // 스케일 변경 속도
    public float targetValue;            // 목표 스케일 값
    private bool isTargetSet = false;    // 목표 스케일 값이 설정되었는지 여부

    void Start()
    {
        InitSettings();
    }
    public void InitSettings()
    {
        if (circleRectTransform != null)
        {
            circleRectTransform.sizeDelta = new Vector2(minScaleAmount, minScaleAmount); // 시작 크기 설정
            //circleTransform.localScale = new Vector3(minScaleAmount, minScaleAmount, 1f); // 시작 스케일 설정
            //UpdateSliderText(minScaleAmount);
        }
        if (sliderText != null)
            sliderText.ChangeTextToImage(0f);
    }

    void Update()
    {
        float currentValue = circleRectTransform.sizeDelta.x; // Width 값을 사용
        //Vector3 currentScale = circleTransform.localScale;
        //float currentValue = currentScale.x; // x축 스케일을 사용

        if (isTargetSet)
        {
            // 목표 값에 거의 즉시 도달
            currentValue = targetValue;
            isTargetSet = false;
            //if (currentValue < targetValue)
            //{
            //    // 현재 스케일이 목표 값보다 작을 경우, 부드럽게 증가
            //    currentValue = Mathf.MoveTowards(currentValue, targetValue, smoothSpeed * Time.deltaTime);
            //}
            //else
            //{
            //    // 목표 값에 도달하면 목표 값 설정을 해제
            //    isTargetSet = false;
            //}
        }
        else
        {
            // 항상 일정하게 스케일 감소
            currentValue = Mathf.MoveTowards(currentValue, minScaleAmount, smoothSpeed * Time.deltaTime);
        }

        // 스케일 값 업데이트
        //circleTransform.localScale = new Vector3(currentValue, currentValue, 1f);
        circleRectTransform.sizeDelta = new Vector2(currentValue, currentValue);
        //UpdateSliderText(currentValue);
    }

    private void UpdateSliderText(float value)
    {
        if (sliderText != null)
        {
            sliderText.ChangeTextToImage(value);
            //sliderText.ChangeTextToImage(value);
        }
    }

    public void SetPunchSliderSpeed(float speed)
    {
        targetValue = Mathf.Clamp(speed, minScaleAmount, maxScaleAmount);  // 스케일 범위 내로 제한
        isTargetSet = true; // 목표 스케일 값 설정
        UpdateSliderText(speed); // 텍스트에 speed 값을 업데이트
    }

    public void Scaling(Vector3 newScale)
    {
        transform.localScale = newScale;
    }
}
