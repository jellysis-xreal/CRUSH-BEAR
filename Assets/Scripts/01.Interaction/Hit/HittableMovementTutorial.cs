using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using EnumTypes;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Sequence = DG.Tweening.Sequence;

public class HittableMovementTutorial : MonoBehaviour
{
    // TODO: 해당 Topping 생성 시 지정해줘야 하는 변수들
    [Header("Setting Variable")] 
    public int arrivalBoxNum = 0; // 목표인 Box index number
    [SerializeField]
    public InteractionSide sideType = InteractionSide.Red;
    private float moveTime = 3.3f; // 토핑의 이동 속도를 결정함
    private float popTime = 0.1f; // 토핑의 점프 시간을 결정함
    public GameObject burstEffect;
    
    [Header("other Variable (AUTO)")] 
    [SerializeField] private GameObject refrigerator;
    [SerializeField] private toppingState curState = toppingState.idle;
    public float distancePlayer = 3.5f;
    public GameObject player;
    public Transform startTransform;
    private BaseObject _baseObject;
    private SkinnedMeshRenderer _meshRenderer;

    //토핑이 움직이기 위한 변수
    private Rigidbody _rigidbody;
    private Vector3 _moveStartPos;
    private Vector3 _arrivalBoxPos;
    private Vector3 _startBoxPos;
    private int shootStandard;

    //토핑이 맞은 후에 활용할 변수
    private float _inTime = 1.5f;
    
    // Bool 값
    //private bool _isInit = false;
    private float _curDistance;
    private bool _isMoved = false;
    private bool _isHitted = false; 
    private bool _isNotHitted = false;
    //private bool _goTo = false;
    
