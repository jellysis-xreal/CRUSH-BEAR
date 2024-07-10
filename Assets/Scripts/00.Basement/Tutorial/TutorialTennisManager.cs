using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
public class TutorialTennisManager : MonoBehaviour
{
    public TutorialTennisType tutorialTennisType;
    private Dictionary<TutorialTennisType, bool> tutorialClearData = new Dictionary<TutorialTennisType, bool>();

    public GameObject[] tennisPrefabs; // 완성된 형태의 테니스 프리팹
    public GameObject[] leftHandGameObjects;
    public GameObject[] rightHandGameObjects;

    public GameObject leftHandRootGameObject;
    public GameObject rightHandRootGameObject;

    public int succeedNumber = 0;
    public int processedNumber = 0;

    /*private void Start()
    {
        Init();
    }*/

    public void InitializeTennis()
    {
        InitTennisTutorialData();
        // InitTennisGameObjectPool();
        InitTennisGameObject();
        GameManager.Wave.currentWave = WaveType.Hitting;
        GameManager.Wave.SetWavePlayer();
        // StartTennisTutorialRoutine();
    }

    private void InitTennisTutorialData()
    {
        tutorialTennisType = TutorialTennisType.LeftHand;
        tutorialClearData.Add(TutorialTennisType.LeftHand, false);
        tutorialClearData.Add(TutorialTennisType.RightHand, false);
    }

    private void InitTennisGameObjectPool()
    {
        int prefabArrayLength = 6; // 게임 오브젝트 사이즈 6, Red 3개 Blue 3개씩 들어감.
        
        leftHandGameObjects = new GameObject[6];
        rightHandGameObjects = new GameObject[6];
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
        foreach (KeyValuePair<TutorialTennisType,bool> keyValuePair in tutorialClearData)
        {
            Debug.Log($"Type {keyValuePair.Key}, isClear : {keyValuePair.Value}");
            if (!keyValuePair.Value) return keyValuePair.Key;
        }
        return TutorialTennisType.Clear;
    }

    public void StartTennisTutorialRoutine()
    {
        StartCoroutine(TennisTutorialRoutine());
    }
    public IEnumerator TennisTutorialRoutine()
    {
        Debug.Log("Start Tutorial Routine");
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
        Debug.Log("Routine Start");

        ResetGameObjectActive();
        
        switch (tutorialTennisType)
        {
            case TutorialTennisType.LeftHand:
                for (int i = 0; i < leftHandGameObjects.Length; i++)
                    leftHandGameObjects[i].GetComponentInChildren<HittableMovementTutorial>().
                        InitializeTopping(tutorialTennisType, 3 + 2f * i);        
                break;
            case TutorialTennisType.RightHand:
                for (int i = 0; i < rightHandGameObjects.Length; i++)
                    rightHandGameObjects[i].GetComponentInChildren<HittableMovementTutorial>().
                        InitializeTopping(tutorialTennisType, 3 + 2f * i);
                break;
        }

        yield return StartCoroutine(WaitUntilProcessedMatchTotalNumber(2));
        Debug.Log($"[Tutorial] Tennis Type {tutorialTennisType} End! You succeed {succeedNumber} Times.");
                
        // 성공 개수 체크, 실패하면 성공 개수 초기화
        if (succeedNumber == 6)
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
            succeedNumber = 0;
            processedNumber = 0;
        }
        else
        {
            Debug.Log($"[Tutorial] You Failed In {tutorialTennisType}! Try Again!!");
            succeedNumber = 0;
            processedNumber = 0;
        }

        yield return null;
    }

    private void ResetGameObjectActive()
    {
        // 꺼진 오브젝트 활성화
        for (int i = 0; i < rightHandRootGameObject.transform.childCount; i++)
        {
            Transform t = rightHandRootGameObject.transform.GetChild(i);
            if(!t.gameObject.activeSelf) t.gameObject.SetActive(true);
        }
        for (int i = 0; i < leftHandRootGameObject.transform.childCount; i++)
        {
            Transform t = leftHandRootGameObject.transform.GetChild(i);
            if(!t.gameObject.activeSelf) t.gameObject.SetActive(true);
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) succeedNumber++;
        if (Input.GetKeyDown(KeyCode.W)) processedNumber++;
    }
}
