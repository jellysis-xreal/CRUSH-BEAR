using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EnumTypes;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class PunchableMovementTutorial : MonoBehaviour, IPunchableMovement
{
    // TODO : Topping 생성 시 지정해줘야 하는 변수들
    [Header("Setting Variable")]
    public int arrivalBoxNum = 0; // 목표인 Box index number
    public float arriveTime; // Node Instantiate

    [Header("other Variable (AUTO)")] 
    private Vector3 targetPosition;
    
    // 토핑이 움직이기 위한 변수 
    public Transform parentTransform;
    private Rigidbody _rigidbody;
    public float _constantSpeed = 0f;
    private Vector3 dir = new Vector3();
    private float moveDistance = 0f;
    
    // 토핑이 맞은, 맞지 않은 후에 활용할 변수
    //private bool _isHit = false;
    private bool _isArrivalAreaHit = false; // 박스 트리거된 이후, 바로 직전의 움직임을 유지할 때 사용하는 변수
    private MeshRenderer _meshRenderer;
    public SpriteRenderer spriteRenderer; 
    private Breakable _breakable;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _breakable = GetComponent<Breakable>();
        _meshRenderer = GetComponent<MeshRenderer>();
        
        parentTransform.gameObject.SetActive(false);
    }

    public void InitiateVariable(int _arrivalBoxNum, float timeToReachPlayer) // 
    {
        parentTransform.transform.position = new Vector3(0, 3, 25);
        arrivalBoxNum = _arrivalBoxNum;
        arriveTime = timeToReachPlayer;

        if(!parentTransform.gameObject.activeSelf) parentTransform.gameObject.SetActive(true);
        _meshRenderer.enabled = true;
        if(spriteRenderer != null) spriteRenderer.enabled = true;

        _rigidbody.WakeUp();
        targetPosition = GameManager.Wave.GetArrivalPosition(arrivalBoxNum);
        _breakable.InitBreakable();

        StartMovement();
    }
    void FixedUpdate()
    {
        if (_isArrivalAreaHit) TriggeredMove();
    }

    public void StartMovement()
    {
        StartCoroutine(Movement());
    }
    IEnumerator Movement()
    {
        _constantSpeed = Vector3.Distance(targetPosition, parentTransform.position) / arriveTime;
        moveDistance = Vector3.Distance(targetPosition, parentTransform.position);
        dir = (targetPosition - parentTransform.position).normalized;

        Debug.Log($"[SYJ] Movement started with speed: {_constantSpeed} and direction: {dir}");

        parentTransform.DOMove(targetPosition, arriveTime).SetEase(Ease.Linear);
        yield return null;
    }

    private void TriggeredMove()
    {
        parentTransform.position += dir * _constantSpeed * Time.fixedDeltaTime;
    }

    // 손에 맞거나 뒤 trigger pad에 닿았을 경우 setActive(false)
    public void EndInteraction()
    {
        Debug.Log("EndInteraction called");

        _meshRenderer.enabled = false;
        if(spriteRenderer != null) spriteRenderer.enabled = false; 
        
        _rigidbody.velocity=Vector3.zero;
        _rigidbody.angularVelocity=Vector3.zero;
        _rigidbody.Sleep();

        _breakable.m_Destroyed = false;
        
        StartCoroutine(ActiveTime(0.1f));
    }

    IEnumerator TriggerArrivalAreaEndInteraction()
    {
        Debug.Log("TriggerArrivalAreaEndInteraction called");
        yield return new WaitForSeconds(2f);
        _breakable.MotionFailed(); //
        Debug.Log("trigger arrival");
        _meshRenderer.enabled = false;
        if(spriteRenderer != null) spriteRenderer.enabled = false; 
        
        _rigidbody.velocity=Vector3.zero;
        _rigidbody.angularVelocity=Vector3.zero;
        _rigidbody.Sleep();

        _breakable.m_Destroyed = false;
        
        StartCoroutine(ActiveTime(0.1f));
    }
    private IEnumerator ActiveTime(float coolTime)
    {
        yield return new WaitForSecondsRealtime(coolTime); // coolTime만큼 활성화
        _meshRenderer.enabled = true;
        _isArrivalAreaHit = false;
        parentTransform.gameObject.SetActive(false); // coolTime 다 됐으니 비활성화
    }
    
    // 도착 이후 인터랙션 종료를 알리기 위함, 플레이어와 상호작용 X
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ArrivalArea") && !_isArrivalAreaHit)
        {
            Debug.Log("Triggered " + other.gameObject.name);
            _isArrivalAreaHit = true;
            StartCoroutine(TriggerArrivalAreaEndInteraction());
        }
    }
}
