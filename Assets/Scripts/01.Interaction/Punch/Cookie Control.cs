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

    public void Init()
    {
        if(rotationTween != null) rotationTween.Kill();
        
        stopDistance = 7f;
        // 원래의 회전 값을 저장
        originalRotation = transform.rotation;

        // 객체를 360도 회전시키는 Tween을 시작
        Debug.Log($"{gameObject.name} rotation Start");
        rotationTween = transform.DORotate(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 360), 2f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);
    }
}

