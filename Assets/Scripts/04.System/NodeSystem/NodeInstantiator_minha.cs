using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using EnumTypes;
using UnityEngine.Serialization;
using UnityEngine.XR.Content.Interaction;

public class NodeInstantiator_minha : MonoBehaviour
{
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
        Debug.Log($"[Node Maker] : Init Topping Pool! This wave is [{wave}]");
        _musicDataIndex = 0;

        InitializeNodeAndPool();
        
        switch (wave)
        {
            case WaveType.Shooting:
                if (shootToppingPool.Length == 0)
                {
                    shootToppingPool = new GameObject[_poolSize]; // 배열 생성
                    // for (int i = 0; i < poolSize; ++i)
                    // {
                    //     GameObject topping = ShootTopping[(i % ShootTopping.Count)];
                    //     toppingPool[i] = Instantiate(topping) as GameObject;
                    //     toppingPool[i].name = "Shoot_" + i;
                    //     toppingPool[i].SetActive(false); // `사용 안함` 상태
                    // }
                }

                break;

            case WaveType.Punching:
                InitPunchToppingPool();
                /*if (punchToppingPool.Length == 0)
                {
                    punchToppingPool = new GameObject[_poolSize]; // 배열 생성
                    for (int i = 0; i < _poolSize; ++i)
                    {
                        GameObject topping = PunchTopping[(i % PunchTopping.Count)];
                        GameObject node = Instantiate(topping);
                        node.SetActive(false);
                        DontDestroyOnLoad(node);
                        punchToppingPool[i] = node;
                        punchToppingPool[i].name = "Punch_" + i;
                    }
                    Debug.Log($"[Node Maker] Generate {punchToppingPool.Length} Punchable Object ");
                }*/
                break;

            case WaveType.Hitting:
                if (hitToppingPool.Length == 0)
                {
                    hitToppingPool = new GameObject[_poolSize];
                    for (int i = 0; i < _poolSize; ++i)
                    {
                        GameObject topping = HitTopping[(i % HitTopping.Count)];
                        GameObject node = Instantiate(topping);
                        node.SetActive(false);
                        DontDestroyOnLoad(node);
                        hitToppingPool[i] = node;
                        hitToppingPool[i].name = "Hit_" + i;
                    }
                    Debug.Log($"[Node Maker] Generate {hitToppingPool.Length} hittable Object ");
                }

                break;
        }

