using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerr : Singleton<GameManagerr>
{
    void Start()
    {
        Init();
    }

    void Init()
    {
        PlayerManager.Instance.playerLifeValue = 3;
    }
}
