using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidConroller : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public Transform playerTransform;
    public float minMovingSpeed = 0.01f;
    public float maxMovingSpeed = 0.5f;
    public float speed;
    public bool isHit;
    private Vector3 dir;
    public GameObject warningSign;
    public GameObject generatedWarningSign;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        speed = Random.Range(minMovingSpeed, maxMovingSpeed);
        isHit = false;
        playerTransform = GameObject.FindWithTag("MainCamera").transform;
        dir = playerTransform.position - transform.position;

        int layerMask = 1 << 7;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, Mathf.Infinity, layerMask))
        {
            Vector3 spawnPosition = hit.point + new Vector3(0f, 0.01f, 0f);
            generatedWarningSign = Instantiate(warningSign, spawnPosition, Quaternion.Euler(90f, 0f, 0f));
        }
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
        //Debug.Log("[Test] Crashed!");
        isHit = true;
        BearManager.instance.DecreaseHearts();
        Destroy(generatedWarningSign);
        Destroy(gameObject);
    }
}
