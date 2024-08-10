using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CircleGaugeController : MonoBehaviour
{
    public RectTransform gaugeRectTransform; // RectTransform of the gauge image
    public TextMeshProUGUI sliderText;

    private float fixedWidth = 6.0f; // ������ Width ��
    public float maxHeight = 30.0f;  // �ִ� Height ��
    private float minHeight = 0.0f;  // �ּ� Height ��
    private float smoothSpeed = 10.0f;   // ũ�� ���� �ӵ�
    public float targetValue;            // ��ǥ ũ�� ��
    private bool isTargetSet = false;    // ��ǥ ���� �����Ǿ����� ����

    void Start()
    {
        InitSettings();
    }

    public void InitSettings()
    {
        if (gaugeRectTransform != null)
        {
            gaugeRectTransform.sizeDelta = new Vector2(fixedWidth, minHeight); // ���� ũ�� ����
        }
        if (sliderText != null)
            sliderText.text = 0.ToString();
    }

    void Update()
    {
        float currentHeight = gaugeRectTransform.sizeDelta.y; // ���� Height ���� ���

        if (isTargetSet)
        {
            // ��ǥ ���� ���� ��� ����
            currentHeight = targetValue;
            isTargetSet = false;
        }
        else
        {
            // �׻� �����ϰ� ũ�� ����
            currentHeight = Mathf.MoveTowards(currentHeight, minHeight, smoothSpeed * Time.deltaTime);
        }

        // ũ�� �� ������Ʈ (Width�� ����, Height�� ����)
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
        targetValue = Mathf.Clamp(height, minHeight, maxHeight);  // ũ�� ���� ���� ����
        isTargetSet = true; // ��ǥ ũ�� �� ����
        UpdateSliderText(height); // �ؽ�Ʈ�� height ���� ������Ʈ
    }

    public void Scaling(Vector3 newScale)
    {
        transform.localScale = newScale;
    }
}
