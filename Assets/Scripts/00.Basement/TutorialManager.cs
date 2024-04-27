using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private TutorialType _tutorialType;
    private Dictionary<TutorialType, bool> tutorialClearData = new Dictionary<TutorialType, bool>();

    public GameObject[] punchPrefab; // 완성된 형태의 펀치 프리팹
    private GameObject[] _zapGameObjects;
    private GameObject[] _hookGameObjects;
    private GameObject[] _upperCutGameObjects;

    public int succeedNumber = 0;
    public int processedNumber = 0;
    private void Start()
    {
        Init();
    }

    public void Init()
    {
        InitTutorialData();
        InitPunchGameObjectPool();
    }

    private void InitTutorialData()
    {
        _tutorialType = TutorialType.Zap;
        tutorialClearData.Add(TutorialType.Zap, false);
        tutorialClearData.Add(TutorialType.Hook, false);
        tutorialClearData.Add(TutorialType.UpperCut, false);
    }

    private void InitPunchGameObjectPool()
    {
        int prefabArrayLength = 6; // 게임 오브젝트 사이즈 6, left 3개 right 3개씩 들어감.
        
        _zapGameObjects = new GameObject[6];
        _hookGameObjects = new GameObject[6];
        _upperCutGameObjects = new GameObject[6];
        for (int i = 0; i < prefabArrayLength; i++)
        {
            _zapGameObjects[i] = Instantiate(punchPrefab[i % 2]); // i에 따라 1, 2번 프리팹
            _hookGameObjects[i] = Instantiate(punchPrefab[i % 2 + 2]); // i에 따라 3, 4번 프리팹
            _upperCutGameObjects[i] = Instantiate(punchPrefab[i % 2 + 4]); // i에 따라 5, 6번 프리팹
        }
    }
    
    public TutorialType GetNonClearTutorialType()
    {
        foreach (KeyValuePair<TutorialType,bool> keyValuePair in tutorialClearData)
        {
            Debug.Log($"Type {keyValuePair.Key}, isClear : {keyValuePair.Value}");
            if (!keyValuePair.Value) return keyValuePair.Key;
        }
        return TutorialType.Clear;
    }
    
    IEnumerator TutorialPunchRoutine()
    {
        Debug.Log("Start Tutorial Routine");
        _tutorialType = GetNonClearTutorialType();
        // 튜토리얼 타입에 따라
        // 레프트 잽, 라이트 잽 각 2번
        // 레프트 훅, 라이트 훅 각 2번
        // 레프트 어퍼컷, 라이트 어퍼컷 각 2번 
        while (_tutorialType != TutorialType.Clear)
        {
            yield return StartCoroutine(RoutineByPunchType(_tutorialType));
            
            // 루틴 끝내고 게임 클리어 확인
            _tutorialType = GetNonClearTutorialType();
        }

        Debug.Log("[Tutorial] All Routine Clear~!");
        yield return null;
    }

    IEnumerator RoutineByPunchType(TutorialType tutorialType)
    {
        // PunchableMovementTutorial Init        
        switch (tutorialType)
        {
            case TutorialType.Zap:
                for (int i = 0; i < _zapGameObjects.Length; i++)
                    _zapGameObjects[i].GetComponentInChildren<PunchableMovementTutorial>().InitiateVariable(i % 2, 3 + 0.5f * i);        
                break;
            case TutorialType.Hook:
                for (int i = 0; i < _hookGameObjects.Length; i++)
                    _hookGameObjects[i].GetComponentInChildren<PunchableMovementTutorial>().InitiateVariable(i % 2, 3 + 0.5f * i);
                break;
            case TutorialType.UpperCut:
                for (int i = 0; i < _upperCutGameObjects.Length; i++)
                    _upperCutGameObjects[i].GetComponentInChildren<PunchableMovementTutorial>().InitiateVariable(i % 2 + 2, 3 + 0.5f * i);
                break;
        }

        yield return StartCoroutine(WaitUntilProcessedNumberMatchSix());
        Debug.Log($"[Tutorial] Punch Type {tutorialType} End! You succeed {succeedNumber} Times.");
                
        // 성공 개수 체크, 실패하면 성공 개수 초기화
        if (succeedNumber == 6)
        {
            Debug.Log($"[Tutorial] You Succeed In {tutorialType}!");
            switch (tutorialType)
            {
                case TutorialType.Zap:
                    tutorialClearData[TutorialType.Zap] = true;            
                    break;
                case TutorialType.Hook:
                    tutorialClearData[TutorialType.Hook] = true;
                    break;
                case TutorialType.UpperCut:
                    tutorialClearData[TutorialType.UpperCut] = true;
                    break;
            }

            succeedNumber = 0;
            processedNumber = 0;
        }
        else
        {
            Debug.Log($"[Tutorial] You Failed In {tutorialType}! Try Again!!");
            succeedNumber = 0;
            processedNumber = 0;
        }

        yield return null;
    }

    IEnumerator WaitUntilProcessedNumberMatchSix()
    {
        while (processedNumber < 6)
        {
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }
}

public enum TutorialType
{
    Zap,
    Hook,
    UpperCut,
    Clear
}