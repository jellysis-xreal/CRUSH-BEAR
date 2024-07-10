using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using EnumTypes;
public class straightMovingToArrivalArea : MonoBehaviour, IMovement
{
    public int arrivalAreaIndex = 0;
    
    private Rigidbody _rigidbody;
    
    public Transform playerTransform;
    public bool isArrivalAreaHit = false;
    public bool isHandAttached = false;
    
    [SerializeField] private float playerAttachdistance;
    [SerializeField] private float playerAttachTime;
    private float _afterAttachTime;

    private ObjectArrivalAreaManager _objectArrivalAreaManager;
    private Transform targetTransform;
    private Vector3 dir = new Vector3();
    
    [Header("have to set")] 
    public float timeToReachPlayer; // 생성 후 플레이어 까지 도달할 시간
    public float generationTime; // 생성시간

    public float constantSpeed = 0f;
    public float equivalentAccelerationSpeed;
    

    public void Init()
    {
        if(arrivalAreaIndex == 0) return;
        _rigidbody = GetComponent<Rigidbody>();
        
        isArrivalAreaHit = false;
        _objectArrivalAreaManager = GameObject.FindWithTag("ArrivalAreaParent").GetComponent<ObjectArrivalAreaManager>();

        targetTransform = _objectArrivalAreaManager.arrivalAreas[arrivalAreaIndex-1];
        CalculateConstantSpeed();
        InteractionType type = GetComponent<BaseObject>().InteractionType;
        if (type == InteractionType.Break) StartCoroutine(RotateMovingBreakObject());
        else if (type == InteractionType.Tear) StartCoroutine(RotateMovingRipObject());
        
        
    }
    private void CalculateConstantSpeed()
    {
        // 속도 = 거리 / 시간
        float time = timeToReachPlayer; // - generationTime;
        constantSpeed = Vector3.Distance(targetTransform.position, transform.position) / time;
        Debug.Log($"{this.gameObject.name} constant speed : {constantSpeed}");
        Debug.Log($"{this.gameObject.name} time : {time}");
        Debug.Log($"{this.gameObject.name} distance : {Vector3.Distance(targetTransform.position, transform.position)}");
    }
    public void StopMoving()
    {
        isHandAttached = true;
    }

    void Update()
    {
        if(!isArrivalAreaHit && !isHandAttached) Move();
        else if(isArrivalAreaHit && !isHandAttached) TriggeredMove();
    }

    void Move()
    {
        //transform.LookAt(transform);
        dir = (targetTransform.position - transform.position).normalized;
        transform.position += dir * constantSpeed * Time.deltaTime;
    }

    void TriggeredMove()
    {
        transform.position += dir * constantSpeed * Time.deltaTime;
    }
    
    IEnumerator RotateMovingBreakObject()
    {
        float time = timeToReachPlayer - 1f;
        transform.DOShakeRotation(time, 150f, 20, 100, true, ShakeRandomnessMode.Harmonic);
        yield return new WaitForSeconds(time);
        transform.DORotate(new Vector3(90f, 0, 0), 1);
    }
    IEnumerator RotateMovingRipObject()
    {
        float time = timeToReachPlayer - 1f;
        transform.LookAt(targetTransform);
        //transform.DOShakePosition(time, 1f, 20);
        transform.DOShakeRotation(time, 150f, 20, 100, true, ShakeRandomnessMode.Harmonic);
        yield return new WaitForSeconds(time);
        transform.DORotate(new Vector3(270f, 0, 0), 1, RotateMode.WorldAxisAdd); //.setter(Quaternion.AngleAxis(-90, Vector3.right));
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ArrivalArea"))
        {
            // Debug.Log($"Trigger {other.GetComponent<ObjectArrivalArea>().boxIndex} box ");
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
            this.enabled = false;
        }
    }
}
