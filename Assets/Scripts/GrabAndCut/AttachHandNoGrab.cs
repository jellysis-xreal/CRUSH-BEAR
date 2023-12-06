using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

public class AttachHandNoGrab : MonoBehaviour
{
    public Transform attachTransform;
    public bool IsAttached = false;

    [Header("For Judgment")] public GameObject HandDestoryer;

    private HandData HandData;
    private PullAndCutNoGrab _pullAndCutNoGrab;

    void Start()
    {
        //_pullAndCutNoGrab = GetComponent<PullAndCutNoGrab>();
        HandData = transform.GetChild(0).GetComponent<HandData>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 부술 물체와 인터렉션 했는데, Hand가 Grab되지 않은 경우
        if (HandDestoryer.activeSelf == false &&
            other.GetComponent<BaseObject>().InteractionType == InteractionType.Break)
            GameManager.Score.Scoring(other.gameObject);

        if (other.gameObject.layer == 10 && other.TryGetComponent(out PullAndCutNoGrab pullAndCutNoGrab) &&
            other.TryGetComponent(out MoveToPlayer moveToPlayer))
        {
            // 찢을 물체와 인터렉션 했는데, Hand가 Grab되어 있는 경우
            if (HandDestoryer.activeSelf == true && HandData.ControllerType == InteractionType.Break)
                GameManager.Score.Scoring(other.gameObject);
            
            other.GetComponent<IMovement>().StopMoving();
            
            _pullAndCutNoGrab = pullAndCutNoGrab; //other.GetComponent<PullAndCutNoGrab>();// //// //v 
            _pullAndCutNoGrab.AttachHand(this.transform, other.ClosestPoint(transform.position));

            moveToPlayer.enabled = false;

            Debug.Log($"hand world Position {transform.position}");
            IsAttached = true;
        }
        else
        {
            IsAttached = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        /*if (other.gameObject.layer == 10)
        {
            _pullAndCutNoGrab.DetachHand();
            _pullAndCutNoGrab = null;    
        }*/
    }
}