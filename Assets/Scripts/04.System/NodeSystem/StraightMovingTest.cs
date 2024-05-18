using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightMovingTest : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public int arrivalAreaIndex = 0;
    private ObjectArrivalAreaManager _objectArrivalAreaManager;
    public bool isArrivalAreaHit;
    private Transform targetTransform;
    private Vector3 dir = new Vector3();

    [Header("have to set")] 
    public float timeToReachPlayer; // 생성 후 플레이어 까지 도달할 시간
    public float generationTime; // 생성시간

    public float constantSpeed;
    public float equivalentAccelerationSpeed;


    //  timeToReachPlayer - generationTime의 값이 생성된 위치에서 
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
        CalculateConstantSpeed();
    }

    
    void Update()
    {
        if(!isArrivalAreaHit) Move();
        else if(isArrivalAreaHit ) TriggeredMove();
    }

    private void CalculateConstantSpeed()
    {
        // 속도 = 거리 / 시간
        float time = timeToReachPlayer - generationTime;
        constantSpeed = Vector3.Distance(targetTransform.position, transform.position) / time;
        //Debug.Log($"constantSpeed : {constantSpeed}");
    }
    
    public void Init()
    {
        if(arrivalAreaIndex == 0) return;
        _rigidbody = GetComponent<Rigidbody>();
        
        isArrivalAreaHit = false;
        _objectArrivalAreaManager = GameObject.FindWithTag("ArrivalAreaParent").GetComponent<ObjectArrivalAreaManager>();

        targetTransform = _objectArrivalAreaManager.arrivalAreas[arrivalAreaIndex-1];
    }
    
    void Move()
    {
        transform.LookAt(transform);
        dir = (targetTransform.position - transform.position).normalized;
        transform.position += dir * constantSpeed * Time.deltaTime;
    }

    void TriggeredMove()
    {
        transform.position += dir * constantSpeed * Time.deltaTime;
    }
    
}
