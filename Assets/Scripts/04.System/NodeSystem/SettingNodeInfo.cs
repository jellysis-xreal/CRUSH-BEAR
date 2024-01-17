using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingNodeInfo : MonoBehaviour
{
    public NodeInfo nodeInfo;
    [Header("Set Node Info! (It's up to the developer)")] 
    [SerializeField] private float generationTime;
    [SerializeField] private float timeToReachPlayer; // 소환 이후 Arrival Area까지의 도달 시간
    [SerializeField] private int arrivalAreaIndex; // 목표로 하는 area의 인덱스
    [SerializeField] private ObjectType objectType;
    [SerializeField] private MovingType movingType;
    [SerializeField] private ObjectNum objectNum;
    
    [Header("MovingType : Straight")]
    [SerializeField] private float movingSpeed;
    
    [Header("MovingType : JumpMoving")]
    public int totalJumpNumberOfTimes; // area에 도달하기 까지 chd 점프할 횟수 

    public enum ObjectType
    {
        Rip,
        Break,
        Avoid
    };

    public enum MovingType
    {
        Straight,
        Jump,
        etc
    };

    public enum ObjectNum
    {
        num0, num1, num2, num3, num4, num5, num6, num7, num8, num9
    };

    private void Awake()
    {
        nodeInfo = new NodeInfo();
        SetNodeInfo();
    }
    private void SetNodeInfo()
    {
        nodeInfo.posX = (float)Math.Round(transform.position.x,2);
        nodeInfo.posY = (float)Math.Round(transform.position.y,2);
        nodeInfo.posZ = (float)Math.Round(transform.position.z,2);
        nodeInfo.generationTime = generationTime;
        nodeInfo.timeToReachPlayer = timeToReachPlayer;
        nodeInfo.arrivalAreaIndex = arrivalAreaIndex;
        nodeInfo.movingSpeed = movingSpeed;
        nodeInfo.objectType = objectType.ToString();
        nodeInfo.movingType = movingType.ToString();
        string objectIndex = objectNum.ToString().Substring(objectNum.ToString().Length - 1);
        nodeInfo.objectNum = int.Parse(objectIndex);
        nodeInfo.totalJumpNumberOfTimes = totalJumpNumberOfTimes;
    }
}
