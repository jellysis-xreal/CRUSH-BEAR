using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

public class ObjectArrivalAreaManager : MonoBehaviour
{
    public Transform[] arrivalAreas;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform IKplayerTransform;

    private void updateArea()
    {
        for (int i = 0; i < arrivalAreas.Length; i++)
            arrivalAreas[i] = this.transform.GetChild(i).transform;
    }
    private void Update()
    {
        updateArea();
        
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
                HitWaveSet();
                break;
            
        }
    }

    public void setting()
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
                HitWaveSet();
                break;
            
        }
    }

    private void HitWaveSet()
    {
        transform.rotation = IKplayerTransform.rotation;
        
        // float x, y, z;
        // if (transform.position.x <= 0) x = -IKplayerTransform.position.x;
        // else x = +IKplayerTransform.position.x;
        // if (transform.position.y <= 0) y = -IKplayerTransform.position.y;
        // else y = +IKplayerTransform.position.y;
        // if (transform.position.z <= 0) z = -IKplayerTransform.position.z;
        // else z = +IKplayerTransform.position.z;
        
        transform.position = IKplayerTransform.forward * 1.3f + Vector3.up * 0.8f;
    }
    
}
