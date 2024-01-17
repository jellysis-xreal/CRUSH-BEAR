using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer lr;
    public Transform playerTransform;
    private Vector3 dir;
    // Start is called before the first frame update
    
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        playerTransform = GameObject.FindWithTag("MainCamera").transform;
        dir = playerTransform.position - transform.position;
        dir.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        // lr.SetPosition(0, transform.position);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, dir, out hit)){
            if (hit.collider)
            {
                lr.SetPosition(1, hit.point);
                // Debug.Log("[TEST] position: " + transform.position + " forward: " + transform.forward);
            } else lr.SetPosition(1, dir*5000);
        }
    }
}
