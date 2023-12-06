using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class straightMovingToArrivalArea : MonoBehaviour, IMovement
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
    private Vector3 dir = new Vector3();

    private void OnEnable()
    {

        // GetComponent<AudioSource>().Play();
    }

    public void Init()
    {
        if(arrivalAreaIndex == 0) return;
        _rigidbody = GetComponent<Rigidbody>();
        
        isArrivalAreaHit = false;
        _objectArrivalAreaManager = GameObject.FindWithTag("ArrivalAreaParent").GetComponent<ObjectArrivalAreaManager>();
        /*Debug.Log("arrivalAreaIndex " + arrivalAreaIndex);
        Debug.Log("_objectArrivalAreaManager" + _objectArrivalAreaManager != null);*/

        targetTransform = _objectArrivalAreaManager.arrivalAreas[arrivalAreaIndex-1];
    }

    void Update()
    {
        if(!isArrivalAreaHit) Move();
        else TriggeredMove();
    }

    void Move()
    {
        transform.LookAt(transform);
        dir = (targetTransform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    void TriggeredMove()
    {
        transform.position += dir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ArrivalArea"))
        {
            Debug.Log($"Trigger {other.GetComponent<ObjectArrivalArea>().boxIndex} box ");
            // other.GetComponent<MeshRenderer>().material.DOColor(Random.ColorHSV(), 1f);
            isArrivalAreaHit = true;
        }
        if (other.tag == "body")
        {
            // 플레이어 공격 성공 처리
            GameManager.Player.MinusPlayerLifeValue();
            gameObject.SetActive(false);
        }
        if (other.CompareTag("TriggerPad"))
        {
            // 뒤에 존재하는 곰돌이 공격 성공 처리
            Debug.Log($"{gameObject.name} Trigger Pad");
            GameManager.Player.MinusPlayerLifeValue();
            other.GetComponent<BGBearManager>().MissNodeProcessing(this.gameObject);
            this.enabled = false;
        }
    }
}
