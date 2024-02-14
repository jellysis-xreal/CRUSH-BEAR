using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

// 정확한 위치와 박자에 노드 트리거 시키기 !!
public class JumpMovementToArrivalArea : MonoBehaviour, IMovement
{
    private Rigidbody _rigidbody;

    #region JumpMovement Variablel
    [Header("Variables Related to JumpMoving ")]
    public int arrivalAreaIndex = 0; // 목표로 하는 area의 인덱스
    public float timeToReachPlayer = 10f; // area까지 걸릴 시간
    public int totalJumpNumberOfTimes = 10; // area에 도달하기 까지 chd 점프할 횟수 n
    public Vector3 lastPositon;// 점프할 때 마다 마지막 떨어질 포지션 계산 다시해야 함.
    public int _jumpedNumberOfTimes = 0; // 생성된 이후로 점프한 횟수
    private int _distanceToNum = 0;
    public float jumpHeight = 0; // ObjectArrivalArea[index].transform.position.y와 동기화
    private List<Vector3> _checkablePositionList = new List<Vector3>(); // 최대 점프 지점, 낙하 지점
    #endregion

    [Space]
    #region AreaVariable
    [Header("Area Variable")]
    [SerializeField] private Transform _areaTransform; // 목표 area(box)의 트랜스폼
    private Vector3 _lastLandingPosition; // 마지막 점프의 착륙 지점
    private ObjectArrivalAreaManager _objectArrivalAreaManager;
    #endregion
    
    private Tween jumpTween;

    public Transform planeTransform;
    [ContextMenu("Set Default Values")]
    public void SetDefaultJumpingValues(float timeToReach)
    {
        //  on my own. Time to Reach Area, Total Jump Number of Times, Jump Height
        // 시작 위치와 1~9 area 위치 사이 거리,
        // 도달할 시간 두 가지를 통해 임의로 변수 값 설정
        
    }

    [ContextMenu("Jump Moving")]
    public void Test()
    {
        /*AssignTargetTransform();
        JumpMovingToTargetTransform();*/
        // FirstJumpLogic();
    }

    public void Init()
    {
        AssignTargetTransform();
        if(arrivalAreaIndex > 0 && arrivalAreaIndex <= 6) StartCoroutine(JumpRoutineAreaIndex1to6());
        else if (arrivalAreaIndex > 6 && arrivalAreaIndex <= 9) StartCoroutine(JumpRoutineAreaIndex7to9());
        
        // SetTime(); // 테스트를 위해 임시로 timeToReachPlayer, totalJumpNumberOfTimes에 가중치를 곱함.
        // JumpMovingToTargetTransform();
    }

    public void StopMoving()
    {
        jumpTween.Kill();
    }

    public void SetTime()
    {
        float dir = Vector3.Distance(transform.position, _areaTransform.position);
        if (dir > 60)
        {
            timeToReachPlayer *= 2f;
            totalJumpNumberOfTimes *= 2;
        }else if (dir > 40)
        {
            timeToReachPlayer *= 1.5f;
            totalJumpNumberOfTimes *= 1;
        }else if (dir > 20)
        {
            timeToReachPlayer *= 1.3f;
            totalJumpNumberOfTimes *= 1;
        }
    }

    IEnumerator JumpRoutineAreaIndex1to6()
    {
        float firstStepTime = 1f;
        JumpStep1(firstStepTime);
        yield return new WaitForSeconds(firstStepTime);
        jumpHeight = _areaTransform.position.y - transform.position.y;
        JumpToTarget1To6Transform();
    }

    IEnumerator JumpRoutineAreaIndex7to9()
    {
        Debug.Log("step 1 7 to 9");
        float firstStepTime = 1f;
        JumpStep1(firstStepTime);
        yield return new WaitForSeconds(firstStepTime);
        // step2 (7~9번 arrival area의 높이보다 일정량 높은 위치까지 점프)
        jumpHeight = _areaTransform.position.y - transform.position.y;
        JumpStep2ToTarget7To9Transform();
    }
    void JumpStep1(float firstStepTime)
    {
        Vector3 nextPos = new Vector3(transform.position.x, planeTransform.position.y, transform.position.z);
        transform.DOJump(nextPos - Vector3.forward * 3f, 0, 1, firstStepTime);
    }
    private void DecideWhatKindOfMovement()
    {
        // 생성 위치 y값에 따라 점프 움직임 로직을 결정한다.
        // arrival area index가 1~6일 경우 (박스의 위치가 바닥과 차이가 있다)
        // arrival area index가 7~9일 경우 (박스의 위치가 바닥과 큰 차이가 없음)
        
        // 1. 생성 위치(날아오는 물체의 position)의 Y값이 arrival area의 Y값보다 높을 경우
        // 2. 생성 위치(날아오는 물체의 position)의 Y값이 arrvial area의 Y값과 비슷할 경우
        // 3. 생성 위치(날아오는 물체의 position)의 Y값이 arrvial area의 Y값보다 작을 경우
        
        
        
    }
    private void AssignTargetTransform()
    {
        // 목표 박스 인데스 Transform 할당
        if(arrivalAreaIndex == 0) return;
        _rigidbody = GetComponent<Rigidbody>();
        _objectArrivalAreaManager = GameObject.FindWithTag("ArrivalAreaParent").GetComponent<ObjectArrivalAreaManager>();
        planeTransform = GameObject.FindWithTag("Plane").transform;
        Debug.Log("arrivalAreaIndex " + arrivalAreaIndex);
        Debug.Log("_objectArrivalAreaManager" + _objectArrivalAreaManager != null);

        _areaTransform = _objectArrivalAreaManager.arrivalAreas[arrivalAreaIndex-1];
        jumpHeight = _areaTransform.position.y - transform.position.y;
    }
    
