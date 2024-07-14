using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
public class TutorialTennisManager : MonoBehaviour
{
    public List<scoreType> scores = new List<scoreType>();
    public List<float> speeds = new List<float>();

    public TutorialTennisType tutorialTennisType;
    private Dictionary<TutorialTennisType, bool> tutorialClearData = new Dictionary<TutorialTennisType, bool>();

    public GameObject[] tennisPrefabs; // 완성된 형태의 테니스 프리팹
    public GameObject[] leftHandGameObjects;
    public GameObject[] rightHandGameObjects;

    public GameObject leftHandRootGameObject;
    public GameObject rightHandRootGameObject;

    public int succeedNumber = 0;
    public int processedNumber = 0;
    public int GetPerfectScoreNumberOfTennis()
    {
        // TODO : 인터랙션한 쿠키 중 퍼펙트 개수를 반환하는 코드;
        int num = 0;
        int startIndex = scores.Count - 1;
        for (int i = startIndex; i > startIndex - 4; i--)
        {
            if (scores[i] == scoreType.Perfect) num++;
        }
        Debug.Log($"Perfect Score : num {num} \\ Index {startIndex} to {startIndex - 4}");
        return num;
    }
    public bool CheckPhase12Criteria()
    {
        int startIndex = scores.Count - 1;
        for (int i = startIndex; i > startIndex - 2; i--)
        {
            if (scores[i] == scoreType.Bad || scores[i] == scoreType.Miss) return false;
        }

        return true;
    }
    

    public void InitializeTennis()
    {
        InitTennisTutorialData();
        InitTennisGameObjectPool();
        // InitTennisGameObject();
        GameManager.Wave.currentWave = WaveType.Hitting;
        GameManager.Wave.SetWavePlayer();
        // StartTennisTutorialRoutine();
        processedNumber = 0; // processedNumber 초기화
        succeedNumber = 0; // succeedNumber 초기화
    }

    private void InitTennisTutorialData()
    {
        tutorialTennisType = TutorialTennisType.LeftHand;
        if (!tutorialClearData.ContainsKey(TutorialTennisType.LeftHand))
        {
            tutorialClearData.Add(TutorialTennisType.LeftHand, false);
        }

        if (!tutorialClearData.ContainsKey(TutorialTennisType.RightHand))
        {
            tutorialClearData.Add(TutorialTennisType.RightHand, false);
        }
    }

    private void InitTennisGameObjectPool()
    {
        int prefabArrayLength = 6; // 게임 오브젝트 사이즈 6, Red 3개 Blue 3개씩 들어감.

        leftHandGameObjects = new GameObject[6]; // 파란색
        rightHandGameObjects = new GameObject[6]; // 빨간색
        for (int i = 0; i < prefabArrayLength; i++)
        {
            leftHandGameObjects[i] = Instantiate(tennisPrefabs[i % 2], leftHandRootGameObject.transform); // i에 따라 1, 2번 프리팹
            rightHandGameObjects[i] = Instantiate(tennisPrefabs[i % 2 + 2], rightHandRootGameObject.transform); // i에 따라 3, 4번 프리팹
        }
    }

    private void InitTennisGameObject()
    {
        leftHandGameObjects = new GameObject[1];
        rightHandGameObjects = new GameObject[1];
        leftHandGameObjects[0] = Instantiate(tennisPrefabs[0], leftHandRootGameObject.transform); // i에 따라 1, 2번 프리팹
        rightHandGameObjects[0] = Instantiate(tennisPrefabs[2], rightHandRootGameObject.transform); // i에 따라 3, 4번 프리팹 
    }
    public TutorialTennisType GetNonClearTutorialType()
    {
        foreach (KeyValuePair<TutorialTennisType, bool> keyValuePair in tutorialClearData)
        {
            Debug.Log($"Type {keyValuePair.Key}, isClear : {keyValuePair.Value}");
            if (!keyValuePair.Value) return keyValuePair.Key;
        }
        return TutorialTennisType.Clear;
    }

    void ResetVariable()
    {
        processedNumber = 0;
        succeedNumber = 0;
    }
    public IEnumerator Phase14Routine()
    {
        // TODO : 우상단 -> 좌하단 -> 좌상단 -> 우하단
        ResetVariable();
        rightHandGameObjects[0].SetActive(true);
        rightHandGameObjects[1].SetActive(true);
        leftHandGameObjects[0].SetActive(true);
        leftHandGameObjects[1].SetActive(true);

        
        rightHandGameObjects[0].GetComponentInChildren<HittableMovementTutorial>().
            InitializeTopping(TutorialTennisType.RightHand, 3 + 2f * 1);
        leftHandGameObjects[0].GetComponentInChildren<HittableMovementTutorial>().
            InitializeTopping(TutorialTennisType.LeftHand, 3 + 2f * 2);
        leftHandGameObjects[1].GetComponentInChildren<HittableMovementTutorial>().
            InitializeTopping(TutorialTennisType.LeftHand, 3 + 2f * 3);
        rightHandGameObjects[1].GetComponentInChildren<HittableMovementTutorial>().
            InitializeTopping(TutorialTennisType.RightHand, 3 + 2f * 4);
        
        yield return StartCoroutine(WaitUntilProcessedMatchTotalNumber(4));
    }

