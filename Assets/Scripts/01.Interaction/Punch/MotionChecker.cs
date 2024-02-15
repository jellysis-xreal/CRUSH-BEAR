using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using Motion = EnumTypes.Motion;

public class MotionChecker : MonoBehaviour
{
    public Motion correctMotion;

    [Space]
    public Breakable breakable;
    public LayerMask layerMask;
    private Collider _collider;
    public BoxCollider _boxCollider;
    public Vector3 triggeredPosition;
    public bool _isTriggered = false; // 트리거 1회만 허용하도록 함. 풀링에 넣고 다시 false로 초기화하기!

    
    private void Awake()
    {
        Init();
    }
    private void Init()
    {
        breakable = GetComponent<Breakable>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // 1. detector.isHooking = true
        // 2. left일 경우 콜라이더의 왼쪽 면 근처에서 트리거 됐는가, (박스 콜라이더 기준 반으로 나눈 영역 트리거)
        // HookMotionDetector detector = other.GetComponent<HookMotionDetector>();

        if(!other.TryGetComponent(out HookMotionDetector detector) || _isTriggered) return;
        
        triggeredPosition = other.ClosestPoint(transform.position);
        _isTriggered = true;
        Debug.Log($"detector exist : {detector}, triggeredPosition : {triggeredPosition} ");
        
        switch (correctMotion)
        {
            case Motion.LeftHook:
                if (CheckHandPosition(detector))
                {
                    breakable.MotionSucceed(detector.handTransform);
                    Debug.Log("LeftHook Succeed!");
                }
                else
                {
                    breakable.MotionFailed(detector.handTransform);
                    Debug.Log("LeftHook Failed!");   
                }
                break;
            case Motion.RightHook:
                if (CheckHandPosition(detector))
                {
                    breakable.MotionSucceed(detector.handTransform);
                    Debug.Log("RightHook Succeed!");
                }
                else
                {
                    breakable.MotionFailed(detector.handTransform);
                    Debug.Log("RightHook Failed!");
                }
                break;
            case Motion.LeftUpperCut:
                if (CheckHandPosition(detector))
                {
                    breakable.MotionSucceed(detector.handTransform);
                    Debug.Log("UpperCut Succeed!");
                }
                else
                {
                    breakable.MotionFailed(detector.handTransform);
                    Debug.Log("UpperCut Failed!");
                }
                break;
            case Motion.RightUpperCut:
                if (CheckHandPosition(detector))
                {
                    breakable.MotionSucceed(detector.handTransform);
                    Debug.Log("UpperCut Succeed!");
                }
                else
                {
                    breakable.MotionFailed(detector.handTransform);
                    Debug.Log("UpperCut Failed!");
                }
                break;
            case Motion.LeftZap:
                if (CheckHandPosition(detector))
                {
                    breakable.MotionSucceed(detector.handTransform);
                    Debug.Log("Left Zap Succeed!");
                }
                else
                {
                    breakable.MotionFailed(detector.handTransform);
                    Debug.Log("Left Zap Failed!");
                }
                break;
            case Motion.RightZap:
                if (CheckHandPosition(detector))
                {
                    breakable.MotionSucceed(detector.handTransform);
                    Debug.Log("Right Zap Succeed!");
                }
                else
                {
                    breakable.MotionFailed(detector.handTransform);
                    Debug.Log("Right Zap Failed!");
                }
                break;
            default:
                Debug.LogError("Motion Detect Error!");
                break;
        }
        // Debug.Log($"Is correct Hooking Motion : CheckHandPosition : {CheckHandPosition(detector)}");
    }

