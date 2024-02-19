using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HapticFeedback : MonoBehaviour
{
    public XRNode controllerNode = XRNode.RightHand; // 진동을 줄 컨트롤러 설정 (오른손 또는 왼손)
    public float duration = 0.1f; // 진동 지속 시간
    public float strength = 0.5f; // 진동 강도 (0.0에서 1.0 사이)

    // 진동을 시작하는 메서드
    public void StartHapticFeedback()
    {
        var devices = new List<UnityEngine.XR.InputDevice>();  // List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(controllerNode, devices);

        foreach (var device in devices)
        {
            if (device.isValid)
            {
                // 진동 실행
                HapticCapabilities capabilities;
                if (device.TryGetHapticCapabilities(out capabilities) && capabilities.supportsImpulse)
                {
                    uint channel = 0;
                    device.SendHapticImpulse(channel, strength, duration);
                }
            }
        }
    }
}