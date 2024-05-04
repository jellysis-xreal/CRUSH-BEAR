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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Player의 Interaction event를 확인하고,
// Perfect/Good/Bad의 점수를 판별합니다

public class ScoreManager : MonoBehaviour
{
    [Header("setting value")]
    public float TotalScore;
    public Transform effectSpawn;
    [SerializeField] private float maxSpeed = 3.0f;
    public TextMesh[] scoreText_mesh = new TextMesh[3];
    public TMP_Text[] comboText = new TMP_Text[3];
    
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

    private SliderController sliderController; //

    // [Header("Score UI")] 
    // [SerializeField] private TextMeshProUGUI scoreText;
    // [SerializeField] private TextMesh scoreText_mesh;
    private enum scoreType
    {
        Perfect,
        Good,
        Bad,
        Failed
    }

    public void Init()
    {
        Debug.Log("Initialize ScoreManager");
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
        if (target.GetComponent<BaseObject>().IsItScored()) return; // Object의 중복 scoring을 방지한다.

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
        {
            score = scoreType.Bad;
            GameManager.Player.MinusPlayerLifeValue(); // 240216 수정함
        }
        else
        {
            if (RHand.ControllerSpeed > standardSpeed || LHand.ControllerSpeed > standardSpeed)
                score = scoreType.Perfect;
            else
                score = scoreType.Good;
        }
        
        target.GetComponent<BaseObject>().SetScoreBool();
        GameManager.Sound.PlayEffect_ToastHit();
        if (SceneManager.GetActiveScene().name == "03.TutorialScene")
        {
            AddScore(score);
            SetScoreEffect(score, target.transform);    
        }
        Debug.Log("[DEBUG]" + target.name + "의 점수는 " + score);
    }

    public void ScoringPunch(GameObject target, bool isPerpect)
    {
        GameObject sliderControllerObject = GameObject.Find("SliderController"); //
        if(sliderControllerObject == null) return;
        sliderController = sliderControllerObject.GetComponent<SliderController>();

        scoreType score;
        if (isPerpect) score = scoreType.Perfect;
        else
        {
            score = scoreType.Bad;
            
            if(GameManager.Wave.currenWaveNum > 1) GameManager.Player.MinusPlayerLifeValue(); // 임시
        }

        AddScore(score);
        SetScoreEffect(score, target.transform);
        GameManager.Sound.PlayEffect_Punch();
        Debug.Log("[DEBUG]" + target.name + "의 점수는 " + score);
        //Debug.Log("[DEBUG]" + target.name + "의 점수는 " + score + " 속도 : "+ RHand.ControllerSpeed + LHand.ControllerSpeed);
        //Debug.Log("====yujin[DEBUG] ScoringPunch PunchGauge 오른손 속도: " + RHand.ControllerSpeed);
        //Debug.Log("====yujin[DEBUG] ScoringPunch PunchGauge 왼손 속도: " + LHand.ControllerSpeed);

        float mPunchSpeed = Math.Max(RHand.ControllerSpeed, LHand.ControllerSpeed) + 1;
        Debug.Log("[Debug]yujin sliderController.SetPunchSliderSpeed : " + mPunchSpeed);
        sliderController.SetPunchSliderSpeed(mPunchSpeed);
    }

    private void setTXT()
    {
        scoreText_mesh[0].text = TotalScore.ToString();
        scoreText_mesh[1].text = TotalScore.ToString();
        scoreText_mesh[2].text = TotalScore.ToString();

        comboText[0].text = "COMBO\n" + GameManager.Combo.comboValue.ToString();
        comboText[1].text = "COMBO\n" + GameManager.Combo.comboValue.ToString();
        comboText[2].text = "COMBO\n" + GameManager.Combo.comboValue.ToString();
    }

    private void AddScore(scoreType score)
    {
        float value = 0;
        switch (score)
        {
            case scoreType.Perfect:
                GameManager.Combo.ActionSucceed();
                value = 100;
                break;
            
            case scoreType.Good:
                GameManager.Combo.ActionSucceed();
                value= 50;
                break;
            
            case scoreType.Bad:
                GameManager.Combo.ActionFailed(); // 목숨깎여야함
                value = 0;
                break;

            case scoreType.Failed:
                GameManager.Combo.ActionFailed(); // 목숨깎여야함
                value = 0;
                break;
        }

        TotalScore += value;
        setTXT();
        GameManager.UI.RequestFloatingUI(value);
    }

    private void SetScoreEffect(scoreType score, Transform transform)
    {
        GameObject effect;
        
        if (score == scoreType.Perfect)
        {
            // Perfect! 글자
            effect = Resources.Load("Prefabs/Effects/Score_perfect") as GameObject;
            Instantiate(effect, transform.position, Quaternion.identity);
            
            // 햅틱 효과
            GameManager.Player.ActiveRightHaptic(0.9f, 0.1f);
            GameManager.Player.ActiveLeftHaptic(0.9f, 0.1f);
            
            // 점수에 따른 효과
            GameObject obj = Instantiate(Perfect_VFX, transform.position, Quaternion.identity);
            Destroy(obj, 2.0f);
        }
        else if (score == scoreType.Good)
        {
            effect = Resources.Load("Prefabs/Effects/Score_good") as GameObject;
            Instantiate(effect, transform.position, Quaternion.identity);
            
            // 햅틱 효과
            GameManager.Player.ActiveRightHaptic(0.6f, 0.1f);
            GameManager.Player.ActiveLeftHaptic(0.6f, 0.1f);
            
        }
        else if (score == scoreType.Bad)
        {
            effect = Resources.Load("Prefabs/Effects/Score_bad") as GameObject;
            Instantiate(effect, transform.position, Quaternion.identity);
            
            // 햅틱 효과
            GameManager.Player.DecreaseRightHaptic(0.2f, 0.1f);
            GameManager.Player.DecreaseLeftHaptic(0.2f, 0.1f);
        }
        else if (score == scoreType.Failed)
        {
            effect = Resources.Load("Prefabs/Effects/Score_Failed") as GameObject;
            Instantiate(effect, transform.position, Quaternion.identity);
            
            // 햅틱 효과
            GameManager.Player.IncreaseRightHaptic(0.2f, 0.2f);
            GameManager.Player.IncreaseLeftHaptic(0.2f, 0.2f);
        }
    }
}
