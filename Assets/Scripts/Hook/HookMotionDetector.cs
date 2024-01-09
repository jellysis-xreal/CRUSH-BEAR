using System;using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
// 컨트롤러의 위치와 회전을 추적하고 Hook 동작을 했는지 판단하는 코드
public class HookMotionDetector : MonoBehaviour
{
    public Controller controller;
    [SerializeField] private HandData handData;
    [SerializeField] private float handVelocityMinimumThreshold = 0.25f;
    [SerializeField] private float handVelocityMaximumThreshold = 5f;
    [SerializeField] private float handVelocity;
    private Coroutine _chekingHookCoroutine;
    public bool coroutineExist = false;
    public bool isHooking = false;
    public InputActionProperty activateAction;

    void Update()
    {
        CheckControllerSpeed();

        handVelocity = handData.ControllerSpeed;
        /*if (_chekingHookCoroutine != null)
        {
            coroutineExist = true;
        }
        else coroutineExist = false;*/
    }

    private void Init()
    {
        
    }

    private bool GetControllerActivateAction()
    {
        // hand의 grab버튼 활성화 확인
        return activateAction.action.IsInProgress();
    }

    // 현재 상황 : 속도 감지 O, Hooking motion 감지 X (주먹찌르기 동작에도 인식함.)
    private void CheckControllerSpeed()
    {
        if(_chekingHookCoroutine != null) return;
        Debug.Log($"handSpeed : {handData.ControllerSpeed}");
        // _chekingHook 코루틴 존재하지 않음, 컨트롤러 속도 > 임계값, grab 버튼 활성화된 경우 회전 감지 시작.
        if(handData.ControllerSpeed < handVelocityMinimumThreshold 
           || handData.ControllerSpeed > handVelocityMaximumThreshold 
           || !GetControllerActivateAction()) return;
        switch (controller)
        {   
            case Controller.leftController:
                _chekingHookCoroutine = StartCoroutine(IsLeftHook());
                Debug.Log("LHand Velocity Value reach a ceration threshold.");
                break;
            case Controller.rightController:
                _chekingHookCoroutine = StartCoroutine(IsRightHook());
                Debug.Log("RHand Velocity Value reach a ceration threshold.");
                break;
        }
    }
    // 코루틴으로 하자. 손의 속도가 일정 값 이상 올라갔을 때 hook 검사

    private IEnumerator IsLeftHook()
    {
        isHooking = false;
        float startAngleY = transform.localEulerAngles.y; 
        while (true)
        {
            // 조건 : 주먹을 쥔 상태, 주먹이 바라보는 방향(hand의 local rotation z방향)의 회전 
            // rotation y 증가하는 형태, 90도 증가 -> 훅
            float currentAngleY = transform.localEulerAngles.y;
            if(currentAngleY < startAngleY)
            {
                Debug.Log("Coroutine end");
                CoroutineEndEvent();
                break;
            }
            Debug.Log($"angleDifference per 1 Frame : {currentAngleY - startAngleY}");
            startAngleY = currentAngleY;
            isHooking = true;
            yield return null;
        }
    }
    private IEnumerator IsRightHook()
    {
        isHooking = false;
        float startAngleY = transform.localEulerAngles.y;
        while (true)
        {
            // 조건 : 주먹을 쥔 상태, 주먹이 바라보는 방향(hand의 local rotation z방향)의 회전 
            // rotation y 감소하는 형태, 90도 감소 -> 훅
            float currentAngleY = transform.localEulerAngles.y;
            if(currentAngleY > startAngleY)
            {
                Debug.Log("Coroutine end");
                CoroutineEndEvent();
                break;
            }
            Debug.Log($"angleDifference per 1 Frame : {currentAngleY - startAngleY}");
            startAngleY = currentAngleY;
            isHooking = true;
            yield return null;
        }
    }

    private void CoroutineEndEvent()
    {
        StopCoroutine(_chekingHookCoroutine);
        _chekingHookCoroutine = null;
        coroutineExist = false;
        isHooking = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        // 1. 컨트롤러의 움직임이 현재 Hook이냐
        // 2. 타이밍이 일치하냐
        // 3. 처음 Trigger된 포인트가 왼쪽(leftHook일 경우) 혹은 오른쪽(rightHook일 경우)
    }
}
public enum Controller
{
    leftController,
    rightController
}