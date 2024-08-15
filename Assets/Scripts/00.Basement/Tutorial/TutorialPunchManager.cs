using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
public class TutorialPunchManager : MonoBehaviour
{
    #region Tutorial Manager

    public List<scoreType> scores = new List<scoreType>();
    public List<float> speeds = new List<float>();

    public IEnumerator SpawnAndHandleCookie()
    {
        // TODO : 생성 코드
        bool isCookieBroken = false;
        GameObject temp = Instantiate(tutorialStartCookiePrefab);
        BreakableButton tempButton = temp.GetComponent<BreakableButton>();
        tempButton.InitSettings();
        tempButton.AddEvent(() => { isCookieBroken = true; temp.SetActive(false); });
        temp.transform.position = new Vector3(0, 1.1f, 0.6f); // JMH
        // 부숴질 때 까지 대기
        yield return new WaitUntil((() => (isCookieBroken)));
    }

    public IEnumerator SpawnAndHandle2CookiesZap()
    {
        // TODO : 생성 코드, 캐싱
        //Debug.Log("쿠키(잽) 두 개 생성");
        zapGameObjects[0].GetComponentInChildren<PunchableMovementTutorial>().InitiateVariable(0, 3 + 2f * 1);        
        zapGameObjects[1].GetComponentInChildren<PunchableMovementTutorial>().InitiateVariable(1, 3 + 2f * 2);

        yield return new WaitForSeconds(10f);

        //Debug.Log("쿠키 두 개 인터랙션 시간 종료");
    }

    public bool CheckCookiesDestroyed()
    {
        // TODO : Spawn And Handle Cookies Zap에서 생성된 쿠키 오브젝트 두 개가 성공적으로 인터랙션됐는지 감지하는 코드
        if (!zapGameObjects[0].activeSelf && !zapGameObjects[1].activeSelf) return true;
        return false;
    }

    public int GetPerfectScoreNumberOfCookie()
    {
        // TODO : 인터랙션한 쿠키 중 퍼펙트 개수를 반환하는 코드;
        int num = 0;
        int startIndex = scores.Count - 1; 
        for (int i = startIndex; i > startIndex - zapGameObjects.Length; i--)
        {
            if (scores[i] != scoreType.Miss) num++;
        }
        //Debug.Log($"Perfect Score : num {num} \\ Index {startIndex} to {startIndex - zapGameObjects.Length}");
        return num;
    }
    public bool Check4CookiesInteractionSucceed()
    {
        // TODO :(왼손) 라이트 훅 → (왼손) 잽 → (오른손) 어퍼컷 → (오른손) 잽
        // 최근 4개의 점수가 Bad, Miss가 아닌 경우에 true 반환
        
        int startIndex = scores.Count - 1; 
        for (int i = startIndex; i > startIndex - 4; i--)
        {
            if (scores[i] == scoreType.Miss) return false;
        }
        return true;
    }
    
    public IEnumerator ZapRoutine()
    {
        // PunchableMovementTutorial Init        
        //Debug.Log("Zap Routine Start");
        ResetVariable();
        
        for (int i = 0; i < zapGameObjects.Length; i++)
            zapGameObjects[i].GetComponentInChildren<PunchableMovementTutorial>().InitiateVariable(i % 2, 3 + 2f * i);

        yield return StartCoroutine(WaitUntilProcessedNumberMatchSix());
        //Debug.Log($"[Tutorial] Punch Type {tutorialPunchType} End! You succeed {succeedNumber} Times.");
        
        yield return null;
    }

    public IEnumerator Phase8Routine()
    {
        // TODO :(왼손) 훅 → (왼손) 잽 → (오른손) 어퍼컷 → (오른손) 잽 생성

        //Debug.Log("Zap Routine Start");
        ResetVariable();
        
        // 레프트 훅 : hookGameObjects[0]
        // 레프트 잽 : zapGameObjects[0]
        // 라이트 어퍼컷 : upperCutGameObjects[1]
        // 라이트 잽 : zapGameObjects[1]
        hookGameObjects[0].GetComponentInChildren<PunchableMovementTutorial>().InitiateVariable(0, 3 + 2f * 1);
        zapGameObjects[0].GetComponentInChildren<PunchableMovementTutorial>().InitiateVariable(0, 3 + 2f * 2);
        upperCutGameObjects[1].GetComponentInChildren<PunchableMovementTutorial>().InitiateVariable(3, 3 + 2f * 3);
        zapGameObjects[1].GetComponentInChildren<PunchableMovementTutorial>().InitiateVariable(1, 3 + 2f * 4);

        yield return StartCoroutine(WaitUntilProcessedNumber(4));
    }
    #endregion
    
    #region Tutorial Basic
    public TutorialPunchType tutorialPunchType;
    private Dictionary<TutorialPunchType, bool> tutorialClearData = new Dictionary<TutorialPunchType, bool>();

    public GameObject[] punchPrefab; // 완성된 형태의 펀치 프리팹
    public GameObject[] zapGameObjects;
    public GameObject[] hookGameObjects;
    public GameObject[] upperCutGameObjects;
    public GameObject[] lowerCutGameObjects;
    public GameObject tutorialStartCookiePrefab;

    public GameObject zapRootGameObject;
    public GameObject hookRootGameObject;
    public GameObject upperCutRootGameObject;
    public GameObject lowerCutRootGameObject;
    
    public int succeedNumber = 0;
    public int processedNumber = 0;
    public void Init()
    {
        //Debug.Log("Tutorial Initialize");
        InitPunchTutorialData();
        InitPunchGameObjectPool();
        // StartTutorialPunchRoutine();
        GameManager.Wave.currentWave = WaveType.Punching;
        GameManager.Wave.SetWavePlayer();
    }

