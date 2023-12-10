using UnityEngine;

public class DataManager
{
    public void Init()
    {
        
    }

    private void LoadInitialWaveData()
    {
        CSVImporter csvWave = new CSVImporter();
        csvWave.OpenFile("Data");
        csvWave.ReadHeader();
        
        string line = csvWave.Readline();

        while (line != null)
        {
            string[] elems = line.Split(',');

            if(elems[0] == "")
            {
                break;
            }

            var NodeInfo = new NodeInfo();
            
            NodeInfo.posX = float.Parse(elems[0]);
            NodeInfo.posY = float.Parse(elems[1]);
            NodeInfo.posZ = float.Parse(elems[2]);
            NodeInfo.generationTime = float.Parse(elems[3]);
            NodeInfo.timeToReachPlayer = float.Parse(elems[4]);
            //NodeInfo.movingSpeed = float.Parse(elems[5]);
            
            NodeInfo.arrivalAreaIndex = int.Parse(elems[5]);
            // stageData.map = (GameManager.MapType)int.Parse(elems[1]);
            // stageData.step = int.Parse(elems[2]);
            // stageData.case_ = int.Parse(elems[3]);
            // stageData.ruid = int.Parse(elems[4]);
            // stageData.count = int.Parse(elems[5]);
            // line = csvStage.Readline();
            //
            // StageData.Add(stageData);
        }
        
    }

}