using System;using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
// 컨트롤러의 위치와 회전을 추적하고 Hook 동작을 했는지 판단하는 코드
public class HookDetector : MonoBehaviour
{
    public Controller controller;
    [SerializeField] private HandData handData;
    [SerializeField] private float handVelocityThreshold = 0.25f;
    private Coroutine _chekingHookRCoroutine;
    public InputActionProperty activateAction;

    void Update()
    {
        CheckControllerSpeed();
    }

    private void Init()
    {
        
    }

    private bool GetControllerActivateAction()
    {
        // hand의 grab버튼 활성화 확인
        return activateAction.action.ReadValue<bool>();
    }

    private void CheckControllerSpeed()
    {
        if(_chekingHookRCoroutine != null || handData.ControllerSpeed < handVelocityThreshold ) return;
        switch (controller)
        {   
            case Controller.leftController:
                _chekingHookRCoroutine = StartCoroutine(IsLeftHook());
                Debug.Log("LHand Velocity Value didn't reach a ceration threshold.");
                break;
            case Controller.rightController:
                _chekingHookRCoroutine = StartCoroutine(IsRightHook());
                Debug.Log("RHand Velocity Value didn't reach a ceration threshold.");
                break;
        }
    }
    // 코루틴으로 하자. 손의 속도가 일정 값 이상 올라갔을 때 hook 검사
    // 로테이션 지속적으로 확인 
    // 주먹쥐었을 때 확인 
    private IEnumerator IsLeftHook()
    {
        
        while (true)
        {
            Debug.Log($"LHand.ControllerSpeed : {handData.ControllerSpeed}");
        }
    }
    private IEnumerator IsRightHook()
    {
        while (true)
        {
            Debug.Log($"RHand.ControllerSpeed : {handData.ControllerSpeed}");
        }
    }
    private bool IsLeftHook1()
    {
        
        
        // 레프트 훅 감지 로직
        // 컨트롤러의 속도와 회전 각도를 분석
        
        // 조건 : 주먹을 쥔 상태, 주먹이 바라보는 방향(hand의 local rotation z방향)의 회전 
        // rotation y 증가하는 형태, 90도 증가 -> 훅
        
        
        return true; // 임의로 true 반환
    }

    private bool IsRightHook1()
    {
        // 라이트 훅 감지
        
        // 조건 : 주먹을 쥔 상태, 주먹이 바라보는 방향(hand의 local rotation z방향)의 회전 
        // rotation y 감소하는 형태, 90도 감소 -> 훅


        return true; // 임의로 true 반환
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