    // 손과 트리거된 순간의 위치를 판단해 올바른 동작인지 체크함.
    private bool CheckHandPosition(HookMotionDetector detector)
    {
        Transform handTransform = DoesHandExistWithinTheRange();
        if (handTransform == null) 
        {
            Debug.Log("hand Transform is null");
            return false; // null로 안 해도 될 듯? 그냥 범위 내 있는지만 조사하고 return?
        }

        Debug.Log($"upperCutMotion {detector.upperCutMotion}, HookMotion {detector.hookMotion}");
        // Hand의 위치까지 검사 완료
        // 오브젝트의 correct Motion과 Motion Detector의 Motion과 일치하면 true 반환 -> Breakable.MotionSucceed() 호출
        
        // 훅 -> 훅모션
        // 어퍼 -> 어퍼컷모션
        // 잽은 위치만 판단하고 넘김
        if((correctMotion == Motion.LeftHook || correctMotion == Motion.RightHook)
           && correctMotion == detector.hookMotion) return true;
        if((correctMotion == Motion.LeftUpperCut || correctMotion == Motion.RightUpperCut)
           && correctMotion == detector.upperCutMotion) return true;
        if((correctMotion == Motion.LeftZap && detector.controller == Controller.LeftController) 
            || correctMotion == Motion.RightZap && (detector.controller == Controller.RightController))
            return true;
        
        return false;
    }

    // 범위 내 손이 존재하는가?
    private Transform DoesHandExistWithinTheRange()
    {
        Vector3 boxCenter = new Vector3();
        Vector3 boxSize = new Vector3(transform.lossyScale.x * _boxCollider.size.x * 1.5f, 
            transform.lossyScale.y * _boxCollider.size.y * 2f,
            transform.lossyScale.z * _boxCollider.size.z * 2f);
        switch (correctMotion)
        {
            case Motion.LeftHook:
                boxCenter = transform.position - new Vector3(transform.lossyScale.x * _boxCollider.size.x / 2, 0, 0);
                break;
            case Motion.RightHook:
                boxCenter = transform.position + new Vector3(transform.lossyScale.x * _boxCollider.size.x / 2, 0, 0);
                break;
            case Motion.LeftUpperCut:
                boxCenter = transform.position - new Vector3(0, transform.lossyScale.y * _boxCollider.size.y / 2, 0);
                break;
            case Motion.RightUpperCut:
                boxCenter = transform.position - new Vector3(0, transform.lossyScale.y * _boxCollider.size.y / 2, 0);
                break;
            case Motion.LeftZap: 
                boxCenter = transform.position - new Vector3(0, 0,transform.lossyScale.z * _boxCollider.size.z / 2);
                break;
            case Motion.RightZap: 
                boxCenter = transform.position - new Vector3(0, 0,transform.lossyScale.z * _boxCollider.size.z / 2);
                break;
            default:
                Debug.Log("correct Motion need to set");
                break;
        }
        
        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize / 2, Quaternion.identity, layerMask);
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log("Detected object: " + hitCollider.gameObject.name);
            return hitCollider.transform;
        }
        return null;
    }
    
    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(triggeredPosition, 0.05f);
        
        // 왼쪽 부분
        
        // Left Hook
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position - new Vector3(transform.lossyScale.x * _boxCollider.size.x/2, 0, 0),
            new Vector3(transform.lossyScale.x * _boxCollider.size.x * 1.5f, 
                transform.lossyScale.y * _boxCollider.size.y * 2f,
                transform.lossyScale.z * _boxCollider.size.z * 2f));

        // Right Hook
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + new Vector3(transform.lossyScale.x * _boxCollider.size.x/2, 0, 0),
            new Vector3(transform.lossyScale.x * _boxCollider.size.x * 1.5f, 
                transform.lossyScale.y * _boxCollider.size.y * 2f,
                transform.lossyScale.z * _boxCollider.size.z * 2f));
        
        
        // left, right Upper CUt
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position - new Vector3(0, transform.lossyScale.y * _boxCollider.size.y/2, 0),
            new Vector3(transform.lossyScale.x * _boxCollider.size.x * 1.5f, 
                transform.lossyScale.y * _boxCollider.size.y * 2f,
                transform.lossyScale.z * _boxCollider.size.z * 2f));
        
        
        // Left, Right Zap
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position - new Vector3(0, 0, transform.lossyScale.z * _boxCollider.size.z/2),
            new Vector3(transform.lossyScale.x * _boxCollider.size.x * 1.5f, 
                transform.lossyScale.y * _boxCollider.size.y * 2f,
                transform.lossyScale.z * _boxCollider.size.z * 2f));
    }*/
}
