using System;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    public Dictionary<uint, MusicData> WaveMusic = new Dictionary<uint, MusicData>();

    [Serializable]
    public struct MusicData
    {
        public uint Difficulty; // (1)Easy  (2)Normal   (3)Hard
        public uint WaveType; // (1)Gun   (2)Punch    (3)ToastHit
        public uint GUID; // Difficulty + WaveType + ID;
        public string MusicName;
        public uint BPM;
        public List<uint[]> NodeData;
    }

    public void Init()
    {
        Debug.Log("Initialize DataManager");
        LoadInitialWaveData();
    }

    private void LoadInitialWaveData()
    {
        CSVImporter csvWave = new CSVImporter();
        csvWave.OpenFile("Data/");
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

                continue;
            }
            else
            {
                uint[] OneBeat = new uint[4];
                OneBeat[0] = uint.Parse(elems[0]);
                OneBeat[1] = uint.Parse(elems[1]);
                OneBeat[2] = uint.Parse(elems[2]);
                OneBeat[3] = uint.Parse(elems[3]);
                Node.Add(OneBeat);
            }
        }
        musicData.NodeData = Node;
        WaveMusic.Add(musicData.GUID, musicData);
    }

}