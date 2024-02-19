using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// XR Controller (Action-Based), XR Origin의 left right Controller gameObject에 추가해서 사용 가능한 코드
// XRBaseInteractable의 이벤트를 활용
[System.Serializable]
public class Haptic
{
    [Range(0, 1)] public float intensity;
    public float duration;

    public void TriggerHaptic(BaseInteractionEventArgs eventArgs)
    {
        if (eventArgs.interactableObject is XRBaseControllerInteractor controllerInteractor)
        {
            TriggerHaptic(controllerInteractor.xrController);
        }
    }

    public void TriggerHaptic(XRBaseController controller)
    {

        if (intensity > 0)
        {
            // controller에 진동을 보내는 코드
            controller.SendHapticImpulse(intensity, duration);
        }
    }
}

public class HapticIneractable : MonoBehaviour
{
    public Haptic hapticOnActivated;
    public Haptic hapticHoverEntered;
    public Haptic hapticHoverExited;
    public Haptic hapticSelectEntered;
    public Haptic hapticSelectExited;
    void Start()
    {
        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();
        
        // XRBaseInteractable의 이벤트 리스너 추가 
        interactable.activated.AddListener(hapticOnActivated.TriggerHaptic);
        interactable.hoverEntered.AddListener(hapticHoverEntered.TriggerHaptic);
        interactable.hoverExited.AddListener(hapticHoverExited.TriggerHaptic);
        interactable.selectEntered.AddListener(hapticSelectEntered.TriggerHaptic);
        interactable.selectExited.AddListener(hapticSelectExited.TriggerHaptic);
    }
}