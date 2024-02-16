using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

public class ObjectArrivalAreaManager : MonoBehaviour
{
    public Transform[] arrivalAreas;
    [SerializeField] private GameObject playerTransform;
    //[SerializeField] private GameObject IKplayerTransform;

    private void Start()
    {
        playerTransform = GameManager.Player.player;
        //IKplayerTransform = GameManager.Player.IK_player;
    }

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
                transform.position = playerTransform.transform.position + Vector3.forward * 0.2f;
                break;
            
            case WaveType.Punching:
                transform.rotation = Quaternion.Euler(0, 0.0f,0);
                transform.position = playerTransform.transform.position + Vector3.forward * 0.2f;
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
                transform.position = playerTransform.transform.position + Vector3.forward * 0.2f;
                break;
            
            case WaveType.Punching:
                transform.rotation = Quaternion.Euler(0, 0.0f,0);
                transform.position = playerTransform.transform.position + Vector3.forward * 0.2f;
                break;
            
            case WaveType.Hitting:
                HitWaveSet();
                break;
            
        }
    }

    private void HitWaveSet()
    {
        //Transform _spawnTransform = NodeInstantiator_minha.HitSpawnTransform.transform;  
        //this.transform.LookAt(_spawnTransform);

        //Vector3 posVector = (_spawnTransform.position - IKplayerTransform.transform.position).normalized;

        transform.position = new Vector3(0.68f, 1.638f, 0.398f);
        transform.rotation = Quaternion.Euler(0, 46.0f, 0);
    }
    
}
