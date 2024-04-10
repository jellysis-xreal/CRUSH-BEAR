using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitTrigger : MonoBehaviour
{
    public bool isTriggered;
    
    private void OnTriggerEnter(Collider other)
    {
        isTriggered = true;
    }

    private void OnTriggerStay(Collider other)
    {
        isTriggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isTriggered = false;
    }
}
