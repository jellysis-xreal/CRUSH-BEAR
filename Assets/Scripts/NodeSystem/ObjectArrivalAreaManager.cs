using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

public class ObjectArrivalAreaManager : MonoBehaviour
{
    public Transform[] arrivalAreas;
    [SerializeField] private Transform playerTransform;
    private void Update()
    {

        switch (GameManager.Wave.GetWaveType())
        {
            case WaveType.Shooting:
                transform.rotation = Quaternion.Euler(0, -60.0f,0);
                transform.position = playerTransform.position + Vector3.forward * 0.2f;
                break;
            
            case WaveType.Punching:
                transform.rotation = Quaternion.Euler(0, 0.0f,0);
                transform.position = playerTransform.position + Vector3.forward * 0.2f;
                break;
            
            case WaveType.Hitting:
                transform.rotation = Quaternion.Euler(0, +60.0f,0);
                transform.position = playerTransform.position + Vector3.forward * 0.8f;
                break;
            
        }
    }
}
