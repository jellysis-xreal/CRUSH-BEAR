using System;using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using UnityEngine.InputSystem;
using Motion = EnumTypes.Motion;

// 컨트롤러의 위치와 회전을 추적하고 Hook 동작을 했는지 판단하는 코드
public class HookMotionDetector : MonoBehaviour
{
    public Controller controller;
    [SerializeField] private Transform handTransform;
    [SerializeField] private HandData handData;
    [SerializeField] private float handVelocityMinimumThreshold = 0.25f;
    [SerializeField] private float handVelocityMaximumThreshold = 5f;
    [SerializeField] private float handVelocity;
    private Coroutine _chekingHookCoroutine;
    private Coroutine _chekingUpperCutCoroutine;
    public bool hookCoroutineExist = false;
    public bool upperCutCoroutineExist = false;
    public InputActionProperty activateAction;
     
    public Motion motion;
    public Motion hookMotion; // LeftHook, RightHook, None만 사용
    public Motion upperCutMotion; // LeftUpperCut, RightUpperCut, None만 사용

    private void Start()
    {
        motion = Motion.None;
    }

    void Update()
    {
        // LeftZap, RightZap, LeftHook, RightHook, LeftUpperCut, RightUpperCut
        // left, right, forward, up, down, back
        
        // hook, upperCut 코루틴 없고 Select 버튼 누르고 있으면 호출
        if(_chekingHookCoroutine == null && GetControllerActivateAction()) CheckHookMotionCondition(); 
        if(_chekingUpperCutCoroutine == null && GetControllerActivateAction()) CheckUpperCutMotionCondition(); 
        
        handVelocity = handData.ControllerSpeed;
        
        if(_chekingHookCoroutine != null) Debug.Log("hook Coroutine exist!");
        if(_chekingUpperCutCoroutine != null) Debug.Log("upper Coroutine exist!");
    }

    private void Init()
    {
        
    }

    public bool GetControllerActivateAction()
    {
        // hand의 grab버튼 활성화 확인
        return activateAction.action.IsInProgress();
    }

    private void CheckHookMotionCondition()
    {
        // 속도 임계 범위 내 존재하지 않으면 반환
        if(handVelocity < handVelocityMinimumThreshold || handVelocity > handVelocityMaximumThreshold) return;

        switch (controller)
        {
            case Controller.LeftController:
                _chekingHookCoroutine = StartCoroutine(IsLeftHook());
                Debug.Log("LHand Velocity Value reach a ceration threshold.");
                break;
            case Controller.RightController:
                _chekingHookCoroutine = StartCoroutine(IsRightHook());
                Debug.Log("RHand Velocity Value reach a ceration threshold.");
                break;
        }
    }

    private void CheckUpperCutMotionCondition()
    {
        if(handVelocity < handVelocityMinimumThreshold || handVelocity > handVelocityMaximumThreshold) return;
        
        switch (controller)
        {
            case Controller.LeftController:
                _chekingUpperCutCoroutine = StartCoroutine(IsUpperCut());
                Debug.Log("LHand Velocity Value reach a ceration threshold.");
                break;
            case Controller.RightController:
                _chekingUpperCutCoroutine = StartCoroutine(IsUpperCut());
                Debug.Log("RHand Velocity Value reach a ceration threshold.");
                break;
        }
    }
    
    private void CheckControllerSpeed()
    {
        // _chekingHook 코루틴 존재하지 않음, 컨트롤러 속도 > 임계값, grab 버튼 활성화된 경우 회전 감지 시작.
       
        Debug.Log($"handSpeed is available: {handVelocity}");
        
        switch (controller)
        {   
            case Controller.LeftController:
                _chekingHookCoroutine = StartCoroutine(IsLeftHook());
                _chekingUpperCutCoroutine = StartCoroutine(IsUpperCut());
                Debug.Log("LHand Velocity Value reach a ceration threshold.");
                break;
            case Controller.RightController:
                _chekingHookCoroutine = StartCoroutine(IsRightHook());
                _chekingUpperCutCoroutine = StartCoroutine(IsUpperCut());
                Debug.Log("RHand Velocity Value reach a ceration threshold.");
                break;
        }
    }
    
