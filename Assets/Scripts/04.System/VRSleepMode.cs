using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;

public class VRSleepMode : MonoBehaviour
{
    private bool isGamePaused = false;
    private InputDevice hmdDevice;

    void Start()
    {
        DontDestroyOnLoad(this);
        StartCoroutine(GetHMDDevice());
        StartCoroutine(CheckHeadsetPresence());
    }

    IEnumerator GetHMDDevice()
    {
        List<InputDevice> devices = new List<InputDevice>();
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
                if (hmdDevice.TryGetFeatureValue(CommonUsages.userPresence, out userPresent))
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
            yield return new WaitForSecondsRealtime(0.1f); // 실제 시간으로 0.1초마다 체크
        }
    }

    void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
        //Debug.Log("게임이 일시 정지되었습니다.");
        GameManager.Sound.PauseAllSound();
        // 여기에 게임 일시 정지에 필요한 추가 기능
    }

    void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
        //Debug.Log("게임이 재개되었습니다.");
        GameManager.Sound.ResumeAllSound();
        // 여기에 게임 재개에 필요한 추가 기능
    }
}