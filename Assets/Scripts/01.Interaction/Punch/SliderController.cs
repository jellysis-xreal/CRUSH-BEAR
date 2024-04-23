using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider mySlider;
    public TextMeshProUGUI sliderText;

    private float maxSliderAmount = 10.0f;
    private float smoothSpeed = 10.0f;  // 슬라이더 값이 변경되는 속도
    public float targetValue;  // 목표 슬라이더 값
    //private float initialDecreasePerSecond = 2.0f; // 초당 감소할 값
    //private float decreaseAcceleration = 0.09f; // 감소 속도 가속도
    //private float baseDecayRate = 3.0f; // 초당 기본 감소율
    //private float currentDecayRate; // 현재 감소율
    //private float timeSinceLastReset = 0; // 리셋 이후 경과 시간

    private float elapsedTime = 0f; // 경과 시간 추적

    void Start()
    {
        if (mySlider != null)
        {
            mySlider.minValue = 0;
            mySlider.maxValue = maxSliderAmount;
            mySlider.value = 0; // 시작 0
            //currentDecayRate = baseDecayRate; // 초기 감소율 설정
            UpdateSliderText(mySlider.value);
        }
    }

    void Update()
    {
        if (mySlider.value < targetValue)
        {
            // 슬라이더 값이 목표 값보다 작을 경우, 부드럽게 증가
            mySlider.value = Mathf.MoveTowards(mySlider.value, targetValue, smoothSpeed * Time.deltaTime);
        }
        else
        {
            // 감소 속도를 계산할 때, 최소 감소 속도를 보장하기 위해 Mathf.Max 함수 사용
            float minimumSpeedFactor = 0.3f;  // 최소 감소 속도를 30%로 유지
            float decreaseSpeed = Mathf.Max(smoothSpeed * (mySlider.value / maxSliderAmount), smoothSpeed * minimumSpeedFactor);
            mySlider.value = Mathf.MoveTowards(mySlider.value, 0, decreaseSpeed * Time.deltaTime);


        }
        UpdateSliderText(mySlider.value);
    }
    private void UpdateSliderText(float value)
    {
        if (sliderText != null)
        {
            float localValue = value;
            sliderText.text = localValue.ToString("0.00");
        }
    }
    public void SetPunchSliderSpeed(float speed)
    {
        targetValue = Mathf.Clamp(speed, 0, mySlider.maxValue);  // 최대값 범위 내로 제한
        mySlider.value = targetValue;
        Debug.Log("[Slider] SetPunchSliderSpeed 속도 " + targetValue);
    }
    //private void ResetDecayRate()
    //{
    //    timeSinceLastReset = 0; // 시간 리셋
    //}
}
