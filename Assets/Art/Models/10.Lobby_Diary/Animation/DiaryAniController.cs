using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class DiaryAniController : MonoBehaviour
{
    public Animator scene1_Anim, scene2_Anim, scene3_Anim, scene4_Anim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            scene1_Anim.SetTrigger("open");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            scene2_Anim.SetTrigger("open");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            scene3_Anim.SetTrigger("open");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            scene4_Anim.SetTrigger("open");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            scene1_Anim.SetTrigger("close");
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            scene2_Anim.SetTrigger("close");
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            scene3_Anim.SetTrigger("close");
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            scene4_Anim.SetTrigger("close");
        }
    }
}
