using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSFX : MonoBehaviour
{

    private void Update()
    {
        // effect test
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("== Q : playEffect [sfx_btn_select] ");

            GameManager.Sound.playEffect("sfx_btn_select");
            // AudioManager.instance.playEffect("sfx_btn_select");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("== W : playEffect [sfx_btn_close] ");
            GameManager.Sound.playEffect("sfx_btn_close");
            // AudioManager.instance.playEffect("sfx_btn_close");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            GameManager.Sound.playEffect("sfx_btn_close");


        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Sound.playEffect("sfx_btn_confirm");


        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            GameManager.Sound.playEffect("sfx_com_countdown");


        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            GameManager.Sound.playEffect("sfx_com_countdown_start");


        }

        // BGM test
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("== Z : playMusic");
            GameManager.Sound.PlayWaveMusic(0);
            // AudioManager.instance.playMusic(0);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("== X : playMusic");
            GameManager.Sound.PlayWaveMusic(1);
            // AudioManager.instance.playMusic(0);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("== C : playMusic");
            GameManager.Sound.PlayWaveMusic(2);
            // AudioManager.instance.playMusic(0);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("== V : playMusic");
            GameManager.Sound.PlayWaveMusic(3);
            // AudioManager.instance.playMusic(0);
        }


        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("== H : stopMusic");

            GameManager.Sound.StopWaveMusic(0);
            // AudioManager.instance.stopMusic();

        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("== J : stopMusic");

            GameManager.Sound.StopWaveMusic(1);
            // AudioManager.instance.stopMusic();

        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("== K : stopMusic");

            GameManager.Sound.StopWaveMusic(2);
            // AudioManager.instance.stopMusic();

        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("== L : stopMusic");

            GameManager.Sound.StopWaveMusic(3);
            // AudioManager.instance.stopMusic();

        }

        

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    Debug.Log("== R : SetMusicVolume");
        //    GameManager.Sound.SetMusicVolume(0.8f);


        //}


        //
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("== P : pauseMusic");
            GameManager.Sound.PauseMusic(0, true);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("== O : RestartMusic");
            GameManager.Sound.RestartMusic(0, true);


        }
    }
}