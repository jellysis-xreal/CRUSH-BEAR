using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using XRController = UnityEngine.InputSystem.XR.XRController;
using System;
using EnumTypes;

[System.Serializable] // 반드시 필요
public class HeartsArray // 행에 해당되는 이름
{
    public GameObject[] hearts = new GameObject[5];
}

public class PlayerManager : MonoBehaviour
{
    [Header("setting(auto)")]
    [SerializeField] public GameObject player;
    //[SerializeField] public GameObject IK_player;
    public GameObject RightController;
    public GameObject LeftController;
    private XRBaseController R_XRController;
    private XRBaseController L_XRController;
    public GameObject RightInteraction;
    public GameObject LeftInteraction;

    [Header("player Life")] public int playerLifeValue = 0;
    public GameObject[] parentUI = new GameObject[3];
    public GameObject[] score_G = new GameObject[3];
    public ParticleSystem minusPrefab;

    
    [Header("Hearts (auto)")] public HeartsArray[] HeartGameObjects = new HeartsArray[3];

    public void Init()
    {
        Debug.Log("Initialize PlayerManager");

        // Game object setting
        player = GameObject.FindWithTag("Player");
        //IK_player = GameObject.FindWithTag("IKPlayer");
        
        DontDestroyOnLoad(player);
        //DontDestroyOnLoad(IK_player);
        
        RightController = Utils.FindChildByRecursion(player.transform, "Right Controller").gameObject;
        LeftController = Utils.FindChildByRecursion(player.transform, "Left Controller").gameObject;
        
        R_XRController = RightController.GetComponent<XRBaseController>();
        L_XRController = LeftController.GetComponent<XRBaseController>();
        
        RightInteraction = Utils.FindChildByRecursion(RightController.transform, "Interaction")
            .gameObject;
        LeftInteraction = Utils.FindChildByRecursion(LeftController.transform, "Interaction")
            .gameObject;
        // Player Life
        playerLifeValue = 5;

        // Initialize GameObject_Hearts
        HeartsInit();
    }

    public void HeartsInit()
    {
        int WaveTypeCount = System.Enum.GetValues(typeof(WaveType)).Length;
        Debug.Log($"WaveTypeCount : {WaveTypeCount}");
        for (int i = 0; i < WaveTypeCount; i++)
        {
            HeartGameObjects[i].hearts[0] = score_G[i].transform.GetChild(0).gameObject;
            HeartGameObjects[i].hearts[1] = score_G[i].transform.GetChild(1).gameObject;
            HeartGameObjects[i].hearts[2] = score_G[i].transform.GetChild(2).gameObject;
            HeartGameObjects[i].hearts[3] = score_G[i].transform.GetChild(3).gameObject;
            HeartGameObjects[i].hearts[4] = score_G[i].transform.GetChild(4).gameObject;
            Debug.Log("[TEST] hearts init " + i.ToString());
        }
    }

    public void PlaySceneUIInit(int idx)
    {
        for (int i = 0; i < parentUI.Length; i++)
        {
            parentUI[i].gameObject.SetActive(false);            
        }
        // Debug.Log($"TypeNum : {(int)GameManager.Wave.currentWave}");
        parentUI[idx].SetActive(true);
    }

    public void FinishSceneUI()
    {
        for (int i = 0; i < parentUI.Length; i++)
        {
            parentUI[i].gameObject.SetActive(false);            
        }
    }
    
    public void SetHearts(int playerLifeValue)
    {
        int WaveTypeCount = System.Enum.GetValues(typeof(WaveType)).Length;
        for (int i = 0; i < WaveTypeCount; i++)
        {
            HeartGameObjects[i].hearts[playerLifeValue - 1].GetComponent<MeshRenderer>().material.color = Color.black;
            minusPrefab.transform.position = HeartGameObjects[(int)GameManager.Wave.currentWave].hearts[playerLifeValue - 1].transform.position;
            minusPrefab.Play();
        }
    }

    // 처음에 3
    public void MinusPlayerLifeValue()
    {
        if (playerLifeValue == 0) return;
        /*for (int i = HeartGameObjects.Length - 1; i >= 0 ; i--)
        {
            HeartGameObjects[i].activeSelf
        }*/
        // 3 - 1 = 2 2번 인덱스 꺼야댐
        SetHearts(playerLifeValue);
        // HeartGameObjects[0].hearts[playerLifeValue - 1].GetComponent<MeshRenderer>().material.color = Color.gray;
        // HeartGameObjects[playerLifeValue - 1].SetActive(false);
        playerLifeValue--;
        Debug.Log("Attack Success player HP -1");
    }

