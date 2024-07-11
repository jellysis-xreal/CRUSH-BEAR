using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class NodeLoader : MonoBehaviour
{
    private NodeInstantiater _nodeInstantiater;
    private static string[] stageDataName;
    public int stageNum; // 스테이지 번호는 instance로 하나만 접근
    /*void Start()
    {
        _nodeInstantiater = GetComponent<NodeInstantiater>();
        NamingJsonFileName();
        NodeInfoSend();
    }*/

    private void OnEnable()
    {
        _nodeInstantiater = GetComponent<NodeInstantiater>();
        NamingJsonFileName();
        NodeInfoSend();
        //Debug.Log("OnEnable! Noad Loader");
    }

    // 불러들일 json file name을 정함.
    private void NamingJsonFileName()
    {
        stageDataName = new string[FindEachStageDataLength()];
        for (int i = 0; i < stageDataName.Length; i++)
        {
            stageDataName[i] = $"StageData{stageNum}_Node{i}";
            //Debug.Log(stageDataName[i]);
        }
    }
    
    // Json 파일을 읽어들여서 Object로 변환하는 코드
    private NodeInfo LoadJsonFile<NodeInfo>(string loadPath, string fileName)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", loadPath, fileName), FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return JsonUtility.FromJson<NodeInfo>(jsonData);
    }

    private void NodeInfoSend()
    {
        _nodeInstantiater.InitArray(stageDataName.Length);
        for (int i = 0; i < stageDataName.Length; i++)
        {
            NodeInfo info = LoadJsonFile<NodeInfo>(Application.dataPath, $"StageData{stageNum}_Node{i}");
            _nodeInstantiater.InstantiateNode(info, info.objectType, info.objectNum);
        }
        // NodeInfo에 맞게 Node 생성된 이후 설정한 generationTime 값에 따라 스테이지 시작 호출
        _nodeInstantiater.StageStart();
    }
    private int FindEachStageDataLength()
    {
        int length = 0;
        while (true)
        {
            try
            {
                FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", Application.dataPath, $"StageData{stageNum}_Node{length}"), FileMode.Open);
                fileStream.Close();
                length += 1;
            }
            catch (Exception e)
            {
                //Debug.Log(e.ToString());
                break;                
                throw;
            }
        }
        return length;
    }
}