    private void JumpToTarget1To6Transform()
    {
        Debug.Log("Jump Step 2 1 to 6 Start");
        // 플레이어 거리까지 점프 수 계산
        // (플레이어, 오브젝트 간 거리)와 점프하는 폭으로 
        transform.LookAt(_areaTransform);
        Vector3 movePosition = GetNextJumpPoint1To6();
    
        // DoJump 애니메이션 실행
        // movePosition을 2칸(한 점프에 해당하도록 변경해야 함. 이 값은 매 점프마다 계산 결과가 바뀜)
        // duration = 플레이어에 도달할 시간 / 총 점프의 수  
        jumpTween = transform.DOJump(transform.position + movePosition, 
                jumpHeight, 1, timeToReachPlayer / totalJumpNumberOfTimes).
            SetEase(Ease.OutSine).OnComplete(() =>
            {
                // 점프 끝난 후 변수 업데이트, 다음 지점 예측
                UpdateVariableWhenJumpDone();
                PredictCheckablePosition();
                
                // 재귀 호출
                JumpToTarget1To6Transform();
            });
    }
    private void JumpStep2ToTarget7To9Transform()
    {
        Debug.Log("Jump Step 2 7 to 9 Start");
        // 플레이어 거리까지 점프 수 계산
        // (플레이어, 오브젝트 간 거리)와 점프하는 폭으로 
        transform.LookAt(_areaTransform);
        Vector3 movePosition = GetNextJumpPoint7To9();
    
        // DoJump 애니메이션 실행
        // movePosition을 2칸(한 점프에 해당하도록 변경해야 함. 이 값은 매 점프마다 계산 결과가 바뀜)
        // duration = 플레이어에 도달할 시간 / 총 점프의 수  
        jumpTween = transform.DOJump(transform.position + movePosition, 
                jumpHeight, 1, timeToReachPlayer / totalJumpNumberOfTimes).
            SetEase(Ease.OutSine).OnComplete(() =>
            {
                // 점프 끝난 후 변수 업데이트, 다음 지점 예측
                UpdateVariableWhenJumpDone();
                bool jumpStepDone = PredictCheckablePosition();
                if(!jumpStepDone) return;
                // 재귀 호출
                JumpStep2ToTarget7To9Transform();
            });
    }
    
    // 점프를 시작할 때 호출됨. 호출되기 전 _checkablePositionList에 값이 존재하면 초기화 후 다시 position 값 생성
    private bool PredictCheckablePosition()
    {
        int remainJumpNumberOfTimes = totalJumpNumberOfTimes - _jumpedNumberOfTimes; 
        Debug.Log($"remainJumpNumberOfTimes : {remainJumpNumberOfTimes}");
        if ((arrivalAreaIndex > 6 && arrivalAreaIndex <= 9) && remainJumpNumberOfTimes == 1)
        {
            jumpTween.Kill();
            JumpStep3ToTarget7To9Transform();
            return false;
        }
        // box 까지의 거리
        Vector3 distanceObjectToArea = new Vector3((_areaTransform.position.x - transform.position.x),
            0, (_areaTransform.position.z - transform.position.z));
        
        // 한 칸에 해당하는 방향 벡터 (모든 점프에서 거의 동일한 간격이어야 함.) 
        Vector3 directionVectorCorrespondingToOne = distanceObjectToArea / (float)(2 * remainJumpNumberOfTimes -1);
        
        // checkablePosition에 position 추가
        for (int i = 1; i < remainJumpNumberOfTimes + 1; i++)
        {
            // 다음 점프의 최상단 지점
            _checkablePositionList.Add(transform.position + directionVectorCorrespondingToOne *(2 * i -1) + Vector3.up * jumpHeight);
            // 낙하 지점을 추가함.
            _checkablePositionList.Add(transform.position + directionVectorCorrespondingToOne * (2 * i));
        }
        
        // 최종 도착 지점 (마지막 점프 후 착지하는 지점)
        lastPositon = transform.position + distanceObjectToArea + directionVectorCorrespondingToOne; //+ directionVectorCorrespondingToOne * _jumpedNumberOfTimes;
        return true;
    }