    // Ending Scene에서 main play 캐릭터 필요 없음
    public void InActivePlayer()
    {
        player.SetActive(false);
        //IK_player.SetActive(false);
    }
    
    // Haptic
    // 기본 진동
    public void ActiveRightHaptic(float amplitude, float duration)
    {
        R_XRController.SendHapticImpulse(amplitude, duration);
    }
    
    public void ActiveLeftHaptic(float amplitude, float duration)
    {
        L_XRController.SendHapticImpulse(amplitude, duration);
    }

    // 반복 진동
    public void RepeatRightHaptic(float amplitude, float duration, int n) // 오른손 진동 n번 반복
    {
        StartCoroutine(RepeatRightHapticCoroutine(amplitude, duration, n));
    }
    private IEnumerator RepeatRightHapticCoroutine(float amplitude, float duration, int n)
    {
        for (int i = 0; i < n; i++)
        {
            R_XRController.SendHapticImpulse(amplitude, duration);
            yield return new WaitForSeconds(0.5f); // 0.5초 대기
        }
    }

    public void RepeatLeftHaptic(float amplitude, float duration, int n) // 왼손 진동 n번 반복
    {
        StartCoroutine(RepeatLeftHapticCoroutine(amplitude, duration, n));
    }
    private IEnumerator RepeatLeftHapticCoroutine(float amplitude, float duration, int n)
    {
        for (int i = 0; i < n; i++)
        {
            L_XRController.SendHapticImpulse(amplitude, duration);
            yield return new WaitForSeconds(0.5f); // 0.5초 대기
        }
    }

    // 점차 약해지는 진동
    public void DecreaseRightHaptic(float startAmplitude, float duration) // 오른손
    {
        StartCoroutine(DecreaseRightHapticCoroutine(startAmplitude, duration));
    }
    private IEnumerator DecreaseRightHapticCoroutine(float startAmplitude, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            float elapsedTime = Time.time - startTime;
            float t = elapsedTime / duration;
            float currentAmplitude = Mathf.Lerp(startAmplitude, 0f, t);

            R_XRController.SendHapticImpulse(currentAmplitude, 0.1f); // 0.1초 간격으로 진동

            yield return null;
        }
        R_XRController.SendHapticImpulse(0f, 0.1f); // 완전 중지
    }

    public void DecreaseLeftHaptic(float startAmplitude, float duration)
    {
        StartCoroutine(DecreaseLeftHapticCoroutine(startAmplitude, duration));
    }
    private IEnumerator DecreaseLeftHapticCoroutine(float startAmplitude, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            float elapsedTime = Time.time - startTime;
            float t = elapsedTime / duration;
            float currentAmplitude = Mathf.Lerp(startAmplitude, 0f, t);

            L_XRController.SendHapticImpulse(currentAmplitude, 0.1f); // 0.1초 간격으로 진동

            yield return null;
        }
        L_XRController.SendHapticImpulse(0f, 0.1f); // 완전 중지
    }

    // 점차 강해지는 진동
    public void IncreaseRightHaptic(float startAmplitude, float duration) // 오른손
    {
        StartCoroutine(IncreaseRightHapticCoroutine(startAmplitude, duration));
    }
    private IEnumerator IncreaseRightHapticCoroutine(float startAmplitude, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            float elapsedTime = Time.time - startTime;
            float t = elapsedTime / duration;
            float currentAmplitude = Mathf.Lerp(0f, startAmplitude, t);

            R_XRController.SendHapticImpulse(currentAmplitude, 0.1f); // 0.1초 간격으로 진동

            yield return null;
        }
        R_XRController.SendHapticImpulse(0f, 0.1f); // 완전 중지
    }

    public void IncreaseLeftHaptic(float startAmplitude, float duration) // 왼손
    {
        StartCoroutine(IncreaseLeftHapticCoroutine(startAmplitude, duration));
    }
    private IEnumerator IncreaseLeftHapticCoroutine(float startAmplitude, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            float elapsedTime = Time.time - startTime;
            float t = elapsedTime / duration;
            float currentAmplitude = Mathf.Lerp(0f, startAmplitude, t);

            L_XRController.SendHapticImpulse(currentAmplitude, 0.1f); // 0.1초 간격으로 진동

            yield return null;
        }
        L_XRController.SendHapticImpulse(0f, 0.1f); // 완전 중지
    }

}
