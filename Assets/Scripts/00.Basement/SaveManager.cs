using DG.Tweening.Plugins.Core.PathCore;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager
{
    // 추후 이사하겠습니다~
    private int stageNumber = 1;
    public SaveData data;
    private string saveFileName = "crushBearSave.json";
    public void LoadSaveData()
    {
        string path = Application.persistentDataPath + "/" + saveFileName;

        if(File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            data = JsonUtility.FromJson<SaveData>(jsonData);
        }
        else
        {
            data = new SaveData(stageNumber);
            SaveLoadData();
        }
    }

    public void SaveLoadData()
    {
        string saveData = JsonUtility.ToJson(data, true);
        string path = Application.persistentDataPath + "/" + saveFileName;
        File.WriteAllText(path, saveData);
    }
}
