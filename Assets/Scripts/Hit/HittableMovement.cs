using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using EnumTypes;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine.Serialization;

public class HittableMovement : MonoBehaviour
{
    // TODO: 해당 Topping 생성 시 지정해줘야 하는 변수들
    [Header("Setting Variable")] 
    public int arrivalBoxNum = 0; // 목표인 Box index number
    public float arriveTime;
    public InteractionSide sideType = InteractionSide.Red;
    [SerializeField] private float moveTime = 2.0f; // 토핑의 이동 속도를 결정함
    [SerializeField] private float _popTime = 1.0f; // 토핑의 점프 시간을 결정함
    
    [Header("other Variable (AUTO)")] 
    [SerializeField] private ObjectArrivalAreaManager arrivalArea; // Scene내의 arrival area
    [SerializeField] private GameObject refrigerator;
    [SerializeField] private toppingState curState = toppingState.idle;
    private float distancePlayer = 3.0f;

    //토핑이 움직이기 위한 변수
    [SerializeField] private float _idleTime;
    private float _moveToppingTime;
    private Rigidbody _rigidbody;
    private Vector3 _moveStartPos;
    private Vector3 _arrivalBoxPos;
    private Vector3 _initVelocity;

    //토핑이 맞은 후에 활용할 변수
    private Vector3 _moveVector;
    private float _moveSpeed;
    float _inTime = 1.5f;

    private float _curDistance;
    private bool _isJumped = false;
    private bool _isMoved = false;
    private bool _isHitted = false;
    
    private float _toppingTime = 0.0f;
    private float _waitStartTime = 0.0f;
    private float _waitTime = 2.0f;
    private bool _isWaiting = false;
    private bool _isNotHitted = false;
    private bool _goTo = false;

    private enum toppingState
    {
        idle,           // 토핑이 생성되었으나, 아직 움직이지 않는 상태
        jump,           // 토핑이 토스트 위로 점프. Player에게 다가갈 준비 상태
        uninteracable,  // Player에게 포물선으로 날아간다. 아직 interact 불가함
        interacable,    // Player가 interaction 가능한 범위내에 있는 경우
        refrigerator    // interact 성공/실패 판정 완료. 냉장고로 들어가기
    }

    private void Awake()
    {
        arrivalArea = GameObject.FindWithTag("ArrivalAreaParent").GetComponent<ObjectArrivalAreaManager>();
        _rigidbody = GetComponent<Rigidbody>();
        
        // TODO: Scene 내에 냉장고 오브젝트에 Refrigerator tag 설정
        refrigerator = GameObject.FindWithTag("Refrigerator");
    }

    private void Start()
    {
        // 생성된 이후, 가만히 있는 시간을 결정합니다.
        // idle time 이후 튀어오르고, moveTime 동안 움직이게 됩니다.
        _idleTime = arriveTime - (_popTime + moveTime + GameManager.Wave.waveTime);
        
        // TODO: 원래는 생성해주면서 call 해줘야 함
        InitialTopping(arrivalBoxNum, arriveTime);
    }

    private void Update()
    {
        _toppingTime += Time.deltaTime;
        

        if (GameManager.Wave.waveTime >= arriveTime + 2.0f)
        {
            curState = toppingState.refrigerator;
            _isNotHitted = true;
        }
        UpdateToppingState();
    }

    private void OnDestroy()
    {
        //Debug.Log(this.name + "Destory 토핑 : " + _toppingTime);
        //Debug.Log(this.name + "포물선으로 날아가는데 걸린 시간은 : " + _testTime);
    }

    /// <summary>
    /// 해당 토핑이 도착하고자하는 arrival area의 index,
    /// 해당 토핑이 도착하고자하는 arrival time을 지정함
    /// </summary>
    /// <param name="arrivalBox">arrival area의 index</param>
    /// <param name="arriveTime">arrival time</param>
    public void InitialTopping(int arrivalBox, float time)
    {
        arrivalArea.setting();
        
        arrivalBoxNum = arrivalBox;
        arriveTime = time;

        _arrivalBoxPos = arrivalArea.arrivalAreas[arrivalBoxNum].position;
    }
    
    public void MoveToPlayer()
    {
        _arrivalBoxPos = arrivalArea.arrivalAreas[arrivalBoxNum].position;
        
        //float timeElapsed = arriveTime - _moveToppingTime;
        Vector3 firstPos = transform.position;
        Vector3 secondPos = firstPos + new Vector3(0, 1.0f, 0);
        Vector3 fourthPos = _arrivalBoxPos;
        
        //Debug.Log(timeElapsed);
        
        transform.DOPath(new[]
            {
                new Vector3(firstPos.x, firstPos.y, firstPos.z),
                new Vector3(secondPos.x, secondPos.y, secondPos.z),
                //new Vector3(thirdPos.x, thirdPos.y, thirdPos.z),
                new Vector3(fourthPos.x, fourthPos.y, fourthPos.z)
            },
            moveTime,
            PathType.CatmullRom, PathMode.Full3D).SetEase(Ease.InQuint);

        _isMoved = true;
    }
    
    public void MoveToPlayer_()
    {
        _arrivalBoxPos = arrivalArea.arrivalAreas[arrivalBoxNum].position;

        float timeElapsed = (GameManager.Wave.waveTime -_moveToppingTime) / moveTime;
        //Debug.Log("움직이는 중 :" + timeElapsed);
        //Vector3 posVector = CalculateBezierCurve(_startPos, _arrivalBoxPos, timeElapsed).normalized;
        Vector3 posVector = CalculateBezierCurve(_moveStartPos, _arrivalBoxPos, timeElapsed);
        this.transform.position = posVector;

        //_rigidbody.AddForce(posVector);
    }
    
