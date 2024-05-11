using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using EnumTypes;
using UnityEngine.Serialization;
using UnityEngine.XR.Content.Interaction;
using Random = UnityEngine.Random;

public class NodeInstantiator_minha : MonoBehaviour
{
    // Refactoring Punch Topping
    public List<GameObject> cookiePrefabs;          // 방향에 따르지 않는 랜덤한 쿠키
    public List<GameObject> cookieDirectionPrefabs; // 쿠키의 타입에 따라 정해짐, 쿠키에 붙일 UI Image
    public List<GameObject> childCollider;          // 쿠키의 타입에 따라 정해짐, 동작을 인식하기 위한 콜라이더.
    

    // Topping Prefabs
    public List<GameObject> ShootTopping;
    public List<GameObject> PunchTopping;
    public List<GameObject> HitTopping;

    // Scene에서 Topping이 생성되는 위치
    [FormerlySerializedAs("GunSpawn")] public static GameObject GunSpawnTransform;
    [FormerlySerializedAs("PunchSpawn")] public static GameObject PunchSpawnTransform;
    [FormerlySerializedAs("HitSpawn")] public static GameObject HitSpawnTransform;

    public int _poolSize = 20; // Object Pool Size
    [SerializeField] private GameObject[] shootToppingPool;
    [SerializeField] private GameObject[] punchToppingPool;
    [SerializeField] private GameObject[] hitToppingPool;

    // Punch Topping Pools
    [SerializeField] public GameObject[] punchLeftZapPool;       // punchType : 1
    [SerializeField] public GameObject[] punchLeftHookPool;      // punchType : 2
    [SerializeField] public GameObject[] punchLeftUpperCutPool;  // punchType : 3
    [SerializeField] public GameObject[] punchRightZapPool;      // punchType : 4
    [SerializeField] public GameObject[] punchRightHookPool;     // punchType : 5
    [SerializeField] public GameObject[] punchRightUpperCutPool; // punchType : 6

    // Hit Topping Pools
    [SerializeField] private GameObject[] hitRedPool;        // InteractionSide(enum) : Red
    [SerializeField] private GameObject[] hitBluePool;       // InteractionSide(enum) : Blue
    
    [SerializeField] public uint _musicDataIndex = 0; //1~ NodeData
    private Queue<NodeInfo> _nodeQueue = new Queue<NodeInfo>();

    private Coroutine _curWaveCoroutine;

    private void Start()
    {
        GunSpawnTransform = transform.GetChild(0).gameObject;
        PunchSpawnTransform = transform.GetChild(1).gameObject;
        HitSpawnTransform = transform.GetChild(2).gameObject;
    }

    public void InitToppingPool(WaveType wave)
    {
        // topping pool 생성해줌.
        // Wave가 처음 실행될 때, 한번 초기화 진행하는 것임
        Debug.Log($"[Node Maker] : Init Topping Pool! This wave is [{wave}]"); //[XMC]
        _musicDataIndex = 0;

        InitializeNodeAndPool();
        
        switch (wave)
        {
            case WaveType.Shooting:
                if (shootToppingPool.Length == 0)
                {
                    shootToppingPool = new GameObject[_poolSize]; // 배열 생성
                }
                break;
            case WaveType.Punching:
                InitPunchToppingPool();
                break;
            case WaveType.Hitting:
                InitHittingToppingPool();
                break;
        }

        _curWaveCoroutine = StartCoroutine(SpawnManager(wave));
        //[XMC]Debug.Log("[Node Maker] Start Coroutine");
    }

