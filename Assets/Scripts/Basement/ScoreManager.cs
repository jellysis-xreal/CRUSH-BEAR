using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using EnumTypes;
using UnityEngine.InputSystem.Controls;
using UnityEngine.PlayerLoop;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

// Player의 Interaction event를 확인하고,
// Perfect/Good/Bad의 점수를 판별합니다

public class ScoreManager : MonoBehaviour
{
    [Header("setting value")]
    public float score;
    [SerializeField] private float maxSpeed = 3.5f;

    [Header("setting(auto)")] 
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject RightController;
    [SerializeField] private GameObject LeftController;
    private InputActionProperty RControllerInput;
    private InputActionProperty LControllerInput;

    [Header("playing value")] 
    [SerializeField] private float RControllerSpeed;
    [SerializeField] private float LControllerSpeed;

    [SerializeField] private InteractionType RControllerType;
    [SerializeField] private InteractionType LControllerType;

    private float standardSpeed;
    private Vector3 RbeforePos, LbeforePos;
    private AttachHandNoGrab RAttachNoGrab;
    private AttachHandNoGrab LAttachNoGrab;

    private enum scoreType
    {
        Perfect,
        Good,
        Bad
    }
    
    public void Init()
    {
        player = GameObject.FindWithTag("Player");
        RightController = Utils.FindChildByRecursion(player.transform, "Right Controller").gameObject;
        LeftController = Utils.FindChildByRecursion(player.transform, "Left Controller").gameObject;
        
        RbeforePos = RightController.transform.position;
        LbeforePos = LeftController.transform.position;

        RControllerInput = RightController.transform.GetChild(0).GetComponent<AnimateHandOnInput>().grabAnimationAction;
        LControllerInput = LeftController.transform.GetChild(0).GetComponent<AnimateHandOnInput>().grabAnimationAction;

        RAttachNoGrab = RightController.GetComponent<AttachHandNoGrab>();
        LAttachNoGrab = LeftController.GetComponent<AttachHandNoGrab>();
        
        standardSpeed = maxSpeed * 0.7f;
    }

    private void Update()
    {
        updateControllerSpeed();
        updateControllerState();
    }

    // Collision 감지가 발생하면 점수를 산정하도록 했다.
    public void Scoring(GameObject target)
    {
        InteractionType targetType = target.GetComponent<BaseObject>().InteractionType;
        scoreType score;
        
        if (targetType == RControllerType || targetType == LControllerType)
        {
            if (RControllerSpeed > standardSpeed || LControllerSpeed > standardSpeed)
                score = scoreType.Perfect;
            else
                score = scoreType.Good;
        }
        else
        {
            // 해당 target에 대한 점수는 Bad
            score = scoreType.Bad;
        }

        Debug.Log(score);
        SetScoreEffect(score, target.transform);
    }

    private void SetScoreEffect(scoreType score, Transform position)
    {
        
    }

    private void updateControllerSpeed()
    {
        Vector3 RcurrentPos = RightController.transform.position;
        Vector3 LcurrentPos = LeftController.transform.position;

        RControllerSpeed = (RbeforePos - RcurrentPos).magnitude / Time.deltaTime;
        LControllerSpeed = (LbeforePos - LcurrentPos).magnitude / Time.deltaTime;
        
        RbeforePos = RcurrentPos;
        LbeforePos = LcurrentPos;
    }

    private void updateControllerState()
    {
        float RgrabValue = RControllerInput.action.ReadValue<float>();
        float LgrabValue = LControllerInput.action.ReadValue<float>();
        
        // 양 손이 normal이라면 Idle state
        if (RgrabValue < 0.7f)
            RControllerType = InteractionType.Idle;
        if (LgrabValue < 0.7f)
            LControllerType = InteractionType.Idle;
        
        //양 손을 펼치고 있고 GrabAttached일 경우 Tear state
        if ((RgrabValue < 0.7f && RAttachNoGrab.IsAttached)
            || (LgrabValue < 0.7f && LAttachNoGrab.IsAttached))
        {
            RControllerType = InteractionType.Tear;
            LControllerType = InteractionType.Tear;
            // TODO: 한 번 찢고나서 Idle 상태로 돌아가야 함
        }

        // 양 손이 Grab이라면 Break state
        if (RgrabValue > 0.7f)
            RControllerType = InteractionType.Break;
        if (LgrabValue > 0.7f)
            LControllerType = InteractionType.Break;

    }
}
