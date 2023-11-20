using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StretchTest : MonoBehaviour
{
    [SerializeField] private ActionBasedController actionBasedController;
    private WaitForSeconds _waitForDetectPositionDelayTime;
    [SerializeField] private float detectPositionDelayTime = 0.05f;
    [SerializeField] private Transform handModelTransform;
    void Start()
    {
        _waitForDetectPositionDelayTime = new WaitForSeconds(detectPositionDelayTime);
        //StartCoroutine(DetectHandPositionChange());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StopTrackingHandPosition();
            StartCoroutine(DetectHandPositionChange());
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            StopTrackingHandPosition();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            StartTrackingHandPosition();
        }
    }

    IEnumerator DetectHandPositionChange()
    {
        handModelTransform.DOMove(handModelTransform.position + Vector3.forward * 1f, 1f).SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutFlash);
        yield return _waitForDetectPositionDelayTime;
    }
    void StopTrackingHandPosition()
    {
        actionBasedController.positionAction.action.Disable();
        //actionBasedController.positionAction.DisableDirectAction();
        //inputAction = actionBasedController.positionAction.action; //new InputAction(actionBasedController.positionAction.action)
        // actionBasedController.positionAction.action.Dispose(); -> action 메모리 자체를 지워서 다른 입력도 안 받음.
        Debug.Log("Stop Tracking Hand Position ");
    }
    void StartTrackingHandPosition()
    {
        handModelTransform.localPosition =  new Vector3(0.000000f, 0.000000f, 0.000000f);//Vector3.zero;
        actionBasedController.positionAction.action.Enable();
        // handModelTransform.localPosition = new Vector3(0.000000f, 0.000000f, 0.000000f);
        // handModelTransform.localPosition = Vector3.zero;
        Debug.Log($"handModelTransform.localPosition : {handModelTransform.localPosition}");
        //actionBasedController.positionAction.action.actionMap.AddAction();
        Debug.Log("Start Tracking Hand Position ");
        //StartCoroutine(DetectHandPositionChange());
    }
}












