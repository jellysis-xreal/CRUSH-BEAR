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
    private Collider _collider;

    private void OnTriggerEnter(Collider other)
    {
        HookMotionDetector detector = other.GetComponent<HookMotionDetector>();
        
        // 1. detector.isHooking = true
        // 2. left일 경우 콜라이더의 왼쪽 면 근처에서 트리거 됐는가, (박스 콜라이더 기준 반으로 나눈 영역 트리거)
        
        // detector.isHooking
        // CheckHandPosition(other.)
        
        
    }

    private bool CheckHandPosition(Transform triggerPosition)
    {

        return true;
    }
}
