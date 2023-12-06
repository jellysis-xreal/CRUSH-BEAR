using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void IsHit()
    {
        Initialize();
        
        isHit = true;
        setBreakTime = true;
    }

    private void MoveShattered()
    {
        //조각들이 흩어지며 떨어지게 업데이트
        foreach (var obj in shatteredObjects)
        {
            Rigidbody objRigidboby = obj.GetComponent<Rigidbody>();

            if (breakTime < 0.3f)
            {
                objRigidboby.AddForce(Vector3.up * 5.0f);
                //Vector3 moveDir = obj.transform.localPosition - Vector3.zero;
                //obj.transform.localPosition += moveDir * Time.deltaTime;
            }

        }
    }
    
    void Update()
    {
        if (isHit && setBreakTime)
        {
            breakTime += Time.deltaTime;
            
            // 조각을 자연스럽게 흩어지게 한뒤, 5s가 지나면 해당 오브젝트를 destory
            if (breakTime < 3.0f)
                MoveShattered();
            else
                Destroy(this.gameObject);
        }
    }
}
