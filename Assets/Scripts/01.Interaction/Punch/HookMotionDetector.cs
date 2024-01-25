using System;using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public bool isHooking = false;
    public bool doingUpperCut = false;
    public InputActionProperty activateAction;

    public MotionChecker.Motion motion;

    private void Start()
    {
        motion = MotionChecker.Motion.zap;
    }

    void Update()
    {
        if(_chekingHookCoroutine == null && GetControllerActivateAction()) CheckHookMotionCondition(); 
        if(_chekingUpperCutCoroutine == null && GetControllerActivateAction()) CheckUpperCutMotionCondition(); 
        
        handVelocity = handData.ControllerSpeed;
        /*if (_chekingHookCoroutine != null)
        {
            coroutineExist = true;
        }
        else coroutineExist = false;*/
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
        if(handVelocity < handVelocityMinimumThreshold || handVelocity > handVelocityMaximumThreshold) return;

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

    private void CheckUpperCutMotionCondition()
    {
        if(handVelocity < handVelocityMinimumThreshold || handVelocity > handVelocityMaximumThreshold) return;
        
        switch (controller)
        {
            case Controller.leftController:
                _chekingUpperCutCoroutine = StartCoroutine(IsUpperCut());
                Debug.Log("LHand Velocity Value reach a ceration threshold.");
                break;
            case Controller.rightController:
                _chekingUpperCutCoroutine = StartCoroutine(IsUpperCut());
                Debug.Log("RHand Velocity Value reach a ceration threshold.");
                break;
        }
    }
    
    private void CheckControllerSpeed()
    {
        /*if (_chekingHookCoroutine != null && _chekingUpperCutCoroutine != null)
        {
            //if(_chekingHookCoroutine != null) Debug.Log("hook Coroutine exist!");
            //if(_chekingUpperCutCoroutine != null) Debug.Log("upper Coroutine exist!");
            return;
        }*/
        // _chekingHook 코루틴 존재하지 않음, 컨트롤러 속도 > 임계값, grab 버튼 활성화된 경우 회전 감지 시작.
        /*if(handVelocity < handVelocityMinimumThreshold 
           || handVelocity > handVelocityMaximumThreshold 
           ) return; // (|| !GetControllerActivateAction())*/
        Debug.Log($"handSpeed is available: {handVelocity}");
        
        switch (controller)
        {   
            case Controller.leftController:
                _chekingHookCoroutine = StartCoroutine(IsLeftHook());
                _chekingUpperCutCoroutine = StartCoroutine(IsUpperCut());
                Debug.Log("LHand Velocity Value reach a ceration threshold.");
                break;
            case Controller.rightController:
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
        // motion = MotionChecker.Motion.zap;
        isHooking = false;
        float startAngleY = handTransform.localEulerAngles.y; 
        while (GetControllerActivateAction())
        {
            Debug.Log("Left Hook Check....");
            if ((transform.localEulerAngles.x > -135 && transform.localEulerAngles.x < -45)
                && (transform.localEulerAngles.z > -135 && transform.localEulerAngles.z < -45))
            {
                Debug.Log("detect Left Hook motion");
                motion = MotionChecker.Motion.leftHook;
                doingUpperCut = true;
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
            // isHooking = true;
            motion = MotionChecker.Motion.leftHook;
            yield return null;
        }
        HookCoroutineEndEvent();
        yield break;
    }
    private IEnumerator IsRightHook()
    {
        // motion = MotionChecker.Motion.zap;
        float startAngleY = handTransform.localEulerAngles.y;
        while (GetControllerActivateAction())
        {
            Debug.Log("Right Hook Check....");
            if ((transform.localEulerAngles.x > -135 && transform.localEulerAngles.x < -45)
                && (transform.localEulerAngles.z > 45 && transform.localEulerAngles.z < 90))
            {
                Debug.Log("detect Right Hook motion");
                motion = MotionChecker.Motion.rightHook;
                // doingUpperCut = true;
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
            // isHooking = true;
            motion = MotionChecker.Motion.rightHook;
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
                case Controller.rightController:
                    // Debug.Log($"rightController Upper Check : {handTransform.localEulerAngles.x}, {handTransform.localEulerAngles.z}");
                    if ((handTransform.localEulerAngles.x < 360 && handTransform.localEulerAngles.x > 225)
                        && (handTransform.localEulerAngles.z < 315 && handTransform.localEulerAngles.z > 225))
                    {
                        Debug.Log("detect Right Upper Cut motion");
                        motion = MotionChecker.Motion.upperCut;
                        // doingUpperCut = true;
                    }
                    break;
                case Controller.leftController:
                    Debug.Log($"leftController Upper Check : {handTransform.localEulerAngles.x}, {handTransform.localEulerAngles.z}");
                    if ((handTransform.localEulerAngles.x < 315 && handTransform.localEulerAngles.x > 225)
                        && (handTransform.localEulerAngles.z > 45 && handTransform.localEulerAngles.z < 135))
                    {
                        Debug.Log("detect Left Upper Cut motion");
                        motion = MotionChecker.Motion.upperCut;
                        // doingUpperCut = true;
                    }
                    break;
            }
            if (handVelocity < handVelocityMinimumThreshold * 0.5f
                || handVelocity > handVelocityMaximumThreshold)
            {
                Debug.Log("End UpperCut coroutine");
                // doingUpperCut = false;
                UpperCutCoroutineEndEvent();
                yield break;
            }
            yield return null;
        }
        UpperCutCoroutineEndEvent();
        yield break;
    }
    private void HookCoroutineEndEvent()
    {
        // StopCoroutine(_chekingHookCoroutine);
        _chekingHookCoroutine = null;
        motion = MotionChecker.Motion.zap;
        // hookCoroutineExist = false;
        // isHooking = false;
    }

    private void UpperCutCoroutineEndEvent()
    {
        // StopCoroutine(_chekingUpperCutCoroutine);
        _chekingUpperCutCoroutine = null;
        motion = MotionChecker.Motion.zap;
        // upperCutCoroutineExist = false;
        // doingUpperCut = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        // 1. 컨트롤러의 움직임이 현재 Hook이냐
        // 2. 처음 Trigger된 포인트가 왼쪽(leftHook일 경우) 혹은 오른쪽(rightHook일 경우)
    }
}
public enum Controller
{
    leftController,
    rightController
}