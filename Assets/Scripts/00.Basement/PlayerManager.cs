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
    [SerializeField] public GameObject IK_player;
    public GameObject RightController;
    public GameObject LeftController;
    private XRBaseController R_XRController;
    private XRBaseController L_XRController;

    [Header("player Life")] public int playerLifeValue = 0;
    public GameObject[] parentUI = new GameObject[3];
    public GameObject[] score_G = new GameObject[3];

    
    [Header("Hearts (auto)")] public HeartsArray[] HeartGameObjects = new HeartsArray[3];

    public void Init()
    {
        Debug.Log("Initialize PlayerManager");

        // Game object setting
        player = GameObject.FindWithTag("Player");
        IK_player = GameObject.FindWithTag("IKPlayer");
        
        DontDestroyOnLoad(player);
        DontDestroyOnLoad(IK_player);
        
        RightController = Utils.FindChildByRecursion(player.transform, "Right Controller").gameObject;
        LeftController = Utils.FindChildByRecursion(player.transform, "Left Controller").gameObject;
        
        R_XRController = RightController.GetComponent<XRBaseController>();
        L_XRController = LeftController.GetComponent<XRBaseController>();
        
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
        Debug.Log($"TypeNum : {(int)GameManager.Wave.currentWave}");
        parentUI[idx].SetActive(true);
    }
    public void setHearts(int playerLifeValue)
    {
        int WaveTypeCount = System.Enum.GetValues(typeof(WaveType)).Length;
        for (int i = 0; i < WaveTypeCount; i++)
        {
            HeartGameObjects[i].hearts[playerLifeValue - 1].GetComponent<MeshRenderer>().material.color = Color.gray;
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
        setHearts(playerLifeValue);
        // HeartGameObjects[0].hearts[playerLifeValue - 1].GetComponent<MeshRenderer>().material.color = Color.gray;
        // HeartGameObjects[playerLifeValue - 1].SetActive(false);
        playerLifeValue--;
        Debug.Log("Attack Success player HP -1");
    }

    public void ActiveRightHaptic(float amplitude, float duration)
    {
        R_XRController.SendHapticImpulse(amplitude, duration);
    }
    
    public void ActiveLeftHaptic(float amplitude, float duration)
    {
        L_XRController.SendHapticImpulse(amplitude, duration);
    } 
}
