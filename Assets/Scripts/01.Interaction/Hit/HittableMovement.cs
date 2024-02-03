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
    private float moveTime = 3.0f; // 토핑의 이동 속도를 결정함
    private float popTime = 0.5f; // 토핑의 점프 시간을 결정함
    
    [Header("other Variable (AUTO)")] 
    [SerializeField] private GameObject refrigerator;
    [SerializeField] private toppingState curState = toppingState.idle;
    private float distancePlayer = 3.5f;
    private GameObject _player;

    //토핑이 움직이기 위한 변수
    [SerializeField] private float _idleTime;
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
    
    private float _waitStartTime = 0.0f;
    private float _waitTime = 1.0f;
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
        _rigidbody = GetComponent<Rigidbody>();
        _player = GameObject.FindWithTag("Player");
    }
    

    private IEnumerator ActiveTime(float coolTime)
    {
        yield return new WaitForSeconds(coolTime); // coolTime만큼 활성화
        gameObject.SetActive(false); // coolTime 다 됐으니 비활성화
    }

    /// <summary>
    /// 해당 토핑이 도착하고자하는 arrival area의 index,
    /// 해당 토핑이 도착하고자하는 arrival time을 지정함
    /// </summary>
    /// <param name="arrivalBox">arrival area의 index</param>
    /// <param name="arriveTime">arrival time</param>


    private void Update()
    {
        //if (GameManager.Instance.currentGameState == GameState.Waving)
        {
            // 현재 토핑 상태 Update
            UpdateToppingState();

            // 처리되지 못한 토핑들 자동 처리
            if (GameManager.Wave.waveTime >= arriveTime + 1.0f && _isMoved)
            {
                //Debug.Log("처리되지 못함");
                curState = toppingState.refrigerator;
                _isNotHitted = true;
            }
        }
    }
    public void InitializeTopping(NodeInfo node)
    {
        arrivalBoxNum = node.arrivalBoxNum;
        arriveTime = node.timeToReachPlayer;
        sideType = node.sideType;

        InitiateVariable();
        _arrivalBoxPos = GameManager.Wave.GetArrivalPosition(arrivalBoxNum);
    }

    private void InitiateVariable()
    {
        _rigidbody.WakeUp();
        refrigerator = GameObject.FindWithTag("Refrigerator"); // TODO: Scene 내에 냉장고 오브젝트에 Refrigerator tag 설정
        curState = toppingState.idle;
        
        // 생성된 이후, 가만히 있는 시간을 결정합니다.
        // idle time 이후 튀어오르고, moveTime 동안 움직이게 됩니다.
        _idleTime = arriveTime - (popTime + moveTime + GameManager.Wave.waveTime);
        //Debug.Log(this.transform.name + "의 Idle time은 " + _idleTime);
        //_waitTime = 2.0f;
        //_inTime = 1.5f;

        _isJumped = false;      // 1) 토스트기 위로 점프했나요?
        _isMoved = false;       // 2) Player를 향해 움직이고 있나요?
        _isHitted = false;      // 3) 막대에 의해 맞았나요?
        _waitStartTime = 0.0f;  // 4) WaitForSeconds() 함수를 위한 초기 시간 변수
        _isWaiting = false;     // 5) WaitForSeconds() 함수를 사용 중인가요?
        _isNotHitted = false;   // 6) Player의 막대를 통해 처리되지 못한 경우
        _goTo = false;          // 7) 냉장고로 향하는 코드를 1번 실행하기 위한 변수
    }
    
    public void MoveToPlayer()
    {
        //float timeElapsed = arriveTime - _moveToppingTime;
        Vector3 firstPos = transform.position;
        Vector3 secondPos = firstPos + new Vector3(0, 0.5f, 0);
        Vector3 fourthPos = _arrivalBoxPos;
        
        this.transform.LookAt(_player.transform);
        
        //Debug.Log(timeElapsed);
        transform.DOJump(transform.position + new Vector3(0, 1.0f, 0),
            2f, 1, popTime);
        
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

    public void GoToRefrigerator()
    {
        Vector3 firstPos = transform.position;
        Vector3 thirdPos = refrigerator.transform.position;
        Vector3 secondPos = firstPos + (thirdPos - firstPos) / 2;

        float height = UnityEngine.Random.Range(1.0f, 3.0f);
        
        transform.DOPath(new[]
            {
                new Vector3(firstPos.x, firstPos.y, firstPos.z),
                new Vector3(secondPos.x, secondPos.y + height, secondPos.z),
                new Vector3(thirdPos.x, thirdPos.y, thirdPos.z),
            },
            _inTime,
            PathType.CatmullRom, PathMode.Full3D);
        this.GetComponent<Rigidbody>().useGravity = false;
        
        _rigidbody.velocity=Vector3.zero;
        _rigidbody.angularVelocity=Vector3.zero;
        _rigidbody.Sleep();
        
        StartCoroutine(ActiveTime(_inTime + 0.5f));
    }

    private void SetWaitTime(float sec)
    {
        // sec초 동안 기다립니다.
        _isWaiting = true;
        _waitStartTime = GameManager.Wave.waveTime;
        _waitTime = sec;
    }

    private void OnCollisionEnter(Collision other)
    {          
        // FOR DEBUG
        Debug.Log("[DEBUG]" + this.transform.name + "이 "+ other.transform.name+ "와 충돌함. \n현재 상태는 " + curState);
        
        if (curState == toppingState.interacable)
        {
            // FOR DEBUG
            Debug.Log("[DEBUG] " + this.transform.name + "이 "+ other.transform.name+ "와 충돌함. \n현재 상태는 " + curState);
            Debug.Log("[DEBUG] "+this.transform.name + "의 충돌 감지 시간은 " + GameManager.Wave.waveTime + ", 목표 시간은 " + arriveTime);
            
            //DOTween.KillAll();

            bool IsRight = false;

            //잘못 충돌한 예외 처리
            if (!other.transform.TryGetComponent(out Rigidbody body))
                return;
            
            // hitter의 side 색과 일치한 topping일 경우
            InteractionSide colSide = (InteractionSide)Enum.Parse(typeof(InteractionSide), body.name);
            if (colSide == sideType)
            {
                IsRight = true;
            }
            else
            {
                // Collider 감지가 잘못된 경우, 예외 처리를 위해서 추가함
                IsRight = UpOrDown(other, colSide); 
                //if (IsRight) Debug.Log("예외 처리 성공");
            }

            // Controller / Hand_R/L의 HandData에서
            // 속도 값 받아와서 Hit force로 사용함
            var parent = other.transform.parent.parent.parent;
            float hitForce = parent.GetChild(0).GetComponent<HandData>().ControllerSpeed * 5.0f;
            
            // 충돌 지점 기준으로 날아가게
            
            Vector3 dir = other.contacts[0].normal.normalized;
            _rigidbody.AddForce(dir * hitForce, ForceMode.Impulse);
            _rigidbody.useGravity = true;
            
            // Set Score & State
            GameManager.Score.ScoringHit(this.gameObject, IsRight);
            curState = toppingState.refrigerator;            

        }
        else
            return;
    }
    
    private bool UpOrDown(Collision _col, InteractionSide type)
    {
        //오른면(초록색)에 존재하면, 위에 부딪혀야 정상적으로 감지 처리가 된 것
        //왼면(빨간색)에 존재하면, 아래에 부딪혀야 정상적으로 감지 처리가 된것
        
        Vector3 distVec = transform.position - _col.transform.position;
        
        //오른손 법칙을 사용해보면 반시계 방향 >> 엄지의 방향이 양수 : 벽의 위에 부딪힘
        //오른손 법칙을 사용해보면 시계 방향 >> 엄지의 방향이 음수 : 벽의 아래에 부딪힘
        if (Vector3.Cross(_col.transform.right, distVec).z > 0)
        {
            //_col.transform.right는 충돌체의 오른쪽 방향벡터
            //Debug.Log("Up : 벽의 위에 부딪힘");
            switch (type)
            {
                case InteractionSide.Red: return true;      //정상적으로 감지가 안된 것임. 따라서 Player는 맞은편으로 친 것임
                case InteractionSide.Green: return false;   //정상적으로 감지가 된 것임. 따라서 Player가 잘못 친 것임
            }
        }
        else
        {
            switch (type)
            {
                case InteractionSide.Red:return false;      //정상적으로 감지가 된 것임. 따라서 Player가 잘못 친 것임
                case InteractionSide.Green:return true;     //정상적으로 감지가 안된 것임. 따라서 Player는 맞은편으로 친 것임
            }
        }

        return false;
    }

    private void CanInteractTopping()
    {
        // refrigerator로 향하는 것이 아니라면(아직 인터렉션을 하지 않았다면), 토핑과 상호작용할 수 있는 상황인지 체크한다.

        // Player와의 거리가 distancePlayer만큼 다가오면 활성화되도록.
        _curDistance = (this.transform.position - _player.transform.position).sqrMagnitude;
        //Debug.Log(_curDistance);
        if (_curDistance <= distancePlayer)
        {
            //Debug.Log("[DEBUG] 충돌 가능합니다.");
            curState = toppingState.interacable;
        }
        else
        {
            curState = toppingState.uninteracable;
        }
    }

    private void JumpOneTime(float time)
    {
        transform.DOJump(transform.position + new Vector3(0, 1.0f, 0),
            2f, 1, time);
    }

    private void SetToppingMove()
    {
        _moveStartPos = transform.position;
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
                    curState = toppingState.uninteracable;
                else
                    _idleTime -= Time.deltaTime;
                break;

            // case toppingState.jump:
            //     if (!_isJumped)
            //     {
            //         JumpOneTime(popTime);
            //         // popTime 동안 wait
            //         //SetWaitTime(popTime);
            //         _isJumped = true;
            //     }
            //     else //if (GameManager.Wave.waveTime >= _waitStartTime + _waitTime)
            //     {
            //         // 해당 오브젝트가 플레이어를 향해 움직이기 위한 설정값 지정
            //         SetToppingMove();
            //         curState = toppingState.uninteracable;
            //     }
            //     break;
            
            case toppingState.uninteracable:
                if (!_isMoved)
                {
                    MoveToPlayer();
                }
                else
                {
                    CanInteractTopping();
                }
                break;

            case toppingState.interacable:
                // 중력의 영향을 받되, 천천히 떨어질 수 있도록 함
                _rigidbody.useGravity = true;
                //_rigidbody.AddForce(0, 0, +1.0f);
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
