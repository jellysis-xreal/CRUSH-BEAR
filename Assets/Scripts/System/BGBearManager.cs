using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGBearManager : MonoBehaviour
{
    [SerializeField] public Animator[] bearAnimators;
    private int _processedBearNumber = 0;
    void Start()
    {
        bearAnimators = GetComponentsInChildren<Animator>();
    }

    public void MissNodeProcessing(GameObject triggeredGameobject)
    {
        PlayFailAnimation();
        AttachDecoItemToBear(triggeredGameobject);   
        _processedBearNumber++;
    }
    private void PlayFailAnimation()
    {
        bearAnimators[_processedBearNumber].SetTrigger("Fail");
    }
    
    private void AttachDecoItemToBear(GameObject triggeredGameobject)
    {
        triggeredGameobject.transform.position = bearAnimators[_processedBearNumber].transform.position;
    }
}
