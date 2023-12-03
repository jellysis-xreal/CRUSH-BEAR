using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class JumpMovingToArrivalArea : MonoBehaviour
{
    public int arrivalAreaIndex = 0;
    
    private Rigidbody _rigidbody;
    public bool isArrivalAreaHit;
    
    private float _afterAttachTime;

    private ObjectArrivalAreaManager _objectArrivalAreaManager;
    private Transform targetTransform;
    private AttackPlayer _attackPlayer;
    private void Start()
    {
    }

    private void OnEnable()
    {
        if(arrivalAreaIndex == 0) return;
        _rigidbody = GetComponent<Rigidbody>();
        isArrivalAreaHit = false;
        _objectArrivalAreaManager = GameObject.FindWithTag("ArrivalAreaParent").GetComponent<ObjectArrivalAreaManager>();
        Debug.Log("arrivalAreaIndex " + arrivalAreaIndex);
        Debug.Log("_objectArrivalAreaManager" + _objectArrivalAreaManager != null);

        targetTransform = _objectArrivalAreaManager.arrivalAreas[arrivalAreaIndex-1];
        
        GetComponent<AudioSource>().Play();
        
        /*_playerBodyTransform = GameObject.FindWithTag("body").transform;
        transform.LookAt(_playerBodyTransform);*/
        JumpMovingToPlayer();
    }

    public int targetBoxIndex = 0; 
    
    public float jumpPower = 1f;
    public float eachJumpTime = 1f;
    private Transform _playerBodyTransform;
    private Tween jumpTween;
    
    // Player => Box[index]
    private void JumpMovingToPlayer()
    {
        // 플레이어 거리까지 점프 수 계산
        // (플레이어, 오브젝트 간 거리)와 점프하는 폭으로 
        transform.LookAt(_playerBodyTransform);
        Vector3 movePosition = new Vector3((_playerBodyTransform.position.x - transform.position.x),
            0, (_playerBodyTransform.position.z - transform.position.z)).normalized; 
        
        // DoJump 애니메이션 실행
        jumpTween = transform.DOJump(transform.position + movePosition, 
                jumpPower, 1, eachJumpTime).
            SetEase(Ease.OutSine).OnComplete(() =>
            {
                // 재귀 호출
                JumpMovingToPlayer();
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
    }
}
