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
    public GameObject GunSpawn;
    public GameObject PunchSpawn;
    public GameObject HitSpawn;

    private int poolSize = 10; // Object Pool Size
    [SerializeField] private GameObject[] shootToppingPool;
    [SerializeField] private GameObject[] punchToppingPool;
    [SerializeField] private GameObject[] hitToppingPool;
    
    private uint musicDataIndex = 0; //1~ NodeData
    private Queue<NodeInfo> _nodeQueue = new Queue<NodeInfo>();

    // private void Start()
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
    // }

    public void InitToppingPool(WaveType wave)
    {
        // topping pool 생성해줌.
        // Wave가 처음 실행될 때, 한번 초기화 진행하는 것임
        switch (wave)
        {
            case WaveType.Shooting:
                if (shootToppingPool.Length == 0)
                {
                    shootToppingPool = new GameObject[poolSize]; // 배열 생성
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
                    punchToppingPool = new GameObject[poolSize]; // 배열 생성
                    for (int i = 0; i < poolSize; ++i)
                    {
                        GameObject topping = PunchTopping[(i % PunchTopping.Count)];
                        GameObject node = Instantiate(topping);
                        node.SetActive(false);
                        DontDestroyOnLoad(node);
                        punchToppingPool[i] = node;
                        punchToppingPool[i].name = "Punch_" + i;
                    }
                }

                break;

            case WaveType.Hitting:
                if (hitToppingPool.Length == 0)
                {
                    hitToppingPool = new GameObject[poolSize];
                    for (int i = 0; i < poolSize; ++i)
                    {
                        GameObject topping = HitTopping[(i % HitTopping.Count)];
                        GameObject node = Instantiate(topping);
                        node.SetActive(false);
                        DontDestroyOnLoad(node);
                        hitToppingPool[i] = node;
                        hitToppingPool[i].name = "Hit_" + i;
                    }
                }

                break;
        }

        StartCoroutine(SpawnManager(wave));
    }

    IEnumerator SpawnManager(WaveType wave)
    {
        while (true)
        {
            // ?초 마다 배열 안에 있는 객체들이 차례대로 생성될 것
            yield return new WaitForSecondsRealtime(0.5f);
            
            // Music data의 4개의 node data를 NodeInfo 형식으로 바꾸어, Enqueue.
            musicDataToNodeInfo(wave);
            musicDataIndex++;
            if (_nodeQueue.Count > 0)
            {
                var tempNodeInfo = _nodeQueue.Dequeue();
                for (int i = 0; i < poolSize; i++)
                {
                    if (wave == WaveType.Shooting)
                    {
                        //tempPool = shootToppingPool;
                    }
                    else if (wave == WaveType.Punching)
                    {
                        //tempPool = punchToppingPool;
                    }
                    else
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
            }
        }
    }

    private bool musicDataToNodeInfo(WaveType wave)
    {
        var data = GameManager.Wave.CurMusicData;
        if (data.BeatNum <= musicDataIndex)
            return false;
        
        float oneBeat = 60.0f / data.BPM;
        var nodes = data.NodeData[(int)musicDataIndex];
        var beatNumber = nodes[0];

        switch (wave)
        {
            case WaveType.Shooting:

                break;

            case WaveType.Punching:

                break;

            case WaveType.Hitting:
                // MusicData에 따른 속성 지정
                // 하나의 beat에 다수의 node가 생성되는 경우를 처리하기 위함
                for (int i = 1; i < 5; i++)
                {
                    if (nodes[i] == 0) continue;
                    
                    var temp = new NodeInfo();
                    temp.spawnPosition = GameManager.Wave.GetSpawnPosition(i-1);
                    temp.arrivalAreaIndex = (i-1);
                    temp.timeToReachPlayer = beatNumber * oneBeat;
                    temp.beatNum = beatNumber;
                    if (nodes[i] == 1) 
                        temp.sideType = InteractionSide.Red;
                    else if (nodes[i] == 2) 
                        temp.sideType = InteractionSide.Green;

                    _nodeQueue.Enqueue(temp); // 4개의 box 중, 동시에 다가오는 node들이 queue에 쌓인다
                }

                break;
        }

        return true;
    }
}
