using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
public class TutorialPunchManager : MonoBehaviour
{
    public TutorialPunchType tutorialPunchType;
    private Dictionary<TutorialPunchType, bool> tutorialClearData = new Dictionary<TutorialPunchType, bool>();

    public GameObject[] punchPrefab; // 완성된 형태의 펀치 프리팹
    public GameObject[] zapGameObjects;
    public GameObject[] hookGameObjects;
    public GameObject[] upperCutGameObjects;

    public GameObject zapRootGameObject;
    public GameObject hookRootGameObject;
    public GameObject upperCutRootGameObject;
    
    public int succeedNumber = 0;
    public int processedNumber = 0;

    public void Init()
    {
        InitPunchTutorialData();
        InitPunchGameObjectPool();
        StartTutorialPunchRoutine();
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
            GameObject gameObject = Instantiate(punchPrefab[i % 2]);
            Debug.Log(gameObject.name);
            zapGameObjects[i] = Instantiate(punchPrefab[i % 2], zapRootGameObject.transform); // i에 따라 1, 2번 프리팹
            hookGameObjects[i] = Instantiate(punchPrefab[i % 2 + 2], hookRootGameObject.transform); // i에 따라 3, 4번 프리팹
            upperCutGameObjects[i] = Instantiate(punchPrefab[i % 2 + 4], upperCutRootGameObject.transform); // i에 따라 5, 6번 프리팹
        }
    }
    
    public TutorialPunchType GetNonClearTutorialType()
    {
        foreach (KeyValuePair<TutorialPunchType,bool> keyValuePair in tutorialClearData)
        {
            Debug.Log($"Type {keyValuePair.Key}, isClear : {keyValuePair.Value}");
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
        Debug.Log("Start Tutorial Routine");
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

        Debug.Log("[Tutorial] All Routine Clear~!");
        yield return null;
    }

    IEnumerator RoutineByPunchType(TutorialPunchType tutorialPunchType)
    {
        // PunchableMovementTutorial Init        
        Debug.Log("Routine Start");
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

    IEnumerator WaitUntilProcessedNumberMatchSix()
    {
        while (processedNumber < 6)
        {
            yield return null;
        }

        Debug.Log("Out");
        yield return null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) succeedNumber++;
        if (Input.GetKeyDown(KeyCode.W)) processedNumber++;
    }
}
