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
    public bool isArrivalAreaHit;

    [SerializeField] private float playerAttachdistance;
    [SerializeField] private float playerAttachTime;
    private float _afterAttachTime;

    private ObjectArrivalAreaManager _objectArrivalAreaManager;
    private Transform targetTransform;
    private AttackPlayer _attackPlayer;
    private void Start()
    {
    }

    private void OnEnable()
    {
        if(arrivalAreaIndex == 0) return;
        _rigidbody = GetComponent<Rigidbody>();
        _attackPlayer = GetComponent<AttackPlayer>();
        //speed = Random.Range(minMovingSpeed, maxMovingSpeed);
        isArrivalAreaHit = false;
        _objectArrivalAreaManager = GameObject.FindWithTag("ArrivalAreaParent").GetComponent<ObjectArrivalAreaManager>();
        Debug.Log("arrivalAreaIndex " + arrivalAreaIndex);
        Debug.Log("_objectArrivalAreaManager" + _objectArrivalAreaManager != null);

        targetTransform = _objectArrivalAreaManager.arrivalAreas[arrivalAreaIndex-1];
        //playerTransform = GameObject.FindWithTag("body").transform;
        GetComponent<AudioSource>().Play();
    }

    void Update()
    {
        if(!isArrivalAreaHit) Move();
    }

    void Move()
    {
        transform.LookAt(transform);
        Vector3 dir = (targetTransform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ArrivalArea"))
        {
            if(isArrivalAreaHit) return;
            isArrivalAreaHit = true;
            _attackPlayer.enabled = true;
            _attackPlayer.DirectionAttack();
            this.enabled = false;
        }
    }
}
