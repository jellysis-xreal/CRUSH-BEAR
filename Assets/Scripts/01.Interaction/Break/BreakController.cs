using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Motion = EnumTypes.Motion;
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
    public List<GameObject> shatteredObjects;
    public List<Vector3> originLocalPositions;

    public Vector3 shatteredVector;

    public Rigidbody[] rbs;
    // public MeshRenderer[] meshRenderers;

    private void Awake()
    {
        originLocalPositions = new List<Vector3>();
        for (int i = 0; i < shatteredObjects.Count; i++)
        {
            originLocalPositions.Add(shatteredObjects[i].transform.localPosition);
        }
    }

    private void Initialize()
    {
        speed = Random.Range(minMovingSpeed, maxMovingSpeed);
        isHit = false;
        setBreakTime = false;
        for (int i = 0; i < shatteredObjects.Count; i++)
        {
            rbs[i].WakeUp();
            shatteredObjects[i].transform.localPosition = originLocalPositions[i];
        }
        if(!gameObject.activeSelf) gameObject.SetActive(true);
    }
    
    public void IsHit(Vector3 shatteredVec) // Failed
    {
        Initialize();
        
        isHit = true;
        setBreakTime = true;

        shatteredVector = shatteredVec;
        StartCoroutine(ShatteredMovement());
    }
    public void IsHit(Motion motionVecor) // Failed  Motino
    {
        Initialize();
        
        isHit = true;
        setBreakTime = true;

        switch (motionVecor)
        {
            case Motion.LeftZap:
                shatteredVector = new Vector3(0,0,1);
                break;
            case Motion.RightZap:
                shatteredVector = new Vector3(0,0,1);
                break;
            case Motion.LeftHook:
                shatteredVector = new Vector3(-1,0,1);
                break;
            case Motion.RightHook:
                shatteredVector = new Vector3(1,0,1);
                break;
            case Motion.LeftUpperCut:
                shatteredVector = new Vector3(0,1,1);
                break;
            case Motion.RightUpperCut:
                shatteredVector = new Vector3(0,1,1);
                break;
            case Motion.LeftLowerCut:
                shatteredVector = new Vector3(0, -1, 1);
                break;
            case Motion.RightLowerCut:
                shatteredVector = new Vector3(0, -1, 1);
                break;
        }
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
            }
        }
    }
    IEnumerator ShatteredMovement()
    {
        for (int i = 0; i < shatteredObjects.Count; i++)
        {
            // Material mat = meshRenderers[i].material;
            //mat.color.a = 0.5f;
            // mat.DOColor(Color.clear, 0.5f);
            // 주먹의 방향에 약간의 랜덤한 변화를 추가
            Vector3 forceDirection = shatteredVector + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));

            rbs[i].AddForce(forceDirection * 15f);
        }
        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < shatteredObjects.Count; i++)
        {
            rbs[i].useGravity = true;
        }
        yield return new WaitForSeconds(0.3f);

        StartCoroutine(ActiveTime(1));
    }
    private IEnumerator ActiveTime(float coolTime)
    {
        yield return new WaitForSecondsRealtime(coolTime); // coolTime만큼 활성화
        for (int i = 0; i < shatteredObjects.Count; i++)
        {
            rbs[i].velocity=Vector3.zero;
            rbs[i].angularVelocity=Vector3.zero;
            rbs[i].Sleep();
        }
        gameObject.SetActive(false);
        isHit = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + shatteredVector);
    }
}
