using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidConroller : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public Transform playerTransform;
    public float minMovingSpeed = 0.5f;
    public float maxMovingSpeed = 1.0f;
    public float speed;
    public bool isHit;
    private Vector3 dir;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        speed = Random.Range(minMovingSpeed, maxMovingSpeed);
        isHit = false;
        playerTransform = GameObject.FindWithTag("MainCamera").transform;
        dir = playerTransform.position - transform.position;
    }

    void Update()
    {
        if(!isHit) Move();
    }

    void Move()
    {
        transform.LookAt(transform);
        // Vector3 dir = playerTransform.position - transform.position;
        transform.position += dir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other){
        Crashed();
    }

    public void Crashed()
    {
        Debug.Log("[Test] Crashed!");
        isHit = true;
        BearManager.instance.DecreaseHearts();
        Destroy(gameObject);
    }
}