    [Space] [Header("Test")] public Transform arrivalBoxTransform;
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
        player = GameObject.FindWithTag("Player");
        _meshRenderer = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
    }
    
    private void Update()
    {
        // CanInteractTopping();
    }

    /// <summary>
    /// 해당 토핑이 도착하고자하는 arrival area의 index,
    /// 해당 토핑이 도착하고자하는 arrival time을 지정함
    /// </summary>
    /// <param name="arrivalBox">arrival area의 index</param>
    /// <param name="arriveTime">arrival time</param>
    
    [ContextMenu("Tennis")]
    public void InitializeTopping(TutorialTennisType hand, float startTime)
    {
        // 왼쪽부터 도착 위치 1, 3, 4, 2
        // 빨 파 파 빨
        // 왼손이고 빨간색이면 1번, 파란색이면 3번 (인덱스는 - 1 하기 :))
        // 왼손이고 빨간색이면 2번, 파란색이면 4번

        popTime = 1f;
        moveTime = 5f;
        InitiateVariable();

        if (hand == TutorialTennisType.LeftHand)
        {
            arrivalBoxNum = (sideType == InteractionSide.Red)?  0 : 2; 
            _arrivalBoxPos = GameManager.Wave.GetArrivalPosition(arrivalBoxNum);
            _startBoxPos = GameManager.Wave.GetSpawnPosition(arrivalBoxNum);
        }
        else if (hand == TutorialTennisType.RightHand)
        {
            arrivalBoxNum = (sideType == InteractionSide.Red)? 1 : 3; 
            _arrivalBoxPos = GameManager.Wave.GetArrivalPosition(arrivalBoxNum);
            _startBoxPos = GameManager.Wave.GetSpawnPosition(arrivalBoxNum);
        }
        // _arrivalBoxPos = GameManager.Wave.GetArrivalPosition(arrivalBoxNum); // arrivalBoxTransform.position; 
        // _startBoxPos = GameManager.Wave.GetSpawnPosition(arrivalBoxNum); // startTransform.position;
        InitializeBeforeStart();

        //_isInit = true;

        StartCoroutine(TennisRoutine(startTime));
    }

    IEnumerator TennisRoutine(float startTime)
    {
        // 지연 시간
        yield return new WaitForSeconds(startTime);

        yield return StartCoroutine(MoveToPlayer());

        GoToRefrigerator();
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
        
        //_isInit = false;
        _isMoved = false;       // 2) Player를 향해 움직이고 있나요?
        _isHitted = false;      // 3) 막대에 의해 맞았나요?
        _isNotHitted = false;   // 6) Player의 막대를 통해 처리되지 못한 경우
        //_goTo = false;          // 7) 냉장고로 향하는 코드를 1번 실행하기 위한 변수
        
        StopCoroutine("ExplodeAfterSeconds");
    }
    
    private void InitializeBeforeStart()
    {
        this.transform.position = _startBoxPos;
        this.transform.LookAt(arrivalBoxTransform);

        _rigidbody.velocity = new Vector3(0f, 0f, 0f);
        _rigidbody.angularVelocity = new Vector3(0f, 0f, 0f);
    }

    public IEnumerator MoveToPlayer()
    {
        curState = toppingState.interacable;
        
        //float timeElapsed = arriveTime - _moveToppingTime;
        Vector3 firstPos = transform.position;

        this.transform.LookAt(player.transform);

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
            PathType.CatmullRom, PathMode.Full3D);
        
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
            PathType.CatmullRom, PathMode.Full3D).SetEase(Ease.InQuint);

        sequence.Append(tweenJump).Append(tweenMove);
        sequence.Play();
        
        yield return new WaitForSeconds(popTime + moveTime + 0.5f);
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

    //private void OnCollisionEnter(Collision other)
    //{
    //    if(_isHitted) return;
    //    CanInteractTopping();
    //    Debug.Log("[DEBUGGING]" + this.transform.name + "이 " + other.transform.name + "와 충돌함. " +
    //              "\n현재 상태는 " + curState + ", bool: " + IsInteractable());

    //    if (other.gameObject.CompareTag("Plane")) return;

    //    if (IsInteractable() || curState == toppingState.interacable)
    //    {
    //        // FOR DEBUG
    //        //Debug.Log("[DEBUG] " + this.transform.name + "이 "+ other.transform.name+ "와 충돌함. \n현재 상태는 " + curState);
    //        Debug.Log("[DEBUG] "+this.transform.name + "의 충돌 감지 시간은 ");

    //        bool IsRight = false;

    //        // 잘못 충돌한 예외 처리
    //        if (!other.transform.TryGetComponent(out Rigidbody body))
    //            return;

    //        //DOTween.KillAll();

    //        // hitter의 side 색과 일치한 topping일 경우
    //        InteractionSide colSide = (InteractionSide)Enum.Parse(typeof(InteractionSide), body.name);
    //        Debug.Log($"hitter Side {colSide}");
    //        if (colSide == sideType)
    //        {
    //            IsRight = true;
    //        }
    //        else
    //        {
    //            // Collider 감지가 잘못된 경우, 예외 처리를 위해서 추가함
    //            IsRight = IsRightJudgment(other, colSide); 
    //            //if (IsRight) 
    //                //Debug.Log("[DEBUG] 예외 처리 성공");
    //            //else
    //                //Debug.Log("[DEBUG] 예외가 아니었군");
    //        }

    //        _isHitted = true;
    //        if (SceneManager.GetActiveScene().name == "03.TutorialScene")
    //        {
    //            if (IsRight)
    //            {
    //                Debug.Log("[Tennis] Succced");
    //                GameManager.TutorialTennis.processedNumber++;
    //                GameManager.TutorialTennis.succeedNumber++;
    //                GameManager.Score.ScoringHit(this.gameObject, IsRight);
    //            }
    //            else
    //            {
    //                Debug.Log("[Tennis] Failed");
    //                GameManager.TutorialTennis.processedNumber++;
    //                GameManager.Score.ScoringHit(this.gameObject, IsRight);
    //            }
    //        }
    //        // Set Score & State
    //        curState = toppingState.refrigerator;
    //    }
    //    else
    //        return;
    //}

    private void OnCollisionEnter(Collision other)
    {
        if (_isHitted) return;

        // Debug.Log("[DEBUGGING]" + this.transform.name + "이 " + other.transform.name + "와 충돌함. " +
        //           "\n현재 상태는 " + curState + ", bool: " + IsInteractable());

        if (other.gameObject.CompareTag("Plane")) return;

        if (curState == toppingState.interacable)
        {
            Debug.Log("[DEBUG] " + this.transform.name + "의 충돌 감지 시간은 ");
            bool IsRight = false;

            if (!other.transform.TryGetComponent(out Rigidbody body))
                return;

            InteractionSide colSide = (InteractionSide)Enum.Parse(typeof(InteractionSide), body.name);
            Debug.Log($"hitter Side {colSide}");
            if (colSide == sideType)
            {
                IsRight = true;
            }
            else
            {
                IsRight = IsRightJudgment(other, colSide);
            }

            _isHitted = true;
            if (SceneManager.GetActiveScene().name == "03.TutorialScene")
            {
                GameManager.TutorialTennis.processedNumber++;
                if (IsRight)
                {
                    Debug.Log("[Tennis] Succeeded");
                    GameManager.TutorialTennis.succeedNumber++;
                }
                else
                {
                    Debug.Log("[Tennis] Failed");
                }
                GameManager.Score.ScoringHit(this.gameObject, IsRight);
            }
            curState = toppingState.refrigerator;
        }
        else
            return;
    }



    private bool IsRightJudgment(Collision _col, InteractionSide type)
    {
        Transform otherSide = _col.transform;

        switch (type)
        {
            case InteractionSide.Red:
                otherSide = _col.transform.parent.GetChild(1); // Blue
                break;

            case InteractionSide.Blue:
                otherSide = _col.transform.parent.GetChild(0); // Red
                break;
        }

        if (otherSide.GetChild(0).TryGetComponent(out HitTrigger hit))
        {
            // Trigger되었다고 인식된 면과
            // 반대 면에서 Triggered라고 판단되어지면, return True 
            if (hit.isTriggered)
                return true;
            else
                return false;
        }

        return false;
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
                case InteractionSide.Blue:
                    //Debug.Log("[SWING] Up : 윗면에 부딪힘, 정상이 아님");
                    return true;      //정상적으로 감지가 안된 것임. 따라서 Player는 맞은편으로 친 것임
                case InteractionSide.Red:
                    //Debug.Log("[SWING] Up : 윗면에 부딪힘, 정상적으로 감지된 것");
                    return false;   //정상적으로 감지가 된 것임. 따라서 Player가 잘못 친 것임
            }
        }
        else
        {
            switch (type)
            {
                case InteractionSide.Blue:
                    //Debug.Log("[SWING] Down : 아래면에 부딪힘, 정상적으로 감지된 것");
                    return false;      //정상적으로 감지가 된 것임. 따라서 Player가 잘못 친 것임
                case InteractionSide.Red :
                    //Debug.Log("[SWING] Down : 아래면에 부딪힘, 정상이 아님");
                    return true;     //정상적으로 감지가 안된 것임. 따라서 Player는 맞은편으로 친 것임
            }
        }

        return false;
    }

    private bool IsInteractable()
    {
        _curDistance = (this.transform.position - player.transform.position).sqrMagnitude;
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
        GameManager.TutorialTennis.processedNumber++;
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
        _meshRenderer.enabled = true;
        _isHitted = false;
    }

}
