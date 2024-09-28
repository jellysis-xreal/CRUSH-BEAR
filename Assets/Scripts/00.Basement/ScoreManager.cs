using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Controls;
using UnityEngine.PlayerLoop;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Motion = EnumTypes.Motion;

// Player의 Interaction event를 확인하고,
// Perfect/Good/Bad의 점수를 판별합니다

public class ScoreManager : MonoBehaviour
{
    [Header("Score value")]
    public float TotalScore;
    [SerializeField] private uint PerfectNum;
    
    [Header("setting value")]
    public Transform effectSpawn;
    [SerializeField] private float maxSpeed = 3.0f;
    public Transform[] scoreText_Transform;
    private TextMeshProUGUI[] scoreText;
    public Transform[] comboText_Transform;
    private TextMeshProUGUI[] comboText;

    [Space(20f)]
    
    [Header("setting(auto)")] 
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject RightController;
    [SerializeField] private GameObject LeftController;
    [SerializeField] private HandData RHand;
    [SerializeField] private HandData LHand;
    [SerializeField] private GameObject mainCam;

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

    private CircleGaugeController circleGaugeController;

    private float _RHandSpeed, _LHandSpeed;

    public void Init()
    {
        //Debug.Log("Initialize ScoreManager");
        TotalScore = 0;
        PerfectNum = 0;

        player = GameManager.Player.player.gameObject; // GameObject.FindWithTag("Player");
        mainCam = GameManager.Player.mainCamera;  // GameObject.FindWithTag("MainCamera");

        RightController = GameManager.Player.RightController; //  player.GetComponent<Player>().R_Controller;
        LeftController = GameManager.Player.LeftController; // player.GetComponent<Player>().L_Controller;

        RHand = GameManager.Player.R_HandData; // player.GetComponent<Player>().R_HandData;
        LHand = GameManager.Player.L_HandData; // player.GetComponent<Player>().L_HandData;
        
        standardSpeed = maxSpeed * 0.6f;

        GameObject circleGaugeControllerObject = GameObject.Find("CircleGaugeController"); //
        if (circleGaugeControllerObject == null)
        {
            //Debug.Log("circleGaugeControllerObject == null");
            return;
        }

        scoreText = new TextMeshProUGUI[scoreText_Transform.Length];
        comboText = new TextMeshProUGUI[comboText_Transform.Length];
        for(int i = 0; i < scoreText.Length; ++i)
        {
            scoreText[i] = scoreText_Transform[i].GetComponent<TextMeshProUGUI>();
            comboText[i] = comboText_Transform[i].GetComponent<TextMeshProUGUI>();
        }
        circleGaugeController = circleGaugeControllerObject.GetComponent<CircleGaugeController>();
        circleGaugeController.InitSettings();
        effectPool = new List<GameObject>();
        vfxPool = new List<GameObject>();
    }

    public uint GetPerfectNum()
    {
        return PerfectNum;
    }
    
    private scoreType ScoreByControllerSpeed(uint targetHand)
    {
        // Perfect, Good, Weak 중
        // 컨트롤러의 속도에 따라서 결정됩니다.
        // targetHand : 0-Right, 1-Left, 2-both
        //Debug.Log("ScoreByControllerSpeed");
        scoreType resultScore = scoreType.Weak;

        float perfect_threshold = RHand.GetPerfectThreshold();
        float good_threshold = RHand.GetGoodThreshold();

        //Debug.Log($"Perfect Threshold: {perfect_threshold}, Good Threshold: {good_threshold}");
        _RHandSpeed = RHand.GetControllerSpeed();
        _LHandSpeed = LHand.GetControllerSpeed();
    
        switch (targetHand)
        {
                
            case 0:
                //Debug.Log($"Right Hand Speed: {RHand.ControllerSpeed}");
                if (_RHandSpeed >= perfect_threshold)
                    resultScore = scoreType.Perfect;
                else if (_RHandSpeed >= good_threshold)
                    resultScore = scoreType.Good;
                break;
            
            case 1:
                
                //Debug.Log($"Left Hand Speed: {LHand.ControllerSpeed}");
                if (_LHandSpeed >= perfect_threshold)
                    resultScore = scoreType.Perfect;
                else if (_LHandSpeed >= good_threshold)
                    resultScore = scoreType.Good;
                break;
            
            case 2:
                //Debug.Log($"Left Hand Speed: {LHand.ControllerSpeed}, Right Hand Speed: {RHand.ControllerSpeed}");
                if ((_LHandSpeed >= perfect_threshold) || (_RHandSpeed >= perfect_threshold))
                    resultScore = scoreType.Perfect;
                else if ((_LHandSpeed >= good_threshold) || (_RHandSpeed >= perfect_threshold))
                    resultScore = scoreType.Good;
                break;
        }
        //Debug.Log($"Result Score: {resultScore}");
        return resultScore;
    }