    private Vector3 GetNextJumpPoint1To6()
    {
        // 계속 나눈다. 처음 시작할 때 거리가 아닌
        int remainJumpNumberOfTimes = totalJumpNumberOfTimes - _jumpedNumberOfTimes; 
        
        // box 까지의 거리
        Vector3 distanceObjectToArea = new Vector3((_areaTransform.position.x - transform.position.x),
            0, (_areaTransform.position.z - transform.position.z));
        // 한 칸에 해당하는 방향 벡터 (모든 점프에서 거의 동일한 간격이어야 함.)
        Vector3 directionVectorCorrespondingToOne = distanceObjectToArea / (float)(2 * remainJumpNumberOfTimes -1);
        
        return directionVectorCorrespondingToOne * 2;
    }
    private Vector3 GetNextJumpPoint7To9()
    {
        // 계속 나눈다. 처음 시작할 때 거리가 아닌
        int remainJumpNumberOfTimes = totalJumpNumberOfTimes - _jumpedNumberOfTimes; 
        
        // box 까지의 거리
        Vector3 distanceObjectToArea = new Vector3((_areaTransform.position.x - transform.position.x),
            0, (_areaTransform.position.z - transform.position.z));
        // 한 칸에 해당하는 방향 벡터 (모든 점프에서 거의 동일한 간격이어야 함.)
        Vector3 directionVectorCorrespondingToOne = distanceObjectToArea / (float)(2 * remainJumpNumberOfTimes -1);
        
        return directionVectorCorrespondingToOne * 2;
    }

    private void JumpStep3ToTarget7To9Transform()
    {
        Debug.Log("Jump Step 3 7 to 9 Start");
        transform.LookAt(_areaTransform);

        Vector3 nextDir = new Vector3(_areaTransform.position.x - transform.position.x,
            transform.position.y, _areaTransform.position.z - transform.position.z);
        // movePosition을 2칸(한 점프에 해당하도록 변경해야 함. 이 값은 매 점프마다 계산 결과가 바뀜)
        // duration = 플레이어에 도달할 시간 / 총 점프의 수  
        jumpTween = transform.DOJump(_areaTransform.position, 
            jumpHeight - 1f, 1, timeToReachPlayer / totalJumpNumberOfTimes).
            SetEase(Ease.OutSine).OnComplete(() =>
            {
                transform.DOJump(transform.position + nextDir * 6f, jumpHeight, 3, 3);
            });
    }
    private void UpdateVariableWhenJumpDone()
    {
        jumpHeight = _areaTransform.position.y - transform.position.y;
        if ((arrivalAreaIndex > 6 && arrivalAreaIndex <= 9)) jumpHeight += 0.5f;
        
        _jumpedNumberOfTimes += 1;
        if(_checkablePositionList.Count > 0) _checkablePositionList.Clear();

        Debug.Log($"totalJumpNumberOfTimes <= _jumpedNumberOfTimes {totalJumpNumberOfTimes <= _jumpedNumberOfTimes}, jumpTween {jumpTween != null}");
        if(totalJumpNumberOfTimes <= _jumpedNumberOfTimes) jumpTween.Kill();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ArrivalArea")
        {
            // Debug.Log($"Trigger {other.GetComponent<ObjectArrivalArea>().boxIndex} box ");
            // other.GetComponent<MeshRenderer>().material.DOColor(Random.ColorHSV(), 1f);
        }
        if (other.tag == "body")
        {
            // 플레이어 공격 성공 처리
            jumpTween.Kill();
            GameManager.Player.MinusPlayerLifeValue();
            gameObject.SetActive(false);
        }
        if (other.CompareTag("TriggerPad"))
        {
            // 뒤에 존재하는 곰돌이 공격 성공 처리
            Debug.Log($"{gameObject.name} Trigger Pad");
            jumpTween.Kill();
            GameManager.Player.MinusPlayerLifeValue();
            other.GetComponent<BGBearManager>().MissNodeProcessing(this.gameObject);
            this.enabled = false;
            // gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        // 현재 위치, 다음 점프 최상단 위치, 다음 점프 바닥 접촉 위치 전체 계산
        Color color = Color.red;
        color.a = 0.2f;
        Gizmos.color = color;
        for (int i = 0; i < _checkablePositionList.Count; i++)
        {
            Gizmos.DrawCube(_checkablePositionList[i], transform.lossyScale / 2);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(lastPositon, Vector3.one);
    }
}

public enum YPosGap
{
    Big, Simillar, Small
}
