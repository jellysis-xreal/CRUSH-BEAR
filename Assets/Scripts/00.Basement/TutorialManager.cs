using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private TutorialType _tutorialType;
    private Dictionary<TutorialType, bool> tutorialClearData = new Dictionary<TutorialType, bool>();

    public GameObject[] punchPrefab;
    private GameObject[] _punchArray;
    
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
        int prefabArrayLength = punchPrefab.Length;
        _punchArray = new GameObject[prefabArrayLength * 2];
        int index = 0;
        for (int i = 0; i < prefabArrayLength; i++)
        {
            _punchArray[index++] = Instantiate(punchPrefab[i]);
            _punchArray[index++] = Instantiate(punchPrefab[i]);
        }
    }
    
    public TutorialType GetNonClearTutorialType()
    {
        foreach (KeyValuePair<TutorialType,bool> keyValuePair in tutorialClearData)
        {
            Debug.Log($"Type {keyValuePair.Key}, isClear : {keyValuePair.Value}");
            if (keyValuePair.Value) return keyValuePair.Key;
        }
        return TutorialType.Zap;
    }
    
    IEnumerator TutorialPunchRoutine()
    {
        _tutorialType = GetNonClearTutorialType();

        
        
        yield return null;
    }
}

public enum TutorialType
{
    Zap,
    Hook,
    UpperCut
}