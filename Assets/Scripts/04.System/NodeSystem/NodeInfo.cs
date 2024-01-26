using UnityEngine;
using EnumTypes;

public class NodeInfo
{
    public float posX;
    public float posY;
    public float posZ;
    public float generationTime;
    public float timeToReachPlayer;
    public int arrivalAreaIndex;
    public string objectType;
    public string movingType;
    public int objectNum;

    public uint beatNum;
    public Vector3 spawnPosition;
    public InteractionSide sideType;
        
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
        arrivalAreaIndex = 0;
        totalJumpNumberOfTimes = 0;
        
        // Hit
        beatNum = 0;
        spawnPosition = new Vector3(0, 0, 0);
        sideType = InteractionSide.Red;
        // arrivalAreaIndex
        // timeToReachPlayer

    }
}
/*
 * Json -> int, float, bool, string, null
 * 생성 위치, 생성 시간, objectType, movingType, 오브젝트 번호
 * 
 * Object Type
 * 0 : Rip 찢어야 하는 오브젝트
 * 1 : Break 부서야 하는 오브젝트
 * 2 : Avoid 피해야 하는 오브젝트
 *
 * Moving Type
 * 0 : Straight : 플레이어를 향해 직선으로 날아옴
 * 1 : Cannon  : 플레이어를 향해 빠르게
 *
 * Object Num -> Object Type을 선택하고 Object Num에 따라 다른 오브젝트가 생성됨
 * Object Type : 0, Object Num : 4 => Type 0번인 Rip 오브젝트 중 4번 오브젝트가 생성됨.
 * 0 ~ 9 : Object Type : Rip items
 * 10 ~ 19 : Object Type : Break items
 * 20 ~ 29 : Object Type : Avoid items
 */