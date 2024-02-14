using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticManager : MonoBehaviour
{
    private static HapticManager instance;

    public static HapticManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HapticManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(HapticManager).Name;
                    instance = obj.AddComponent<HapticManager>();
                }
            }
            return instance;
        }
    }

    //// ������ ����Ű�� �Լ�
    //public void TriggerHapticPattern(OVRInput.Controller controllerMask, float vibrationStrength, float duration, int numVibrations, float interval)
    //{
    //    StartCoroutine(HapticPatternCoroutine(controllerMask, vibrationStrength, duration, numVibrations, interval));
    //}

    //// ���� ������ �����ϴ� �ڷ�ƾ
    //private IEnumerator HapticPatternCoroutine(OVRInput.Controller controllerMask, float vibrationStrength, float duration, int numVibrations, float interval)
    //{
    //    Debug.Log("== HapticPatternCoroutine ");
    //    for (int i = 0; i < numVibrations; i++)
    //    {
    //        OVRInput.SetControllerVibration(vibrationStrength, vibrationStrength, controllerMask);
    //        yield return new WaitForSeconds(duration);
    //        OVRInput.SetControllerVibration(0, 0, controllerMask);
    //        yield return new WaitForSeconds(interval);
    //    }
    //}
}
