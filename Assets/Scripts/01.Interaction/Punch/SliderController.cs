using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class SliderController : MonoBehaviour
{
    public Slider mySlider;
    public TextMeshProUGUI sliderText;

    private float maxSliderAmount = 7.5f; // 최대 값 10*3/4로 수정
    private float smoothSpeed = 10.0f;  // 슬라이더 값이 변경되는 속도
    public float targetValue;  // 목표 슬라이더 값

    private float SliderWidth = 140f; // 슬라이더 width 값

    public Image sliderFillImage;  // 슬라이더의 색상을 변경할 Image 컴포넌트 참조
    public float highestValue = 0f;  // 최대 속도 변수

    public RectTransform highest_RectTransform;
    public RectTransform slider_RectTransform;
    public RectTransform currentValue_RectTransform;
    public GameObject highestPointMarker; // 최대값 화살표
    public GameObject currentValueMarker; // 현재값 화살표

    [Header("Slider Colors")]
    [SerializeField] private Color lowColor = Color.yellow; // 30% 이하 색상
    [SerializeField] private Color midColor = new Color(1f, 0.5f, 0f); // 30%-70% 색상
    [SerializeField] private Color highColor = Color.red; // 70% 이상 색상

    void Start()
    {
        if (mySlider != null)
        {
            mySlider.minValue = 0;
            mySlider.maxValue = maxSliderAmount;
            mySlider.value = 0; // 시작 0

            UpdateSliderText(mySlider.value);
            sliderFillImage = mySlider.fillRect.GetComponent<Image>();  // 슬라이더의 Fill Image

            //highest_RectTransform = highestPointMarker.GetComponent<RectTransform>();
            currentValue_RectTransform = currentValueMarker.GetComponent<RectTransform>(); // 현재 값 화살표 초기화

            slider_RectTransform = mySlider.GetComponent<RectTransform>();
            SliderWidth = slider_RectTransform.rect.width;
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
        UpdateSliderColor(mySlider.value / maxSliderAmount);
        //UpdateHighestValue(mySlider.value);// 최대값 표시 안하기로 함
        UpdateCurrentValueMarker();
    }
    private void UpdateSliderText(float value)
    {
        if (sliderText != null)
        {
            sliderText.text = value.ToString("0.0");
        }
    }
    private void UpdateSliderColor(float percentage)
    {
        if (percentage < 0.3f)
        {
            sliderFillImage.color = lowColor;  // 30% 이하
        }
        else if (percentage < 0.7f)
        {
            sliderFillImage.color = midColor;  // 30%~70%
        }
        else
        {
            sliderFillImage.color = highColor;  // 70% 이상
        }
    }

    private void UpdateHighestValue(float currentValue)
    {
        if (currentValue > highestValue)
        {
            highestValue = currentValue;
            UpdateHighestPointMarker();  // 화살표 위치 업데이트
        }
    }

    private void UpdateHighestPointMarker()
    {
        if (highestPointMarker != null && mySlider != null)
        {
            float normalizedHighestValue = highestValue / maxSliderAmount;
            float markerPositionX = normalizedHighestValue * SliderWidth;
            Vector3 markerPosition = new Vector3(markerPositionX, 0, 0);
            highest_RectTransform.anchoredPosition3D = markerPosition;
        }
    }
    private void UpdateCurrentValueMarker()
    {
        if (currentValueMarker != null && mySlider != null)
        {
            float normalizedCurrentValue = mySlider.value / maxSliderAmount;
            float markerPositionX = normalizedCurrentValue * SliderWidth;
            Vector3 markerPosition = new Vector3(markerPositionX, 0, 0);
            currentValue_RectTransform.anchoredPosition3D = markerPosition;
        }
    }

    public void ResetHighest() // 게임 시작 시 초기화
    {
        highestValue = 0f;
        highest_RectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
    }


    public void SetPunchSliderSpeed(float speed)
    {
        targetValue = Mathf.Clamp(speed, 0, mySlider.maxValue);  // 최대값 범위 내로 제한
        mySlider.value = targetValue;
        //Debug.Log("[Slider] SetPunchSliderSpeed 속도 " + targetValue);
    }
    //private void ResetDecayRate()
    //{
    //    timeSinceLastReset = 0; // 시간 리셋
    //}
}
