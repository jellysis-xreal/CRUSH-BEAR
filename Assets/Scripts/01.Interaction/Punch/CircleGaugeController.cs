using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CircleGaugeController : MonoBehaviour
{
    public RectTransform circleRectTransform;
    //public Transform circleTransform; // Circle Sprite�� Transform
    public TextToImage sliderText;
    //public TextToImage sliderText;// TextToImage ������Ʈ�� ���� ����

    public float maxScaleAmount = 14.0f; // �ִ� Width/Height
    private float minScaleAmount = 0.0f; // �ּ� Width/Height
    //private float maxScaleAmount = 7.0f; // �ִ� ������
    //private float minScaleAmount = 1.0f; // �ּ� ������
    private float smoothSpeed = 10.0f;   // ������ ���� �ӵ�
    public float targetValue;            // ��ǥ ������ ��
    private bool isTargetSet = false;    // ��ǥ ������ ���� �����Ǿ����� ����

    void Start()
    {
        InitSettings();
    }
    public void InitSettings()
    {
        if (circleRectTransform != null)
        {
            circleRectTransform.sizeDelta = new Vector2(minScaleAmount, minScaleAmount); // ���� ũ�� ����
            //circleTransform.localScale = new Vector3(minScaleAmount, minScaleAmount, 1f); // ���� ������ ����
            //UpdateSliderText(minScaleAmount);
        }
        if (sliderText != null)
            sliderText.ChangeTextToImage(0f);
    }

    void Update()
    {
        float currentValue = circleRectTransform.sizeDelta.x; // Width ���� ���
        //Vector3 currentScale = circleTransform.localScale;
        //float currentValue = currentScale.x; // x�� �������� ���

        if (isTargetSet)
        {
            // ��ǥ ���� ���� ��� ����
            currentValue = targetValue;
            isTargetSet = false;
            //if (currentValue < targetValue)
            //{
            //    // ���� �������� ��ǥ ������ ���� ���, �ε巴�� ����
            //    currentValue = Mathf.MoveTowards(currentValue, targetValue, smoothSpeed * Time.deltaTime);
            //}
            //else
            //{
            //    // ��ǥ ���� �����ϸ� ��ǥ �� ������ ����
            //    isTargetSet = false;
            //}
        }
        else
        {
            // �׻� �����ϰ� ������ ����
            currentValue = Mathf.MoveTowards(currentValue, minScaleAmount, smoothSpeed * Time.deltaTime);
        }

        // ������ �� ������Ʈ
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
        targetValue = Mathf.Clamp(speed, minScaleAmount, maxScaleAmount);  // ������ ���� ���� ����
        isTargetSet = true; // ��ǥ ������ �� ����
        UpdateSliderText(speed); // �ؽ�Ʈ�� speed ���� ������Ʈ
    }

    public void Scaling(Vector3 newScale)
    {
        transform.localScale = newScale;
    }
}
