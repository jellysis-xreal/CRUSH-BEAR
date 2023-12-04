using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 정확한 위치와 박자에 노드 트리거 시키기 !!
public class JumpMovingToArrivalArea : MonoBehaviour
{
    public int arrivalAreaIndex = 0; // 목표로 하는 area의 인덱스
    public float timeToReachArea = 10f; // area까지 걸릴 시간
    public float totalJumpNumberOfTimes = 10; // area에 도달하기 까지 점프할 횟수
    private int jumpedNumberOfTimes = 0; // 생성된 이후로 점프한 횟수
    public float jumpHeight = 0; // ObjectArrivalArea[index].transform.position.y와 동기화

    private Transform _targetTransform; // 6.5 칸이 되어야 함. 
    
    private Tween jumpTween;

    [ContextMenu("Set Default Values on my own. " +
                 "Time to Reach Area, " +
                 "Total Jump Number of Times" +
                 "Jump Height")]
    private void SetDefaultJumpingValues(float timeToReach)
    {
        // 시작 위치와 1~9 area 위치 사이 거리, 도달할 시간 두 가지를 통해 임의로 변수 값 설정
    }
    
    private void JumpMovingToTargetTransform()
    {
        // 플레이어 거리까지 점프 수 계산
        // (플레이어, 오브젝트 간 거리)와 점프하는 폭으로 
        transform.LookAt(_targetTransform);
        Vector3 movePosition = new Vector3((_targetTransform.position.x - transform.position.x),
            0, (_targetTransform.position.z - transform.position.z)).normalized; 
    
        // DoJump 애니메이션 실행
        jumpTween = transform.DOJump(transform.position + movePosition, 
                jumpHeight, 1, timeToReachArea / totalJumpNumberOfTimes).
            SetEase(Ease.OutSine).OnComplete(() =>
            {
                // 재귀 호출
                JumpMovingToTargetTransform();
            });
    }

    private Vector3 GetNextJumpPoint()
    {
        return new Vector3(1, 1, 1);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ArrivalArea")
        {
            Debug.Log($"Trigger {other.GetComponent<ObjectArrivalArea>().boxIndex} box ");
            other.GetComponent<MeshRenderer>().material.DOColor(Random.ColorHSV(), 1f);
        }
    
        if (other.tag == "body")
        {
            // 공격 성공 처리
            jumpTween.Kill();
            PlayerManager.Instance.MinusPlayerLifeValue();
            gameObject.SetActive(false);
        }
    }
}

/*public int arrivalAreaIndex = 0;

private Rigidbody _rigidbody;
public bool isArrivalAreaHit;

private float _afterAttachTime;

private ObjectArrivalAreaManager _objectArrivalAreaManager;
private Transform targetTransform;
private AttackPlayer _attackPlayer;

public float jumpPower = 1f;
public float eachJumpTime = 1f;


private void OnEnable()
{
    if(arrivalAreaIndex == 0) return;
    _rigidbody = GetComponent<Rigidbody>();
    isArrivalAreaHit = false;
    _objectArrivalAreaManager = GameObject.FindWithTag("ArrivalAreaParent").GetComponent<ObjectArrivalAreaManager>();
    Debug.Log("arrivalAreaIndex " + arrivalAreaIndex);
    Debug.Log("_objectArrivalAreaManager" + _objectArrivalAreaManager != null);

    targetTransform = _objectArrivalAreaManager.arrivalAreas[arrivalAreaIndex-1];
    
    // GetComponent<AudioSource>().Play();
    
    JumpMovingToTargetTransform();
}


// Player => Box[index]
private void JumpMovingToTargetTransform()
{
    // 플레이어 거리까지 점프 수 계산
    // (플레이어, 오브젝트 간 거리)와 점프하는 폭으로 
    transform.LookAt(targetTransform);
    Vector3 movePosition = new Vector3((targetTransform.position.x - transform.position.x),
        0, (targetTransform.position.z - transform.position.z)).normalized; 
    
    // DoJump 애니메이션 실행
    jumpTween = transform.DOJump(transform.position + movePosition, 
            jumpPower, 1, eachJumpTime).
        SetEase(Ease.OutSine).OnComplete(() =>
        {
            // 재귀 호출
            JumpMovingToTargetTransform();
        });
}

private void OnTriggerEnter(Collider other)
{
    if (other.tag == "ArrivalArea")
    {
        Debug.Log($"Trigger {other.GetComponent<ObjectArrivalArea>().boxIndex} box ");
        other.GetComponent<MeshRenderer>().material.DOColor(Random.ColorHSV(), 1f);
    }
    
    if (other.tag == "body")
    {
        // 공격 성공 처리
        jumpTween.Kill();
        PlayerManager.Instance.MinusPlayerLifeValue();
        gameObject.SetActive(false);
    }
}*/

