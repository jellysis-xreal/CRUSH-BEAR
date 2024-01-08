using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.PlayerLoop;
using EnumTypes;

public class HittableMovement : MonoBehaviour
{
    [Header("Setting Variable")] 
    public int arrivalBoxNum = 0; // 목표인 Box index number
    public float arriveTime;
    public float maxHeight = 5.0f;
    public InteractionSide sideType = InteractionSide.Red;
    
    [Header("other Variable")] 
    [SerializeField] private ObjectArrivalAreaManager arrivalArea; // Scene내의 arrival area
    [SerializeField] private toppingState curState = toppingState.idle;
    
    //토핑이 움직이기 위한 변수
    private float _startTime;
    private float _popTime = 1.5f;
    private Vector3 _startPos;
    [SerializeField] private Vector3 _arrivalBoxPos;

    //토핑이 맞은 후에 활용할 변수
    private Vector3 _moveVector;
    private float _moveSpeed;
    
    //방향 측정을 위한 변수
    private Vector3 _beforeVector;

    private enum toppingState
    {
        idle,
        approach,
        interacable,
        refrigerator
    }

    private void Awake()
    {
        arrivalArea = GameObject.FindWithTag("ArrivalAreaParent").GetComponent<ObjectArrivalAreaManager>();
        // 해당 오브젝트가 생성된 시간과 위치를 저장
        _startTime = Time.time;
        _startPos = transform.position;

        _beforeVector = transform.position;

        // TODO: test 위함
        //InitialTopping(1, 5.0f);
        MoveToPlayer();
    }

    /// <summary>
    /// 해당 토핑이 도착하고자하는 arrival area의 index,
    /// 해당 토핑이 도착하고자하는 arrival time을 지정함
    /// </summary>
    /// <param name="arrivalBox">arrival area의 index</param>
    /// <param name="arriveTime">arrival time</param>
    public void InitialTopping(int arrivalBox, float time)
    {
        arrivalBoxNum = arrivalBox;
        arriveTime = time;

        _arrivalBoxPos = arrivalArea.arrivalAreas[arrivalBoxNum].position;
    }

    public void MoveToPlayer()
    {
        Vector3 firstPos = transform.position;
        Vector3 secondPos = firstPos + new Vector3(0, maxHeight, 0);
        Vector3 thirdPos = secondPos + (_arrivalBoxPos - secondPos) / 2;
        Vector3 fourthPos = _arrivalBoxPos;

        transform.DOPath(new[]
            {
                new Vector3(firstPos.x, firstPos.y, firstPos.z),
                new Vector3(secondPos.x, secondPos.y, secondPos.z),
                //new Vector3(thirdPos.x, thirdPos.y, thirdPos.z),
                new Vector3(fourthPos.x, fourthPos.y, fourthPos.z)
            },
            arriveTime - _startTime,
            PathType.CatmullRom, PathMode.Full3D).SetEase(Ease.InQuint);
    }

    public void GoToRefrigerator()
    {
        
    }

    private void UpdateHitterSpeed()
    {
        Vector3 currentPos = this.transform.position;
        _moveVector = (_beforeVector - currentPos).normalized;
        _moveSpeed = (_beforeVector - currentPos).magnitude / Time.deltaTime;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        //if (other.transform.CompareTag(""))
    }

    private void Update()
    {
        UpdateHitterSpeed();
    }
}
