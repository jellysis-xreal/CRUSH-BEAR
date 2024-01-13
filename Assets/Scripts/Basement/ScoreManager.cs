using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using EnumTypes;
using TMPro;
using UnityEngine.InputSystem.Controls;
using UnityEngine.PlayerLoop;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

// Player의 Interaction event를 확인하고,
// Perfect/Good/Bad의 점수를 판별합니다

public class ScoreManager : MonoBehaviour
{
    [Header("setting value")]
    public float TotalScore;
    public Transform effectSpawn;
    [SerializeField] private float maxSpeed = 3.0f;

    
    [Header("setting(auto)")] 
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject RightController;
    [SerializeField] private GameObject LeftController;
    [SerializeField] private HandData RHand;
    [SerializeField] private HandData LHand;
    [SerializeField] private GameObject Perfect_VFX;
    [SerializeField] private GameObject Good_VFX;
    [SerializeField] private GameObject Bad_VFX;
    
    private float standardSpeed;
    private Vector3 RbeforePos, LbeforePos;
    private AttachHandNoGrab RAttachNoGrab;
    private AttachHandNoGrab LAttachNoGrab;

    [Header("Score UI")] 
    [SerializeField] private TextMeshProUGUI scoreText;
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

        RHand = RightController.transform.GetChild(0).GetComponent<HandData>();
        LHand = LeftController.transform.GetChild(0).GetComponent<HandData>();

        standardSpeed = maxSpeed * 0.6f;

        Perfect_VFX = Resources.Load("VFX/FireWorkCelebrationEffect02") as GameObject;
    }

    // Collision 감지가 발생하면 점수를 산정하도록 했다.
    public void Scoring(GameObject target)
    {
        if (target.GetComponent<BaseObject>().IsItScored())
            return; // Object의 중복 scoring을 방지한다.
        
        InteractionType targetType = target.GetComponent<BaseObject>().InteractionType;
        scoreType score;

        switch (targetType)
        {
            case InteractionType.Break:
                if (targetType == RHand.ControllerType)
                {
                    if (RHand.ControllerSpeed > standardSpeed)
                        score = scoreType.Perfect;
                    else
                        score = scoreType.Good;
                }
                else if (targetType == LHand.ControllerType)
                {
                    if (LHand.ControllerSpeed > standardSpeed)
                        score = scoreType.Perfect;
                    else
                        score = scoreType.Good;
                }
                else
                {
                    score = scoreType.Bad;
                    // TODO: player 몸에 붙게 처리 + 목숨 -1
                    Destroy(target, 0.5f);
                }

                break;

            case InteractionType.Tear:
                if (RHand.ControllerType == InteractionType.Tear && LHand.ControllerType == InteractionType.Tear)
                {
                    if (RHand.ControllerSpeed > standardSpeed || LHand.ControllerSpeed > standardSpeed)
                        score = scoreType.Perfect;
                    else
                        score = scoreType.Good;
                }
                else
                {
                    score = scoreType.Bad;
                    // TODO: player 몸에 붙게 처리 + 목숨 -1
                    Destroy(target, 0.5f);
                }
                break;

            default:
            {
                score = scoreType.Bad;
                // TODO: player 몸에 붙게 처리 + 목숨 -1
                Destroy(target, 0.5f);
                break;
            }
                
        }

        target.GetComponent<BaseObject>().SetScoreBool();
        AddScore(score);
        SetScoreEffect(score, target.transform);
        Debug.Log(target.name + "의 점수는 " + score);
    }

    public void ScoringHit(GameObject target, bool IsRightSide)
    {
        if (target.GetComponent<BaseObject>().IsItScored())
            return; // Object의 중복 scoring을 방지한다.
        
        scoreType score;
        
        if (!IsRightSide)
            score = scoreType.Bad;
        else
        {
            if (RHand.ControllerSpeed > standardSpeed || LHand.ControllerSpeed > standardSpeed)
                score = scoreType.Perfect;
            else
                score = scoreType.Good;
        }
        
        target.GetComponent<BaseObject>().SetScoreBool();
        AddScore(score);
        SetScoreEffect(score, target.transform);
        Debug.Log("[DEBUG]" + target.name + "의 점수는 " + score);
    }

    private void AddScore(scoreType score)
    {
        switch (score)
        {
            case scoreType.Perfect:
                TotalScore += 100;
                scoreText.text = TotalScore.ToString();
                break;
            
            case scoreType.Good:
                TotalScore += 50;
                scoreText.text = TotalScore.ToString();
                break;
            
            case scoreType.Bad:
                TotalScore += 0;
                scoreText.text = TotalScore.ToString();
                break;
        }
    }

    private void SetScoreEffect(scoreType score, Transform position)
    {
        GameObject effect;

        if (score == scoreType.Perfect)
        {
            // Perfect! 글자
            effect = Resources.Load("Prefabs/Effects/Score_perfect") as GameObject;
            Instantiate(effect, effectSpawn.position, Quaternion.identity);
            
            // 점수에 따른 효과
            GameObject obj = Instantiate(Perfect_VFX, position.position, Quaternion.identity);
            Destroy(obj, 3.0f);
        }
        else if (score == scoreType.Good)
        {
            effect = Resources.Load("Prefabs/Effects/Score_good") as GameObject;
            Instantiate(effect, effectSpawn.position, Quaternion.identity);
        }
        else if (score == scoreType.Bad)
        {
            effect = Resources.Load("Prefabs/Effects/Score_bad") as GameObject;
            Instantiate(effect, effectSpawn.position, Quaternion.identity);
        }
    }
}
