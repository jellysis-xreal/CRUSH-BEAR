using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SplitObjectManager360 : MonoBehaviour
{
    // Player(XR Origin)을 향해 여러 방향에서 날아온다.
    public Transform[] areas;
    
    public GameObject splitableGameObject;
    public GameObject bouncedOffGameObject;
    
    public float minSpawnTime = 2.0f;
    public float maxSpawnTime = 4.0f;

    private float _width;
    private float _height;
    private float _length;
    void Start()
    {
        areas = GetComponentsInChildren<Transform>(false);
        if (areas.Length > 0)
        {
            areas = areas.Skip(1).ToArray();
        }
        
        _width = transform.lossyScale.x;
        _height = transform.lossyScale.y;
        _length = transform.lossyScale.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(CreateRandomPosition());
        }
    }

    IEnumerator CreateRandomPosition()
    {
        while (true)
        {
            // 랜덤한 대기 시간
            float randomDelay = Random.Range(minSpawnTime, maxSpawnTime);

            yield return new WaitForSeconds(randomDelay);

            int index = Random.Range(0, areas.Length);
            ReadScale(areas[index]);
            
            float randomPosX = Random.Range(areas[index].position.x - _width / 2, areas[index].position.x + _width / 2);
            float randomPosY = Random.Range(areas[index].position.y - _height / 2, areas[index].position.y + _height / 2);
            float randomPosZ = Random.Range(areas[index].position.z - _length / 2, areas[index].position.z + _length / 2);

            int whichObject = Random.Range(0, 2);
            if(whichObject == 0) Instantiate(bouncedOffGameObject, new Vector3(randomPosX,randomPosY,randomPosZ) ,Quaternion.identity);
            else if(whichObject == 1) Instantiate(splitableGameObject, new Vector3(randomPosX,randomPosY,randomPosZ) ,Quaternion.identity);
        }
    }

    void ReadScale(Transform areaTransform)
    {
        _width = areaTransform.lossyScale.x;
        _height = areaTransform.lossyScale.y;
        _length = areaTransform.lossyScale.z;
    }
    /*private void OnDrawGizmosSelected()
    {
        // 생성 영역에 대한 범위 알려줌.
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + new Vector3(_width/2,_height/2, _length/2),0.5f);
        Gizmos.DrawSphere(transform.position + new Vector3(_width/2,-_height/2, _length/2),0.5f);
        Gizmos.DrawSphere(transform.position + new Vector3(-_width/2,_height/2, _length/2),0.5f);
        Gizmos.DrawSphere(transform.position + new Vector3(-_width/2,-_height/2, _length/2),0.5f);
        Gizmos.DrawSphere(transform.position + new Vector3(_width/2,_height/2, -_length/2),0.5f);
        Gizmos.DrawSphere(transform.position + new Vector3(_width/2,-_height/2, -_length/2),0.5f);
        Gizmos.DrawSphere(transform.position + new Vector3(-_width/2,_height/2, -_length/2),0.5f);
        Gizmos.DrawSphere(transform.position + new Vector3(-_width/2,-_height/2, -_length/2),0.5f);
    }*/
}
