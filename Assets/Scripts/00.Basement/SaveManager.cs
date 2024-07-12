using DG.Tweening.Plugins.Core.PathCore;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager
{
    public SaveData data;
    private string saveFileName = "crushBearSave.json";
    public bool isTutorialClear { get { return data.isUnlocked[0]; } }
    public void LoadSaveData()
    {
        string path = Application.persistentDataPath + "/" + saveFileName;
        Debug.Log(path);
        if(File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            data = JsonUtility.FromJson<SaveData>(jsonData);
            Debug.Log("肺靛己傍");
        }
        else
        {
            int stageNumber = GameManager.Data.stageData.Length;
            data = new SaveData(stageNumber);
            SaveLoadData();
            Debug.Log("颇老 积己");
        }
    }

    public void SaveLoadData()
    {
        string saveData = JsonUtility.ToJson(data, true);
        string path = Application.persistentDataPath + "/" + saveFileName;
        File.WriteAllText(path, saveData);
    }
    public void ClearTutorial()
    {
        data.isFirst = false;
        data.isUnlocked[0] = true;
        SaveLoadData();
    }
    public void SaveLoadData(int ID, int currentScore)
    {
        int unlockID = GameManager.Data.stageData[ID].unlockID;
        data.isUnlocked[unlockID] = true;
        data.currentScore[ID] = currentScore;
        SaveLoadData();
    }
}
