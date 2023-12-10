using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectArrivalAreaManager : MonoBehaviour
{
    public Transform[] arrivalAreas;
    [SerializeField] private Transform playerTransform;
    private void Update()
    {
        transform.position = playerTransform.position + Vector3.forward;
    }
}