    void InitHittingToppingPool()
    {
        if (hitToppingPool.Length == 0)
        {
            hitToppingPool = new GameObject[_poolSize];
            for (int i = 0; i < _poolSize/2; ++i)
            {
                GameObject topping = HitTopping[(i % 2)];
                GameObject node = Instantiate(topping);
                node.SetActive(false);
                DontDestroyOnLoad(node);
                hitToppingPool[i] = node;
                hitToppingPool[i].name = "Hit_R_" + i;
            }

            for (int i = _poolSize / 2; i < _poolSize; ++i)
            {
                GameObject topping = HitTopping[(2 + i % 2)];
                GameObject node = Instantiate(topping);
                node.SetActive(false);
                DontDestroyOnLoad(node);
                hitToppingPool[i] = node;
                hitToppingPool[i].name = "Hit_B_" + i;
            }
            //[XMC]Debug.Log($"[Node Maker] Generate {hitToppingPool.Length} hittable Object ");
        }
    }

    IEnumerator SpawnManager(WaveType wave)
    {
        while (true) // 지금은 10개 생성, 대기 Queue 10개까지 됨.
        {
            // ?초 마다 배열 안에 있는 객체들이 차례대로 생성될 것
            //TODO: XMC 임시
            float _time = 0.0f;
            if (GameManager.Wave.currentWave == WaveType.Punching) _time = 0.2f;
            else if (GameManager.Wave.currentWave == WaveType.Hitting) _time = 0.05f;
            
            yield return new WaitForSecondsRealtime(_time);
            
            // Music data의 4개의 node data를 NodeInfo 형식으로 바꾸어, Enqueue.
            if (_nodeQueue.Count < 10)
            {
                MusicDataToNodeInfo(wave);
                _musicDataIndex++;
            }
            else if (_nodeQueue.Count > 0)
            {
                // Dequeue하고 토핑 풀에 값 하나 넣고, 존재하는 것에 다 넣었으면(Dequeue) 반복문 종료.
                // poolsize를 체크하다가 10개 미만일 경우에 Queue에 대해 다시 넣을 준비해야 함.
                
                NodeInfoToMusicData(wave);
            }
        }
    }
    
    public void NotesExistButAreNLongerEnqueued()
    {
        // MusicDataToNodeInfo에서 60초 동안 존재하는 note만 있었다.
        // 120비트까지 생김
        // try catch에서 오류 발생 시 = 더이상 읽을 노트가 없을 시에 stop코루틴을 했음.
        StopCoroutine(_curWaveCoroutine);
    }

    public void FinishAllWaveNode()
    {
        foreach (var shoot in shootToppingPool) Destroy(shoot);
        // foreach (var punch in punchToppingPool) Destroy(punch);
        foreach (var punch in punchLeftZapPool) Destroy(punch);
        foreach (var punch in punchLeftHookPool) Destroy(punch);
        foreach (var punch in punchLeftUpperCutPool) Destroy(punch);
        foreach (var punch in punchRightZapPool) Destroy(punch);
        foreach (var punch in punchRightHookPool) Destroy(punch);
        foreach (var punch in punchRightUpperCutPool) Destroy(punch);
        
        foreach (var hit in hitToppingPool) Destroy(hit);
        _nodeQueue.Clear();
        
        StopCoroutine(_curWaveCoroutine);
    }
    
    private void InitializeNodeAndPool()
    {
        foreach (var shoot in shootToppingPool) shoot.SetActive(false);
        // foreach (var punch in punchToppingPool) punch.SetActive(false);
        foreach (var punch in punchLeftZapPool) punch.SetActive(false);
        foreach (var punch in punchLeftHookPool) punch.SetActive(false);
        foreach (var punch in punchLeftUpperCutPool) punch.SetActive(false);
        foreach (var punch in punchRightZapPool) punch.SetActive(false);
        foreach (var punch in punchRightHookPool) punch.SetActive(false);
        foreach (var punch in punchRightUpperCutPool) punch.SetActive(false);
        
        foreach (var hit in hitToppingPool) hit.SetActive(false);
        
        _nodeQueue.Clear();
    }
    