    public void ScoringMiss(GameObject target)
    {
        scoreType score = scoreType.Miss;
        AddScore(score);
        
        // Swing은 뒤로 넘어간 오브젝트 miss 처리
        //if (GameManager.Wave.GetWaveType() == WaveType.Hitting)
        //    target.GetComponent<BaseObject>().SetScoreBool();
        
        // 플레이어의 위치와 방향을 가져옵니다.
        Vector3 playerPosition = player.transform.position;
        Vector3 playerDirection = mainCam.transform.forward;

        Vector3 rightDirection = Quaternion.Euler(0, 30, 0) * playerDirection;
        
        float distance = 2.0f; 
        Vector3 targetPosition = playerPosition + rightDirection * distance;
        
        SetScoreEffect(score, targetPosition);   
    }
    
    public void ScoringHit(GameObject target, bool IsRightSide)
    {
        _RHandSpeed = RHand.GetControllerSpeed();
        _LHandSpeed = LHand.GetControllerSpeed();
        
        if (target.GetComponent<BaseObject>().IsItScored())
            return; // Object의 중복 scoring을 방지한다.

        scoreType score;

        if (!IsRightSide)
        {
            score = scoreType.Bad;
        }
        else
        {
            // R, L
            score = ScoreByControllerSpeed(2);
        }
        
        target.GetComponent<BaseObject>().SetScoreBool();
        GameManager.Sound.PlayEffect_ToastHit();
        if (SceneManager.GetActiveScene().name != "03.TutorialScene")
        {
            AddScore(score);
            Vibrate(score);
            SetScoreEffect(score, target.transform.position);    
        }
        else
        {
            GameManager.TutorialTennis.scores.Add(score);
            GameManager.TutorialTennis.speeds.Add(Math.Max(_RHandSpeed, _LHandSpeed));
            // 튜토리얼
            if (score == scoreType.Perfect)
            {
                GameManager.TutorialTennis.succeedNumber++;
            }
            GameManager.TutorialTennis.processedNumber++;
        }
        //Debug.Log("[DEBUG]" + target.name + "의 점수는 " + score);
    }

    public void ScoringPunch(GameObject target, bool isPerpect, EnumTypes.Motion motion = Motion.None) // SYJ
    {
        scoreType resultScore = scoreType.Weak;

        float perfect_threshold = RHand.GetPerfectThreshold();
        float good_threshold = RHand.GetGoodThreshold();

        //Debug.Log($"Perfect Threshold: {perfect_threshold}, Good Threshold: {good_threshold}");
        _RHandSpeed = RHand.GetControllerSpeed();
        _LHandSpeed = LHand.GetControllerSpeed();
    
        // coreType score = scoreType.Bad;
        
        // 0 ~ 1 : Weak
        // 1 ~ 2 : Good
        // 2이상  : Perfect
        if (isPerpect)
        {
            if (motion == Motion.LeftZap || motion == Motion.LeftHook || motion == Motion.LeftUpperCut ||
                motion == Motion.LeftLowerCut)
            {
                //Debug.Log($"Left Hand Speed: {LHand.ControllerSpeed}");
                if (_LHandSpeed >= perfect_threshold)
                    resultScore = scoreType.Perfect;
                else if (_LHandSpeed >= good_threshold)
                    resultScore = scoreType.Good;
                
                //Debug.Log("[SYJ DEBUG]" + target.name + "의 점수는 " + score + " 속도 : " + LHand.ControllerSpeed);
            }
            else if (motion == Motion.RightZap || motion == Motion.RightHook || motion == Motion.RightUpperCut || motion == Motion.RightLowerCut)
            {
                if (_RHandSpeed >= perfect_threshold)
                    resultScore = scoreType.Perfect;
                else if (_RHandSpeed >= good_threshold)
                    resultScore = scoreType.Good;
                //Debug.Log("[SYJ DEBUG]" + target.name + "의 점수는 " + score + " 속도 : " + RHand.ControllerSpeed);

            }

        }
        else
        {
            resultScore = scoreType.Bad; 
        }
        if (resultScore == scoreType.Perfect || resultScore == scoreType.Good)
            GameManager.Sound.PlayEffect_Punch();
        
        // Debug.Log("Scoring Punch " + resultScore);       
        AddScore(resultScore);
        //Vibrate(score);
        Vibrate(resultScore);
        
        SetScoreEffect(resultScore, target.transform.position);
        //Debug.Log("[DEBUG]" + target.name + "의 점수는 " + score);
        //Debug.Log("[DEBUG]" + target.name + "의 점수는 " + score + " 속도 : "+ RHand.ScoreByControllerSpeed + LHand.ScoreByControllerSpeed);
        float mPunchSpeed = Math.Max(_RHandSpeed, _LHandSpeed);
        if (SceneManager.GetActiveScene().name == "03.TutorialScene")
        {
            GameManager.TutorialPunch.scores.Add(resultScore);
            // GameManager.TutorialPunch.speeds.Add(Math.Max(_RHandSpeed, _LHandSpeed));
        }
        // Debug.Log("[Debug]yujin sliderController.SetPunchSliderSpeed : " + mPunchSpeed);
        //sliderController.SetPunchSliderSpeed(mPunchSpeed);

        // 원형 슬라이더 값 설정
        if (resultScore == scoreType.Perfect)
        {
            circleGaugeController.SetGaugeHeight(circleGaugeController.maxHeight);
            //circleGaugeController.SetPunchSliderSpeed(circleGaugeController.maxScaleAmount);
        } else
        {
            circleGaugeController.SetGaugeHeight(mPunchSpeed);
            //circleGaugeController.SetPunchSliderSpeed(mPunchSpeed);
        }
    }

