using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private StageUI[] stageUI;
    [SerializeField] private StageData[] stageData;
    // 출시까지 기능구현을 하지는 않을 생각. 다만 미리 만들어 놓았음 0번을 왼쪽, 1번을 오른쪽으로 할 예정
    [SerializeField] private Button[] arrowButtons;
    private int currentIndex;

    private void Awake()
    {
        InitSettings();
    }
    public void InitSettings()
    {
        //추후 게임 시작할 때로 변경예정.
        currentIndex = 0;
        GameManager.Instance.Save.LoadSaveData();
        for(int i = 0; i < stageUI.Length; ++i)
        {
            stageUI[i].InitSettings(stageData,i);            
        }
        
    }
}
