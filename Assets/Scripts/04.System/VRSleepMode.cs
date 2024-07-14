using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class VRSleepMode : MonoBehaviour
{
    private bool isGamePaused = false;
    private UnityEngine.XR.InputDevice hmdDevice;
    public InputActionAsset inputActions; // Input Action Asset을 에디터에서 할당
    private InputAction menuButtonAction;

    private bool isPausedByUser = false;
    private List<InputAction> allActions;

    void Start()
    {
        DontDestroyOnLoad(this);
        StartCoroutine(GetHMDDevice());
        StartCoroutine(CheckHeadsetPresence());
        allActions = new List<InputAction>();
        foreach (var map in inputActions.actionMaps)
        {
            allActions.AddRange(map.actions);
        }
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
                    if (!userPresent && !isGamePaused && !isPausedByUser)
                    {
                        PauseGame();
                    }
                    else if (userPresent && isGamePaused && !isPausedByUser)
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
        // 여기에 게임 일시 정지에 필요한 추가 기능
        if (allActions != null)
        {
            foreach (var action in allActions)
            {
                if (action != menuButtonAction)
                {
                    action.Disable();
                }
            }
        }
        if (isPausedByUser)
        {
            StartCoroutine(ResumeTimer());
        }
    }

    void ResumeGame()
    {
        Debug.Log("리슘!");
        isGamePaused = false;
        Time.timeScale = 1f;
        //Debug.Log("게임이 재개되었습니다.");
        if(GameManager.Instance != null)
            GameManager.Sound.ResumeAllSound();
        // 여기에 게임 재개에 필요한 추가 기능
        if(allActions != null)
        {
            foreach (var action in allActions)
            {
                if (action != menuButtonAction)
                {
                    action.Enable();
                }
            }
        }
    }

    private IEnumerator ResumeTimer()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        ResumeGame();
        isPausedByUser = false;
    }

    void OnEnable()
    {
        // RightHand Controller Action Map에서 menuButton 액션을 찾습니다.
        var rightHandMap = inputActions.FindActionMap("XRI RightHand");
        menuButtonAction = rightHandMap.FindAction("menuButton");

        // 메뉴 버튼 액션에 이벤트 핸들러를 추가합니다.
        menuButtonAction.performed += OnMenuButtonPressed;
        menuButtonAction.Enable();
    }

    void OnDisable()
    {
        // 메뉴 버튼 액션에서 이벤트 핸들러를 제거합니다.
        menuButtonAction.performed -= OnMenuButtonPressed;
        menuButtonAction.Disable();
    }

    void OnMenuButtonPressed(InputAction.CallbackContext context)
    {
        if (!isPausedByUser)
        {
            isPausedByUser = true;
            PauseGame();
        }
    }
}