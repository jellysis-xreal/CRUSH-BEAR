// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Meta.XR;
//
// // XR Controller (Action-Based), XR Origin의 left right Controller gameObject에 추가해서 사용 가능한 코드
// // XRBaseInteractable의 이벤트를 활용
// [System.Serializable]
// public class Haptic
// {
//     [Range(0, 1)] public float intensity;
//     public float duration;
//
//     public void TriggerHaptic(BaseInteractionEventArgs eventArgs)
//     {
//         if (eventArgs.interactorObject is XRBaseControllerInteractor controllerInteractor)
//         {
//             TriggerHaptic(controllerInteractor.xrController);
//         }
//     }
//
//     public void TriggerHaptic(XRBaseController controller)
//     {
//         if (intensity > 0)
//         {
//             // controller에 진동을 보내는 코드
//             controller.SendHapticImpulse(intensity, duration);
//         }
//     }
// }
//
// public class HapticInteractable : MonoBehaviour
// {
//     public Haptic hapticOnActivated;
//     public Haptic hapticHoverEntered;
//     public Haptic hapticHoverExited;
//     public Haptic hapticSelectEntered;
//     public Haptic hapticSelectExited;
//
//     void Start()
//     {
//         // Meta Interaction Toolkit의 XRBaseInteractable을 가져옴
//         XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();
//         
//         // XRBaseInteractable의 이벤트 리스너 추가
//         interactable.activated.AddListener((eventArgs) => hapticOnActivated.TriggerHaptic(eventArgs));
//         interactable.hoverEntered.AddListener((eventArgs) => hapticHoverEntered.TriggerHaptic(eventArgs));
//         interactable.hoverExited.AddListener((eventArgs) => hapticHoverExited.TriggerHaptic(eventArgs));
//         interactable.selectEntered.AddListener((eventArgs) => hapticSelectEntered.TriggerHaptic(eventArgs));
//         interactable.selectExited.AddListener((eventArgs) => hapticSelectExited.TriggerHaptic(eventArgs));
//     }
// }