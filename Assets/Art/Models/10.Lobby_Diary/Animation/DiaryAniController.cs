using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class DiaryAniController : MonoBehaviour
{
    public Animator scene1_Anim, scene2_Anim, scene3_Anim, scene4_Anim;
    public Animator book_Anim;
    public int bookPage = 0;
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
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            nextPage();
        }
    }

    public void nextPage()
    {
        switch(bookPage)
        {
            case 0:
                bookPage++;
                book_Anim.SetTrigger("next");
                StartCoroutine(openPage());
                break;
            case 1:
                bookPage++;
                scene1_Anim.SetTrigger("close");
                book_Anim.SetTrigger("next");
                StartCoroutine(openPage());
                break;
            case 2:
                bookPage++;
                scene2_Anim.SetTrigger("close");
                book_Anim.SetTrigger("next");
                StartCoroutine(openPage());
                break;
            case 3:
                bookPage++;
                scene3_Anim.SetTrigger("close");
                book_Anim.SetTrigger("next");
                StartCoroutine(openPage());
                break;
            case 4:
                bookPage = 0;
                scene4_Anim.SetTrigger("close");
                //book_Anim.SetTrigger("next");
                //StartCoroutine(openPage());
                break;
        }
    }

    IEnumerator openPage()
    {
        yield return new WaitForSeconds(1.0f);
        switch (bookPage)
        {
            case 1:
                scene1_Anim.SetTrigger("open");
                break;
            case 2:
                scene2_Anim.SetTrigger("open");
                break;
            case 3:
                scene3_Anim.SetTrigger("open");
                break;
            case 4:
                scene4_Anim.SetTrigger("open");
                break;
        }
    }
}
