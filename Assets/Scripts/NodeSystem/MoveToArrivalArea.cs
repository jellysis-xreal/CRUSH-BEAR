using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToArrivalArea : MonoBehaviour
{
    public int arrivalAreaIndex = 0;
    
    private Rigidbody _rigidbody;
    
    public Transform playerTransform;
    public float minMovingSpeed = 1.0f;
    public float maxMovingSpeed = 3.0f;
    public float speed;
    public bool isHit;

    [SerializeField] private float playerAttachdistance;
    [SerializeField] private float playerAttachTime;
    private float _afterAttachTime;

    private ObjectArrivalAreaManager _objectArrivalAreaManager;
    private Transform targetTransform;
    private void Start()
    {
        /*_rigidbody = GetComponent<Rigidbody>();
        //speed = Random.Range(minMovingSpeed, maxMovingSpeed);
        isHit = false;
        _objectArrivalAreaManager = GameObject.FindWithTag("ArrivalArea").GetComponent<ObjectArrivalAreaManager>();
        Debug.Log("arrivalAreaIndex " + arrivalAreaIndex);
        Debug.Log("_objectArrivalAreaManager" + _objectArrivalAreaManager != null);

        targetTransform = _objectArrivalAreaManager.arrivalAreas[arrivalAreaIndex-1];
        //playerTransform = GameObject.FindWithTag("body").transform;*/
    }

    private void OnEnable()
    {
        if(arrivalAreaIndex == 0) return;
        _rigidbody = GetComponent<Rigidbody>();
        //speed = Random.Range(minMovingSpeed, maxMovingSpeed);
        isHit = false;
        _objectArrivalAreaManager = GameObject.FindWithTag("ArrivalArea").GetComponent<ObjectArrivalAreaManager>();
        Debug.Log("arrivalAreaIndex " + arrivalAreaIndex);
        Debug.Log("_objectArrivalAreaManager" + _objectArrivalAreaManager != null);

        targetTransform = _objectArrivalAreaManager.arrivalAreas[arrivalAreaIndex-1];
        //playerTransform = GameObject.FindWithTag("body").transform;
        GetComponent<AudioSource>().Play();
    }

    void Update()
    {
        if(!isHit) Move();
    }

    void Move()
    {
        transform.LookAt(transform);
        Vector3 dir = (targetTransform.position - transform.position).normalized;
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
        if (other.CompareTag("body"))
        {
            isHit = true;
        }
    }
}
