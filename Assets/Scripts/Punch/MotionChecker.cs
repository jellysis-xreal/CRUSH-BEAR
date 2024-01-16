using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionChecker : MonoBehaviour
{
    public enum Motion
    {
        zap, leftHook, rightHook
    }
    public Motion correctMotion;
    public LayerMask layerMask;
    private Collider _collider;
    public BoxCollider _boxCollider;
    public Vector3 triggeredPosition;
    
    private void OnTriggerEnter(Collider other)
    {
        
        // 1. detector.isHooking = true
        // 2. left일 경우 콜라이더의 왼쪽 면 근처에서 트리거 됐는가, (박스 콜라이더 기준 반으로 나눈 영역 트리거)
        // HookMotionDetector detector = other.GetComponent<HookMotionDetector>();

        triggeredPosition = other.ClosestPoint(transform.position);
        // Debug.Log($"correct Position : {triggeredPosition} {CheckHandPosition(triggeredPosition)}");
        HookMotionDetector detector = other.GetComponent<HookMotionDetector>();
        
        if(detector.isHooking && CheckHandPosition(triggeredPosition, detector)) Debug.Log("Correct Behaviour!");
        else Debug.Log("Wrong Behaviour!");
        
        // Debug.Log($"Is correct Hooking Motion : isHooking {detector.isHooking}, CheckHandPosition : {CheckHandPosition(triggeredPosition, detector)} => {detector.isHooking && CheckHandPosition(triggeredPosition, detector)}");
    }

    private bool CheckHandPosition(Vector3 triggerPosition, HookMotionDetector detector)
    {
        // 트리거된 순간, 실제 transform.scale * boxCollider.size.x@@ 
        // _boxCollider.size.

        Transform handTransform = DetectHand();
        if (handTransform == null) return false;
        Debug.Log($"handTransform.localEulerAngles.y {handTransform.localEulerAngles.y}");
        
        // 추후 점수 측정 방식 -> handTransform.localEulerAngles.y로 각도 측정, 
        // 0~20, 160~180  => 실패
        // 20~40, 140~160 => 0.7
        // 40~60, 120~140 => 0.8
        // 60~80, 100~120 => 0.9
        // 80~100         => 1

        switch (detector.controller)
        {
            case Controller.leftController:
                if (correctMotion != Motion.leftHook) return false;
                if ((handTransform.localEulerAngles.y < 20 && handTransform.localEulerAngles.y >= 0) ||
                    (handTransform.localEulerAngles.y < 180 && handTransform.localEulerAngles.y >= 160))
                    return false;
                else
                    return true;
                break;
            case Controller.rightController:
                if (correctMotion != Motion.rightHook) return false;
                if ((handTransform.localEulerAngles.y > -20 && handTransform.localEulerAngles.y <= 0) ||
                    (handTransform.localEulerAngles.y > -180 && handTransform.localEulerAngles.y <= 160))
                    return false;
                else
                    return true;
                break;
        }

        return false;
    }

    private Transform DetectHand()
    {
        Vector3 boxCenter = new Vector3();
        Vector3 boxSize = new Vector3(transform.lossyScale.x * _boxCollider.size.x * 1.5f, 
            transform.lossyScale.y * _boxCollider.size.y * 2f,
            transform.lossyScale.z * _boxCollider.size.z * 2f);
        switch (correctMotion)
        {
            case Motion.leftHook:
                boxCenter = transform.position - new Vector3(transform.lossyScale.x * _boxCollider.size.x / 2, 0, 0);
                break;
            case Motion.rightHook:
                boxCenter = transform.position + new Vector3(transform.lossyScale.x * _boxCollider.size.x / 2, 0, 0);
                break;
            case Motion.zap:
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
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(triggeredPosition, 0.05f);
        
        // 왼쪽 부분
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position - new Vector3(transform.lossyScale.x * _boxCollider.size.x/2, 0, 0),
            new Vector3(transform.lossyScale.x * _boxCollider.size.x * 1.5f, 
                transform.lossyScale.y * _boxCollider.size.y * 2f,
                transform.lossyScale.z * _boxCollider.size.z * 2f));
        
        // 오른쪽 부분
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + new Vector3(transform.lossyScale.x * _boxCollider.size.x/2, 0, 0),
            new Vector3(transform.lossyScale.x * _boxCollider.size.x * 1.5f, 
                transform.lossyScale.y * _boxCollider.size.y * 2f,
                transform.lossyScale.z * _boxCollider.size.z * 2f));
        
    }
}
