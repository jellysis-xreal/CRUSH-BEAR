using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using XRController = UnityEngine.InputSystem.XR.XRController;

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
    public GameObject[] HeartGameObjects;


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
        HeartGameObjects[playerLifeValue - 1].GetComponent<MeshRenderer>().material.color = Color.gray;
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
