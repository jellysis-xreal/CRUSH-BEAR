using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class JumpMoving : MonoBehaviour
{
    public float jumpPower = 1f;
    private Transform _playerBodyTransform;
    private Tween jumpTween;
    private void OnEnable()
    {
        _playerBodyTransform = GameObject.FindWithTag("body").transform;
        transform.LookAt(_playerBodyTransform);
        JumpMovingToPlayer();
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.K)) JumpMovingToPlayer();
    }
    //
    private void JumpMovingToPlayer()
    {
        // 플레이어 거리까지 점프 수 계산
        // (플레이어, 오브젝트 간 거리)와 점프하는 폭으로 
        transform.LookAt(_playerBodyTransform);
        Vector3 movePosition = new Vector3((_playerBodyTransform.position.x - transform.position.x),
                    0, (_playerBodyTransform.position.z - transform.position.z)).normalized; 
        
        // DoJump 애니메이션 실행
        jumpTween = transform.DOJump(transform.position + movePosition, 
                jumpPower, 1, 1).
            SetEase(Ease.OutSine).OnComplete(() =>
        {
            // 재귀 호출
            JumpMovingToPlayer();
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "body")
        {
            // 공격 성공 처리
            jumpTween.Kill();
            PlayerManager.Instance.MinusPlayerLifeValue();
            gameObject.SetActive(false);
        }
    }
}