    // 각각의 노드에 세팅이 필요한 값들을 NodeInfo 타입으로 지정.
    private void MusicDataToNodeInfo(WaveType wave)
    {
        var data = GameManager.Wave.CurMusicData;

        uint[] nodes;
        float oneBeat = 60.0f / data.BPM;
        
        // Dequeue할 노드가 poolsize 만큼 남았지만
        // if (_nodeQueue.Count < 10) else if (_nodeQueue.Count > 0)
        // _nodeQueue.Count가 10을 안 넘는 순간(더이상 EnQueue할 MusicData가 없는 순간) index error 발생
        // 해당 index error를 처리하기 위한 try catch문. 다른 방법은 없을까..?
        try
        {
            nodes = data.NodeData[(int)_musicDataIndex];
        }
        catch (Exception e)
        {
            // NodeInfoToMusicData(wave); 
            // isWaveFinished = true;
            Debug.Log("Error 더이상 Enqueue할 data없음."); //[XMC]
            StopCoroutine(_curWaveCoroutine);
            return;
            throw;
        }
        // Debug.Log($"m to n {wave}, musicDataIndex : {_musicDataIndex}, {nodes}");
        // var nodes = data.NodeData[(int)_musicDataIndex];
        var beatNumber = nodes[0];
        GameManager.Wave.loadedBeatNum = (int)beatNumber;

        switch (wave)
        {
            case WaveType.Shooting:
                
                break;

            case WaveType.Punching:
                for (int i = 1; i < 5; i++)
                {
                    // MusicData에 따른 속성 지정
                    // 하나의 beat에 다수의 node가 생성되는 경우를 처리하기 위함

                    if (nodes[i] == 0) continue;

                    var temp = new NodeInfo();
                    temp.spawnPosition = GameManager.Wave.GetSpawnPosition(1);
                    temp.arrivalBoxNum = (i - 1);
                    temp.timeToReachPlayer = beatNumber * oneBeat;
                    temp.beatNum = beatNumber;

                    // Punch Type 별 인덱스 입력, NodeInfo To MusicData에서 punch에 필요한 UI와 콜라이더 붙임.
                    temp.punchTypeIndex = nodes[i]; 
                    
                    _nodeQueue.Enqueue(temp);
                    Debug.Log($"[Node Maker] Enqueue! {wave} {temp.beatNum}  nodeQueue.Count : {_nodeQueue.Count}"); //[XMC]
                    // 4개의 box 중, 동시에 다가오는 node들이 queue에 쌓인다
                }

                break;

            case WaveType.Hitting:
                // MusicData에 따른 속성 지정
                // 하나의 beat에 다수의 node가 생성되는 경우를 처리하기 위함
                for (int i = 1; i < 5; i++)
                {
                    if (nodes[i] == 0) continue;
                    
                    var temp = new NodeInfo();
                    temp.spawnPosition = GameManager.Wave.GetSpawnPosition(((i-1)));
                    temp.arrivalBoxNum = (i-1);
                    temp.timeToReachPlayer = beatNumber * oneBeat;
                    temp.beatNum = beatNumber;
                    if (nodes[i] == 1) 
                        temp.sideType = InteractionSide.Red;
                    else if (nodes[i] == 2) 
                        temp.sideType = InteractionSide.Blue;
                    
                    _nodeQueue.Enqueue(temp);
                    //[XMC]Debug.Log($"[Node Maker] Enqueue! {wave} Beat {temp.beatNum}  nodeQueue.Count : {_nodeQueue.Count}");
                    // 4개의 box 중, 동시에 다가오는 node들이 queue에 쌓인다
                    //Debug.Log(beatNumber + "의 실행 시간은 " + temp.timeToReachPlayer);
                }

                break;
        }
    }

