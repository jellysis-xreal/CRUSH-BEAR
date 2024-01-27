using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private int _indexToActive = 0; // gameObjectsByStage[_indexToActive]로 접근하게 됨.
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
            if (nodeInfo.movingType == "Straight")
            {
                straightMovingToArrivalArea move =  gameObjectsByStage[_indexToBeAdded].AddComponent<straightMovingToArrivalArea>();
                move.arrivalAreaIndex = nodeInfo.arrivalBoxNum;
                move.timeToReachPlayer = nodeInfo.timeToReachPlayer;
            }else if (nodeInfo.movingType == "Jump")
            {
                JumpMovementToArrivalArea move = gameObjectsByStage[_indexToBeAdded].AddComponent<JumpMovementToArrivalArea>();
                move.arrivalAreaIndex = nodeInfo.arrivalBoxNum;
                move.timeToReachPlayer = nodeInfo.timeToReachPlayer;
                move.totalJumpNumberOfTimes = nodeInfo.totalJumpNumberOfTimes;
                // 점프 횟수, 플레이어까지 도달하는 총 시간
            } 
            
            generationTimesByStage[_indexToBeAdded] = nodeInfo.generationTime;
            gameObjectsByStage[_indexToBeAdded].SetActive(false);
            _indexToBeAdded += 1;

        }else if (objectType == "Break")
        {
            gameObjectsByStage[_indexToBeAdded] = Instantiate(breakObjects[objectNum], new Vector3(nodeInfo.posX, nodeInfo.posY, nodeInfo.posZ), Quaternion.identity);
            if (nodeInfo.movingType == "Straight")
            {
                straightMovingToArrivalArea move = gameObjectsByStage[_indexToBeAdded].AddComponent<straightMovingToArrivalArea>();
                move.arrivalAreaIndex = nodeInfo.arrivalBoxNum;
                move.timeToReachPlayer = nodeInfo.timeToReachPlayer;
            }else if (nodeInfo.movingType == "Jump")
            {
                JumpMovementToArrivalArea move = gameObjectsByStage[_indexToBeAdded].AddComponent<JumpMovementToArrivalArea>();
                move.arrivalAreaIndex = nodeInfo.arrivalBoxNum;
                move.timeToReachPlayer = nodeInfo.timeToReachPlayer;
                move.totalJumpNumberOfTimes = nodeInfo.totalJumpNumberOfTimes;
            } 
            
            generationTimesByStage[_indexToBeAdded] = nodeInfo.generationTime;
            gameObjectsByStage[_indexToBeAdded].SetActive(false);
            _indexToBeAdded += 1;
        }/*else if (objectType == "Avoid")
        {
            gameObjectsByStage[_indexToBeAdded] = Instantiate(avoidObjects[objectNum], new Vector3(nodeInfo.posX, nodeInfo.posY, nodeInfo.posZ), Quaternion.identity);
            gameObjectsByStage[_indexToBeAdded].GetComponent<MoveToPlayer>().speed = nodeInfo.movingSpeed;
            generationTimesByStage[_indexToBeAdded] = nodeInfo.generationTime;
            gameObjectsByStage[_indexToBeAdded].SetActive(false);
            _indexToBeAdded += 1;
        }*/
        
        
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

        for (int i = 0; i < timeAscendingOrder.Length; i++)
        {
            // generationTimesByStage의 값이 오름차순으로 몇 번째에 위치하는지 반환
            ascendingOrder[i] = Array.IndexOf(timeAscendingOrder, generationTimesByStage[i]);
        }
        // ascendingOrder의 중복되는 값에 대해 오름차순 재설정.
        ProcessingDuplicateValues();
    }

    private void ProcessingDuplicateValues()
    {
        // 오름차순 정렬 과정에서 발생하는 이슈 
        // 0, 3, 6, 6, 6, 10, 1, 1, 11, 4, 9, 5와 같이 중복된 값엔 같은 순위가 부여된 상황이 있을 수 있다.
        // 재정렬 결과는 아래와 같다.
        // 0, 3, 6, 7, 8, 10, 1, 2, 11, 4, 9, 5
        
        // 1. 중복된 값을 찾아서 가져오기
        // 2. 중복된 값과 해당 값의 인덱스 가져오기  
        // 3. 중복되는 값의 인덱스의 값 수정하기 
        // 사실 중복된 generation times by stage 값을 조금만 변경시켜주면 문제가 해결될 일이다.
        
        // 중복된 값을 찾아서 가져오기
        var duplicateValues = ascendingOrder
            .GroupBy(value => value)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key);

        // 중복된 값과 해당 값의 인덱스 가져오기
        foreach (int duplicateValue in duplicateValues)
        {
            IEnumerable<int> indices = ascendingOrder
                .Select((value, index) => new { Value = value, Index = index })
                .Where(item => item.Value == duplicateValue)
                .Select(item => item.Index);
            
            Debug.Log("Duplicate Value: " + duplicateValue);
            int newValue = duplicateValue;
            foreach (int index in indices)
            {
                // 중복되는 인덱스의 값 수정하기
                ascendingOrder[index] = newValue;
                newValue++;
                Debug.Log("Index: " + index);
            }
        }
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
            gameObjectsByStage[Array.IndexOf(ascendingOrder, _indexToActive)].GetComponent<IMovement>().Init();
            _indexToActive++;
        }

        if (generationTimesByStage.Length == _indexToActive) isStageBegan = false;
    }
    public void ActiveByGenerationTime()
    {
        
    }

}
