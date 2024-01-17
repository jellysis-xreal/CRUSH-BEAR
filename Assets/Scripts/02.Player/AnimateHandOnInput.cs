using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty grabAnimationAction;
    public Animator handAnimator;

    [Space] public InputActionProperty selectAction;
    public InputActionProperty activateAction;

    public bool isSelected = false;
    public bool isActivated = false;
    public GameObject destroyer;

    private HandData HandData;

    private void Start()
    {
        HandData = GetComponent<HandData>();
    }

    private void Update()
    {
        float triggerValue = pinchAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);

        float grabValue = grabAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", grabValue);

        isSelected = selectAction.action.IsPressed();
        isActivated = activateAction.action.IsPressed();

        // HandDestroyerUpdate(isSelected);
    }

    private void HandDestroyerUpdate(bool isSelected)
    {
        // 일정 속도 이상 움직여야만 판정 시작
        if (isSelected && HandData.IsMoveQuickly())
            destroyer.SetActive(true); 
        else 
            destroyer.SetActive(false);
    }
}
