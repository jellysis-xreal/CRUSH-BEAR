using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;

public class VRSleepMode : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this);
    }
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // 게임 일시 정지 로직
            PauseGame();
        }
        else
        {
            // 게임 재개 로직
            ResumeGame();
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            // 게임 일시 정지 로직
            PauseGame();
        }
        else
        {
            // 게임 재개 로직
            ResumeGame();
        }
    }
    void PauseGame()
    {
        Debug.Log("퍼즈!");
        Time.timeScale = 0f;
        //Debug.Log("게임이 일시 정지되었습니다.");
        if (GameManager.Instance != null)
        {
            GameManager.Sound.PauseAllSound();
        }
    }

    void ResumeGame()
    {
        Debug.Log("리슘!");
        Time.timeScale = 1f;
        //Debug.Log("게임이 재개되었습니다.");
        if (GameManager.Instance != null)
            GameManager.Sound.ResumeAllSound();
    }
}