        _curWaveCoroutine = StartCoroutine(SpawnManager(wave));
        Debug.Log("[Node Maker] Start Coroutine");
    }

    IEnumerator SpawnManager(WaveType wave)
    {
        // while (!isWaveFinished)
        // break
        while (true) // 지금은 10개 생성, 대기 Queue 10개까지 됨.
        {
            // Debug.Log($"coroutine~ this wave is {wave}");
            // ?초 마다 배열 안에 있는 객체들이 차례대로 생성될 것
            yield return new WaitForSecondsRealtime(0.05f);
            
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
        foreach (var punch in punchToppingPool) Destroy(punch);
        foreach (var hit in hitToppingPool) Destroy(hit);
        _nodeQueue.Clear();
        
        StopCoroutine(_curWaveCoroutine);
    }
    
    private void InitializeNodeAndPool()
    {
        foreach (var shoot in shootToppingPool) shoot.SetActive(false);
        foreach (var punch in punchToppingPool) punch.SetActive(false);
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
            Debug.Log("Error 더이상 Enqueue할 data없음.");
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

                    temp.punchTypeIndex = nodes[i]; // Punch Type 별 인덱스 입력
                    
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
                    
                    _nodeQueue.Enqueue(temp);
                    Debug.Log($"[Node Maker] Enqueue! {wave} {temp.beatNum}  nodeQueue.Count : {_nodeQueue.Count}");
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
                    Debug.Log($"[Node Maker] Enqueue! {wave} Beat {temp.beatNum}  nodeQueue.Count : {_nodeQueue.Count}");
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
                Debug.Log($"[Node Maker] Dequeue! {wave} {tempNodeInfo.beatNum} nodeQueue.Count : {_nodeQueue.Count}");
            }
            else if (wave == WaveType.Punching)
            {
                //tempPool = punchToppingPool; 
                if (tempNodeInfo.beatNum != 0)
                {
                    Debug.Log("[Node Maker] Dequeue 저장하고 다시 시도");
                    GameObject[] poolsToUse = GetObjectPool(tempNodeInfo.punchTypeIndex);
                    if(poolsToUse[i].activeSelf == true) 
                    {
                        continue; // 이미 setactive(true)인 상태인 오브젝트면 넘어감!!
                    }
                    Debug.Log($"[Node Maker] Dequeue! {wave} {tempNodeInfo.beatNum} nodeQueue.Count : {_nodeQueue.Count}");
                    
                    poolsToUse[i].SetActive(true);
                    poolsToUse[i].transform.position = tempNodeInfo.spawnPosition;
                    StartCoroutine(poolsToUse[i].GetComponentInChildren<PunchaleMovement>().InitializeToppingRoutine(tempNodeInfo));
                    poolsToUse[i].GetComponentInChildren<Breakable>().InitBreakable();
                    break;
                }
                    
                tempNodeInfo = _nodeQueue.Dequeue(); // nodeInfo 노드 하나에 해당하는 값  
                GameObject[] punchPoolsToUse = GetObjectPool(tempNodeInfo.punchTypeIndex);
                for (int j = 0; j < punchPoolsToUse.Length; j++)
                {
                    Debug.Log($"[Node Maker] punchPoolsToUse[{j}] {punchPoolsToUse[j].name} is {punchPoolsToUse[j].activeSelf}");
                }
                

                if(punchPoolsToUse[i].activeSelf == true) 
                {
                    continue; // 이미 setactive(true)인 상태인 오브젝트면 넘어감!!
                }
                Debug.Log($"[Node Maker] Dequeue! {wave} {tempNodeInfo.beatNum} nodeQueue.Count : {_nodeQueue.Count}");

                punchPoolsToUse[i].SetActive(true);
                punchPoolsToUse[i].transform.position = tempNodeInfo.spawnPosition;
                StartCoroutine(punchPoolsToUse[i].GetComponentInChildren<PunchaleMovement>().InitializeToppingRoutine(tempNodeInfo));
                punchPoolsToUse[i].GetComponentInChildren<Breakable>().InitBreakable();
                    
                /*punchToppingPool[i].transform.position = tempNodeInfo.spawnPosition;
                punchToppingPool[i].GetComponentInChildren<PunchaleMovement>().InitializeTopping(tempNodeInfo);
                punchToppingPool[i].GetComponentInChildren<Breakable>().InitBreakable();
                punchToppingPool[i].SetActive(true);*/
                break;
            }
            else if(wave == WaveType.Hitting)
            { //템프노트인포가 남아있는지, 위치는 어디로 초기화되는지
                if (hitToppingPool[i].activeSelf == true) continue; // 이미 setactive(true)인 상태인 오브젝트면 넘어감!!
                
                tempNodeInfo = _nodeQueue.Dequeue(); // nodeInfo 노드 하나에 해당하는 값  
                Debug.Log($"[Node Maker] Dequeue! {wave} {tempNodeInfo.beatNum} nodeQueue.Count : {_nodeQueue.Count}");
                
                hitToppingPool[i].transform.position = tempNodeInfo.spawnPosition;
                hitToppingPool[i].GetComponent<HittableMovement>().InitializeTopping(tempNodeInfo);
                hitToppingPool[i].SetActive(true);
                //Debug.Log(musicDataIndex + " Beat " + hitToppingPool[i].name + " 생성함");
                break;
            }
        }
        Debug.Log($"[Node] Note Info -> Music data {(int)_musicDataIndex}");
    }
    
    // tempNodeInfo.sideType,tempNodeInfo.punchTypeIndex에 따라 해당하는 오브젝트 풀을 반환함. 
    private GameObject[] GetObjectPool(uint punchTypeIndex)
    {
        switch (punchTypeIndex)
        {
            case 1:
                return punchLeftZapPool;
                break;
            case 2:
                return punchLeftHookPool;
                break;
            case 3:
                return punchLeftUpperCutPool;
                break;
            case 4:
                return punchRightZapPool;
                break;
            case 5:
                return punchRightHookPool;
                break;
            case 6:
                return punchRightUpperCutPool;
                break;
        }
        return null;
    }
    private GameObject[] GetObjectPool(InteractionSide sideType)
    {
        switch (sideType)
        {
            case InteractionSide.Red:
                return hitRedPool;
                break;
            case InteractionSide.Blue:
                return hitBluePool;
                break;
        }
        return null;
    }

    private void InitPunchToppingPool()
    {
        int poolSize = 20;
        if (punchLeftZapPool.Length == 0)
        {
            punchLeftZapPool = new GameObject[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                GameObject topping = PunchTopping[0];
                GameObject node = Instantiate(topping);
                node.SetActive(false);
                DontDestroyOnLoad(node);
                punchLeftZapPool[i] = node;
                punchLeftZapPool[i].name = "Punch_LeftZap" + i;
            }
        }
        if (punchLeftHookPool.Length == 0)
        {
            punchLeftHookPool = new GameObject[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                GameObject topping = PunchTopping[1];
                GameObject node = Instantiate(topping);
                node.SetActive(false);
                DontDestroyOnLoad(node);
                punchLeftHookPool[i] = node;
                punchLeftHookPool[i].name = "Punch_LeftHook" + i;
            }
        }if (punchLeftUpperCutPool.Length == 0)
        {
            punchLeftUpperCutPool = new GameObject[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                GameObject topping = PunchTopping[2];
                GameObject node = Instantiate(topping);
                node.SetActive(false);
                DontDestroyOnLoad(node);
                punchLeftUpperCutPool[i] = node;
                punchLeftUpperCutPool[i].name = "Punch_LeftUpperCut" + i;
            }
        }if (punchRightZapPool.Length == 0)
        {
            punchRightZapPool = new GameObject[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                GameObject topping = PunchTopping[3];
                GameObject node = Instantiate(topping);
                node.SetActive(false);
                DontDestroyOnLoad(node);
                punchRightZapPool[i] = node;
                punchRightZapPool[i].name = "Punch_RightZap" + i;
            }
        }if (punchRightHookPool.Length == 0)
        {
            punchRightHookPool = new GameObject[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                GameObject topping = PunchTopping[4];
                GameObject node = Instantiate(topping);
                node.SetActive(false);
                DontDestroyOnLoad(node);
                punchRightHookPool[i] = node;
                punchRightHookPool[i].name = "Punch_RightHook" + i;
            }
        }
        if (punchRightUpperCutPool.Length == 0)
        {
            punchRightUpperCutPool = new GameObject[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                GameObject topping = PunchTopping[5];
                GameObject node = Instantiate(topping);
                node.SetActive(false);
                DontDestroyOnLoad(node);
                punchRightUpperCutPool[i] = node;
                punchRightUpperCutPool[i].name = "Punch_RightUpperCut" + i;
            }
        }
    }
}
