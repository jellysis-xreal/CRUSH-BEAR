using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EnumTypes;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class PunchableMovement : MonoBehaviour, IPunchableMovement
{
    // TODO : Topping 생성 시 지정해줘야 하는 변수들
    [Header("Setting Variable")]
    // public int arrivalBoxNum = 0; // 목표인 Box index number
    // public float arriveTime; // Node Instantiate
    public uint beatNum;
    public uint typeIndex;

    [Header("other Variable (AUTO)")] 
    private Vector3 targetPosition;
    
    // 토핑이 움직이기 위한 변수 
    //public Transform parentTransform;
    public Rigidbody _rigidbody;
    private Vector3 dir = new Vector3();
    //public CookieControl cookieControl;
    //private float moveDistance = 0f;
    private int shootStandard;
    
    // 토핑이 맞은, 맞지 않은 후에 활용할 변수
    //private bool _isHit = false;
    private bool _isArrivalAreaHit = false; // 박스 트리거된 이후, 바로 직전의 움직임을 유지할 때 사용하는 변수
    public MeshRenderer _meshRenderer;
    public SpriteRenderer spriteRenderer; 
    public Breakable _breakable;
    public CookieControl _cookieControl;
    private CancellationTokenSource breakCancel;
    public void StartMovement()
    {
        
    }
  
    public void InitializeToppingRoutine(NodeInfo node)
    {
        breakCancel = new CancellationTokenSource();
        _isArrivalAreaHit = false;
        // arrivalBoxNum = node.arrivalBoxNum;
        // arriveTime = node.timeToReachPlayer;
        transform.rotation = Quaternion.identity;
        
        _meshRenderer.enabled = true;
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        
        _rigidbody.velocity=Vector3.zero;
        _rigidbody.angularVelocity=Vector3.zero;
        _rigidbody.WakeUp();
        
        transform.position = GameManager.Wave.GetSpawnPosition(node.arrivalBoxNum);
        targetPosition = GameManager.Wave.GetArrivalPosition(node.arrivalBoxNum);

        dir = transform.position - targetPosition;
        shootStandard = GameManager.Instance.Metronome.shootStandard;
        GameManager.Instance.Metronome.BindEvent(CheckBeat);
        // _cookieControl.Init();
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

        ActiveTime(1f).Forget();
    }

    async UniTask TriggerArrivalAreaEndInteraction(CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return;
        await UniTask.WaitForSeconds(1, cancellationToken: token);
        _meshRenderer.enabled = false;
        if(spriteRenderer != null) spriteRenderer.enabled = false; 
        else if(transform.childCount == 2)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.enabled = false;
        }
        if(!_breakable.m_Destroyed) GameManager.Score.ScoringMiss(this.gameObject);
        
        _rigidbody.velocity=Vector3.zero;
        _rigidbody.angularVelocity=Vector3.zero;
        _rigidbody.Sleep();

        // _breakable.m_Destroyed = false;
        
        ActiveTime(1f).Forget();
    }
    async UniTask ActiveTime(float coolTime)
    {
        if(breakCancel != null)
        {
            breakCancel.Cancel();
            breakCancel.Dispose();
            breakCancel = null;
        }
        await UniTask.WaitForSeconds(coolTime);
        transform.gameObject.SetActive(false); // coolTime 다 됐으니 비활성화
        _breakable.m_Destroyed = false;
        _meshRenderer.enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ArrivalArea") && !_isArrivalAreaHit)
        {
            _isArrivalAreaHit = true;
            if(breakCancel != null)
                TriggerArrivalAreaEndInteraction(breakCancel.Token).Forget();
        }
    }

    public void CheckBeat(int currentBeat)
    {
        if (beatNum  <= currentBeat + shootStandard)
        {
            transform.position = dir * ((beatNum - currentBeat)/ (float)shootStandard);
            // Debug.LogWarning("[YES] "+beatNum +"번째 현재 모양 :" + _breakable._childTriggerChecker.handMotion);
            transform.DOMove(targetPosition, (float)GameManager.Instance.Metronome.secondsPerBeat * (beatNum - currentBeat)).SetEase(Ease.Linear)
                .OnComplete(() => transform.DOMoveZ(-0.3f,0.5f));
            _cookieControl.Init();
            GameManager.Instance.Metronome.UnBindEvent(CheckBeat);
        }
        else
        {
            return;
            //Debug.LogWarning("[NO] "+beatNum +"번째 현재 모양 :" + _breakable._childTriggerChecker.handMotion);
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
