using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachHandNoGrab : MonoBehaviour
{
    public PullAndCutNoGrab _pullAndCutNoGrab;
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
        if (other.gameObject.layer == 10)
        {
            _pullAndCutNoGrab = other.GetComponent<PullAndCutNoGrab>();
            _pullAndCutNoGrab.AttachHand(this.transform, other.ClosestPoint(transform.position));
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        /*if (other.gameObject.layer == 10)
        {
            _pullAndCutNoGrab.DetachHand(this.transform);
            _pullAndCutNoGrab = null;    
        }*/
    }
}
