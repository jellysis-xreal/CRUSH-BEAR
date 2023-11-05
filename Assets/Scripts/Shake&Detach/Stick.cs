using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Stick : MonoBehaviour
{
    public bool isAttached = false;
    private Transform controllerTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HandController"))
        {
            isAttached = true;
            controllerTransform = other.transform;
        }
    }

    private void Update()
    {
        if (isAttached && controllerTransform != null)
        {
            transform.position = controllerTransform.position;
            transform.rotation = controllerTransform.rotation;
        }
    }
}
