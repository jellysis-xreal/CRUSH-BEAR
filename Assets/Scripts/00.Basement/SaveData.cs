using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public bool[] isUnlocked;
    public int[] currentScore;

    public SaveData(int stageNumber)
    {
        isUnlocked = new bool[stageNumber];
        currentScore = new int[stageNumber];
    }
}
