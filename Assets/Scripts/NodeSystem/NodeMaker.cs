using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class NodeMaker : MonoBehaviour
{
    public int stageNum; // 스테이지 번호는 instance로 하나만 접근
    private static string[] stageDataName;
    
    [SerializeField] private SettingNodeInfo[] settingNodeInfos;
    [SerializeField] private NodeInfo[] nodeInfos;

    [SerializeField] private GameObject nodeLoaderGameObject;
    void Start()
    {
        InitializeNode();
        NamingJsonFileName();
        
        for (int i = 0; i < settingNodeInfos.Length; i++)
        {
            string jsonData = ObjectToJson(settingNodeInfos[i].nodeInfo); //nodeInfos[i]);
            CreateOneNodeJsonFile(Application.dataPath, stageDataName[i], jsonData);
            Debug.Log($"Path : {Application.dataPath}, jsonData length : {jsonData.Length}  "+jsonData);    
        }
        
        // 생성 완료 후 Node Load하기
        nodeLoaderGameObject.SetActive(true);
    }
    private void InitializeNode()
    {
        settingNodeInfos = new SettingNodeInfo[transform.childCount];
        settingNodeInfos = GetComponentsInChildren<SettingNodeInfo>();
    }
    private void NamingJsonFileName()
    {
        stageDataName = new string[settingNodeInfos.Length];
        for (int i = 0; i < settingNodeInfos.Length; i++)
        {
            stageDataName[i] = $"Stage{stageNum}_Node{i}";
            Debug.Log(stageDataName[i]);
        }
    }

    // Object를 Json파일로 변환하기
    string ObjectToJson(object obj)
    {
        return JsonUtility.ToJson(obj);
    }
    // 문자열로 만든 Json 데이터 '오브젝트 하나'를 파일로 저장하는 코드
    void CreateOneNodeJsonFile(string createPath, string fileName, string jsonData)
    {
        FileStream fileStream = new FileStream(String.Format("{0}/{1}.json", createPath, fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data,0,data.Length);
        fileStream.Close();
    }
}
// 한 노드 당 210 Byte
// 노드 10000개 => 2.10MB
// 찍어 내는 거 너무 귀찮아. AI 해줘