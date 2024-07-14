using System;
using UnityEngine;
using EnumTypes;

[Serializable]
public class NodeInfo
{
    public float posX;
    public float posY;
    public float posZ;
    public float generationTime;
    public float timeToReachPlayer;
    public int arrivalBoxNum;
    public string objectType;
    public string movingType;
    public int objectNum;

    public uint beatNum;
    public Vector3 spawnPosition;
    public InteractionSide sideType;

    public uint punchTypeIndex;
        
    // 직선 운동, 속도는 자동으로 지정됨. 여기서 정할 수 있는 것은 등속도인가? 등가속도인가?
    public float movingSpeed; 
    // 점프 운동
    public int totalJumpNumberOfTimes;

    public NodeInfo()
    {
        // 필드 초기화 코드
        // posX = 0f; posY = 0f; posZ = 0f;
        generationTime = 0f;
        timeToReachPlayer = 0f;
        movingSpeed = 0f;
        objectType = "";
        movingType = "";
        objectNum = 0;
        arrivalBoxNum = 0;
        totalJumpNumberOfTimes = 0;
        beatNum = 0;
        
        // Hit
        spawnPosition = new Vector3(0, 0, 0);
        sideType = InteractionSide.Red;
        
        // Punch
        punchTypeIndex = 0;
    }
}
/*
 * Json -> int, float, bool, string, null
 * 생성 위치, 생성 시간, objectType, movingType, 오브젝트 번호
 *
 * 펀치 타입 별 인덱스
 1 - 빨강 레프트 잽
 2 - 빨강 레프트 훅
 3 - 빨강 레프트 어퍼컷
 4 - 파랑 라이트 잽
 5 - 파랑 라이트 훅
 6 - 파랑 라이트 어퍼컷
 */
 
 