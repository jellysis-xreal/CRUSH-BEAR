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

    private float breakTime = 0.0f;
    private List<GameObject> shatteredObjects = new List<GameObject>();

    private void Start()
    {
        speed = Random.Range(minMovingSpeed, maxMovingSpeed);
        isHit = false;
    }

    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
        playerTransform = GameObject.FindWithTag("body").transform;
        
        for (int i = 0; i < transform.childCount; i++)
            shatteredObjects.Add(transform.GetChild(i).gameObject);
    }

    public void IsHit()
    {
        isHit = true;
        Destroy(this.gameObject, 3.0f);
    }

    private void MoveShattered()
    {
        //조각들이 흩어지며 떨어지게 업데이트
        foreach (var obj in shatteredObjects)
        {
            Rigidbody objRigidboby = obj.GetComponent<Rigidbody>();
            
            Vector3 moveDir = obj.transform.localPosition - Vector3.zero;
            obj.transform.localPosition = moveDir * 30.0f * Time.deltaTime;

            if (breakTime < 2.0f)
                objRigidboby.AddForce(Vector3.up * 5.0f);
            else
                objRigidboby.AddForce(Vector3.down * 1.0f);

        }
    }
    
    void Update()
    {
        if (!isHit)
            Move();
        else
        {
            breakTime += Time.deltaTime;
            MoveShattered();
        }
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
