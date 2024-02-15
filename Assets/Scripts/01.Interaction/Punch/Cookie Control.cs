using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CookieControl : MonoBehaviour
{
    public Vector3 playerPosition; // 플레이어의 Transform
    public float stopDistance = 5f; // 회전을 멈출 거리
    public float rotationDuration = 1f; // 회전 완료까지 걸리는 시간

    private Quaternion originalRotation; // 원래의 회전 값
    private bool isRotating = true; // 회전 여부
    private Tweener rotationTween; // 회전 Tween

    public void Init(Vector3 targetPosition)
    {
        this.playerPosition = targetPosition;
    }
    void Start()
    {
        // 원래의 회전 값을 저장
        originalRotation = transform.rotation;

        // 객체를 360도 회전시키는 Tween을 시작
        rotationTween = transform.DORotate(new Vector3(0, 0, 360), 10f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);
    }
    
    void Update()
    {
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition); //playerTransform.position);

        // 만약 플레이어와의 거리가 stopDistance보다 작다면 회전을 멈춤
        if (distanceToPlayer < stopDistance && isRotating)
        {
            // Tween을 멈추고 원래의 회전 값으로 자연스럽게 회전
            rotationTween.Kill();
            transform.DORotate(originalRotation.eulerAngles, rotationDuration).SetEase(Ease.InOutQuad);
            isRotating = false;
        }
        // 거리가 다시 멀어졌을 때 다시 회전을 시작
        else if (distanceToPlayer >= stopDistance && !isRotating)
        {
            // 원래의 회전 값에서 360도 회전까지의 Tween을 시작
            rotationTween = transform.DORotate(new Vector3(0, 0, 360), 10f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear);
            isRotating = true;
        }
    }
}

