using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EnumTypes;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class PunchableMovement : MonoBehaviour, IPunchableMovement
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
    //public Transform parentTransform;
    private Rigidbody _rigidbody;
    public float _constantSpeed = 0f;
    private Vector3 dir = new Vector3();
    //public CookieControl cookieControl;
    private float moveDistance = 0f;
    private int shootStandard;
    
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

    public void StartMovement()
    {
        
    }
  
    public IEnumerator InitializeToppingRoutine(NodeInfo node)
    {
        _isArrivalAreaHit = false;
        arrivalBoxNum = node.arrivalBoxNum;
        arriveTime = node.timeToReachPlayer;
        transform.rotation = Quaternion.identity;
        
        // Debug.Log($"[Punch] time diff {arriveTime - GameManager.Wave.waveTime} -> {transform.name}  ");
        // Debug.Log($"[Punch] Init {transform.name} ");
        //cookieControl.Init(targetPosition);
        GameManager.Instance.Metronome.BindEvent(CheckBeat);
        _meshRenderer.enabled = true;
        if (spriteRenderer != null) spriteRenderer.enabled = true;

        _rigidbody.WakeUp();
        this.transform.position = GameManager.Wave.GetSpawnPosition(arrivalBoxNum);
        targetPosition = GameManager.Wave.GetArrivalPosition(arrivalBoxNum);

        dir = transform.position - targetPosition;
        shootStandard = GameManager.Instance.Metronome.shootStandard;
        if (beatNum < shootStandard)
        {
            transform.position = dir * beatNum / shootStandard;
            Debug.Log($"{beatNum}번째 노드 생성됨");
            yield return new WaitUntil(() => GameManager.Instance.Metronome.IsBeated());
            transform.DOMove(targetPosition, (float)GameManager.Instance.Metronome.secondsPerBeat * Mathf.Min(shootStandard, beatNum)).SetEase(Ease.Linear);
        }
        else
            transform.position = dir;
        yield return null;
    }

    
    // 손에 맞거나 뒤 trigger pad에 닿았을 경우 setActive(false)
    public void EndInteraction()
    {
        _meshRenderer.enabled = false;
        if(spriteRenderer != null) spriteRenderer.enabled = false;
        else if(transform.childCount == 2)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.enabled = false;
        }
        
        _rigidbody.velocity=Vector3.zero;
        _rigidbody.angularVelocity=Vector3.zero;
        _rigidbody.Sleep();

        StartCoroutine(ActiveTime(1f));
    }

    IEnumerator TriggerArrivalAreaEndInteraction()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("trigger arrival");
        _meshRenderer.enabled = false;
        if(spriteRenderer != null) spriteRenderer.enabled = false; 
        else if(transform.childCount == 2)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.enabled = false;
        }
        _isArrivalAreaHit = false;
        _rigidbody.velocity=Vector3.zero;
        _rigidbody.angularVelocity=Vector3.zero;
        _rigidbody.Sleep();

        // _breakable.m_Destroyed = false;
        
        StartCoroutine(ActiveTime(1f));
    }
    private IEnumerator ActiveTime(float coolTime)
    {
        yield return new WaitForSecondsRealtime(coolTime); // coolTime만큼 활성화
        transform.gameObject.SetActive(false); // coolTime 다 됐으니 비활성화
        _breakable.m_Destroyed = false;
        _meshRenderer.enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ArrivalArea"))
        {
            // Debug.Log($"[Punch] Arrive! {beatNum} Beat ");
            _isArrivalAreaHit = true;
            StartCoroutine(TriggerArrivalAreaEndInteraction());
            // StartCoroutine(TriggeredMovement());
        }
        if (other.CompareTag("TriggerPad"))
        {
            // 뒤에 존재하는 곰돌이 공격 성공 처리
            // Debug.Log($"{gameObject.name} Trigger Pad");
            //Debug.Log($"End Interaction {gameObject.name} Trigger Trigger Pad {other.name}");

            _isArrivalAreaHit = true;
            StartCoroutine(TriggerArrivalAreaEndInteraction());
            // StartCoroutine(TriggeredMovement());
            // GameManager.Player.MinusPlayerLifeValue();
            // other.GetComponent<BGBearManager>().MissNodeProcessing(this.gameObject);
        }
    }

    public void CheckBeat(int currentBeat)
    {
        if (beatNum < shootStandard)
            return;
        if(beatNum  == currentBeat + shootStandard)
        {
            // Debug.Log($"{currentBeat}번째 노드 생성됨");
            transform.DOMove(targetPosition, (float)GameManager.Instance.Metronome.secondsPerBeat * Mathf.Min(shootStandard, beatNum)).SetEase(Ease.Linear);
        }
    }

    #region Legacy Code
    /*private void InitiateVariableEarly()
    {
        _meshRenderer.enabled = true;
        if(spriteRenderer != null) spriteRenderer.enabled = true; 
        
        _rigidbody.WakeUp();
        //this.transform.position = GameManager.Wave.GetSpawnPosition(arrivalBoxNum);
        targetPosition = GameManager.Wave.GetArrivalPosition(arrivalBoxNum);

        dir = parentTransform.position - targetPosition;
        if (beatNum < 7)
            parentTransform.position = dir * beatNum / 7;
        else
            parentTransform.position = dir;
        // Debug.Log($"[punch] pos {parentTransform.name} dir : {dir}, value : {((7 - (arriveTime - GameManager.Wave.waveTime)) / 7f)}");
        // Debug.Log($"[punch] pos {parentTransform.name} : {parentTransform.position}");
        parentTransform.DOMove(targetPosition, (float)GameManager.Instance.Metronome.secondsPerBeat * Mathf.Min(7, beatNum)).SetEase(Ease.Linear);
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
        _constantSpeed = Vector3.Distance(targetPosition, parentTransform.position) / time;
        Debug.Log($"[punch] constant speed : {_constantSpeed} name : {parentTransform.gameObject.name} pos : {parentTransform.position}");
        // Debug.Log($"[punch] {parentTransform.gameObject.name} time : {time}, arriveTime : {arriveTime} ");
        Debug.Log($"[punch] distance : {Vector3.Distance(targetPosition, transform.position)} {parentTransform.gameObject.name} ");
    }

    private void Move()
    {
        dir = (targetPosition - parentTransform.position).normalized;
        parentTransform.position += dir * _constantSpeed * Time.fixedDeltaTime;
    }

    IEnumerator Movement(float time)
    {
        parentTransform.DOMove(targetPosition, time).SetEase(Ease.Linear);
        yield return null;
    }
    IEnumerator Movement()
    {
        Debug.Log($"[Punch] Movement {GameManager.Wave.currentBeatNum} Start Pos {parentTransform.position}, Time : {arriveTime - GameManager.Wave.waveTime}");
        float time = (float)GameManager.Instance.Metronome.secondsPerBeat * 7;
        _constantSpeed = Vector3.Distance(targetPosition, parentTransform.position) / time;
        moveDistance = Vector3.Distance(targetPosition, parentTransform.position);
        dir = (targetPosition - parentTransform.position).normalized;
        
        parentTransform.DOMove(targetPosition, time).SetEase(Ease.Linear);
        yield return null;
    }

    private void TriggeredMove()
    {
        parentTransform.position += dir * _constantSpeed * Time.fixedDeltaTime;
    }
    IEnumerator TriggeredMovement()
    {
        Debug.Log($"[Punch] TriggerMove Start {parentTransform.name} ");
        float triggerdTime = Time.time;
        while (Time.time - triggerdTime < 1)
        {
            parentTransform.position += dir * _constantSpeed * Time.fixedDeltaTime;
            yield return null;
        }
        //parentTransform.DOMove(parentTransform.position += dir * 5f, 2f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(1f);
        Debug.Log($"[Punch] TriggerMove Finished {parentTransform.name} ");
    }*/

    #endregion
}
