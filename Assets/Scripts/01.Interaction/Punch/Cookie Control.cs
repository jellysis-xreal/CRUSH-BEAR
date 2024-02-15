using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CookieControl : MonoBehaviour
{
    public Vector3 playerPosition; // �÷��̾��� Transform
    public float stopDistance = 5f; // ȸ���� ���� �Ÿ�
    public float rotationDuration = 1f; // ȸ�� �Ϸ���� �ɸ��� �ð�

    private Quaternion originalRotation; // ������ ȸ�� ��
    private bool isRotating = true; // ȸ�� ����
    private Tweener rotationTween; // ȸ�� Tween

    public void Init(Vector3 targetPosition)
    {
        this.playerPosition = targetPosition;
    }
    void Start()
    {
        // ������ ȸ�� ���� ����
        originalRotation = transform.rotation;

        // ��ü�� 360�� ȸ����Ű�� Tween�� ����
        rotationTween = transform.DORotate(new Vector3(0, 0, 360), 10f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);
    }
    
    void Update()
    {
        // �÷��̾���� �Ÿ� ���
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition); //playerTransform.position);

        // ���� �÷��̾���� �Ÿ��� stopDistance���� �۴ٸ� ȸ���� ����
        if (distanceToPlayer < stopDistance && isRotating)
        {
            // Tween�� ���߰� ������ ȸ�� ������ �ڿ������� ȸ��
            rotationTween.Kill();
            transform.DORotate(originalRotation.eulerAngles, rotationDuration).SetEase(Ease.InOutQuad);
            isRotating = false;
        }
        // �Ÿ��� �ٽ� �־����� �� �ٽ� ȸ���� ����
        else if (distanceToPlayer >= stopDistance && !isRotating)
        {
            // ������ ȸ�� ������ 360�� ȸ�������� Tween�� ����
            rotationTween = transform.DORotate(new Vector3(0, 0, 360), 10f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear);
            isRotating = true;
        }
    }
}

