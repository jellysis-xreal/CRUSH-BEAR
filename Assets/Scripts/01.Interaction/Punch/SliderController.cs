using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider mySlider;
    public TextMeshProUGUI sliderText;

    private float maxSliderAmount = 10.0f;
    private float smoothSpeed = 10.0f;  // �����̴� ���� ����Ǵ� �ӵ�
    public float targetValue;  // ��ǥ �����̴� ��
    //private float initialDecreasePerSecond = 2.0f; // �ʴ� ������ ��
    //private float decreaseAcceleration = 0.09f; // ���� �ӵ� ���ӵ�
    //private float baseDecayRate = 3.0f; // �ʴ� �⺻ ������
    //private float currentDecayRate; // ���� ������
    //private float timeSinceLastReset = 0; // ���� ���� ��� �ð�

    private float elapsedTime = 0f; // ��� �ð� ����

    void Start()
    {
        if (mySlider != null)
        {
            mySlider.minValue = 0;
            mySlider.maxValue = maxSliderAmount;
            mySlider.value = 0; // ���� 0
            //currentDecayRate = baseDecayRate; // �ʱ� ������ ����
            UpdateSliderText(mySlider.value);
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
        targetValue = Mathf.Clamp(speed, 0, mySlider.maxValue);  // �ִ밪 ���� ���� ����
        mySlider.value = targetValue;
        Debug.Log("[Slider] SetPunchSliderSpeed �ӵ� " + targetValue);
    }
    //private void ResetDecayRate()
    //{
    //    timeSinceLastReset = 0; // �ð� ����
    //}
}
