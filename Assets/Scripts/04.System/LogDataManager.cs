using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LogDataManager : MonoBehaviour
{
    private static LogDataManager instance;
    public static LogDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    [Header("must Set User Info")]
    public int userNumber;

    private void CreateUserData()
    {
        if (userNumber > 0)
        {
            string fileName = $"User{userNumber}_Data.csv";
            string filePath = Path.Combine(Application.dataPath, fileName);
            
            if (!File.Exists(filePath))
            {
                SetInitialData(filePath);
            }
            else
            {
                Debug.Log($"{filePath}에 이미 파일이 존재합니다.");
            }
        }
        else
        {
            Debug.LogError("데이터가 입렫되지 않았습니다.");
        }
    }
    private void SetInitialData(string filePath)
    {
        // 파일 내용을 생성합니다.
        List<string> lines = new List<string>()
        { 
            $"User Num,{userNumber}",
            $"파일 생성 일시,{DateTime.Now.ToString("yyyy-MM-dd_HH-mm")}",
        };
        File.WriteAllLines(filePath, lines);
        Debug.Log($"Initial data created! {filePath}");
    }
}