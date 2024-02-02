using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using EnumTypes;
using UnityEngine.Serialization;

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

    private int _poolSize = 10; // Object Pool Size
    [SerializeField] private GameObject[] shootToppingPool;
    [SerializeField] private GameObject[] punchToppingPool;
    [SerializeField] private GameObject[] hitToppingPool;
    
    private uint _musicDataIndex = 0; //1~ NodeData
    private Queue<NodeInfo> _nodeQueue = new Queue<NodeInfo>();

    private Coroutine _curWaveCoroutine;

    /*// private void Start()
    // {
    //     // Resource에서 받아오기
    //     object[] GunObjects = Resources.LoadAll("Interaction/Gun");
    //     object[] BreakObjects = Resources.LoadAll("Interaction/Break");
    //     object[] HitObjects = Resources.LoadAll("Interaction/Hit");
    //     
    //     for (int i = 0; i < GunObjects.Length; i++)
    //     {
    //         GameObject go = GunObjects[i] as GameObject;
    //         GunTopping.Add(go);
    //     }
    //     
    //     for (int i = 0; i < BreakObjects.Length; i++)
    //     {
    //         GameObject go = BreakObjects[i] as GameObject;
    //         PunchTopping.Add(go);
    //     }
    //     
    //     for (int i = 0; i < HitObjects.Length; i++)
    //     {
    //         GameObject go = HitObjects[i] as GameObject;
    //         HitTopping.Add(go);
    //     }
    // }*/

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
                if (punchToppingPool.Length == 0)
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
                }

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
    }

    IEnumerator SpawnManager(WaveType wave)
    {
        // while (!isWaveFinished)
        // break
        while (true) // 지금은 10개 생성, 대기 Queue 10개까지 됨.
        {
            // Debug.Log($"coroutine~ this wave is {wave}");
            // ?초 마다 배열 안에 있는 객체들이 차례대로 생성될 것
            yield return new WaitForSecondsRealtime(0.1f);
            
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

                /*var tempNodeInfo = _nodeQueue.Dequeue(); // nodeInfo 노드 하나에 해당하는 값  
                Debug.Log($"[Node Maker] Dequeue! nodeQueue.Count : {_nodeQueue.Count}");
                for (int i = 0; i < _poolSize; i++)
                {
                    if (wave == WaveType.Shooting)
                    {
                        //tempPool = shootToppingPool;
                    }
                    else if (wave == WaveType.Punching)
                    {
                        //tempPool = punchToppingPool;
                        if(punchToppingPool[i].activeSelf == true) continue; // 이미 setactive(true)인 상태인 오브젝트면 넘어감!!

                        punchToppingPool[i].transform.position = tempNodeInfo.spawnPosition;
                        punchToppingPool[i].GetComponent<PunchaleMovement>().InitializeTopping(tempNodeInfo);
                        punchToppingPool[i].SetActive(true);
                        break;
                    }
                    else if(wave == WaveType.Hitting)
                    {
                        if (hitToppingPool[i].activeSelf == true) // 이미 setactive(true)인 상태인 오브젝트면 넘어감!!
                            continue;
                        hitToppingPool[i].transform.position = tempNodeInfo.spawnPosition;
                        hitToppingPool[i].GetComponent<HittableMovement>().InitializeTopping(tempNodeInfo);
                        hitToppingPool[i].SetActive(true);
                        //Debug.Log(musicDataIndex + " Beat " + hitToppingPool[i].name + " 생성함");
                        break;
                    }
                }

                // Debug.Log($"Node Info Dequeue");*/
            }
        }
        
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
            StopCoroutine(_curWaveCoroutine);
            return;
            throw;
        }
        // Debug.Log($"m to n {wave}, musicDataIndex : {_musicDataIndex}, {nodes}");
        // var nodes = data.NodeData[(int)_musicDataIndex];
        var beatNumber = nodes[0];

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
                    temp.arrivalBoxNum = i;
                    
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
                    temp.spawnPosition = GameManager.Wave.GetSpawnPosition(2);
                    temp.arrivalBoxNum = (i-1);
                    temp.timeToReachPlayer = beatNumber * oneBeat;
                    temp.beatNum = beatNumber;
                    if (nodes[i] == 1) 
                        temp.sideType = InteractionSide.Red;
                    else if (nodes[i] == 2) 
                        temp.sideType = InteractionSide.Green;
                    
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

        for (int i = 0; i < _poolSize; i++)
        {
            if (wave == WaveType.Shooting)
            {
                //tempPool = shootToppingPool;
                var tempNodeInfo = _nodeQueue.Dequeue(); // nodeInfo 노드 하나에 해당하는 값  
                Debug.Log($"[Node Maker] Dequeue! {wave} {tempNodeInfo.beatNum} nodeQueue.Count : {_nodeQueue.Count}");
            }
            else if (wave == WaveType.Punching)
            {
                //tempPool = punchToppingPool;
                if(punchToppingPool[i].activeSelf == true) continue; // 이미 setactive(true)인 상태인 오브젝트면 넘어감!!
                
                var tempNodeInfo = _nodeQueue.Dequeue(); // nodeInfo 노드 하나에 해당하는 값  
                Debug.Log($"[Node Maker] Dequeue! {wave} {tempNodeInfo.beatNum} nodeQueue.Count : {_nodeQueue.Count}");
                
                punchToppingPool[i].transform.position = tempNodeInfo.spawnPosition;
                punchToppingPool[i].GetComponent<PunchaleMovement>().InitializeTopping(tempNodeInfo);
                punchToppingPool[i].SetActive(true);
                break;
            }
            else if(wave == WaveType.Hitting)
            {
                if (hitToppingPool[i].activeSelf == true) continue; // 이미 setactive(true)인 상태인 오브젝트면 넘어감!!
                
                var tempNodeInfo = _nodeQueue.Dequeue(); // nodeInfo 노드 하나에 해당하는 값  
                Debug.Log($"[Node Maker] Dequeue! {wave} {tempNodeInfo.beatNum} nodeQueue.Count : {_nodeQueue.Count}");
                
                hitToppingPool[i].transform.position = tempNodeInfo.spawnPosition;
                hitToppingPool[i].GetComponent<HittableMovement>().InitializeTopping(tempNodeInfo);
                hitToppingPool[i].SetActive(true);
                //Debug.Log(musicDataIndex + " Beat " + hitToppingPool[i].name + " 생성함");
                break;
            }
        }
    }
}
