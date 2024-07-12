using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public bool isFirst;
    public bool[] isUnlocked;
    public int[] currentScore;

    public SaveData(int stageNumber)
    {
        isFirst = true;
        isUnlocked = new bool[stageNumber];
        currentScore = new int[stageNumber];
    }
}
