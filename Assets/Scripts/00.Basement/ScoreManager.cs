using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using EnumTypes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Controls;
using UnityEngine.PlayerLoop;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Motion = EnumTypes.Motion;

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
    
    [Space(20f)]
    
    [Header("setting(auto)")] 
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject RightController;
    [SerializeField] private GameObject LeftController;
    [SerializeField] private HandData RHand;
    [SerializeField] private HandData LHand;
    
    
    [Space(20f)]

    [Header("Prefab Setting")] 
    public GameObject Score_Perfect_UI;
    public GameObject Score_Good_UI;
    public GameObject Score_Bad_UI;
    public GameObject Score_Weak_UI;
    public GameObject Score_Miss_UI;
    
    public GameObject Score_Perfect_VFX;
    public GameObject Score_Good_VFX;
    public GameObject Score_Bad_VFX;
    
    [Space(20f)]
    
    [Header("Object Pooling")]
    [SerializeField] private List<GameObject> effectPool = new List<GameObject>();
    [SerializeField] private List<GameObject> vfxPool = new List<GameObject>();
    
    private float standardSpeed;
    private Vector3 RbeforePos, LbeforePos;
    private AttachHandNoGrab RAttachNoGrab;
    private AttachHandNoGrab LAttachNoGrab;

    private CircleGaugeController circleGaugeController;


    public void Init()
    {
        Debug.Log("Initialize ScoreManager");
        
        player = GameObject.FindWithTag("Player");
        RightController = Utils.FindChildByRecursion(player.transform, "Right Controller").gameObject;
        LeftController = Utils.FindChildByRecursion(player.transform, "Left Controller").gameObject;

        RHand = RightController.transform.GetChild(0).GetComponent<HandData>();
        LHand = LeftController.transform.GetChild(0).GetComponent<HandData>();

        standardSpeed = maxSpeed * 0.6f;

        GameObject circleGaugeControllerObject = GameObject.Find("CircleGaugeController"); //
        if (circleGaugeControllerObject == null)
        {
            Debug.Log("circleGaugeControllerObject == null");
            return;
        }
        circleGaugeController = circleGaugeControllerObject.GetComponent<CircleGaugeController>();

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
        if (SceneManager.GetActiveScene().name != "03.TutorialScene")
        {
            AddScore(score);
            SetScoreEffect(score, target.transform);    
        }
        Debug.Log("[DEBUG]" + target.name + "의 점수는 " + score);
    }

    public void ScoringPunch(GameObject target, bool isPerpect, EnumTypes.Motion motion = Motion.None)
    {
        scoreType score = scoreType.Bad;
        
        // 0 ~ 1 : Weak
        // 1 ~ 2 : Good
        // 2이상  : Perfect
        if (isPerpect)
        {
            if (motion == Motion.LeftZap || motion == Motion.LeftHook || motion == Motion.LeftUpperCut)
            {
                if (LHand.ControllerSpeed < 1) score = scoreType.Weak;
                else if (LHand.ControllerSpeed < 2) score = scoreType.Good;
                else if (LHand.ControllerSpeed > 2) score = scoreType.Perfect;
            }
            else if (motion == Motion.RightZap || motion == Motion.RightHook || motion == Motion.RightUpperCut)
            {
                if (LHand.ControllerSpeed < 1) score = scoreType.Weak;
                else if (LHand.ControllerSpeed < 2) score = scoreType.Good;
                else if (LHand.ControllerSpeed > 2) score = scoreType.Perfect;
            }
        }
        else
        {
            score = scoreType.Bad;
            
            if(GameManager.Wave.currenWaveNum > 1) GameManager.Player.MinusPlayerLifeValue(); // 임시
        }
        Debug.Log("Scoring Punch " + score);
        AddScore(score);
        SetScoreEffect(score, target.transform);
        GameManager.Sound.PlayEffect_Punch();
        //Debug.Log("[DEBUG]" + target.name + "의 점수는 " + score);
        //Debug.Log("[DEBUG]" + target.name + "의 점수는 " + score + " 속도 : "+ RHand.ControllerSpeed + LHand.ControllerSpeed);
        float mPunchSpeed = Math.Max(RHand.ControllerSpeed, LHand.ControllerSpeed);
        // Debug.Log("[Debug]yujin sliderController.SetPunchSliderSpeed : " + mPunchSpeed);
        //sliderController.SetPunchSliderSpeed(mPunchSpeed);

        // 원형 슬라이더 값 설정
        if (score == scoreType.Perfect)
        {
            circleGaugeController.SetPunchSliderSpeed(circleGaugeController.maxScaleAmount);
        } else
        {
            circleGaugeController.SetPunchSliderSpeed(mPunchSpeed);
        }
    }

    private void setTXT()
    {
        scoreText_mesh[0].text = TotalScore.ToString();
        scoreText_mesh[1].text = TotalScore.ToString();
        scoreText_mesh[2].text = TotalScore.ToString();

        comboText[0].text = GameManager.Combo.comboValue.ToString();
        comboText[1].text = GameManager.Combo.comboValue.ToString();
        comboText[2].text = GameManager.Combo.comboValue.ToString();
    }

    private void AddScore(scoreType score)
    {
        float value = 0;
        switch (score)
        {
            case scoreType.Perfect:
                GameManager.Combo.ActionSucceed();
                value = 150.0f;
                break;
            
            case scoreType.Good:
                GameManager.Combo.ActionSucceed();
                value= 100.0f;
                break;
            case scoreType.Weak:
                GameManager.Combo.ActionSucceed();
                value= 20.0f;
                break;
            case scoreType.Bad:
                //GameManager.Combo.ActionSucceed(); // [SYJ] 임시 게이지 테스트
                //value = 50; // [SYJ] 임시 게이지 테스트
                GameManager.Combo.ActionFailed(); // 목숨깎여야함
                value = 0.0f;
                break;

            case scoreType.Miss:
                GameManager.Combo.ActionFailed(); // 목숨깎여야함
                value = 0.0f;
                break;
        }

        TotalScore += value;
        setTXT();
        GameManager.UI.RequestFloatingUI(value);
    }

    private void SetScoreEffect(scoreType score, Transform effectPos)
    {
        GameObject effect;
        
        // Perfect, Good, Bad UI Effect
        // effect 객체를 요청할 때마다 풀에서 사용 가능한 객체를 반환하도록 변경
        effect = GetPooledEffect(score);
        if (effect == null)
        {
            // 사용 가능한 객체가 없으면 새로운 객체를 생성하고 풀에 추가
            effect = CreateNewEffect(score);
            effectPool.Add(effect);
        }
        effect.transform.position = effectPos.position;
        effect.SetActive(true);
        StartCoroutine(DisableAfterSeconds(effect, 1.0f));

        
        // Perfect, Good, Bad VFX Effect
        // [240511] TODO- 점수에 따른 효과
        // VFX 자체의 부하가 심하다고 판단하여 우선 사용하지 않음
        // GameObject obj = GetPooledVFX(score);
        // if (obj == null)
        // {
        //     obj = CreateNewVFX(score);
        //     vfxPool.Add(obj);
        // }
        // obj.transform.position = effectPos.position;
        // obj.SetActive(true);
        // StartCoroutine(DisableAfterSeconds(obj, 2.0f));
        
        // Haptic Effect
        switch (score)
        {
            case scoreType.Perfect:
                GameManager.Player.ActiveRightHaptic(0.9f, 0.1f);
                GameManager.Player.ActiveLeftHaptic(0.9f, 0.1f);
                break;

            case scoreType.Good:
                GameManager.Player.ActiveRightHaptic(0.6f, 0.1f);
                GameManager.Player.ActiveLeftHaptic(0.6f, 0.1f);
                break;
            
            case scoreType.Weak:
                GameManager.Player.ActiveRightHaptic(0.4f, 0.1f);
                GameManager.Player.ActiveLeftHaptic(0.4f, 0.1f);
                break;

            
            case scoreType.Bad:
                GameManager.Player.DecreaseRightHaptic(0.2f, 0.1f);
                GameManager.Player.DecreaseLeftHaptic(0.2f, 0.1f);
                break;
            case scoreType.Miss:
                GameManager.Player.IncreaseRightHaptic(0.2f, 0.2f);
                GameManager.Player.IncreaseLeftHaptic(0.2f, 0.2f);
                break;
        }

        if (score == scoreType.Perfect)
        {
            // 점수에 따른 효과
            //GameObject obj = Instantiate(Perfect_VFX, effectPos.position, Quaternion.identity);
            //Destroy(obj, 2.0f);
        }
        
    }
    
    private GameObject GetPooledEffect(scoreType score)
    {
        foreach (var effect in effectPool)
        {
            var effectScript = effect.GetComponent<ScoreEffect>();
            if (!effect.activeInHierarchy
                && effectScript != null
                && effectScript.scoreType == score)
            {
                return effect;
            }
        }

        return null;
    }
    
    private GameObject GetPooledVFX(scoreType score)
    {
        foreach (var vfx in vfxPool)
        {
            if (!vfx.activeInHierarchy
                && vfx.name == score.ToString())
            {
                return vfx;
            }
        }

        return null;
    }

    private GameObject CreateNewEffect(scoreType score)
    {
        GameObject effect = null;

        switch (score)
        {
            case scoreType.Perfect:
                effect = Score_Perfect_UI;
                break;
            case scoreType.Good:
                effect = Score_Good_UI;
                break;
            case scoreType.Weak:
                effect = Score_Weak_UI;
                break;
            case scoreType.Bad:
                effect = Score_Bad_UI;
                break;
            case scoreType.Miss:
                effect = Score_Miss_UI;
                break;
        }

        if (effect != null)
        {
            effect = Instantiate(effect);
            effect.name = score.ToString();
            effect.SetActive(false);
        }

        return effect;
    }
    
    private GameObject CreateNewVFX(scoreType score)
    {
        GameObject vfx = null;

        // [240511] TODO: 우선 정해진 VFX가 없으므로 Perfect만 사용
        vfx = Score_Perfect_VFX;
        // switch (score)
        // {
        //     case scoreType.Perfect:
        //         vfx = Score_Perfect_VFX;
        //         break;
        // }

        if (vfx != null)
        {
            vfx = Instantiate(vfx);
            vfx.name = score.ToString();
            vfx.SetActive(false);
        }

        return vfx;
    }
    
    private IEnumerator DisableAfterSeconds(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
}
