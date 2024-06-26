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

        CreateUserData();
    }

    [Header("must Set User Info")]
    public int userNumber;

    [Header("Variables (Auto)")] public string filePath;
    
    [ContextMenu("Create User Data")]
    private void CreateUserData()
    {
        if (userNumber > 0)
        {
            string fileName = $"User{userNumber}_Data.csv";
            filePath = Path.Combine(Application.dataPath, fileName);
            
            if (!File.Exists(filePath))
            {
                SetInitialData();
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
    private void SetInitialData()
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
    
    // 특정 유저 번호에 속도 데이터를 추가하는 메서드
    public void AppendSpeedData(int userNumber, float speed)
    {
        List<string> lines = new List<string>(File.ReadAllLines(filePath));
        string targetPrefix = userNumber.ToString();

        // 특정 유저 번호가 포함된 행 찾기
        bool rowFound = false;
        for (int i = 1; i < lines.Count; i++) // 헤더를 건너뛰고 시작
        {
            if (lines[i].StartsWith(targetPrefix))
            {
                // 기존 행에 속도 데이터 추가
                lines[i] += "," + speed.ToString();
                rowFound = true;
                break;
            }
        }

        // 해당 행이 없으면 새로운 행 추가
        if (!rowFound)
        {
            string newRow = targetPrefix + "," + speed.ToString();
            lines.Add(newRow);
        }

        File.WriteAllLines(filePath, lines);
    }

}