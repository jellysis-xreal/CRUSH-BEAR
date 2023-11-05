using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NodeInstantiater : MonoBehaviour
{
    public GameObject[] ripObjects;
    public GameObject[] breakObjects;
    public GameObject[] avoidObjects;

    [SerializeField] private float stageStartTime;
    [SerializeField] private bool isStageBegan = false;

    [SerializeField] private float[] generationTimesByStage;
    [SerializeField] private GameObject[] gameObjectsByStage;
    [SerializeField] private int[] ascendingOrder; // 생성 시간의 오름차순의 순서가 저장된 ascendingOrder
    private int _indexToBeAdded = 0;
    private int _indexToActive = 0; // gameObjectsByStage[_indexToActive]로 접근하게 됨.
    private void Update()
    {
        if(isStageBegan) CheckGenerationTime();
    }
    public void InitArray(int nodeCount)
    {
        // Instatiate할 때 생성 시간과 생성할 오브젝트에 대한 정보를 담아야 한다.
        // 생성 시간이 무분별하게 분류되어 있다. 분류하기 위한 배열 생성.ㄹ
        generationTimesByStage = new float[nodeCount];
        gameObjectsByStage = new GameObject[nodeCount];
        ascendingOrder = new int[nodeCount];
    }
    public void InstantiateNode(NodeInfo nodeInfo, string objectType, int objectNum)
    {
        Debug.Log("objectType : "+objectType + "_indexToBeAdded : "+_indexToBeAdded);
        if (objectType == "Rip")
        {
            gameObjectsByStage[_indexToBeAdded] = Instantiate(ripObjects[objectNum], new Vector3(nodeInfo.posX, nodeInfo.posY, nodeInfo.posZ), Quaternion.identity);
            generationTimesByStage[_indexToBeAdded] = nodeInfo.generationTime;
            gameObjectsByStage[_indexToBeAdded].SetActive(false);
            _indexToBeAdded += 1;

        }else if (objectType == "Break")
        {
            gameObjectsByStage[_indexToBeAdded] = Instantiate(breakObjects[objectNum], new Vector3(nodeInfo.posX, nodeInfo.posY, nodeInfo.posZ), Quaternion.identity);
            generationTimesByStage[_indexToBeAdded] = nodeInfo.generationTime;
            gameObjectsByStage[_indexToBeAdded].SetActive(false);
            _indexToBeAdded += 1;
        }else if (objectType == "Avoid")
        {
            gameObjectsByStage[_indexToBeAdded] = Instantiate(avoidObjects[objectNum], new Vector3(nodeInfo.posX, nodeInfo.posY, nodeInfo.posZ), Quaternion.identity);
            generationTimesByStage[_indexToBeAdded] = nodeInfo.generationTime;
            gameObjectsByStage[_indexToBeAdded].SetActive(false);
            _indexToBeAdded += 1;
        }
    }

    public void StageStart()
    {
        GetAscendingOrder();
        isStageBegan = true;
        stageStartTime = Time.time; // 스테이지 시작 시간 기억, 이후 생성될 오브젝트와 시간 비교하기 위함.
        _indexToActive = 0;
    }
    public void GetAscendingOrder()
    {
        float[] timeAscendingOrder = (float[])generationTimesByStage.Clone();
        Array.Sort(timeAscendingOrder);
        // ascendingOrder 배열은 오름차순으로 정렬된 배열
        // 오름차순 순서 출력
        
        // 동시 생성
        // timeAscendingOrder은 오름차순 정렬  generationTimesByStage[i]가 어디에 위치했는지 결정
        // generationTime이 동일한 경우 -> Array.IndexOf()는 같은 순위를 준다.
        // 

        int sortingIndex;
        for (int i = 0; i < timeAscendingOrder.Length; i++)
        {
            // sortingIndex = Array.IndexOf(timeAscendingOrder, generationTimesByStage[i]);s
            ascendingOrder[i] = Array.IndexOf(timeAscendingOrder, generationTimesByStage[i]);
            // generationTimesByStage의 값들이 오름차순으로 몇 번째에 위치하는지 반환한다.
            Debug.Log($"ascendingOrder{i}번 인덱스의 오름차순 순서는 {Array.IndexOf(timeAscendingOrder, generationTimesByStage[i])}입니다.");
        }
        Debug.Log("깔끔하쥬?");
    }
    public void GetAscendingOrder1()
    {
        float[] timeAscendingOrder = (float[])generationTimesByStage.Clone();
        Array.Sort(timeAscendingOrder);
        // ascendingOrder 배열은 오름차순으로 정렬된 배열
        // 오름차순 순서 출력
        
        // 동시 생성
        // timeAscendingOrder은 오름차순 정렬  generationTimesByStage[i]가 어디에 위치했는지 결정
        // generationTime이 동일한 경우 -> Array.IndexOf()는 같은 순위를 준다.
        // 

        int sortingIndex;
        for (int i = 0; i < timeAscendingOrder.Length; i++)
        {
            // sortingIndex = Array.IndexOf(timeAscendingOrder, generationTimesByStage[i]);s
            ascendingOrder[i] = Array.IndexOf(timeAscendingOrder, generationTimesByStage[i]);
            // generationTimesByStage의 값들이 오름차순으로 몇 번째에 위치하는지 반환한다.
            Debug.Log($"ascendingOrder{i}번 인덱스의 오름차순 순서는 {Array.IndexOf(timeAscendingOrder, generationTimesByStage[i])}입니다.");
        }
        Debug.Log("깔끔하쥬?");
    }
    private void CheckGenerationTime()
    {
        // Update에서 오브젝트 별 생성 시간 체크함.
        // 생성할 차례인 오브젝트의 등록된 생성 시간이 지나면 해당 오브젝트를 .SetActive(true); 
        // 생성 시간의 오름차순의 순서가 저장된 ascendingOrder에서 현재 생성해야 할 오브젝트의 인덱스.
        // Debug.Log($"Check Generation Time : Array.IndexOf(ascendingOrder, _indexToActive) : {Array.IndexOf(ascendingOrder, _indexToActive)}");
        
        
        if (Time.time - stageStartTime > generationTimesByStage[Array.IndexOf(ascendingOrder, _indexToActive)]) 
        {
            gameObjectsByStage[Array.IndexOf(ascendingOrder, _indexToActive)].SetActive(true);
            _indexToActive++;
        }

        if (generationTimesByStage.Length == _indexToActive) isStageBegan = false;
    }
    public void ActiveByGenerationTime()
    {
        
    }

}
