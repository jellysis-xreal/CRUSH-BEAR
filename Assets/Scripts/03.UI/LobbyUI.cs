using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private StageUI[] stageUI;
    [SerializeField] private StageData[] stageData;
    // ��ñ��� ��ɱ����� ������ ���� ����. �ٸ� �̸� ����� ������ 0���� ����, 1���� ���������� �� ����
    [SerializeField] private Button[] arrowButtons;
    //private int currentIndex;

    private void Awake()
    {
        InitSettings();
    }
    public void InitSettings()
    {
        //���� ���� ������ ���� ���濹��.
        //currentIndex = 0;
        GameManager.Instance.Save.LoadSaveData();
        for(int i = 0; i < stageUI.Length; ++i)
        {
            stageUI[i].InitSettings(stageData,i);            
        }
        
    }
}
