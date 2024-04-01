using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using Motion = EnumTypes.Motion;

public class PunchMotionChecker : MonoBehaviour
{
    private ChildTriggerChecker _childTriggerChecker;
    private Breakable _breakable;
    public Motion correctMotion = Motion.None;
    private bool CheckAdditionalCondition()
    {
        // 추가 조건 검사. 프레임 사이에 콜라이더를 지나 자식의 콜라이더에 트리거되지 않았을 경우를 대비한 메서드
        bool isRightPosition = false;
        
        
        
        return isRightPosition;
    }
    
    private void Awake()
    {
        if (_childTriggerChecker != null)
        {
            _childTriggerChecker = GetComponentInChildren<ChildTriggerChecker>();
            correctMotion = _childTriggerChecker.handMotion;
        }
        if(_breakable != null) _breakable = GetComponent<Breakable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destroyer"))
        {
            if (_childTriggerChecker.isTriggered)
            {
                _breakable.MotionSucceed(correctMotion);
                Debug.Log("Motion succeed! (child.isTriggered True!)");
            }
            else if(CheckAdditionalCondition())
            {
                _breakable.MotionSucceed(correctMotion);
                Debug.Log("Motion succeed! (child.isTriggered True!, Additional Condition True)");
            }
            else
            {
                _breakable.MotionFailed();
            }
        }
    }
}
