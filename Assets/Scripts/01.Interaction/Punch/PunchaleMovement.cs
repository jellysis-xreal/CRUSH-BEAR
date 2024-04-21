using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EnumTypes;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class PunchaleMovement : MonoBehaviour
{
    // TODO : Topping 생성 시 지정해줘야 하는 변수들
    [Header("Setting Variable")]
    public int arrivalBoxNum = 0; // 목표인 Box index number
    public float arriveTime; // Node Instantiate
    public uint beatNum;
    public uint typeIndex;

    [Header("other Variable (AUTO)")] 
    private Vector3 targetPosition;
    
    // 토핑이 움직이기 위한 변수 
    // public Transform parentTransform;
    private Rigidbody _rigidbody;
    public float _constantSpeed = 0f;
    private Vector3 dir = new Vector3();
    public CookieControl cookieControl;
    private float moveDistance = 0f;
    
    // 토핑이 맞은, 맞지 않은 후에 활용할 변수
    private bool _isHit = false;
    private bool _isArrivalAreaHit = false; // 박스 트리거된 이후, 바로 직전의 움직임을 유지할 때 사용하는 변수
    private MeshRenderer _meshRenderer;
    public SpriteRenderer spriteRenderer; 
    private Breakable _breakable;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _breakable = GetComponent<Breakable>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public IEnumerator InitializeToppingRoutine(NodeInfo node)
    {
        _isArrivalAreaHit = false;
        arrivalBoxNum = node.arrivalBoxNum;
        arriveTime = node.timeToReachPlayer;
        
        Debug.Log($"[Punch] time diff {arriveTime - GameManager.Wave.waveTime}");
        if (arriveTime - GameManager.Wave.waveTime < 7)
        {
            Debug.Log($"[Punch] Init Early {transform.name} ");
            // cookieControl.Init(targetPosition);
            InitiateVariableEarly();
            yield break;
        }
        
        while (arriveTime - GameManager.Wave.waveTime > 7)
        {
            yield return null;
        }

        Debug.Log($"[punch] InitVar {gameObject.name} ");
        // cookieControl.Init(targetPosition);
        InitiateVariable();
    }

    private void InitiateVariableEarly()
    {
        _meshRenderer.enabled = true;
        if(spriteRenderer != null) spriteRenderer.enabled = true; 
        
        _rigidbody.WakeUp();
        //this.transform.position = GameManager.Wave.GetSpawnPosition(arrivalBoxNum);
        targetPosition = GameManager.Wave.GetArrivalPosition(arrivalBoxNum);

        dir = targetPosition - transform.position;
        transform.position += dir * ((7 - (arriveTime - GameManager.Wave.waveTime)) / 7f);
        StartCoroutine(Movement(arriveTime - GameManager.Wave.waveTime));
    }
    private void InitiateVariable()
    {
        _meshRenderer.enabled = true;
        if(spriteRenderer != null) spriteRenderer.enabled = true; 
        
        _rigidbody.WakeUp();
        //this.transform.position = GameManager.Wave.GetSpawnPosition(arrivalBoxNum);
        targetPosition = GameManager.Wave.GetArrivalPosition(arrivalBoxNum);

        StartCoroutine(Movement());
        // CalculateConstantSpeed();
    }
    void FixedUpdate()
    {
        if (_isArrivalAreaHit) TriggeredMove();
    }
    private void CalculateConstantSpeed()
    {
        // 속도 = 거리 / 시간
        // 도달까지 걸리는 시간 = 최종 도착 시간 - 현재 시간
        float time = arriveTime - GameManager.Wave.waveTime;
        _constantSpeed = Vector3.Distance(targetPosition, transform.position) / time;
        /*Debug.Log($"[punch] constant speed : {_constantSpeed} name : {transform.gameObject.name} pos : {transform.position}");
        Debug.Log($"[punch] distance : {Vector3.Distance(targetPosition, transform.position)} {transform.gameObject.name} ");*/
    }

    IEnumerator Movement(float time)
    {
        _constantSpeed = Vector3.Distance(targetPosition, transform.position) / time;
        dir = (targetPosition - transform.position).normalized;
        transform.DOMove(targetPosition, time).SetEase(Ease.Linear);
        yield return null;
    }
    IEnumerator Movement()
    {
        Debug.Log($"[Punch] Movement {GameManager.Wave.currentBeatNum} Start Pos {transform.position}, Time : {arriveTime - GameManager.Wave.waveTime}");
        float time = arriveTime - GameManager.Wave.waveTime;
        moveDistance = Vector3.Distance(targetPosition, transform.position);
        _constantSpeed = moveDistance / time;
        dir = (targetPosition - transform.position).normalized;
        
        transform.DOMove(targetPosition, arriveTime - GameManager.Wave.waveTime).SetEase(Ease.Linear);
        yield return null;
    }

    private void TriggeredMove()
    {
        transform.position += dir * _constantSpeed * Time.fixedDeltaTime;
    }
    

    // 손에 맞거나 뒤 trigger pad에 닿았을 경우 setActive(false)
    public void EndInteraction()
    {
        _meshRenderer.enabled = false;
        if(spriteRenderer != null) spriteRenderer.enabled = false; 
        
        _rigidbody.velocity=Vector3.zero;
        _rigidbody.angularVelocity=Vector3.zero;
        _rigidbody.Sleep();

        _breakable.m_Destroyed = false;

        StartCoroutine(ActiveTime(1f));
    }

    IEnumerator TriggerArrivalAreaEndInteraction()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("trigger arrival");
        _meshRenderer.enabled = false;
        if(spriteRenderer != null) spriteRenderer.enabled = false; 
        
        _isArrivalAreaHit = false;
        _rigidbody.velocity=Vector3.zero;
        _rigidbody.angularVelocity=Vector3.zero;
        _rigidbody.Sleep();

        _breakable.m_Destroyed = false;
        
        StartCoroutine(ActiveTime(1f));
    }
    private IEnumerator ActiveTime(float coolTime)
    {
        yield return new WaitForSecondsRealtime(coolTime); // coolTime만큼 활성화
        _meshRenderer.enabled = true;
        transform.gameObject.SetActive(false); // coolTime 다 됐으니 비활성화
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ArrivalArea"))
        {
            Debug.Log($"[Punch] Arrive! {beatNum} Beat ");
            _isArrivalAreaHit = true;
            StartCoroutine(TriggerArrivalAreaEndInteraction());
        }
        if (other.CompareTag("TriggerPad"))
        {
            // 뒤에 존재하는 곰돌이 공격 성공 처리
            // Debug.Log($"{gameObject.name} Trigger Pad");
            //Debug.Log($"End Interaction {gameObject.name} Trigger Trigger Pad {other.name}");

            _isArrivalAreaHit = true;
            StartCoroutine(TriggerArrivalAreaEndInteraction());
        }
    }
}