    // right controller eulerAnlge.x -90 부근, eulerAnlge.z -90 부근,
    // left controller eulerAnlge.x -90 부근, eulerAnlge.z 90 부근
    private IEnumerator IsLeftHook()
    {
        float startAngleY = handTransform.localEulerAngles.y; 
        while (GetControllerActivateAction())
        {
            Debug.Log("Left Hook Check....");
            if ((transform.localEulerAngles.x > -135 && transform.localEulerAngles.x < -45)
                && (transform.localEulerAngles.z > -135 && transform.localEulerAngles.z < -45))
            {
                Debug.Log("detect Left Hook motion");
                hookMotion = Motion.LeftHook;
            }
            // 조건 : 주먹을 쥔 상태, 주먹이 바라보는 방향(hand의 local rotation z방향)의 회전 
            // rotation y 증가하는 형태, 90도 증가 -> 훅
            float currentAngleY = handTransform.localEulerAngles.y;
            if((currentAngleY < startAngleY) ||handVelocity < handVelocityMinimumThreshold 
                                             || handVelocity > handVelocityMaximumThreshold)
            {
                Debug.Log("left hook Coroutine end");
                HookCoroutineEndEvent();
                yield break;
            }
            // Debug.Log($"angleDifference per 1 Frame : {currentAngleY - startAngleY}");
            startAngleY = currentAngleY;
            hookMotion = Motion.LeftHook;
            yield return null;
        }
        HookCoroutineEndEvent();
        yield break;
    }
    private IEnumerator IsRightHook()
    {
        float startAngleY = handTransform.localEulerAngles.y;
        while (GetControllerActivateAction())
        {
            Debug.Log("Right Hook Check....");
            if ((transform.localEulerAngles.x > -135 && transform.localEulerAngles.x < -45)
                && (transform.localEulerAngles.z > 45 && transform.localEulerAngles.z < 90))
            {
                Debug.Log("detect Right Hook motion");
                hookMotion = Motion.RightHook;
            }
            // 조건 : 주먹을 쥔 상태, 주먹이 바라보는 방향(hand의 local rotation z방향)의 회전 
            // rotation y 감소하는 형태, 90도 감소 -> 훅
            float currentAngleY = handTransform.localEulerAngles.y;
            if(currentAngleY > startAngleY ||handVelocity < handVelocityMinimumThreshold 
                                           || handVelocity > handVelocityMaximumThreshold)
            {
                Debug.Log("right hook Coroutine end");
                HookCoroutineEndEvent();
                yield break;
            }
            // Debug.Log($"angleDifference per 1 Frame : {currentAngleY - startAngleY}");
            startAngleY = currentAngleY;
            hookMotion = Motion.RightHook;
            yield return null;
        }
        HookCoroutineEndEvent();
        yield break;
    }

    // 왼손 오른손 코드 분리 switch 반복..
    private IEnumerator IsUpperCut()
    {
        while (GetControllerActivateAction())
        {
            Debug.Log("Upper Cut Check....");
            switch (controller)
            {
                case Controller.RightController:
                    // Debug.Log($"rightController Upper Check : {handTransform.localEulerAngles.x}, {handTransform.localEulerAngles.z}");
                    if ((handTransform.localEulerAngles.x < 360 && handTransform.localEulerAngles.x > 225)
                        && (handTransform.localEulerAngles.z < 315 && handTransform.localEulerAngles.z > 225))
                    {
                        Debug.Log("detect Right Upper Cut motion");
                        upperCutMotion = Motion.RightUpperCut;
                    }
                    break;
                case Controller.LeftController:
                    // Debug.Log($"leftController Upper Check : {handTransform.localEulerAngles.x}, {handTransform.localEulerAngles.z}");
                    if ((handTransform.localEulerAngles.x < 315 && handTransform.localEulerAngles.x > 225)
                        && (handTransform.localEulerAngles.z > 45 && handTransform.localEulerAngles.z < 135))
                    {
                        Debug.Log("detect Left Upper Cut motion");
                        upperCutMotion = Motion.LeftUpperCut;
                    }
                    break;
            }
            if (handVelocity < handVelocityMinimumThreshold * 0.5f
                || handVelocity > handVelocityMaximumThreshold)
            {
                Debug.Log("End UpperCut coroutine");
                UpperCutCoroutineEndEvent();
                yield break;
            }
            yield return null;
        }
        UpperCutCoroutineEndEvent();
        yield break;
    }
    private void HookCoroutineEndEvent() // 다 잽으로 초기화? 아니야 None으로 초기화 제대로 측정하자
    {
        // StopCoroutine(_chekingHookCoroutine);
        _chekingHookCoroutine = null;
        hookMotion = Motion.None;
        // hookCoroutineExist = false;
    }

    private void UpperCutCoroutineEndEvent()
    {
        // StopCoroutine(_chekingUpperCutCoroutine);
        _chekingUpperCutCoroutine = null;
        upperCutMotion = Motion.None;
        // upperCutCoroutineExist = false;
    }
}