    private void NodeInfoToMusicData(WaveType wave)
    {
        if(_nodeQueue.Count == 0) return;
        // Dequeue하고 토핑 풀에 값 하나 넣고, 존재하는 것에 다 넣었으면(Dequeue) 반복문 종료.

        NodeInfo tempNodeInfo = new NodeInfo();
        for (int i = 0; i < _poolSize; i++)
        {
            if (wave == WaveType.Shooting)
            {
                //tempPool = shootToppingPool;
                tempNodeInfo = _nodeQueue.Dequeue(); // nodeInfo 노드 하나에 해당하는 값  
                //[XMC]Debug.Log($"[Node Maker] Dequeue! {wave} {tempNodeInfo.beatNum} nodeQueue.Count : {_nodeQueue.Count}");
            }
            else if (wave == WaveType.Punching)
            {
                if (tempNodeInfo.beatNum != 0)
                {
                    // Debug.Log("[Node Maker] Dequeue 저장하고 다시 시도"); //[XMC]

                    if(punchToppingPool[i].activeSelf == true) 
                    {
                        // 이미 setactive(true)인 상태인 오브젝트면 = 이미 활성화돼서 초기화되면 안되는 상태일 경우는 다음 인덱스로 넘어감.
                        continue; 
                    }
                    Debug.Log($"[Node Maker] Dequeue {punchToppingPool[i].name}! {wave} {tempNodeInfo.beatNum} nodeQueue.Count : {_nodeQueue.Count}");
                    
                    punchToppingPool[i].SetActive(true);
                    
                    // PunchableMovement 초기화
                    PunchableMovement punchableMovement = punchToppingPool[i].GetComponent<PunchableMovement>();
                    punchableMovement.transform.position = tempNodeInfo.spawnPosition;
                    punchableMovement.beatNum = tempNodeInfo.beatNum;
                    StartCoroutine(punchableMovement.InitializeToppingRoutine(tempNodeInfo));

                    // Breakable 초기화
                    SetPunchType(punchToppingPool[i], tempNodeInfo.punchTypeIndex, punchableMovement);
                    // punchToppingPool[i].GetComponent<Breakable>().InitBreakable();
                   
                    break;
                }
                    
                // nodeInfo 노드 하나에 해당하는 값
                tempNodeInfo = _nodeQueue.Dequeue();   

                if(punchToppingPool[i].activeSelf == true) 
                {
                    continue; // 이미 setactive(true)인 상태인 오브젝트면 넘어감!!
                }
                Debug.Log($"[Node Maker] Dequeue {punchToppingPool[i].name}! {wave} {tempNodeInfo.beatNum} nodeQueue.Count : {_nodeQueue.Count}"); //[XMC]

                punchToppingPool[i].SetActive(true);
                
                // PunchableMovement 초기화
                PunchableMovement movement = punchToppingPool[i].GetComponent<PunchableMovement>();
                movement.transform.position = tempNodeInfo.spawnPosition;
                movement.beatNum = tempNodeInfo.beatNum;
                StartCoroutine(movement.InitializeToppingRoutine(tempNodeInfo));

                SetPunchType(punchToppingPool[i], tempNodeInfo.punchTypeIndex, movement);
                // punchToppingPool[i].GetComponent<Breakable>().InitBreakable();

                break;
            }
            else if(wave == WaveType.Hitting)
            { //템프노트인포가 남아있는지, 위치는 어디로 초기화되는지
                if (hitToppingPool[i].activeSelf == true) continue; // 이미 setactive(true)인 상태인 오브젝트면 넘어감!!
                if (_nodeQueue.Peek().sideType == InteractionSide.Red && i > 9) continue;   // 0~9 만 Red Object. 아니면 넘어감
                if (_nodeQueue.Peek().sideType == InteractionSide.Blue && i < 10) continue;  // 10~19만 Blue Object. 아니면 넘어감
                
                tempNodeInfo = _nodeQueue.Dequeue(); // nodeInfo 노드 하나에 해당하는 값  
                //[XMC]Debug.Log($"[Node Maker] Dequeue! {wave} {tempNodeInfo.beatNum} nodeQueue.Count : {_nodeQueue.Count}");
                
                hitToppingPool[i].transform.position = tempNodeInfo.spawnPosition;
                hitToppingPool[i].GetComponent<HittableMovement>().InitializeTopping(tempNodeInfo);
                hitToppingPool[i].SetActive(true);
                //Debug.Log(musicDataIndex + " Beat " + hitToppingPool[i].name + " 생성함");
                break;
            }
        }
    }
    
