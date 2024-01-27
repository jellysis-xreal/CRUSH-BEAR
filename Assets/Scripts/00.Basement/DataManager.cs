using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager
{
    public Dictionary<uint, MusicData> waveMusicData = new Dictionary<uint, MusicData>();

    [Serializable]
    public struct MusicData
    {
        public uint Difficulty; // (1)Easy  (2)Normal   (3)Hard
        public uint WaveType; // (1)Gun   (2)Punch    (3)ToastHit
        public uint GUID; // Difficulty + WaveType + ID;
        public string MusicName;
        public uint BPM;
        public uint BeatNum;
        public List<uint[]> NodeData;
    }

    public void Init()
    {
        Debug.Log("Initialize DataManager");
        LoadInitialWaveData();
    }

    // csv 파일의 값들을 읽어 MusicData 구조체 내에 값을 저장한다. 추후 GameManager.Data.GetMusicData(uint index)로 노래에 대한 값들을 읽어온다.
    private void LoadInitialWaveData()
    {
        CSVImporter csvWave = new CSVImporter();
        if (!csvWave.OpenFile("Data/1_witchtea"))
        {
            Debug.Log("Read File Error");
            return;
        }

        csvWave.ReadHeader();
        string line = csvWave.Readline();

        //노래 정보
        var musicData = new MusicData();
        List<uint[]> Node = new List<uint[]>();
        
        while (line != null)
        {
            string[] elems = line.Split(',');

            if (elems[0] == "")
            {
                break;
            }

            if (elems[0] == "#")
            {
                musicData.Difficulty = uint.Parse(elems[1]);
                musicData.WaveType = uint.Parse(elems[2]);
                musicData.GUID = uint.Parse(elems[3]);
                musicData.MusicName = elems[4];
                musicData.BPM = uint.Parse(elems[5]);
                musicData.BeatNum = uint.Parse(elems[6]);

                line = csvWave.Readline();
                continue;
            }
            else
            {
                uint[] OneBeat = new uint[5];
                OneBeat[0] = uint.Parse(elems[0]);  // Beat
                OneBeat[1] = uint.Parse(elems[1]);  // Box 1
                OneBeat[2] = uint.Parse(elems[2]);  // Box 2
                OneBeat[3] = uint.Parse(elems[3]);  // Box 3
                OneBeat[4] = uint.Parse(elems[4]);  // Box 4
                Node.Add(OneBeat);
            }
            
            line = csvWave.Readline();
        }
        musicData.NodeData = Node.ToList();
        // Debug.Log("Data Count : "+ musicData.NodeData.Count);
        // Debug
        // foreach (var node in musicData.NodeData)
        // {
        //     Debug.Log(node[0] + " " + node[1] + " " + node[2] + " " + node[3]);
        // }
        waveMusicData.Add(musicData.GUID, musicData);
        Debug.Log($"DataManager : [Done] Load Wave Music Data {musicData.MusicName}");
    }

    public MusicData GetMusicData(uint id)
    {
        Debug.Log("DataManager : [Done] Load music data " + "GUID : "+id+" Music Name :" +waveMusicData[id].MusicName);
        return waveMusicData[id];
    }

    private void DebugNodeData()
    {
        
    }

}