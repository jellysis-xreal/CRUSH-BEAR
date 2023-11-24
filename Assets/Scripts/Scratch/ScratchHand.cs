using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScratchHand : MonoBehaviour
{
    [Header("Game Object")]
    public GameObject R_Controller;
    public GameObject L_Controller;
    public GameObject R_ScratchNails;
    public GameObject L_ScratchNails;

    [Header("Setting Value")]
    [SerializeField] private float activeSpeed = 1.0f;
    [SerializeField] private bool activeRCut = false;
    [SerializeField] private bool activeLCut = false;

    [Header("For Check Value")]
    [SerializeField] private float R_Speed;
    [SerializeField] private float L_Speed;
    
    private Vector3 R_LastPosition;
    private Vector3 L_LastPosition;
    
    void GetHandSpeed()
    {
        R_Speed = ((R_Controller.transform.position - R_LastPosition).magnitude / Time.deltaTime);
        R_LastPosition = R_Controller.transform.position;

        L_Speed = ((L_Controller.transform.position - L_LastPosition).magnitude / Time.deltaTime);
        L_LastPosition = L_Controller.transform.position;
        
        //Debug.Log("오른손의 속도는 " + R_Speed);
        //Debug.Log("왼손의 속도는 " + L_Speed);
    }

    void SetHandNails()
    {
        R_ScratchNails.SetActive(activeRCut);
        L_ScratchNails.SetActive(activeLCut);
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        GetHandSpeed();
        
        activeRCut = (R_Speed > activeSpeed) ? true : false;
        activeLCut = (L_Speed > activeSpeed) ? true : false;
        
        SetHandNails();
    }
}
