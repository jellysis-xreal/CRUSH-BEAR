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
    
    private int poolSize = 10;       // Object Pool Size
    [SerializeField] private GameObject[] toppingPool = new GameObject[] { };

    private void Start()
    {
        toppingPool = new GameObject[poolSize]; // 배열 생성
    }
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

    public void Init(WaveType wave)
    {
        // topping pool 교체
        switch (wave)
        {
            case WaveType.Shooting:
                // for (int i = 0; i < poolSize; ++i)
                // {
                //     GameObject topping = ShootTopping[(i % ShootTopping.Count)];
                //     toppingPool[i] = Instantiate(topping) as GameObject;
                //     toppingPool[i].name = "Shoot_" + i;
                //     toppingPool[i].SetActive(false); // `사용 안함` 상태
                // }
                break;

            case WaveType.Punching:
                for (int i = 0; i < poolSize; ++i)
                {
                    GameObject topping = PunchTopping[(i % PunchTopping.Count)];
                    toppingPool[i] = Instantiate(topping) as GameObject;
                    toppingPool[i].name = "Punch_" + i;
                    toppingPool[i].SetActive(false); // `사용 안함` 상태
                }
                break;

            case WaveType.Hitting:
                for (int i = 0; i < poolSize; ++i)
                {
                    GameObject topping = HitTopping[(i % HitTopping.Count)];
                    toppingPool[i] = Instantiate(topping) as GameObject;
                    toppingPool[i].name = "Hit_" + i;
                    toppingPool[i].SetActive(false); // `사용 안함` 상태
                }
                break;
        }
        StartCoroutine(SpawnManager());
    }

    IEnumerator SpawnManager()
    {
        while (Time.timeScale == 0)
        {
            // 2초 마다 배열 안에 있는 객체들이 차례대로 생성될 것
            yield return new WaitForSeconds(2.0f);

            for (int i = 0; i < poolSize; i++)
            {
                if (toppingPool[i].activeSelf == true) // 이미 setactive(true)인 상태인 오브젝트면 넘어감!!
                    continue;

                float x = UnityEngine.Random.Range(-5, 6);
                float z = UnityEngine.Random.Range(-5, 6);

                toppingPool[i].transform.position = new Vector3(x, 0.5f, z); // 랜덤한 위치
                toppingPool[i].SetActive(true); // `사용 함` 상태

                break;
            }
        }
    }
}
