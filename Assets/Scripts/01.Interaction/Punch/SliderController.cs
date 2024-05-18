using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class SliderController : MonoBehaviour
{
    public Slider mySlider;
    public TextMeshProUGUI sliderText;

    private float maxSliderAmount = 7.5f; // �ִ� �� 10*3/4�� ����
    private float smoothSpeed = 10.0f;  // �����̴� ���� ����Ǵ� �ӵ�
    public float targetValue;  // ��ǥ �����̴� ��

    private float SliderWidth = 140f; // �����̴� width ��

    public Image sliderFillImage;  // �����̴��� ������ ������ Image ������Ʈ ����
    public float highestValue = 0f;  // �ִ� �ӵ� ����

    public RectTransform highest_RectTransform;
    public RectTransform slider_RectTransform;
    public RectTransform currentValue_RectTransform;
    public GameObject highestPointMarker; // �ִ밪 ȭ��ǥ
    public GameObject currentValueMarker; // ���簪 ȭ��ǥ

    [Header("Slider Colors")]
    [SerializeField] private Color lowColor = Color.yellow; // 30% ���� ����
    [SerializeField] private Color midColor = new Color(1f, 0.5f, 0f); // 30%-70% ����
    [SerializeField] private Color highColor = Color.red; // 70% �̻� ����

    void Start()
    {
        if (mySlider != null)
        {
            mySlider.minValue = 0;
            mySlider.maxValue = maxSliderAmount;
            mySlider.value = 0; // ���� 0

            UpdateSliderText(mySlider.value);
            sliderFillImage = mySlider.fillRect.GetComponent<Image>();  // �����̴��� Fill Image

            //highest_RectTransform = highestPointMarker.GetComponent<RectTransform>();
            currentValue_RectTransform = currentValueMarker.GetComponent<RectTransform>(); // ���� �� ȭ��ǥ �ʱ�ȭ

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
        //UpdateHighestValue(mySlider.value);// �ִ밪 ǥ�� ���ϱ�� ��
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
            sliderFillImage.color = lowColor;  // 30% ����
        }
        else if (percentage < 0.7f)
        {
            sliderFillImage.color = midColor;  // 30%~70%
        }
        else
        {
            sliderFillImage.color = highColor;  // 70% �̻�
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

    public void ResetHighest() // ���� ���� �� �ʱ�ȭ
    {
        highestValue = 0f;
        highest_RectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
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
