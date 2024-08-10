using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Motion = EnumTypes.Motion;

public class ChildTriggerChecker : MonoBehaviour
{
    public bool isTriggered = false;
    public Motion handMotion = Motion.None;
    
    private async void OnTriggerEnter(Collider other)
    {
        if(isTriggered || !other.CompareTag("Destroyer")) return;

        isTriggered = true;
        await UniTask.Delay(1000);
        isTriggered = false;
    }
}
