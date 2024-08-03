using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using EnumTypes;
using Sequence = DG.Tweening.Sequence;

public class HittableMovement : MonoBehaviour
{
    // TODO: 해당 Topping 생성 시 지정해줘야 하는 변수들
    [Header("Setting Variable")] 
    public int arrivalBoxNum = 0; // 목표인 Box index number
    [SerializeField] private uint beatNum;
    public InteractionSide sideType = InteractionSide.Red;
    private float moveTime = 3.3f; // 토핑의 이동 속도를 결정함
    private float popTime = 0.1f; // 토핑의 점프 시간을 결정함
    public GameObject burstEffect;
    private bool Debugging = false;
    
    [Header("other Variable (AUTO)")] 
    [SerializeField] private GameObject refrigerator;
    [SerializeField] private toppingState curState = toppingState.idle;
    private float distancePlayer = 3.5f;
    private GameObject _player;
    private BaseObject _baseObject;
    private MeshRenderer _meshRenderer;

    //토핑이 움직이기 위한 변수
    private Rigidbody _rigidbody;
    private Vector3 _moveStartPos;
    private Vector3 _arrivalBoxPos;
    private Vector3 _startBoxPos;
    private int shootStandard;
    private float _averageSpeed;

    //토핑이 맞은 후에 활용할 변수
    private float _inTime = 1.5f;
    
