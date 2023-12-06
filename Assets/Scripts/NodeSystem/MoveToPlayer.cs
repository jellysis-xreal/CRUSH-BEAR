using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoveToPlayer : MonoBehaviour
{
    private Rigidbody _rigidbody;
    
    public Transform playerTransform;
    public float minMovingSpeed = 1.0f;
    public float maxMovingSpeed = 3.0f;
    public float speed;
    public bool isHit;

    [SerializeField] private float playerAttachdistance;
    [SerializeField] private float playerAttachTime;
    private float _afterAttachTime;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        //speed = Random.Range(minMovingSpeed, maxMovingSpeed);
        isHit = false;
        playerTransform = GameObject.FindWithTag("body").transform;
    }

    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
        //speed = Random.Range(minMovingSpeed, maxMovingSpeed);
        isHit = false;
        playerTransform = GameObject.FindWithTag("body").transform;
        GetComponent<AudioSource>().Play();
    }

    void Update()
    {
        if(!isHit) Move();
    }

    void Move()
    {
        transform.LookAt(transform);
        Vector3 dir = (playerTransform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        /*if (dir.sqrMagnitude < playerAttachdistance)
        {
            playerAttachTime = Time.time;
        }*/
    }
    
    public void ReflectionMove(Vector3 dir)
    {
        Debug.Log("Reflection Moving, dir : "+dir);
        isHit = true;
        _rigidbody.AddForce(dir,ForceMode.Impulse);
        //_rigidbody.AddForceAtPosition(dir, transform.position, ForceMode.Impulse);
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
