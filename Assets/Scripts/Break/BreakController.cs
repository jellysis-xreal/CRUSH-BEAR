using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    
    public Transform playerTransform;
    public float minMovingSpeed = 1.0f;
    public float maxMovingSpeed = 3.0f;
    public float speed;
    public bool isHit;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        speed = Random.Range(minMovingSpeed, maxMovingSpeed);
        isHit = false;
        playerTransform = GameObject.FindWithTag("MainCamera").transform;
    }

    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
        speed = Random.Range(minMovingSpeed, maxMovingSpeed);
        isHit = false;
        playerTransform = GameObject.FindWithTag("MainCamera").transform;
    }

    void Update()
    {
        if(!isHit) Move();
    }

    void Move()
    {
        transform.LookAt(transform);
        Vector3 dir = playerTransform.position - transform.position;
        transform.position += dir * speed * Time.deltaTime;
    }

    public void ReflectionMove(Vector3 dir)
    {
        Debug.Log("Reflection Moving, dir : "+dir);
        isHit = true;
        _rigidbody.AddForce(dir,ForceMode.Impulse);
        //_rigidbody.AddForceAtPosition(dir, transform.position, ForceMode.Impulse);
    }
}