    // Bool 값
    private bool _isInit = false;
    private float _curDistance;
    private bool _isMoved = false;
    private bool _isHitted = false; 
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
        _baseObject = GetComponent<BaseObject>();
        _rigidbody = GetComponent<Rigidbody>();
        _player = GameObject.FindWithTag("Player");
        _meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        
        if (Debugging)
            curState = toppingState.interacable;
    }

    /// <summary>
    /// 해당 토핑이 도착하고자하는 arrival area의 index,
    /// 해당 토핑이 도착하고자하는 arrival time을 지정함
    /// </summary>
    /// <param name="arrivalBox">arrival area의 index</param>
    /// <param name="arriveTime">arrival time</param>

    public void InitializeTopping(NodeInfo node)
    {
        arrivalBoxNum = node.arrivalBoxNum;
        beatNum = node.beatNum;
        sideType = node.sideType;
        
        shootStandard = GameManager.Instance.Metronome.shootStandard;
        popTime = (float)GameManager.Instance.Metronome.secondsPerBeat;
        moveTime = popTime * (shootStandard - 1);
        InitiateVariable();
        _arrivalBoxPos = GameManager.Wave.GetArrivalPosition(arrivalBoxNum);
        _startBoxPos = GameManager.Wave.GetSpawnPosition(arrivalBoxNum);
        InitializeBeforeStart();

        _isInit = true;
        GameManager.Instance.Metronome.BindEvent(UpdateToppingState);
        GameManager.Instance.Metronome.BindEvent(RemoveTopping);
    }

    private void InitiateVariable()
    {
        refrigerator = GameObject.FindWithTag("Refrigerator"); // TODO: Scene 내에 냉장고 오브젝트에 Refrigerator tag 설정
        curState = toppingState.idle;
        
        // 생성된 이후, 가만히 있는 시간을 결정합니다.
        // idle time 이후 튀어오르고, moveTime 동안 움직이게 됩니다.
        //Debug.Log(this.transform.name + "의 Idle time은 " + _idleTime);
        InitateBoolean();
    }

    private void InitateBoolean()
    {
        _baseObject.InitScoreBool();
        
        _isInit = false;
        _isMoved = false;       // 2) Player를 향해 움직이고 있나요?
        _isHitted = false;      // 3) 막대에 의해 맞았나요?
        _isNotHitted = false;   // 6) Player의 막대를 통해 처리되지 못한 경우
        _goTo = false;          // 7) 냉장고로 향하는 코드를 1번 실행하기 위한 변수

        StopCoroutine("ExplodeAfterSeconds");
    }
    
    private void InitializeBeforeStart()
    {
        this.transform.position = _startBoxPos;
        this.transform.LookAt(GameManager.Player.player.transform);

        _rigidbody.velocity = new Vector3(0f, 0f, 0f);
        _rigidbody.angularVelocity = new Vector3(0f, 0f, 0f);
    }

    private void IsToppingArrived(Vector3 firstPos)
    {
        // Debug.Log("[JMH][DEBUG] " + this.transform.name + "이 도착함.");
        
        // 토핑이 점프하는 전체 거리를 계산합니다.
        Vector3 jumpStartPos = new Vector3(firstPos.x, firstPos.y, firstPos.z);
        Vector3 jumpEndPos = new Vector3(firstPos.x, firstPos.y + 2.0f, firstPos.z);
        float jumpDistance = Vector3.Distance(jumpStartPos, jumpEndPos);

        // 평균 속도를 계산합니다.
        _averageSpeed = jumpDistance / moveTime;
        _rigidbody.useGravity = true;
        _rigidbody.AddForce(transform.forward * _averageSpeed * 10.0f, ForceMode.VelocityChange);
    }

    public void MoveToPlayer()
    {
        Vector3 firstPos = transform.position;

        this.transform.LookAt(_player.transform);

        Sequence sequence = DOTween.Sequence();

        //Debug.Log(timeElapsed);
        //Tween tweenJump = transform.DOJump(firstPos + new Vector3(0, 1.0f, 0), 2f, 1, popTime);

        Tween tweenJump = transform.DOPath(new[]
            {
                new Vector3(firstPos.x, firstPos.y, firstPos.z),
                new Vector3(firstPos.x, firstPos.y + 1.0f, firstPos.z),
                new Vector3(firstPos.x, firstPos.y + 2.0f, firstPos.z)
            },
            popTime,
            PathType.CatmullRom, PathMode.Full3D).Pause();

        firstPos = transform.position;
        Vector3 upVector = transform.up.normalized * 4.0f;
        Vector3 forwardVector = transform.forward.normalized * 5.0f;
        Tween tweenMove = transform.DOPath(new[]
            {
                new Vector3(firstPos.x, firstPos.y, firstPos.z),
                new Vector3(firstPos.x, firstPos.y, firstPos.z) + upVector + forwardVector,
                new Vector3(_arrivalBoxPos.x, _arrivalBoxPos.y, _arrivalBoxPos.z)
            },
            moveTime,
            PathType.CatmullRom, PathMode.Full3D).SetEase(Ease.InQuint).Pause();
        tweenMove.onComplete = () =>
        {
            IsToppingArrived(firstPos);
        };
        sequence.Append(tweenJump).Append(tweenMove);

        //popTime = secondsPerBeat이다
        sequence.Goto(popTime * (shootStandard - (beatNum - GameManager.Instance.Metronome.currentBeat)));
        sequence.Play();
    }
    public void GoToRefrigerator()
    {
        Vector3 firstPos = transform.position;
        Vector3 thirdPos = refrigerator.transform.position;
        Vector3 secondPos = firstPos + (thirdPos - firstPos) / 2;

        float height = UnityEngine.Random.Range(1.0f, 3.0f);

        Tween tween = transform.DOPath(new[]
        {
            new Vector3(firstPos.x, firstPos.y, firstPos.z),
            new Vector3(secondPos.x, secondPos.y + height, secondPos.z),
            new Vector3(thirdPos.x, thirdPos.y, thirdPos.z),
        }, _inTime, PathType.CatmullRom, PathMode.Full3D);

        tween.onComplete = () =>
        {
            UnactiveObject();
        };
    }

    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log("[DEBUG] " + this.transform.name + "이 " + other.transform.name + "와 충돌함. \n현재 상태는 " + curState);
        if (_baseObject.IsItScored()) return;
        if (other.gameObject.CompareTag("Plane")) return;
        // if (!Debugging)
        // {
        //     CanInteractTopping();
        //     if (!IsInteractable()) return;
        //     if (curState != toppingState.interacable) return;
        // }
        
        // FOR DEBUG
        // Debug.Log("[DEBUG][JMH]" + this.transform.name + "이 " + other.transform.name + "와 충돌함. " +
        //           "\n현재 상태는 " + curState + ", bool: " + IsInteractable());
        //Debug.Log("[DEBUG] "+this.transform.name + "의 충돌 감지 시간은 " + GameManager.Wave.waveTime + ", 현재 비트는 " + beatNum);

        bool IsRight = false;

        // 잘못 충돌한 예외 처리
        if (!other.transform.TryGetComponent(out Rigidbody body))
            return;

        // hitter의 side 색과 일치한 topping일 경우
        InteractionSide colSide = (InteractionSide)Enum.Parse(typeof(InteractionSide), body.name);

        // Collider 감지가 잘못된 경우, 예외 처리를 위해서 추가함
        IsRight = IsRightJudgment(other, colSide);

        // Controller / Hand_R/L의 HandData에서 속도 값 받아와서 Hit force로 사용함
        var parent = other.transform.parent;
        float forceMagnitude = parent.GetComponent<Hitter>().handData.ControllerSpeed;
        //Debug.Log(forceMagnitude);
        forceMagnitude = Mathf.Clamp(forceMagnitude, 6.0f, 10.0f);
        
        // // 충돌 지점 기준으로 날아가
        Vector3 refrigeratorPosition = refrigerator.transform.position;
        Vector3 directionToRefrigerator = (refrigeratorPosition - transform.position).normalized;
        directionToRefrigerator.y += 0.3f;

        // 계산한 방향으로 힘을 가합니다.
        _rigidbody.useGravity = true;
        _rigidbody.AddForce(directionToRefrigerator * forceMagnitude, ForceMode.Impulse);

        // For Debug
        //Debug.Log("[SWING][SCORE] " + this.transform.name + "의 Side는 " + sideType + ", " + other.transform.name +
        //          "와 충돌함. 따라서 " + IsRight);

        // Set Score & State
        if (!Debugging) GameManager.Score.ScoringHit(this.gameObject, IsRight);
        _baseObject.SetScoreBool();
        curState = toppingState.refrigerator;
    }

    private bool IsRightJudgment(Collision _col, InteractionSide type)
    {
        if (type == sideType)
        {
            //Debug.Log("[SWING] 옳은 면에 맞았음");
            return true;
        }
        else
            return false;
    }

    private bool UpOrDown(Transform _col, InteractionSide type)
    {
        //오른면(초록색)에 존재하면, 위에 부딪혀야 정상적으로 감지 처리가 된 것
        //왼면(빨간색)에 존재하면, 아래에 부딪혀야 정상적으로 감지 처리가 된것

        Vector3 distVec = transform.position - _col.transform.position;

        //오른손 법칙을 사용해보면 반시계 방향 >> 엄지의 방향이 양수 : 벽의 위에 부딪힘
        //오른손 법칙을 사용해보면 시계 방향 >> 엄지의 방향이 음수 : 벽의 아래에 부딪힘
        if (Vector3.Cross(_col.right, distVec).z > 0)
        {
            //Debug.Log("[SWING] " + type + "가 앞을 바라본다");
            if (type == sideType)
                return true;
        }

        //Debug.Log("[SWING] " + type + "가 뒤을 바라본다");
        return false;
    }

    private bool IsInteractable()
    {
        _curDistance = (this.transform.position - _player.transform.position).sqrMagnitude;
        if (_curDistance <= distancePlayer)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    private void CanInteractTopping()
    {
        // refrigerator로 향하는 것이 아니라면(아직 인터렉션을 하지 않았다면), 토핑과 상호작용할 수 있는 상황인지 체크한다.

        // Player와의 거리가 distancePlayer만큼 다가오면 활성화되도록.
        if (IsInteractable())
            curState = toppingState.interacable;
        else
            curState = toppingState.uninteracable;
    }
    private IEnumerator ExplodeAfterSeconds(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        
        _meshRenderer.enabled = false;
        
        burstEffect.SetActive(true);
        ParticleSystem vfx = burstEffect.GetComponent<ParticleSystem>();
        vfx.Play();

        // ParticleSystem의 재생이 끝난 후에 GameObject를 비활성화
        yield return new WaitForSeconds(vfx.main.duration);
        vfx.Stop();
        burstEffect.SetActive(false);
        UnactiveObject();
    }

    private void UnactiveObject()
    {
        this.gameObject.SetActive(false);
        
        InitateBoolean();
        _meshRenderer.enabled = true;
        
        _rigidbody.useGravity = false;
        _rigidbody.velocity = new Vector3(0f, 0f, 0f);
        _rigidbody.angularVelocity = new Vector3(0f, 0f, 0f);
    }

    private void OnEnable()
    {
        burstEffect.SetActive(false);
        if (_meshRenderer == null)
            _meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        _meshRenderer.enabled = true;
    }

    private void UpdateToppingState(int beat)
    {
        switch (curState)
        {
            case toppingState.idle:
                if (beatNum <= beat + shootStandard)
                {
                    curState = toppingState.uninteracable;
                    MoveToPlayer();
                    _isMoved = true;
                    // 혹시 중간 지점에서 시작할 때 이미 interactable이 되는 구간을 지나왔다면 보정함.
                    CanInteractTopping();
                }
                break;

            case toppingState.uninteracable:
                if (_isMoved)
                    CanInteractTopping();
                break;

            case toppingState.interacable:
                _rigidbody.useGravity = true;
                break;

            case toppingState.refrigerator:
                if (this.gameObject.activeSelf == true &&!_isHitted && !_isNotHitted)
                {
                    GoToRefrigerator();
                    
                    if (this.gameObject.activeSelf == true)
                        StartCoroutine(ExplodeAfterSeconds(0.5f));
                    _isNotHitted = false;
                    _isHitted = true;
                    //Debug.Log("[DEBUG][JMH] "+this.gameObject.name+" 맞았다 Check");
                }

                if (this.gameObject.activeSelf == true && _isNotHitted && !_goTo)
                {
                    //Debug.Log("[DEBUG][JMH] 처리되지 못한 토핑 처리됨");
                    if (!_baseObject.IsItScored())
                    {
                        GameManager.Score.ScoringMiss(this.gameObject);
                        _baseObject.SetScoreBool();
                    }

                    GoToRefrigerator();
                    
                    _goTo = true;
                }
                
                break;
        }
    }

    private void RemoveTopping(int beat)
    {
        if (beat >= beatNum + 1 && _isMoved)
        {
            //Debug.Log("처리되지 못함");
            curState = toppingState.refrigerator;
            _isNotHitted = true;
        }
    }
}
