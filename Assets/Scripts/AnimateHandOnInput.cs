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

    private void Update()
    {
        float triggerValue = pinchAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);

        float grabValue = grabAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", grabValue);
        
        isSelected = selectAction.action.IsPressed();
        isActivated = activateAction.action.IsPressed();
        
        HandDestroyerUpdate(isSelected);
    }

    private void HandDestroyerUpdate(bool isSelected)
    {
        if (isSelected) destroyer.SetActive(true); 
        else destroyer.SetActive(false);
    }
}
