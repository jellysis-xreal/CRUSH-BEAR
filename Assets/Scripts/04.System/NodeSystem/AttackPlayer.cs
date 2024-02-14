using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class AttackPlayer : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private Transform _playerBodyTransform;
    private void Start()
    {
        
    }

    private void OnEnable()
    {
        Debug.Log("Init Attack");
        _meshRenderer = GetComponent<MeshRenderer>();
        _playerBodyTransform = GameObject.FindWithTag("body").transform;
    }

    public void DirectionAttack()
    {
        float attackPrepareDuration = 1f;
        float attackingTime = 1f;
        
        Shake(attackPrepareDuration);
        
        // _meshRenderer.material.color = Color.red;
        _meshRenderer.material.DOColor(Random.ColorHSV(), 1f);
        Vector3 oppositePosition =
            transform.position - (_playerBodyTransform.position - transform.position).normalized * 2f;
        transform.DOMove(oppositePosition, 1f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            transform.DOMove(_playerBodyTransform.position, 1f).SetEase(Ease.InOutFlash);
        });
    }

    private void Shake(float attackPrepareDuration)
    {
        const float strength = 0.5f;

        transform.DOShakePosition(attackPrepareDuration, strength);
        transform.DOShakeRotation(attackPrepareDuration, strength);
        transform.DOShakeScale(attackPrepareDuration, strength);
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "body")
        {
            // 공격 성공 처리
            GameManager.Player.MinusPlayerLifeValue();
            gameObject.SetActive(false);
        }
    }
}
