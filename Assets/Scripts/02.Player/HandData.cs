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
    //[SerializeField] private AttachHandNoGrab AttachNoGrab;
    
    [Header("playing value")] 
    public InteractionType ControllerType;
    public float ControllerSpeed;
    public Vector3 ControllerVector;
    
    private Vector3 beforePos;

    private void Awake()
    {
        Controller = transform.parent.gameObject;
        ControllerInput = GetComponent<AnimateHandOnInput>().grabAnimationAction;
    }
    private void Update()
    {
        updateControllerSpeed();
        //updateControllerState();
    }

    private void updateControllerSpeed()
    {
        Vector3 currentPos = Controller.transform.position;
        ControllerSpeed = (beforePos - currentPos).magnitude / Time.deltaTime;
        ControllerVector = (beforePos - currentPos).normalized;
        beforePos = currentPos;
    }
    
    // private void updateControllerState()
    // {
    //     float grabValue = ControllerInput.action.ReadValue<float>();
    //     
    //     // 손이 normal이라면 Idle state
    //     if (grabValue < GrabValue)
    //         ControllerType = InteractionType.Idle;
    //
    //     //양 손을 펼치고 있고 GrabAttached일 경우 Tear state
    //     if (grabValue < GrabValue && AttachNoGrab.IsAttached)
    //     {
    //         ControllerType = InteractionType.Tear;
    //     }
    //
    //     // 양 손이 Grab이라면 Break state
    //     if (grabValue > GrabValue)
    //         ControllerType = InteractionType.Break;
    // }
    public bool IsMoveQuickly()
    {
        return ControllerSpeed > SpeedValue;
    }
    
}