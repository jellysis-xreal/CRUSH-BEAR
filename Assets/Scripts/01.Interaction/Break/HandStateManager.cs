using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandStateManager : MonoBehaviour
{
    // 주먹을 쥔 채 5프레임 간의 위치 변화를 저장해 방향 벡터를 설정한다.
    public Vector3 recentDirection;
    [SerializeField] private MoveToPlayer moveToPlayer;
    [SerializeField] private float punchPower;
    private void Start()
    {
        
    }

    private void OnEnable()
    {
        StartCoroutine(ReadRecentDirection());
    }

    IEnumerator ReadRecentDirection()
    {
        while (true)
        {
            // 이전 포지션 저장
            Vector3 prevHandPos = transform.position;
        
            // direction 세팅 대기 시간
            yield return new WaitForSeconds(0.05f);

            // 0.1초 전 후 위치 비교로 방향벡터 저장
            recentDirection = transform.position - prevHandPos;            
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            moveToPlayer = collision.gameObject.GetComponent<MoveToPlayer>();
            Debug.Log("moveToPlayer Cache is "+ moveToPlayer+" Vector3 "+recentDirection);
            moveToPlayer.ReflectionMove(recentDirection * punchPower);
        }
    }
}
