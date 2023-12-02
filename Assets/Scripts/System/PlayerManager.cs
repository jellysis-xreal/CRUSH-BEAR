using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    // 싱글톤화
    public int playerLifeValue = 0;

    public GameObject[] HeartGameObjects;

    // 처음에 3
    public void MinusPlayerLifeValue()
    {
        if(playerLifeValue == 0) return;
        /*for (int i = HeartGameObjects.Length - 1; i >= 0 ; i--)
        {
            HeartGameObjects[i].activeSelf
        }*/
        // 3 - 1 = 2 2번 인덱스 꺼야댐
        HeartGameObjects[playerLifeValue- 1].GetComponent<MeshRenderer>().material.color = Color.gray;
        // HeartGameObjects[playerLifeValue - 1].SetActive(false);
        playerLifeValue--; 
        Debug.Log("Attack Success player HP -1");
    }
}
