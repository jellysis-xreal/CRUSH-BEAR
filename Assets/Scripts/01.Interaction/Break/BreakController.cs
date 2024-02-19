using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class BreakController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    
    public float minMovingSpeed = 1.0f;
    public float maxMovingSpeed = 3.0f;
    public float speed;
    
    public bool isHit = false;
    public bool setBreakTime = false;
    
    [SerializeField] private float breakTime = 0.0f;
    private List<GameObject> shatteredObjects = new List<GameObject>();

    public Vector3 shatteredVector;
    
    private void Start()
    {
        //Initialize();
    }

    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Initialize()
    {
        speed = Random.Range(minMovingSpeed, maxMovingSpeed);
        isHit = false;
        setBreakTime = false;

        for (int i = 0; i < transform.childCount; i++)
            shatteredObjects.Add(transform.GetChild(i).gameObject);
    }
    
    public void IsHit(Vector3 shatteredVec) // Failed
    {
        Initialize();
        
        isHit = true;
        setBreakTime = true;

        shatteredVector = shatteredVec;
        StartCoroutine(ShatteredMovement());
    }
    public void IsHit() // Failed
    {
        Initialize();
        
        isHit = true;
        setBreakTime = true;

        shatteredVector = new Vector3(0,0,0);
        StartCoroutine(ShatteredMovement());
    }
    private void MoveShattered()
    {
        //조각들이 흩어지며 떨어지게 업데이트
        foreach (var obj in shatteredObjects)
        {
            Rigidbody objRigidboby = obj.GetComponent<Rigidbody>();

            Vector3 local = obj.transform.localPosition.normalized;
            if (breakTime < 0.2f)
            {   
                objRigidboby.AddForce(shatteredVector * 10f);
                // objRigidboby.AddForce(Vector3.up * 3.0f);
                
                /*objRigidboby.AddForce(local * 2.0f);
                objRigidboby.AddForce(Vector3.up * 3.0f);
                // TODO: 현재 토핑이 반대로 돌아감. 수정한 뒤에 Vector.back으로!
                objRigidboby.AddForce(Vector3.forward * 2.0f);*/
                //Vector3 moveDir = obj.transform.localPosition - Vector3.zero;
                //obj.transform.localPosition += moveDir * Time.deltaTime;
            }
        }
    }
    IEnumerator ShatteredMovement()
    {
        Rigidbody[] rbs = new Rigidbody[shatteredObjects.Count];
        for (int i = 0; i < shatteredObjects.Count; i++)
        {
            rbs[i] = shatteredObjects[i].GetComponent<Rigidbody>();
            rbs[i].gameObject.GetComponent<MeshRenderer>().material.DOColor(Color.red,2f);
            // 주먹의 방향에 약간의 랜덤한 변화를 추가
            Vector3 forceDirection = shatteredVector + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
            
            rbs[i].AddForce(forceDirection * 6f);
        }
        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < shatteredObjects.Count; i++)
        {
            rbs[i].useGravity = true;
        }

        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
    void Update()
    {
        /*if (isHit && setBreakTime)
        {
            breakTime += Time.deltaTime;
            
            // 조각을 자연스럽게 흩어지게 한뒤, 5s가 지나면 해당 오브젝트를 destory
            if (breakTime < 3.0f)
                MoveShattered();
            else
                Destroy(this.gameObject);
        }*/
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + shatteredVector);
    }
}
