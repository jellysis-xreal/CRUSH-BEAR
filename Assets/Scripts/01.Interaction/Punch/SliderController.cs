using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class SliderController : MonoBehaviour
{
    public Slider mySlider;
    public TextMeshProUGUI sliderText;

    private float maxSliderAmount = 10.0f;
    private float smoothSpeed = 10.0f;  // �����̴� ���� ����Ǵ� �ӵ�
    public float targetValue;  // ��ǥ �����̴� ��

    private float SliderWidth = 140f; // �����̴� width ��


    public Image sliderFillImage;  // �����̴��� ������ ������ Image ������Ʈ ����
    public float highestValue = 0f;  // �ִ� �ӵ� ����

    public RectTransform highest_RectTransform;
    public RectTransform slider_RectTransform;
    public GameObject highestPointMarker; // ȭ��ǥ ���� ������Ʈ
    void Start()
    {
        if (mySlider != null)
        {
            mySlider.minValue = 0;
            mySlider.maxValue = maxSliderAmount;
            mySlider.value = 0; // ���� 0

            UpdateSliderText(mySlider.value);
            sliderFillImage = mySlider.fillRect.GetComponent<Image>();  // �����̴��� Fill Image

            highest_RectTransform = highestPointMarker.GetComponent<RectTransform>();

            slider_RectTransform = mySlider.GetComponent<RectTransform>();
            SliderWidth = slider_RectTransform.rect.width;
        }
    }

    void Update()
    {
        if (mySlider.value < targetValue)
        {
            // �����̴� ���� ��ǥ ������ ���� ���, �ε巴�� ����
            mySlider.value = Mathf.MoveTowards(mySlider.value, targetValue, smoothSpeed * Time.deltaTime);
        }
        else
        {
            // ���� �ӵ��� ����� ��, �ּ� ���� �ӵ��� �����ϱ� ���� Mathf.Max �Լ� ���
            float minimumSpeedFactor = 0.3f;  // �ּ� ���� �ӵ��� 30%�� ����
            float decreaseSpeed = Mathf.Max(smoothSpeed * (mySlider.value / maxSliderAmount), smoothSpeed * minimumSpeedFactor);
            mySlider.value = Mathf.MoveTowards(mySlider.value, 0, decreaseSpeed * Time.deltaTime);


        }
        UpdateSliderText(mySlider.value);
        UpdateSliderColor(mySlider.value / maxSliderAmount);
        UpdateHighestValue(mySlider.value);
    }
    private void UpdateSliderText(float value)
    {
        if (sliderText != null)
        {
            float localValue = value;
            sliderText.text = localValue.ToString("0.00");
        }
    }
    private void UpdateSliderColor(float percentage)
    {
        if (percentage < 0.3f)
        {
            sliderFillImage.color = Color.yellow;  // 30% ������ �� ���
        }
        else if (percentage < 0.7f)
        {
            sliderFillImage.color = new Color(1f, 0.5f, 0f);  // 30%~70%�� �� ��Ȳ
        }
        else
        {
            sliderFillImage.color = Color.red;  // 70% �̻��� �� ����
        }
    }

    private void UpdateHighestValue(float currentValue) 
    {
        if (currentValue > highestValue)
        {
            highestValue = currentValue;
            UpdateHighestPointMarker();  // ȭ��ǥ ��ġ ������Ʈ
        }
    }

    private void UpdateHighestPointMarker()
    {
        if (highestPointMarker != null && mySlider != null)
        {
            float normalizedHighestValue = highestValue / maxSliderAmount;
            float markerPositionX = normalizedHighestValue * SliderWidth;
            Vector3 markerPosition = new Vector3(markerPositionX, 0,0);
            highest_RectTransform.anchoredPosition3D = markerPosition;
        }
    }

    public void ResetHighest() // ���� ���� �� �ʱ�ȭ
    {
        highestValue = 0f;
        highest_RectTransform.anchoredPosition3D = new Vector3(0,0,0);
    }


    public void SetPunchSliderSpeed(float speed)
    {
        targetValue = Mathf.Clamp(speed, 0, mySlider.maxValue);  // �ִ밪 ���� ���� ����
        mySlider.value = targetValue;
        //Debug.Log("[Slider] SetPunchSliderSpeed �ӵ� " + targetValue);
    }
    //private void ResetDecayRate()
    //{
    //    timeSinceLastReset = 0; // �ð� ����
    //}
}
