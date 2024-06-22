using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
using UnityEngine.InputSystem;

public class HandData : MonoBehaviour
{
    public enum HandModelType {Left, Right}
    public HandModelType handModelType;
    public Transform root;
    public Animator animator;
    public Transform[] fingerBones;

    //[Header("setting value")] 
    private float SpeedValue = 1.5f; //최소한 움직여야 하는 속도
    private float GrabValue = 0.7f;
    
    [Header("setting(auto)")] 
    [SerializeField] private GameObject Controller;
    [SerializeField] private InputActionProperty ControllerInput;
    [SerializeField] private float maxSpeed = 0.0f;
    [SerializeField] private float perfectThresholdSpeed = 0.0f;
    [SerializeField] private float goodThresholdSpeed = 0.0f;
    
    [Header("playing value")] 
    public InteractionType ControllerType;
    public float ControllerSpeed;
    public Vector3 ControllerVector;
    
    private Vector3 beforePos;
    private Vector3 currentPos;

    private List<float> controllerSpeedQueue = new List<float>();
    private float settingTime = 5.0f;
    private float _currentSettingTime = 0.0f;
    private bool isSet = false;
    
    
    private void Awake()
    {
        Controller = transform.parent.gameObject;
        ControllerInput = GetComponent<AnimateHandOnInput>().grabAnimationAction;
    }
    private void Update()
    {
        if (_currentSettingTime < settingTime)
        { 
            //Debug.Log($"[JMH][HandData] SettingTime: {_currentSettingTime}");
            QueueControllerSpeed();
            _currentSettingTime += Time.deltaTime;
        }
        else
        {
            if (!isSet)
            {
                SetControllerMaxSpeed();
                isSet = true;
            }
        }

        updateControllerSpeed();
    }

    private void updateControllerSpeed()
    {
        currentPos = Controller.transform.position;
        ControllerSpeed = (beforePos - currentPos).magnitude / Time.deltaTime;
        ControllerVector = (beforePos - currentPos).normalized;
        beforePos = currentPos;
    }
    
    public bool IsMoveQuickly()
    {
        return ControllerSpeed > SpeedValue;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    public float GetPerfectThreshold()
    {
        return perfectThresholdSpeed;
    }
    
    public float GetGoodThreshold()
    {
        return goodThresholdSpeed;
    }
    
    private void QueueControllerSpeed()
    {
        currentPos = Controller.transform.position;
        ControllerSpeed = (beforePos - currentPos).magnitude / Time.deltaTime;
        
        // Add the current speed to the queue
        controllerSpeedQueue.Add(ControllerSpeed);
        
        beforePos = currentPos;
    }
    
    private void SetControllerMaxSpeed()
    {
        // Sort the list
        controllerSpeedQueue.Reverse();
        
        // 상위 10개 값의 평균으로 구합니다.
        for (int i = 0; i < 10; i++)
        {
            maxSpeed += controllerSpeedQueue[i];
        }
        maxSpeed /= 10;
        
        // Player가 제대로 흔들지 못했을 경우, 진행을 위한 속도 설정
        if (maxSpeed < 1.0f)
        {
            maxSpeed = 3.2f;
        }

        perfectThresholdSpeed = maxSpeed * 0.6f;
        goodThresholdSpeed = maxSpeed * 0.2f;
            
        Debug.Log($"[JMH][HandData] Set Max Speed: {maxSpeed:F5}");
    }
    
}