    public void setTXT()
    {
        if(scoreText == null) return;
        foreach(var text in scoreText)
        {
            text.text = TotalScore.ToString();
        }

        foreach (var text in comboText)
        {
            text.text = GameManager.Combo.comboValue.ToString();
        }
    }

    private void AddScore(scoreType score)
    {
        float value = 0;
        switch (score)
        {
            case scoreType.Perfect:
                // 정확하게 충돌+속도 60% 이상 = 150점
                GameManager.Combo.ActionSucceed();
                value = 150.0f * GameManager.Combo.comboMultiflier;
                PerfectNum++;
                break;
            case scoreType.Good:
                // 정확한 방식+속도 20% 이상 = 100점
                GameManager.Combo.ActionSucceed();
                value= 100.0f * GameManager.Combo.comboMultiflier;
                break;
            case scoreType.Weak:
                // 정확한 방식+속도 20% 미만 = 20점, 목숨 유지
                GameManager.Combo.ActionSucceed();
                value= 20.0f * GameManager.Combo.comboMultiflier;
                break;
            case scoreType.Bad:
                // 부정확한 방식 = 0점, 목숨 1개 감소
                GameManager.Combo.ActionFailed(); // 목숨 -1
                GameManager.Player.MinusPlayerLifeValue();
                value = 0.0f;
                break;
            case scoreType.Miss:
                // 피격을 하지 않은 경우 = 0점, 목숨 1개 감소
                GameManager.Combo.ActionFailed(); // 목숨 -1
                GameManager.Player.MinusPlayerLifeValue();
                value = 0.0f;
                break;
        }

        TotalScore += value;
        setTXT();
        // TODO : Turn Off Floating UI
        //GameManager.UI.RequestFloatingUI(value);
    }

    private void SetScoreEffect(scoreType score, Vector3 effectPos)
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
        effect.transform.position = effectPos;
        effect.SetActive(true);
        StartCoroutine(DisableAfterSeconds(effect, 1.0f));

        
        // Perfect, Good, Bad VFX Effect
        // [240511] TODO- 점수에 따른 효과
        // VFX 자체의 부하가 심하다고 판단하여 우선 사용하지 않음
        GameObject obj = GetPooledVFX(score);
        if (obj == null)
        {
            obj = CreateNewVFX(score);
            if(obj != null)
                vfxPool.Add(obj);
        }
        //obj.transform.position = effectPos;
        if(obj != null)
        {
            obj.SetActive(true);
            StartCoroutine(DisableAfterSeconds(obj, 2.0f));
        }
        
        

        if (score == scoreType.Perfect)
        {
            // 점수에 따른 효과
            //GameObject obj = Instantiate(Perfect_VFX, effectPos.position, Quaternion.identity);
            //Destroy(obj, 2.0f);
        }
        
    }

    private void Vibrate(scoreType score)
    {
        // Haptic Effect
        Debug.Log("Vi");
        switch (score)
        {
            case scoreType.Perfect:
                GameManager.Player.ActiveRightHaptic(0.95f, 0.15f);
                GameManager.Player.ActiveLeftHaptic(0.95f, 0.15f);
                break;

            case scoreType.Good:
                GameManager.Player.ActiveRightHaptic(0.7f, 0.15f);
                GameManager.Player.ActiveLeftHaptic(0.7f, 0.15f);
                break;
            
            case scoreType.Weak:
                GameManager.Player.ActiveRightHaptic(0.5f, 0.15f);
                GameManager.Player.ActiveLeftHaptic(0.5f, 0.15f);
                break;

            
            case scoreType.Bad:
                GameManager.Player.DecreaseRightHaptic(0.2f, 0.15f);
                GameManager.Player.DecreaseLeftHaptic(0.2f, 0.15f);
                break;
            case scoreType.Miss:
                GameManager.Player.IncreaseRightHaptic(0.2f, 0.15f);
                GameManager.Player.IncreaseLeftHaptic(0.2f, 0.15f);
                break;
        }
    }
    private GameObject GetPooledEffect(scoreType score) // 
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
            // effect.name = score.ToString();
            effect.SetActive(false);
        }

        return effect;
    }
    
    private GameObject CreateNewVFX(scoreType score)
    {
        GameObject vfx = null;

        // [240511] TODO: 우선 정해진 VFX가 없으므로 Perfect만 사용
        //vfx = Score_Perfect_VFX;
        switch (score)
        {
            case scoreType.Perfect:
                 vfx = Score_Perfect_VFX;
                 break;
            case scoreType.Good:
                vfx = Score_Good_VFX;
                break;
            case scoreType.Bad:
                vfx = Score_Good_VFX;
                break;
            case scoreType.Weak:
                vfx = Score_Good_VFX;
                break;
            default:
                break;
        }

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
        if(obj != null)
            obj.SetActive(false);
    }
}