    private void InitPunchTutorialData()
    {
        tutorialPunchType = TutorialPunchType.Zap;
        tutorialClearData.Add(TutorialPunchType.Zap, false);
        tutorialClearData.Add(TutorialPunchType.Hook, false);
        tutorialClearData.Add(TutorialPunchType.UpperCut, false);
    }

    private void InitPunchGameObjectPool()
    {
        int prefabArrayLength = 6; // 게임 오브젝트 사이즈 6, left 3개 right 3개씩 들어감.
        
        zapGameObjects = new GameObject[6];
        hookGameObjects = new GameObject[6];
        upperCutGameObjects = new GameObject[6];
        for (int i = 0; i < prefabArrayLength; i++)
        {
            // GameObject gameObject = Instantiate(punchPrefab[i % 2]);
            //Debug.Log(gameObject.name);
            zapGameObjects[i] = Instantiate(punchPrefab[i % 2], zapRootGameObject.transform); // i에 따라 1, 2번 프리팹
            hookGameObjects[i] = Instantiate(punchPrefab[i % 2 + 2], hookRootGameObject.transform); // i에 따라 3, 4번 프리팹
            upperCutGameObjects[i] = Instantiate(punchPrefab[i % 2 + 4], upperCutRootGameObject.transform); // i에 따라 5, 6번 프리팹
        }
    }
    
    public TutorialPunchType GetNonClearTutorialType()
    {
        foreach (KeyValuePair<TutorialPunchType,bool> keyValuePair in tutorialClearData)
        {
            //Debug.Log($"Type {keyValuePair.Key}, isClear : {keyValuePair.Value}");
            if (!keyValuePair.Value) return keyValuePair.Key;
        }
        return TutorialPunchType.Clear;
    }

    public void StartTutorialPunchRoutine()
    {
        StartCoroutine(TutorialPunchRoutine());
    }
    IEnumerator TutorialPunchRoutine()
    {
        //Debug.Log("Start Tutorial Routine");
        tutorialPunchType = GetNonClearTutorialType();
        // 튜토리얼 타입에 따라
        // 레프트 잽, 라이트 잽 각 2번
        // 레프트 훅, 라이트 훅 각 2번
        // 레프트 어퍼컷, 라이트 어퍼컷 각 2번 
        while (tutorialPunchType != TutorialPunchType.Clear)
        {
            yield return StartCoroutine(RoutineByPunchType(tutorialPunchType));
            
            // 루틴 끝내고 게임 클리어 확인
            tutorialPunchType = GetNonClearTutorialType();
        }

        //Debug.Log("[Tutorial] All Routine Clear~!");
        yield return null;
    }

    public IEnumerator RoutineByPunchType(TutorialPunchType tutorialPunchType)
    {
        // PunchableMovementTutorial Init        
        //Debug.Log("Routine Start");
        switch (tutorialPunchType)
        {
            case TutorialPunchType.Zap:
                for (int i = 0; i < zapGameObjects.Length; i++)
                    zapGameObjects[i].GetComponentInChildren<PunchableMovementTutorial>().InitiateVariable(i % 2, 3 + 2f * i);        
                break;
            case TutorialPunchType.Hook:
                for (int i = 0; i < hookGameObjects.Length; i++)
                    hookGameObjects[i].GetComponentInChildren<PunchableMovementTutorial>().InitiateVariable(i % 2, 3 + 2f * i);
                break;
            case TutorialPunchType.UpperCut:
                for (int i = 0; i < upperCutGameObjects.Length; i++)
                    upperCutGameObjects[i].GetComponentInChildren<PunchableMovementTutorial>().InitiateVariable(i % 2, 3 + 2f * i);
                break;
        }

        yield return StartCoroutine(WaitUntilProcessedNumberMatchSix());
        Debug.Log($"[Tutorial] Punch Type {tutorialPunchType} End! You succeed {succeedNumber} Times.");
                
        // 성공 개수 체크, 실패하면 성공 개수 초기화
        if (succeedNumber == 6)
        {
            Debug.Log($"[Tutorial] You Succeed In {tutorialPunchType}!");
            switch (tutorialPunchType)
            {
                case TutorialPunchType.Zap:
                    tutorialClearData[TutorialPunchType.Zap] = true;            
                    break;
                case TutorialPunchType.Hook:
                    tutorialClearData[TutorialPunchType.Hook] = true;
                    break;
                case TutorialPunchType.UpperCut:
                    tutorialClearData[TutorialPunchType.UpperCut] = true;
                    break;
            }

            succeedNumber = 0;
            processedNumber = 0;
        }
        else
        {
            Debug.Log($"[Tutorial] You Failed In {tutorialPunchType}! Try Again!!");
            succeedNumber = 0;
            processedNumber = 0;
        }

        yield return null;
    }

    void ResetVariable()
    {
        processedNumber = 0;
        succeedNumber = 0;
    }
    IEnumerator WaitUntilProcessedNumberMatchSix()
    {
        while (processedNumber < 6)
        {
            yield return null;
        }
        yield return new WaitForSeconds(5f);
        yield return null;
    }

    IEnumerator WaitUntilProcessedNumber(int num)
    {
        while (processedNumber < num)
        {
            yield return null;
        }
        yield return new WaitForSeconds(5f);
    }
    #endregion
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) succeedNumber++;
        if (Input.GetKeyDown(KeyCode.W)) processedNumber++;
    }
}
