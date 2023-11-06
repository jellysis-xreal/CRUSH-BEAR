using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingNodeInfo : MonoBehaviour
{
    public NodeInfo nodeInfo;
    [Header("Set Node Info! (It's up to the developer)")] 
    [SerializeField] private float generationTime;
    [SerializeField] private float movingSpeed;
    [SerializeField] private ObjectType objectType;
    [SerializeField] private MovingType movingType;
    [SerializeField] private ObjectNum objectNum;
    public enum ObjectType
    {
        Rip,
        Break,
        Avoid
    };

    public enum MovingType
    {
        Straight,
        Cannon,
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
        nodeInfo.movingSpeed = movingSpeed;
        nodeInfo.objectType = objectType.ToString();
        nodeInfo.movingType = movingType.ToString();
        string objectIndex = objectNum.ToString().Substring(objectNum.ToString().Length - 1);
        nodeInfo.objectNum = int.Parse(objectIndex);
    }
}
