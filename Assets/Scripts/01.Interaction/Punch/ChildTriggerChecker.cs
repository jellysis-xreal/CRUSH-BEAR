using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Motion = EnumTypes.Motion;

public class ChildTriggerChecker : MonoBehaviour
{
    public bool isTriggered = false;
    public Motion handMotion = Motion.None;
    private IEnumerator ChangeIsTriggeredField()
    {
        isTriggered = true;
        Debug.Log($"[Motion] {Time.time} {transform.root.name} is Triggered {isTriggered}");
        yield return new WaitForSeconds(1f);
        isTriggered = false; // 1초 뒤 false로 초기화
        yield break;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(isTriggered) return;
        if (other.CompareTag("Destroyer")) StartCoroutine(ChangeIsTriggeredField());
    }
}