    public IEnumerator TennisPhase12Routine()
    {
        ResetVariable();
        leftHandGameObjects[0].SetActive(true);
        rightHandGameObjects[0].SetActive(true);
        
        leftHandGameObjects[0].GetComponentInChildren<HittableMovementTutorial>().
            InitializeTopping(tutorialTennisType, 3 + 2f * 1);
        rightHandGameObjects[0].GetComponentInChildren<HittableMovementTutorial>().
            InitializeTopping(tutorialTennisType, 3 + 2f * 2);

        yield return StartCoroutine(WaitUntilProcessedMatchTotalNumber(2));
    }
    public IEnumerator TennisTutorialRoutine()
    {
        //Debug.Log("Start Tutorial Routine - TennisTutorialRoutine() 함수 호출");
        tutorialTennisType = GetNonClearTutorialType();

        while (tutorialTennisType != TutorialTennisType.Clear)
        {
            yield return StartCoroutine(RoutineByTennisType(tutorialTennisType));

            // 루틴 끝내고 게임 클리어 확인
            tutorialTennisType = GetNonClearTutorialType();
        }

        Debug.Log("[Tutorial] All Routine Clear~!");
        yield return null;
    }

    IEnumerator RoutineByTennisType(TutorialTennisType tutorialTennisType)
    {
        // HittableMovementTutorial Init        
        //Debug.Log("Routine Start - RoutineByTennisType() 함수 호출");

        ResetGameObjectActive();
        processedNumber = 0; // processedNumber 초기화
        succeedNumber = 0; // succeedNumber 초기화

        switch (tutorialTennisType)
        {
            case TutorialTennisType.LeftHand:
                for (int i = 0; i < 2; i++)
                    leftHandGameObjects[i].GetComponentInChildren<HittableMovementTutorial>().
                        InitializeTopping(tutorialTennisType, 3 + 2f * i);
                break;
            case TutorialTennisType.RightHand:
                for (int i = 0; i < 2; i++)
                    rightHandGameObjects[i].GetComponentInChildren<HittableMovementTutorial>().
                        InitializeTopping(tutorialTennisType, 3 + 2f * i);
                break;
        }

        // yield return StartCoroutine(WaitUntilProcessedMatchTotalNumber(4));// 총 4개의 토핑을 처리
        Debug.Log($"[Tutorial] Tennis Type {tutorialTennisType} End! You succeed {succeedNumber} Times.");

        // 성공 개수 체크, 실패하면 성공 개수 초기화
        if (CheckPhase12Criteria())
        {
            Debug.Log($"[Tutorial] You Succeed In {tutorialTennisType}!");
            switch (tutorialTennisType)
            {
                case TutorialTennisType.LeftHand:
                    tutorialClearData[TutorialTennisType.LeftHand] = true;
                    break;
                case TutorialTennisType.RightHand:
                    tutorialClearData[TutorialTennisType.RightHand] = true;
                    break;
            }
        }
        else
        {
            Debug.Log($"[Tutorial] You Failed In {tutorialTennisType}! Try Again!!");
        }
        yield return null;
    }

    private void ResetGameObjectActive()
    {
        // 꺼진 오브젝트 활성화
        for (int i = 0; i < rightHandRootGameObject.transform.childCount; i++)
        {
            Transform t = rightHandRootGameObject.transform.GetChild(i);
            if (!t.gameObject.activeSelf) t.gameObject.SetActive(true);
        }
        for (int i = 0; i < leftHandRootGameObject.transform.childCount; i++)
        {
            Transform t = leftHandRootGameObject.transform.GetChild(i);
            if (!t.gameObject.activeSelf) t.gameObject.SetActive(true);
        }

    }
    IEnumerator WaitUntilProcessedMatchTotalNumber(int totalNum = 6)
    {
        while (processedNumber < totalNum)
        {
            yield return null;
        }

        Debug.Log("Out");
        yield return null;
    }

    public bool Check4FruitInteractionPerfect()
    {
        // TODO : 최근 4개의 점수 중 2개 이상이 perfect인 경우에 true 반환

        int startIndex = scores.Count - 1;
        int perfectNum = 0;
        for (int i = startIndex; i > startIndex - 4; i--)
        {
            if (scores[i] == scoreType.Perfect) perfectNum++;
        }

        if (perfectNum >= 2) return true;
        else return false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) succeedNumber++;
        if (Input.GetKeyDown(KeyCode.W)) processedNumber++;
    }
}