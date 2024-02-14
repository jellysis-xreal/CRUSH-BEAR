using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;

public class UI_ControlWave : MonoBehaviour
{
    [SerializeField] private GameObject[] GO_nodeLines;
    [SerializeField] private GameObject[] GO_scoreCanvas;
    private WaveType curWave;
        
    // Start is called before the first frame update
    void Start()
    {
        InitWave();
        curWave = GameManager.Wave.GetWaveType();
    }

    // Update is called once per frame
    void Update()
    {
        if (curWave != GameManager.Wave.GetWaveType())
        {
            UpdateWave(GameManager.Wave.GetWaveType());
        }
    }

    void InitWave()
    {
        foreach (GameObject nodeline in GO_nodeLines) {
            // Debug.Log("[TEST] node set active false");
            nodeline.SetActive(false);
        }
        foreach (GameObject scoreCanvas in GO_scoreCanvas) {
            // Debug.Log("[TEST] scoreCanvas set active false");
            scoreCanvas.SetActive(false);
        }
    }

    void UpdateWave(WaveType currentWave)
    {
        curWave = currentWave;
        InitWave();
        switch(curWave)
        {
            case WaveType.Shooting:
                GO_nodeLines[0].SetActive(true);
                GO_scoreCanvas[0].SetActive(true);
                break;
            case WaveType.Punching:
                GO_nodeLines[1].SetActive(true);
                GO_scoreCanvas[1].SetActive(true);
                break;
            case WaveType.Hitting:
                GO_nodeLines[2].SetActive(true);
                GO_scoreCanvas[2].SetActive(true);
                break;
        }
    }
}