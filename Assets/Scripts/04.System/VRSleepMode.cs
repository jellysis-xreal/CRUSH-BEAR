using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class VRSleepMode : MonoBehaviour
{
    private bool isGamePaused = false;
    private UnityEngine.XR.InputDevice hmdDevice;

    void Start()
    {
        DontDestroyOnLoad(this);
        StartCoroutine(GetHMDDevice());
        StartCoroutine(CheckHeadsetPresence());
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
    IEnumerator GetHMDDevice()
    {
        List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, devices);
        while (devices.Count == 0)
        {
            yield return null;
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, devices);
        }
        hmdDevice = devices[0];
    }

    IEnumerator CheckHeadsetPresence()
    {
        while (true)
        {
            if (hmdDevice.isValid)
            {
                bool userPresent;
                if (hmdDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.userPresence, out userPresent))
                {
                    if (!userPresent && !isGamePaused)
                    {
                        PauseGame();
                    }
                    else if (userPresent && isGamePaused)
                    {
                        ResumeGame();
                    }
                }
            }
            yield return new WaitForSecondsRealtime(3f); // 실제 시간으로 3초마다 체크
        }
    }

    void PauseGame()
    {
        Debug.Log("퍼즈!");
        isGamePaused = true;
        Time.timeScale = 0f;
        //Debug.Log("게임이 일시 정지되었습니다.");
        if (GameManager.Instance != null)
            GameManager.Sound.PauseAllSound();
    }

    void ResumeGame()
    {
        Debug.Log("리슘!");
        isGamePaused = false;
        Time.timeScale = 1f;
        //Debug.Log("게임이 재개되었습니다.");
        if (GameManager.Instance != null)
            GameManager.Sound.ResumeAllSound();
    }
}