    // tempNodeInfo.sideType,tempNodeInfo.punchTypeIndex에 따라 해당하는 오브젝트 풀을 반환함. 
    void SetPunchType(GameObject punchGameObject, uint typeIndex, PunchableMovement movement)
    {
        
        Debug.Log($"[Motion] {punchGameObject.name} Set Punch Type");
        // [Punch] 오브젝트 풀의 재사용성을 높이기 위해, 각 쿠키의 요소를 동적으로 변경 
        if (movement.typeIndex != 0)
        {
            // 이미 존재하면 prevTypeIndex와 typeIndex를 비교하고 삭제, 생성
            // 추후 콜라이더는 위치 조정만 하는 방향으로 수정
            if (movement.typeIndex == typeIndex)
            {
                switch (typeIndex)
                {
                    case 1: // 레프트 잽
                        punchGameObject.transform.rotation = Quaternion.Euler(270f, 0f, 0f);
                        break;
                    case 2: // 레프트 훅
                        punchGameObject.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                        break;
                    case 3: // 레프트 어퍼컷
                        punchGameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        break;
                    case 4: // 라이트 잽
                        punchGameObject.transform.rotation = Quaternion.Euler(270f, 0f, 0f);
                        break;
                    case 5: // 라이트 훅
                        punchGameObject.transform.rotation = Quaternion.Euler(0f, 0f, 270f);
                        break;
                    case 6: // 라이트 어퍼컷
                        punchGameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                movement.typeIndex = typeIndex;
                
                // 쿠키 원본은 유지, UI, Image(자식 게임오브젝트) 삭제
                Transform[] allChildrenExcludingThis 
                    = punchGameObject.transform.GetComponentsInChildren<Transform>(true)
                        .Where(t => t != punchGameObject.transform).ToArray();
                foreach (var childTransform in allChildrenExcludingThis)
                {
                    Debug.Log($"[Motion] {punchGameObject.name} destroy {childTransform.gameObject}");
                    childTransform.parent = null;
                    Destroy(childTransform.gameObject);
                }
                        
                // typeIndex에 맞는 자식 게임오브젝트 생성
                // TODO : 펀치 타입에 따라 rotation 설정 childCollider를 할당하고 오브젝트의 rotation을 바꿈
                Debug.Log($"[Motion] {punchGameObject.name} Set Punch Type  typeIndex : {typeIndex}");
                switch (typeIndex)
                {
                    case 1: // 레프트 잽
                        Instantiate(childCollider[(int)typeIndex - 1], punchGameObject.transform).transform.localScale = Vector3.one;
                        punchGameObject.transform.rotation = Quaternion.Euler(270f, 0f, 0f);
                        break;
                    case 2: // 레프트 훅
                        Instantiate(childCollider[(int)typeIndex- 1], punchGameObject.transform).transform.localScale = Vector3.one;
                        punchGameObject.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                        Instantiate(cookieDirectionPrefabs[0], punchGameObject.transform); 
                        break;
                    case 3: // 레프트 어퍼컷
                        Instantiate(childCollider[(int)typeIndex- 1], punchGameObject.transform).transform.localScale = Vector3.one;
                        Instantiate(cookieDirectionPrefabs[1], punchGameObject.transform);
                        break;
                    case 4: // 라이트 잽
                        Instantiate(childCollider[(int)typeIndex- 1], punchGameObject.transform).transform.localScale = Vector3.one;
                        punchGameObject.transform.rotation = Quaternion.Euler(270f, 0f, 0f);
                        break;
                    case 5: // 라이트 훅
                        Instantiate(childCollider[(int)typeIndex- 1], punchGameObject.transform).transform.localScale = Vector3.one;
                        punchGameObject.transform.rotation = Quaternion.Euler(0f, 0f, 270f);
                        Instantiate(cookieDirectionPrefabs[2], punchGameObject.transform);
                        break;
                    case 6: // 라이트 어퍼컷
                        Instantiate(childCollider[(int)typeIndex- 1], punchGameObject.transform).transform.localScale = Vector3.one;
                        Instantiate(cookieDirectionPrefabs[3], punchGameObject.transform);
                        break;
                    default:
                        break;
                }
            }

        }
        else if (movement.typeIndex == 0)
        {
            movement.typeIndex = typeIndex;
            Debug.Log($"[Motion] {punchGameObject.name} Set Punch Type  typeIndex : {typeIndex}");
            switch (typeIndex)
            {
                case 1:
                    Instantiate(childCollider[(int)typeIndex - 1], punchGameObject.transform).transform.localScale = Vector3.one;
                    punchGameObject.transform.rotation = Quaternion.Euler(270f, 0f, 0f);
                    break;
                case 2:
                    Instantiate(childCollider[(int)typeIndex- 1], punchGameObject.transform).transform.localScale = Vector3.one;
                    punchGameObject.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                    Instantiate(cookieDirectionPrefabs[0], punchGameObject.transform); 
                    break;
                case 3:
                    Instantiate(childCollider[(int)typeIndex- 1], punchGameObject.transform).transform.localScale = Vector3.one;
                    Instantiate(cookieDirectionPrefabs[1], punchGameObject.transform);
                    break;
                case 4:
                    Instantiate(childCollider[(int)typeIndex- 1], punchGameObject.transform).transform.localScale = Vector3.one;
                    punchGameObject.transform.rotation = Quaternion.Euler(270f, 0f, 0f);
                    break;
                case 5:
                    Instantiate(childCollider[(int)typeIndex- 1], punchGameObject.transform).transform.localScale = Vector3.one;
                    punchGameObject.transform.rotation = Quaternion.Euler(0f, 0f, 270f);
                    Instantiate(cookieDirectionPrefabs[2], punchGameObject.transform);
                    break;
                case 6:
                    Instantiate(childCollider[(int)typeIndex- 1], punchGameObject.transform).transform.localScale = Vector3.one;
                    Instantiate(cookieDirectionPrefabs[3], punchGameObject.transform);
                    break;
                default:
                    break;
            }    
        }
        punchGameObject.GetComponent<Breakable>().InitBreakable();
        
        // typeIndex에 해당하는 콜라이더와 방향 UI가 이미 존재하면 생성하지 않는다.
        // 타입에 맞는 UI와 콜라이더를 자식으로 붙임. 
        // temp.punchTypeIndex = node[i]
        // node[i] -> 펀치 타입 인덱스  노드 큐에 enqueue하는 곳
        /*
        1 - 빨강 레프트 잽
        2 - 빨강 레프트 훅
        3 - 빨강 레프트 어퍼컷
        4 - 파랑 라이트 잽
        5 - 파랑 라이트 훅
        6 - 파랑 라이트 어퍼컷
        */
    }

    private void InitPunchToppingPool()
    {
        Debug.Log("Init Punch Topping Pool");
        int poolSize = 20;

        if (punchToppingPool.Length == 0)
        {
            punchToppingPool = new GameObject[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                // 쿠키 프리팹 랜덤 생성 (타입 지정되지 않은 상태)
                GameObject topping = cookiePrefabs[Random.Range(0, 3)];
                GameObject node = Instantiate(topping);
                node.SetActive(false);

                // 토핑 각각을 DDOL할 이유가 없다.
                // DontDestroyOnLoad(node);
                
                punchToppingPool[i] = node;
                punchToppingPool[i].name = "Punch_" + i;
            }
        }
    }
}