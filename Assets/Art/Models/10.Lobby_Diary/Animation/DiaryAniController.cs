using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class DiaryAniController : MonoBehaviour
{
    public Animator scene1_Anim, scene2_Anim, scene3_Anim, scene4_Anim;
    public Animator book_Anim;
    public int bookPage = 0;
    public GameObject text1, text2, text3, text4;
    public bool returned = true;
    public Animator text_animation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            nextPage();
        }
    }

    // public void setIsPlaying(bool isPlaying_) {
    //     isPlaying = isPlaying_;
    // }

    // public bool getIsPlaying() {
    //     return isPlaying;
    // }


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
                text1.SetActive(false);
                book_Anim.SetTrigger("next");
                StartCoroutine(openPage());
                break;
            case 2:
                bookPage++;
                scene2_Anim.SetTrigger("close");
                text2.SetActive(false);
                book_Anim.SetTrigger("next");
                StartCoroutine(openPage());
                break;
            case 3:
                bookPage++;
                scene3_Anim.SetTrigger("close");
                text3.SetActive(false);
                book_Anim.SetTrigger("next");
                StartCoroutine(openPage());
                break;
            case 4:
                bookPage = 0;
                scene4_Anim.SetTrigger("close");
                text4.SetActive(false);
                book_Anim.SetTrigger("next");
                StartCoroutine(openPage());
                break;
        }
    }

    IEnumerator openPage()
    {
        yield return new WaitForSeconds(1.0f);
        // isPlaying = false;
        // Debug.Log("[TEST] isPlaying: " + isPlaying.ToString());
        switch (bookPage)
        {
            case 1:
                scene1_Anim.SetTrigger("open");
                text1.SetActive(true);
                text_animation.Play("ui_pop1");
                break;
            case 2:
                scene2_Anim.SetTrigger("open");
                text2.SetActive(true);
                text_animation.Play("ui_pop2");
                break;
            case 3:
                scene3_Anim.SetTrigger("open");
                text3.SetActive(true);
                text_animation.Play("ui_pop3");
                break;
            case 4:
                scene4_Anim.SetTrigger("open");
                text4.SetActive(true);
                text_animation.Play("ui_pop4");
                break;
            case 0:
                GameManager.UI.SetRayOn(false);
                GameManager.Instance.LobbyToWave();
                break;
        }
    }
}