    // 베지어 곡선을 이용한 포물선 운동 계산
    private Vector3 CalculateBezierCurve(Vector3 start, Vector3 end, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * start; // (1-t)^2 * start
        point += 2 * u * t * (start + (end - start) * 0.5f); // 2(1-t)t * midpoint
        point += tt * end; // t^2 * end

        return point;
    }

    public void GoToRefrigerator()
    {
        Vector3 firstPos = transform.position;
        Vector3 thirdPos = refrigerator.transform.position;
        Vector3 secondPos = firstPos + (thirdPos - firstPos) / 2;

        transform.DOPath(new[]
            {
                new Vector3(firstPos.x, firstPos.y, firstPos.z),
                new Vector3(secondPos.x, secondPos.y + 1.0f, secondPos.z),
                new Vector3(thirdPos.x, thirdPos.y, thirdPos.z),
            },
            _inTime,
            PathType.CatmullRom, PathMode.Full3D);
        this.GetComponent<Rigidbody>().useGravity = false;

        Debug.Log(this.transform.name + "냉장고로!");
        Destroy(this.gameObject, _inTime + 0.5f);
    }

    private void WaitForSeconds(float sec)
    {
        // sec초 동안 기다립니다.
        _isWaiting = true;
        _waitStartTime = GameManager.Wave.waveTime;
        _waitTime = sec;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (curState == toppingState.interacable)
        {
            DOTween.KillAll();
            
            Debug.Log("[DEBUG] 충돌 : " + other.transform.name);
            bool IsRight = false;

            if (!other.transform.TryGetComponent(out Rigidbody body))
                return;
            
            // hitter의 side 색과 일치한 topping일 경우
            if (body.name == sideType.ToString()) IsRight = true;
            
            // Controller / Hand_R/L의 HandData에서
            // 속도 값 받아와서 Hit force로 사용함
            var parent = other.transform.parent.parent.parent;
            float hitForce = parent.GetChild(0).GetComponent<HandData>().ControllerSpeed * 5.0f;
            
            // 충돌 지점 기준으로 날아가게
            //Vector3 dir = transform.position - other.gameObject.transform.position;
            Vector3 dir = other.contacts[0].normal.normalized;
            //dir = dir.normalized;
            _rigidbody.AddForce(dir * hitForce, ForceMode.Impulse);

            GameManager.Score.ScoringHit(this.gameObject, IsRight);
            Debug.Log("[DEBUG] 충돌 감지, 시간은 " + GameManager.Wave.waveTime);
            curState = toppingState.refrigerator;
        }
        else
            return;
    }

    private void CanInteractTopping()
    {
        // Player와의 거리가 distancePlayer만큼 다가오면 활성화되도록.
        _curDistance = (this.transform.position - GameManager.Player.player.transform.position).sqrMagnitude;
        if (_curDistance <= distancePlayer)
            curState = toppingState.interacable;
        else
            curState = toppingState.uninteracable;
    }

    private void JumpOneTime(float time)
    {
        transform.DOJump(transform.position + new Vector3(0, 2.0f, 0),
            2f, 1, time);
        _isJumped = true;
    }

    private void SetToppingMove()
    {
        _moveStartPos = transform.position;
        _moveToppingTime = GameManager.Wave.waveTime;
        //float leftTime = arriveTime - _startTime;
        
        // A에서 B까지의 거리와 방향 계산
        Vector3 toTarget = _arrivalBoxPos - _moveStartPos;
        float distance = toTarget.magnitude;
        Vector3 direction = toTarget.normalized;

        // 초기 속도 계산
        //_initVelocity = direction * (distance / leftTime);
        //GetComponent<Rigidbody>().velocity = _initVelocity;
    }
    
    private void UpdateToppingState()
    {
        switch (curState)
        {
            case toppingState.idle:
                if (_idleTime <= 0.0f)
                    curState = toppingState.jump;
                else
                    _idleTime -= Time.deltaTime;
                break;

            case toppingState.jump:
                if (!_isJumped)
                {
                    JumpOneTime(_popTime);
                    // popTime 동안 wait
                    WaitForSeconds(_popTime);
                }
                else
                {
                    // 해당 오브젝트가 플레이어를 향해 움직이기 위한 설정값 지정
                    if (_isWaiting && GameManager.Wave.waveTime >= _waitStartTime + _waitTime)
                    {
                        SetToppingMove();
                        curState = toppingState.uninteracable;
                        _isWaiting = false;
                    }
                }

                break;

            case toppingState.uninteracable:
                if (!_isMoved)
                {
                    MoveToPlayer();
                    _isMoved = true;
                    curState = toppingState.interacable;
                }

                // refrigerator로 향하는 것이 아니라면(아직 인터렉션을 하지 않았다면), 토핑과 상호작용할 수 있는 상황인지 체크한다.
                CanInteractTopping();

                break;

            case toppingState.interacable:
                // 중력의 영향을 받되, 천천히 떨어질 수 있도록 함
                _rigidbody.useGravity = true;
                _rigidbody.AddForce(0, 0, +3.0f);

                //잠시 topping이 날아갈 시간 주기
                WaitForSeconds(_inTime);
                break;

            case toppingState.refrigerator:
                if (!_isHitted)
                {
                    GoToRefrigerator();
                    _isHitted = true;
                }

                if (_isNotHitted && !_goTo)
                {
                    GoToRefrigerator();
                    _goTo = true;
                }
                break;
        }
    }
}
