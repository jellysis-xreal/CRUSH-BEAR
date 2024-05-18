using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class RubberArmController : MonoBehaviour
{
    [SerializeField] private ActionBasedController actionBasedController;

    [SerializeField] private Transform bodyTransform;
    [SerializeField] private float minimumDistanceToDetectBodyAndHand = 0.3f; // body, hand의 거리가 10cm 보다 가까운 지점에서 시작해야 차징됨.
    [SerializeField] private float maximumDistanceToDetectEachHand = 0.3f; // body, hand의 거리가 30cm 보다 먼 지점에 위치하게 되면 발사.
    [SerializeField] private float detectPositionDelayTime = 0.05f;
    private WaitForSeconds _waitForDetectPositionDelayTime;
    private InputAction inputAction;

    [SerializeField] private Transform handModelTransform;
    void Start()
    {
        _waitForDetectPositionDelayTime = new WaitForSeconds(detectPositionDelayTime);
        StartCoroutine(DetectHandPositionChange());
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StopTrackingHandPosition();
            handModelTransform.DOMoveX(1, 1).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutFlash);
        } 
        if (Input.GetKeyDown(KeyCode.I))
        {
            handModelTransform.DOMoveX(1, 1).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutFlash);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartTrackingHandPosition();
        }
    }
        
    // 늘어난 상태에서 Grab 가능. Grab 하자마자. 모든 action이 Enable된다. DoMove 메서드 취소됨.
    // 1. Grab한다. => action Enable됨.
    // 2. 다시 StopTrackingHandPositioin 호출. (여기서 잡은 상태로 딸려와야 함.)
    // 3. XR Grab Interactable Select Entered 이벤트에 추가된 원래 위치로 돌아가는 메서드 실행.
    IEnumerator DetectHandPositionChange()
    {
        Vector3 beginHandPosition = Vector3.zero;
        Vector3 endHandPosition = Vector3.zero;

        while (true)
        {
            //Debug.Log($"{name} Distance between body and hand : {Vector3.Distance(transform.position, bodyTransform.position)}");
            if (Vector3.Distance(transform.position, bodyTransform.position) < minimumDistanceToDetectBodyAndHand)
            {
                //Debug.Log($"{name} // Set Start HandTransform");
                beginHandPosition = transform.position;   
            }
            if(beginHandPosition != Vector3.zero)
            {
                endHandPosition = transform.position;
                float dist = Vector3.Distance(beginHandPosition, endHandPosition);
                //Debug.Log($"{name} dist : {dist}");
                if ( dist > maximumDistanceToDetectEachHand)
                {
                    StartCoroutine(StretchRubberArm(dist, beginHandPosition, endHandPosition-beginHandPosition));
                    break;
                }
            }
            yield return _waitForDetectPositionDelayTime;
        }

    }
    // 실제 세상에서 컨트롤러를 놓았다 잡으면 자연스레 actionBasedController.positionAction.action.Enable()가 호출되는 듯
    // 

    IEnumerator StretchRubberArm(float chargingPower, Vector3 startPosition, Vector3 dir)
    {
        // 고무고무팔!
        // 1. Stop Tracking
        // 2. startTransform을 기준으로 팔 늘이고 다시 줄이기
        // 3. Start Tracking
        // Hand_right의 world position 기억해서 Hand_right에서 world position으로 초기화하고 시작
        Vector3 handTransformOrigin = handModelTransform.position;
        StopTrackingHandPosition();
        yield return new WaitForSeconds(0.1f);
        float movingTime = chargingPower * 1f;
        // Debug.Log($"Strecth Charging Power {chargingPower}, moving Time : {movingTime}");
        
        handModelTransform.position = handTransformOrigin;
        handModelTransform.DOMove(handModelTransform.position + dir * 3f, movingTime).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Flash);
        yield return new WaitForSeconds(movingTime * 2);
        StartTrackingHandPosition();
    }

    public void Test()
    {
        StopTrackingHandPosition();
        StartTrackingHandPosition();
    }
    public void StopTrackingHandPosition()
    {
        actionBasedController.positionAction.action.Disable();
        //actionBasedController.positionAction.DisableDirectAction();
        //inputAction = actionBasedController.positionAction.action; //new InputAction(actionBasedController.positionAction.action)
        // actionBasedController.positionAction.action.Dispose(); -> action 메모리 자체를 지워서 다른 입력도 안 받음.
        Debug.Log("Stop Tracking Hand Position ");
    }
    public void StartTrackingHandPosition()
    {
        handModelTransform.localPosition =  new Vector3(0.000000f, 0.000000f, 0.000000f);//Vector3.zero;
        actionBasedController.positionAction.action.Enable();
        // handModelTransform.localPosition = new Vector3(0.000000f, 0.000000f, 0.000000f);
        // handModelTransform.localPosition = Vector3.zero;
        Debug.Log($"handModelTransform.localPosition : {handModelTransform.localPosition}");
        //actionBasedController.positionAction.action.actionMap.AddAction();
        Debug.Log("Start Tracking Hand Position ");
        StartCoroutine(DetectHandPositionChange());
    }
}
