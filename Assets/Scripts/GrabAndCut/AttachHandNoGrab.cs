using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachHandNoGrab : MonoBehaviour
{
    public PullAndCutNoGrab _pullAndCutNoGrab;
    public Transform attachTransform;
    public bool IsAttached = false;
    
    void Start()
    {
        //_pullAndCutNoGrab = GetComponent<PullAndCutNoGrab>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10 && other.TryGetComponent(out PullAndCutNoGrab pullAndCutNoGrab) && other.TryGetComponent(out MoveToPlayer moveToPlayer))
        {
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
