using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

public class PunchaleMovement : MonoBehaviour
{
    // TODO : Topping 생성 시 지정해줘야 하는 변수들
    [Header("Setting Variable")]
    public int arrivalBoxNum = 0; // 목표인 Box index number
    public float arriveTime; // Node Instantiate

    [Header("other Variable (AUTO)")] 
    [SerializeField] private ObjectArrivalAreaManager arrivalArea;
    private Transform targetTransform;
    
    // 토핑이 움직이기 위한 변수 
    private Rigidbody _rigidbody;
    private float _constantSpeed = 0f;
    private Vector3 dir = new Vector3();
    
    // 토핑이 맞은, 맞지 않은 후에 활용할 변수
    private bool _isHit = false;
    private bool _isArrivalAreaHit = false; // 박스 트리거된 이후, 바로 직전의 움직임을 유지할 때 사용하는 변수 

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void InitializeTopping(NodeInfo node)
    {
        arrivalBoxNum = node.arrivalBoxNum;
        arriveTime = node.timeToReachPlayer;

        InitiateVariable();
        arrivalArea.setting();
    }

    private void InitiateVariable()
    {
        _rigidbody.WakeUp();
        arrivalArea = GameObject.FindWithTag("ArrivalAreaParent").GetComponent<ObjectArrivalAreaManager>();
        targetTransform = arrivalArea.arrivalAreas[arrivalBoxNum-1];
        
        CalculateConstantSpeed();
    }
    void FixedUpdate()
    {
        if(!_isArrivalAreaHit) Move();
        else TriggeredMove();
    }
    private void CalculateConstantSpeed()
    {
        // 속도 = 거리 / 시간
        float time = arriveTime;
        _constantSpeed = Vector3.Distance(targetTransform.position, transform.position) / time;
        Debug.Log($"{this.gameObject.name} constant speed : {_constantSpeed}");
        Debug.Log($"{this.gameObject.name} time : {time}");
        Debug.Log($"{this.gameObject.name} distance : {Vector3.Distance(targetTransform.position, transform.position)}");
    }

    private void Move()
    {
        dir = (targetTransform.position - transform.position).normalized;
        transform.position += dir * _constantSpeed * Time.fixedDeltaTime;
    }
    private void TriggeredMove()
    {
        transform.position += dir * _constantSpeed * Time.fixedDeltaTime;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ArrivalArea"))
        {
            // Debug.Log($"Trigger {other.GetComponent<ObjectArrivalArea>().boxIndex} box ");
            // other.GetComponent<MeshRenderer>().material.DOColor(Random.ColorHSV(), 1f);
            _isArrivalAreaHit = true;
        }
        if (other.tag == "body")
        {
            // 플레이어 공격 성공 처리
            GameManager.Player.MinusPlayerLifeValue();
            gameObject.SetActive(false);
        }
        if (other.CompareTag("TriggerPad"))
        {
            // 뒤에 존재하는 곰돌이 공격 성공 처리
            Debug.Log($"{gameObject.name} Trigger Pad");
            GameManager.Player.MinusPlayerLifeValue();
            other.GetComponent<BGBearManager>().MissNodeProcessing(this.gameObject);
            this.enabled = false;
        }
    }
}
