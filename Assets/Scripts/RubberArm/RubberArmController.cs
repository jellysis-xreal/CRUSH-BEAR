using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RubberArmController : MonoBehaviour
{
    [SerializeField] private ActionBasedController actionBasedController;

    [SerializeField] private Transform bodyTransform;
    [SerializeField] private float minimumDistanceToDetect = 0.1f; // body, hand의 거리가 10cm 보다 가까운 지점에서 시작해야 차징됨.
    [SerializeField] private float maximumDistanceToDetect = 0.3f; // body, hand의 거리가 30cm 보다 먼 지점에 위치하게 되면 발사.
    [SerializeField] private float detectPositionDelayTime = 0.05f;
    private WaitForSeconds _waitForDetectPositionDelayTime;
    void Start()
    {
        _waitForDetectPositionDelayTime = new WaitForSeconds(detectPositionDelayTime);
        //StartCoroutine(DetectHandPositionChange());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DetectHandPositionChange()
    {
        Vector3 beginHandPosition = Vector3.zero;
        Vector3 endHandPosition = Vector3.zero;

        while (true)
        {
            Debug.Log($"{name} // Vector3.Distance(transform.position, bodyTransform.position) : {Vector3.Distance(transform.position, bodyTransform.position)}");
            if (Vector3.Distance(transform.position, bodyTransform.position) < minimumDistanceToDetect)
            {
                Debug.Log($"{name} // Set Start HandTransform");
                beginHandPosition = transform.position;   
            }
            else if(beginHandPosition != Vector3.zero)
            {
                endHandPosition = transform.position;
                float dist = Vector3.Distance(beginHandPosition, endHandPosition);
                Debug.Log($"{name} // start : {beginHandPosition}, end : {endHandPosition}, dist : {dist}");
                if ( dist > maximumDistanceToDetect)
                {
                    StartCoroutine(StretchRubberArm(dist, beginHandPosition, endHandPosition-beginHandPosition));
                    break;
                }
            }
            yield return _waitForDetectPositionDelayTime;
        }

    }

    IEnumerator StretchRubberArm(float chargingPower, Vector3 startPosition, Vector3 dir)
    {
        // 고무고무팔!
        // 1. Stop Tracking
        // 2. startTransform을 기준으로 팔 늘이고 다시 줄이기

        StopTrackingHandPosition();
        float movingTime = chargingPower * 20f;
        Debug.Log($"Strecth Charging Power {chargingPower}, Time");
        transform.DOMove(startPosition + dir * 10f, movingTime).SetLoops(1);
        yield return new WaitForSeconds(movingTime);
        StartTrackingHandPosition();
    }
    
    void StopTrackingHandPosition()
    {
        actionBasedController.positionAction.action.Disable();
        Debug.Log("Stop Tracking Hand Position ");
    }
    void StartTrackingHandPosition()
    {
        actionBasedController.positionAction.action.Enable();
        Debug.Log("Start Tracking Hand Position ");
    }
}
