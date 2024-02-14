using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSFX : MonoBehaviour
{

    public float vibrationStrength = 0.5f;
    public float duration = 0.1f;
    public int numVibrations = 3;
    public float interval = 0.5f;


    //private void Start()
    //{
    //    soundManager = FindObjectOfType<SoundManager>();
    //}

    private void Update()
    {
        // 'q' 키
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("== Q : playEffect [sfx_btn_select] ");
            AudioManager.instance.playEffect("sfx_btn_select");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("== W : playEffect [sfx_btn_close] ");
            AudioManager.instance.playEffect("sfx_btn_close");

        }

        // BGM test
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("== Z : playMusic");
            //AudioManager.instance.PlayWaveMusic(0);
            AudioManager.instance.playMusic("kawaii");

        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("== X : stopMusic");
            AudioManager.instance.stopMusic();

        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("== C : pauseMusic");
            AudioManager.instance.pauseMusic();


        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("== V : resumeMusic");
            AudioManager.instance.resumeMusic();


        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("== A : playMusic");
            AudioManager.instance.playMusic("pastel_heart");

        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("SetMusicVolume(0.2f)");
            AudioManager.instance.SetMusicVolume(0.2f);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("SetMusicVolume(0.8f)");
            AudioManager.instance.SetMusicVolume(0.8f);
        }



        // Haptic test

        // p 키
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("== P key down");

            // HapticManager를 통해 진동 패턴을 실행
            //HapticManager.Instance.TriggerHapticPattern(OVRInput.Controller.RTouch, vibrationStrength, duration, numVibrations, interval);
        }
    }
}
